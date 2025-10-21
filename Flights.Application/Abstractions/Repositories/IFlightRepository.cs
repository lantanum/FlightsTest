using Flights.Domain.Flights;


namespace Flights.Application.Abstractions.Repositories
{
    public interface IFlightRepository : IRepository<Flight>
    {
        Task<IReadOnlyList<Flight>> GetFilteredAsync(string? origin, string? destination, CancellationToken ct);
    }
}