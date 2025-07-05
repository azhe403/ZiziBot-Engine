using System.Text;
using AsyncAwaitBestPractices;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using ZiziBot.Common.Types;
using ZiziBot.Common.Utils;

namespace ZiziBot.Infrastructure.LoggerSink;

public class TelegramSink : ILogEventSink
{
    public string BotToken { get; init; }
    public long ChatId { get; init; }
    public long ThreadId { get; set; }

    private string ApiUrl => $"https://api.telegram.org/bot{BotToken}/sendMessage";

    public void Emit(LogEvent logEvent)
    {
        var exception = logEvent.Exception;

        if (ChatId == 0 || string.IsNullOrWhiteSpace(BotToken))
        {
            return;
        }

        var htmlMessage = HtmlMessage.Empty
            .CodeBr(logEvent.RenderMessage()).Br();

        htmlMessage.Text("Date: ").CodeBr(logEvent.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"));

        foreach (var (key, value) in logEvent.Properties.Select(x => (x.Key, x.Value.ToString().Replace("\"", ""))))
        {
            htmlMessage.Bold($"{key}: ");

            if (value.IsValidUrl())
            {
                htmlMessage.TextBr(value);
            }
            else if (value.Contains('.'))
            {
                var end = value.Split(".").LastOrDefault();
                htmlMessage.CodeBr(end);
            }
            else
            {
                htmlMessage.CodeBr(value);
            }
        }

        if (exception != null)
        {
            htmlMessage.Bold("Message: ").CodeBr(exception.Message)
                .Bold("Source: ").CodeBr(exception.Source ?? "N/A")
                .Bold("Type: ").CodeBr(exception.GetType().Name);
        }

        var stackFrame = exception?.ToStackTrace()
            .GetFrames()
            .FirstOrDefault(frame => frame.GetFileLineNumber() > 0);

        if (stackFrame != null)
        {
            htmlMessage
                .Bold("File: ").CodeBr(stackFrame.GetFileName().GetFileName())
                .Bold("Coordinate: ").CodeBr($"{stackFrame.GetFileLineNumber()}:{stackFrame.GetFileColumnNumber()}")
                .Bold("Namespace: ").CodeBr(stackFrame.GetMethod()!.DeclaringType!.Namespace!)
                .Bold("Assembly: ").CodeBr(stackFrame.GetMethod()!.DeclaringType!.Assembly.GetName().Name ?? string.Empty)
                .Bold("Assembly Version: ").CodeBr(VersionUtil.GetVersion())
                .Bold("Assembly Location: ").CodeBr(stackFrame.GetMethod()!.DeclaringType!.Assembly.Location);
        }

        htmlMessage.Bold("#").Text(logEvent.Level.ToString()).Text(" ").Text($"#LOG_{logEvent.Timestamp:yyyyMMdd}");

        SendMessageText(htmlMessage.ToString()).SafeFireAndForget(ex => Log.Error(ex, "Error when sending Telegram message: {ex}", ex.Message));
    }

    private async Task SendMessageText(string message)
    {
        var client = new HttpClient();
        var payload = new {
            chat_id = ChatId,
            message_thread_id = ThreadId,
            text = message,
            parse_mode = "HTML",
            disable_web_page_preview = true
        };

        var json = payload.ToJson();

        var responseMessage = await client.PostAsync(ApiUrl, new StringContent(json, Encoding.UTF8, "application/json"));
        Log.Verbose("Telegram response: {response}", await responseMessage.Content.ReadAsStringAsync());
    }
}

public static class TelegramSinkExtension
{
    public static LoggerSinkConfiguration Telegram(
        this LoggerSinkConfiguration configuration,
        string? botToken,
        long? chatId,
        long? threadId,
        LogEventLevel logEventLevel = LogEventLevel.Warning
    )
    {
        if (botToken == null || chatId == 0)
        {
            return configuration;
        }

        configuration.Sink(logEventSink: new TelegramSink() {
            BotToken = botToken,
            ChatId = chatId ?? 0,
            ThreadId = threadId ?? 0
        }, logEventLevel);

        return configuration;
    }
}