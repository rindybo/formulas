using System.Text.Json.Serialization;

namespace BinanceSimulator.Configuration;

public sealed class AppConfig
{
    [JsonPropertyName("binance")]
    public required BinanceConfig Binance { get; init; }

    [JsonPropertyName("strategy")]
    public required StrategyConfig Strategy { get; init; }

    [JsonPropertyName("simulation")]
    public required SimulationConfig Simulation { get; init; }
}

public sealed class BinanceConfig
{
    [JsonPropertyName("apiKey")]
    public string? ApiKey { get; init; }

    [JsonPropertyName("apiSecret")]
    public string? ApiSecret { get; init; }

    [JsonPropertyName("symbol")]
    public required string Symbol { get; init; }

    [JsonPropertyName("klineIntervalMinutes")]
    public int KlineIntervalMinutes { get; init; } = 1;

    [JsonPropertyName("klinesLimit")]
    public int KlinesLimit { get; init; } = 500;

    [JsonPropertyName("testNet")]
    public bool UseTestNet { get; init; }
}

public sealed class StrategyConfig
{
    [JsonPropertyName("shortWindow")]
    public int ShortWindow { get; init; } = 7;

    [JsonPropertyName("longWindow")]
    public int LongWindow { get; init; } = 25;

    [JsonPropertyName("threshold")]
    public decimal Threshold { get; init; } = 0.002m;
}

public sealed class SimulationConfig
{
    [JsonPropertyName("initialQuoteBalance")]
    public decimal InitialQuoteBalance { get; init; } = 1000m;

    [JsonPropertyName("baseAsset")]
    public required string BaseAsset { get; init; }

    [JsonPropertyName("quoteAsset")]
    public required string QuoteAsset { get; init; }

    [JsonPropertyName("tradingFeeRate")]
    public decimal TradingFeeRate { get; init; } = 0.00075m;
}
