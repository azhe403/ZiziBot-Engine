namespace ZiziBot.Application.MediatR;

public class ServiceResult
{
    public string Message { get; set; }

    public ServiceResult Complete(string message)
    {
        Message = message;
        return this;
    }
}