using System.Collections.Generic;
using BinanceSimulator.Configuration;
using BinanceSimulator.Models;

namespace BinanceSimulator.Strategies;

public sealed class MovingAverageStrategy : ITradingStrategy
{
    private readonly StrategyConfig _config;

    public MovingAverageStrategy(AppConfig appConfig)
    {
        _config = appConfig.Strategy;
        if (_config.ShortWindow <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(appConfig), "Short moving average window must be greater than zero.");
        }

        if (_config.LongWindow <= _config.ShortWindow)
        {
            throw new ArgumentException("Long moving average window must be greater than the short window.");
        }
    }

    public TradeSignal Evaluate(IReadOnlyList<KlineData> history)
    {
        if (history.Count < _config.LongWindow)
        {
            return new TradeSignal(TradeAction.Hold, "Insufficient history");
        }

        decimal shortAverage = Average(history, _config.ShortWindow);
        decimal longAverage = Average(history, _config.LongWindow);
        decimal delta = (shortAverage - longAverage) / longAverage;

        if (delta >= _config.Threshold)
        {
            return new TradeSignal(TradeAction.Buy, $"Short MA above long MA by {delta:P2}");
        }

        if (delta <= -_config.Threshold)
        {
            return new TradeSignal(TradeAction.Sell, $"Short MA below long MA by {delta:P2}");
        }

        return new TradeSignal(TradeAction.Hold, $"MA delta {delta:P2} within threshold");
    }

    private static decimal Average(IReadOnlyList<KlineData> history, int window)
    {
        decimal sum = 0m;
        int startIndex = history.Count - window;
        for (int i = startIndex; i < history.Count; i++)
        {
            sum += history[i].Close;
        }

        return sum / window;
    }
}
