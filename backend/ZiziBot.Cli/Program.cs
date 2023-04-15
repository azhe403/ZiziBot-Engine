// See https://aka.ms/new-console-template for more information

using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using Microsoft.Extensions.Hosting;

await CmdRoot.CreateCommandLineBuilder()
    .UseHost(x => Host.CreateDefaultBuilder(),
        configureHost: hostBuilder => {
            hostBuilder
                .ConfigureSerilog()
                .ConfigureAppConfiguration(builder => {
                    // builder.LoadSettings();
                })
                .ConfigureServices(services => {
                    services.AddTools();
                });
        })
    .UseDefaults()
    .Build()
    .InvokeAsync(args);