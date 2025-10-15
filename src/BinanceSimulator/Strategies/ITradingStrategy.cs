using System.Collections.Generic;
using BinanceSimulator.Models;

namespace BinanceSimulator.Strategies;

public enum TradeAction
{
    Hold,
    Buy,
    Sell
}

public sealed record TradeSignal(TradeAction Action, string Reason);

public interface ITradingStrategy
{
    TradeSignal Evaluate(IReadOnlyList<KlineData> history);
}
