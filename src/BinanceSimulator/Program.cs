using BinanceSimulator.Configuration;
using BinanceSimulator.Services;
using BinanceSimulator.Strategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

string configurationPath = args.Length > 0 ? args[0] : "appsettings.json";
AppConfig appConfig = ConfigLoader.Load(configurationPath);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
    options.SingleLine = true;
});

builder.Services.AddSingleton(appConfig);
builder.Services.AddSingleton<IBinanceMarketDataService, BinanceMarketDataService>();
builder.Services.AddSingleton<ITradingStrategy, MovingAverageStrategy>();
builder.Services.AddSingleton<TradingSimulator>();

using IHost host = builder.Build();

TradingSimulator simulator = host.Services.GetRequiredService<TradingSimulator>();
await simulator.RunAsync();
