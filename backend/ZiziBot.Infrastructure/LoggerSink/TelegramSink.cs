using System.Text;
using AsyncAwaitBestPractices;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;

namespace ZiziBot.Infrastructure.LoggerSink;

public class TelegramSink : ILogEventSink
{
    public string? BotToken { get; init; }
    public long? ChatId { get; init; }
    public long? ThreadId { get; set; }

    private string ApiUrl => $"https://api.telegram.org/bot{BotToken}/sendMessage";

    public void Emit(LogEvent logEvent)
    {
        var exception = logEvent.Exception;

        if (ChatId == 0 || BotToken == null)
        {
            return;
        }

        var htmlMessage = HtmlMessage.Empty
            .BoldBr("🛑 EventLog")
            .CodeBr(logEvent.RenderMessage()).Br()
            .Bold("Level: ").CodeBr(logEvent.Level.ToString())
            .Bold("Timestamp: ").CodeBr(logEvent.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"));

        foreach (var (key, value) in logEvent.Properties)
        {
            htmlMessage.Bold($"{key}: ").CodeBr(value.ToString());
        }

        if (exception != null)
        {
            htmlMessage.Bold("Message: ").CodeBr(exception.Message)
                .Bold("Source: ").CodeBr(exception.Source ?? "N/A")
                .Bold("Type: ").CodeBr(exception.GetType().Name);
            // .Bold("Exception: ").CodeBr(exception.GetType().Name)
            // .Bold("Request: ").CodeBr(typeof(TRequest).Name);
        }

        var stackFrame = exception?.ToStackTrace()
            .GetFrames()
            .FirstOrDefault(frame => frame.GetFileLineNumber() > 0);

        if (stackFrame != null)
        {
            htmlMessage
                .Bold("File: ").CodeBr(stackFrame.GetFileName()!)
                .Bold("Coordinate: ").CodeBr($"{stackFrame.GetFileLineNumber()}:{stackFrame.GetFileColumnNumber()}")
                .Bold("Namespace: ").CodeBr(stackFrame.GetMethod()!.DeclaringType!.Namespace!)
                .Bold("Assembly: ")
                .CodeBr(stackFrame.GetMethod()!.DeclaringType!.Assembly.GetName().Name ?? string.Empty)
                .Bold("Assembly Version: ")
                .CodeBr(stackFrame.GetMethod()!.DeclaringType!.Assembly.GetName().Version!.ToString())
                .Bold("Assembly Location: ").CodeBr(stackFrame.GetMethod()!.DeclaringType!.Assembly.Location);
        }

        SendMessageText(htmlMessage.ToString()).SafeFireAndForget();
    }

    private async Task SendMessageText(string message)
    {
        var client = new HttpClient();
        var payload = new
        {
            chat_id = ChatId,
            message_thread_id = ThreadId,
            text = message,
            parse_mode = "HTML",
        };

        var json = payload.ToJson();

        var responseMessage = await client.PostAsync(ApiUrl, new StringContent(json, Encoding.UTF8, "application/json"));
        SelfLog.WriteLine("Telegram response: {response}", await responseMessage.Content.ReadAsStringAsync());
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

        configuration.Sink(logEventSink: new TelegramSink()
        {
            BotToken = botToken,
            ChatId = chatId,
            ThreadId = threadId
        }, logEventLevel);

        return configuration;
    }
}