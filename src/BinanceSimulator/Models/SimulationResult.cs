namespace BinanceSimulator.Models;

public sealed record SimulationResult(
    decimal FinalQuoteBalance,
    decimal FinalBaseBalance,
    decimal FinalPortfolioValue,
    IReadOnlyList<TradeRecord> Trades);
