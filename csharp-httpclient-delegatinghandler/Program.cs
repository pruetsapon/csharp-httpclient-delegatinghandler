using csharp_httpclient_delegatinghandler;
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
}