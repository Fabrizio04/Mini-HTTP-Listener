using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mini_HTTP_Listener.configuration;
using Mini_HTTP_Listener.service;
using Mini_HTTP_Listener.response;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, configuration) =>
    {

        configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

    })
    .ConfigureServices((context, services) =>
    {

        var configurationRoot = context.Configuration;
        services.Configure<Mini_HTTP_Listener_Configuration>(configurationRoot.GetSection(nameof(Mini_HTTP_Listener_Configuration)));

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders()
            .SetMinimumLevel(LogLevel.Trace)
            .AddNLog(new NLogLoggingConfiguration(configurationRoot.GetSection("NLog")));
        });

        services.AddHostedService<Mini_HTTP_Listener_Service>();
        services.AddSingleton<Response>();
        services.AddSingleton<Mini_HTTP_Listener_StaticPath>();

    })
    .Build();

// For testing a method available in the Service Provider
// using IServiceScope serviceScope = host.Services.CreateScope();
// IServiceProvider provider = serviceScope.ServiceProvider;
// <Interface or Class> obj = provider.GetRequiredService<Interface or Class>();
// obj.methodName();

await host.RunAsync();