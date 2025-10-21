using Flights.Application.Abstractions;
using Flights.Application.Abstractions.Repositories;
using Flights.Domain.Flights;
using Microsoft.Extensions.Caching.Memory;


namespace Flights.Infrastructure.Cache
{
    public class FlightMemoryCache : IFlightCache
    {
        private readonly IMemoryCache _cache;
        private readonly IFlightRepository _repo;
        private static readonly string Key = "flights_all";
        private static readonly MemoryCacheEntryOptions Opts = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) };


        public FlightMemoryCache(IMemoryCache cache, IFlightRepository repo)
        { _cache = cache; _repo = repo; }


        public async Task<IReadOnlyList<Flight>> GetAllAsync(CancellationToken ct)
        {
            if (_cache.TryGetValue(Key, out IReadOnlyList<Flight>? list) && list is not null) return list;
            var data = await _repo.ListAsync(null, ct);
            _cache.Set(Key, data, Opts);
            return data;
        }
        public Task InvalidateAsync(CancellationToken ct) { _cache.Remove(Key); return Task.CompletedTask; }
        public async Task RefreshAsync(CancellationToken ct) { _cache.Remove(Key); await GetAllAsync(ct); }
    }
}