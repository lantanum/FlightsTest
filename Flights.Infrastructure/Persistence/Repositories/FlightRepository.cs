using Flights.Application.Abstractions.Repositories;
using Flights.Domain.Flights;
using Microsoft.EntityFrameworkCore;


namespace Flights.Infrastructure.Persistence.Repositories
{
    public class FlightRepository : EfRepository<Flight>, IFlightRepository
    {
        public FlightRepository(AppDbContext db) : base(db) { }


        public async Task<IReadOnlyList<Flight>> GetFilteredAsync(string? origin, string? destination, CancellationToken ct)
        {
            var q = _db.Flights.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(origin)) q = q.Where(f => f.Origin == origin);
            if (!string.IsNullOrWhiteSpace(destination)) q = q.Where(f => f.Destination == destination);
            return await q.OrderBy(f => f.Arrival).ToListAsync(ct);
        }
    }
}