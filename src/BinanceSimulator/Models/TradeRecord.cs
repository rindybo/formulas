namespace BinanceSimulator.Models;

public enum TradeSide
{
    Buy,
    Sell
}

public sealed record TradeRecord(
    DateTime Timestamp,
    TradeSide Side,
    decimal Price,
    decimal Quantity,
    decimal QuoteBalanceAfter,
    decimal BaseBalanceAfter,
    string Reason);
