using Microsoft.AspNetCore.SignalR;
using Serilog.Sinks.AspNetCore.SignalR.Interfaces;

namespace ZiziBot.Infrastructure.LoggerSink;

public class LogHub : Hub<IHub>
{

}