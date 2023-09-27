using csharp_httpclient_delegatinghandler;
using csharp_httpclient_delegatinghandler.Configurations;
using csharp_httpclient_delegatinghandler.Handlers;
using csharp_httpclient_delegatinghandler.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{environmentName}.json", true, true)
        .AddEnvironmentVariables()
        .Build();

var loggerConfig = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<AppSettings>(configuration);
builder.Services.AddSingleton(configuration.GetSection(typeof(AuthenticationConfiguration).Name).Get<AuthenticationConfiguration>() ?? throw new Exception("authentication service config is null."));
builder.Services.AddSingleton(configuration.GetSection(typeof(DataConfiguration).Name).Get<DataConfiguration>() ?? throw new Exception("data service config is null."));
builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<AuthenticationDelegatingHandler>();
builder.Services.AddHttpClient<IDataService, DataService>()
                .AddHttpMessageHandler<AuthenticationDelegatingHandler>();
builder.Services.AddMemoryCache();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(loggerConfig);

using IHost host = builder.Build();

RunningApplication(host.Services);

await host.RunAsync();

static void RunningApplication(IServiceProvider hostProvider)
{
    using IServiceScope serviceScope = hostProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;
    var logger = provider.GetRequiredService<ILogger<AppSettings>>();
    var dataService = provider.GetRequiredService<IDataService>();
    var version = dataService.GetVersion().Result;

    logger.LogInformation($"version: {version}");
}