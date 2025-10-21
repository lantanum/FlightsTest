using Flights.Domain.Flights;

namespace Flights.Application.Abstractions
{
    public interface IFlightCache
    {
        Task<IReadOnlyList<Flight>> GetAllAsync(CancellationToken ct);
        Task InvalidateAsync(CancellationToken ct);
        Task RefreshAsync(CancellationToken ct);
    }
}