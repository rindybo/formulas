using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceSimulator.Configuration;
using BinanceSimulator.Models;
using BinanceSimulator.Strategies;
using Microsoft.Extensions.Logging;

namespace BinanceSimulator.Services;

public sealed class TradingSimulator
{
    private readonly IBinanceMarketDataService _marketDataService;
    private readonly ITradingStrategy _strategy;
    private readonly SimulationConfig _simulationConfig;
    private readonly BinanceConfig _binanceConfig;
    private readonly ILogger<TradingSimulator> _logger;

    public TradingSimulator(
        AppConfig appConfig,
        IBinanceMarketDataService marketDataService,
        ITradingStrategy strategy,
        ILogger<TradingSimulator> logger)
    {
        _marketDataService = marketDataService;
        _strategy = strategy;
        _simulationConfig = appConfig.Simulation;
        _binanceConfig = appConfig.Binance;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<KlineData> klines = await _marketDataService.GetHistoricalKlinesAsync(cancellationToken);
        if (klines.Count == 0)
        {
            _logger.LogWarning("No kline data returned for {Symbol}. Simulation aborted.", _binanceConfig.Symbol);
            return;
        }

        List<KlineData> history = new();
        List<TradeRecord> trades = new();
        decimal quoteBalance = _simulationConfig.InitialQuoteBalance;
        decimal baseBalance = 0m;

        _logger.LogInformation(
            "Starting simulation for {Symbol} with {QuoteBalance} {QuoteAsset}.",
            _binanceConfig.Symbol,
            quoteBalance,
            _simulationConfig.QuoteAsset);

        foreach (KlineData kline in klines)
        {
            history.Add(kline);
            TradeSignal signal = _strategy.Evaluate(history);

            switch (signal.Action)
            {
                case TradeAction.Buy when quoteBalance > 0:
                    decimal quantity = quoteBalance / kline.Close;
                    decimal feeBase = quantity * _simulationConfig.TradingFeeRate;
                    decimal netQuantity = quantity - feeBase;
                    baseBalance += netQuantity;
                    quoteBalance = 0m;
                    trades.Add(new TradeRecord(kline.OpenTime, TradeSide.Buy, kline.Close, netQuantity, quoteBalance, baseBalance, signal.Reason));
                    _logger.LogInformation("[{Time}] BUY {Quantity} {Base} at {Price} because {Reason}", kline.OpenTime, netQuantity, _simulationConfig.BaseAsset, kline.Close, signal.Reason);
                    break;

                case TradeAction.Sell when baseBalance > 0:
                    decimal grossQuote = baseBalance * kline.Close;
                    decimal feeQuote = grossQuote * _simulationConfig.TradingFeeRate;
                    decimal netQuote = grossQuote - feeQuote;
                    quoteBalance += netQuote;
                    trades.Add(new TradeRecord(kline.OpenTime, TradeSide.Sell, kline.Close, baseBalance, quoteBalance, 0m, signal.Reason));
                    _logger.LogInformation("[{Time}] SELL {Quantity} {Base} at {Price} because {Reason}", kline.OpenTime, baseBalance, _simulationConfig.BaseAsset, kline.Close, signal.Reason);
                    baseBalance = 0m;
                    break;

                default:
                    _logger.LogDebug("[{Time}] HOLD at price {Price} because {Reason}", kline.OpenTime, kline.Close, signal.Reason);
                    break;
            }
        }

        decimal finalPrice = klines[^1].Close;
        decimal finalPortfolioValue = quoteBalance + (baseBalance * finalPrice);

        _logger.LogInformation("Simulation finished. Quote balance: {Quote}, Base balance: {Base}, Portfolio value: {Value} {QuoteAsset}",
            quoteBalance,
            baseBalance,
            finalPortfolioValue,
            _simulationConfig.QuoteAsset);

        _logger.LogInformation("Total trades executed: {Count}", trades.Count);

        if (trades.Count == 0)
        {
            _logger.LogWarning("No trades were executed. Consider adjusting your strategy thresholds.");
        }
    }
}
