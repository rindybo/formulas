namespace BinanceSimulator.Models;

public sealed record KlineData(
    DateTime OpenTime,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume);
