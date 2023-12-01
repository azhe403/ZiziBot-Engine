using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;

namespace ZiziBot.Console.Pages;

public partial class LogViewer
{
    [Inject]
    protected IJSRuntime JSRuntime { get; set; }

    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected DialogService DialogService { get; set; }

    [Inject]
    protected TooltipService TooltipService { get; set; }

    [Inject]
    protected ContextMenuService ContextMenuService { get; set; }

    [Inject]
    protected NotificationService NotificationService { get; set; }

    [Inject]
    protected IHostEnvironment HostEnvironment { get; set; }

    private List<Log> Logs { get; set; } = new List<Log>();

    private List<Log> LogsFiltered = new List<Log>();

    protected override async Task OnInitializedAsync()
    {
        var hubConnection = new HubConnectionBuilder()
                            .WithUrl(NavigationManager.ToAbsoluteUri("/api/logging"), options => {
                                options.HttpMessageHandlerFactory = factory => {
                                    if (factory is HttpClientHandler clientHandler)
                                    {
                                        clientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                                            HostEnvironment.IsDevelopment();
                                    }

                                    return factory;
                                };
                            })
                            .Build();

        hubConnection.On<Log>("SendLogAsObject", async (data) =>
        {
            Logs.Insert(0, data);
            LogsFiltered = Logs.Take(100).ToList();

            await InvokeAsync(StateHasChanged);
        });

        await hubConnection.StartAsync();
    }
}

public class Log
{
    public string id { get; set; }
    public string timestamp { get; set; }
    public string level { get; set; }
    public string message { get; set; }
    public string exception { get; set; }
}