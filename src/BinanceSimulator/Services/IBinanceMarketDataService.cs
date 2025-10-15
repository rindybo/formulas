using System.Threading;
using System.Threading.Tasks;
using BinanceSimulator.Models;

namespace BinanceSimulator.Services;

public interface IBinanceMarketDataService : IAsyncDisposable
{
    Task<IReadOnlyList<KlineData>> GetHistoricalKlinesAsync(CancellationToken cancellationToken = default);
}
