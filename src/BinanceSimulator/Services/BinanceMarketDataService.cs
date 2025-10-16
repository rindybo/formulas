using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Net;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects;
using BinanceSimulator.Configuration;
using BinanceSimulator.Models;
using Microsoft.Extensions.Logging;
using CryptoExchange.Net.Authentication;


namespace BinanceSimulator.Services;

public sealed class BinanceMarketDataService : IBinanceMarketDataService
{
    private readonly BinanceConfig _config;
    private readonly ILogger<BinanceMarketDataService> _logger;
    private readonly BinanceClient _client;

    public BinanceMarketDataService(AppConfig appConfig, ILogger<BinanceMarketDataService> logger)
    {
        _config = appConfig.Binance;
        _logger = logger;

        BinanceClientOptions options = new();
        if (!string.IsNullOrWhiteSpace(_config.ApiKey) && !string.IsNullOrWhiteSpace(_config.ApiSecret))
        {
            options.ApiCredentials = new ApiCredentials(_config.ApiKey, _config.ApiSecret);
        }

        if (_config.UseTestNet)
        {
            options.SpotApiOptions = new BinanceApiClientOptions
            {
                BaseAddress = BinanceApiAddresses.TestNet.SpotRestAddress
            };
        }

        _client = new BinanceClient(options);
    }

    public async Task<IReadOnlyList<KlineData>> GetHistoricalKlinesAsync(CancellationToken cancellationToken = default)
    {
        KlineInterval interval = IntervalFromMinutes(_config.KlineIntervalMinutes);
        _logger.LogInformation("Requesting {Limit} klines for {Symbol} at {Interval} interval", _config.KlinesLimit, _config.Symbol, interval);

        var response = await _client.SpotApi.ExchangeData.GetKlinesAsync(
            _config.Symbol,
            interval,
            limit: _config.KlinesLimit,
            cancellationToken: cancellationToken);

        if (!response.Success)
        {
            throw new InvalidOperationException($"Failed to retrieve klines: {response.Error}");
        }

        return response.Data
            .Select(k => new KlineData(k.OpenTime, k.OpenPrice, k.HighPrice, k.LowPrice, k.ClosePrice, k.Volume))
            .ToList();
    }

    private static KlineInterval IntervalFromMinutes(int minutes)
    {
        return minutes switch
        {
            1 => KlineInterval.OneMinute,
            3 => KlineInterval.ThreeMinutes,
            5 => KlineInterval.FiveMinutes,
            15 => KlineInterval.FifteenMinutes,
            30 => KlineInterval.ThirtyMinutes,
            60 => KlineInterval.OneHour,
            120 => KlineInterval.TwoHour,
            240 => KlineInterval.FourHour,
            360 => KlineInterval.SixHour,
            720 => KlineInterval.TwelveHour,
            1440 => KlineInterval.OneDay,
            4320 => KlineInterval.ThreeDay,
            10080 => KlineInterval.OneWeek,
            43200 => KlineInterval.OneMonth,
            _ => throw new ArgumentOutOfRangeException(nameof(minutes), minutes, "Unsupported kline interval")
        };
    }

    public ValueTask DisposeAsync()
    {
        _client.Dispose();
        return ValueTask.CompletedTask;
    }
}
