using System.Text;
using AsyncAwaitBestPractices;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;

namespace ZiziBot.Infrastructure.LoggerSink;

public class TelegramSink : ILogEventSink
{
    public required string BotToken { get; set; }
    public required long ChatId { get; set; }

    private string ApiUrl => $"https://api.telegram.org/bot{BotToken}/sendMessage";

    public void Emit(LogEvent logEvent)
    {
        var exception = logEvent.Exception;
        var stackFrame = exception.ToStackTrace()
            .GetFrames()
            .FirstOrDefault(frame => frame.GetFileLineNumber() > 0);

        if (ChatId == 0)
        {
            return;
        }

        var htmlMessage = HtmlMessage.Empty
            .BoldBr("🛑 EventLog")
            .Bold("Message: ").CodeBr(exception.Message)
            .Bold("Source: ").CodeBr(exception.Source ?? "N/A")
            .Bold("Type: ").CodeBr(exception.GetType().Name);
        // .Bold("Exception: ").CodeBr(exception.GetType().Name)
        // .Bold("Request: ").CodeBr(typeof(TRequest).Name);

        if (stackFrame != null)
        {
            htmlMessage
                .Bold("File: ").CodeBr(stackFrame.GetFileName()!)
                .Bold("Coordinate: ").CodeBr($"{stackFrame.GetFileLineNumber()}:{stackFrame.GetFileColumnNumber()}")
                .Bold("Namespace: ").CodeBr(stackFrame.GetMethod()!.DeclaringType!.Namespace!)
                .Bold("Assembly: ").CodeBr(stackFrame.GetMethod()!.DeclaringType!.Assembly.GetName().Name ?? string.Empty)
                .Bold("Assembly Version: ").CodeBr(stackFrame.GetMethod()!.DeclaringType!.Assembly.GetName().Version!.ToString())
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
            text = message,
            parse_mode = "HTML",
        };

        var json = payload.ToJson();

        var responseMessage = await client.PostAsync(ApiUrl, new StringContent(json, Encoding.UTF8, "application/json"));
        SelfLog.WriteLine("Telegram response: {response}", await responseMessage.Content.ReadAsStringAsync());
    }
}