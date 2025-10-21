using Flights.Application.Abstractions;
using Flights.Application.Abstractions.Repositories;
using Flights.Application.Common;
using Flights.Application.Flights.Dtos;
using MediatR;

namespace Flights.Application.Flights.Queries
{
    public record GetFlightsQuery(string? Origin, string? Destination) : IRequest<Result<IReadOnlyList<FlightDto>>>;


    public class GetFlightsQueryHandler : IRequestHandler<GetFlightsQuery, Result<IReadOnlyList<FlightDto>>>
    {
        private readonly IFlightCache _cache;
        private readonly IFlightRepository _repo;
        public GetFlightsQueryHandler(IFlightCache cache, IFlightRepository repo)
        {
            _cache = cache;
            _repo = repo;
        }
        public async Task<Result<IReadOnlyList<FlightDto>>> Handle(GetFlightsQuery request, CancellationToken ct)
        {
            var flights = await _cache.GetAllAsync(ct);
            var q = flights.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.Origin)) q = q.Where(f => f.Origin == request.Origin);
            if (!string.IsNullOrWhiteSpace(request.Destination)) q = q.Where(f => f.Destination == request.Destination);

            var res = q.OrderBy(f => f.Arrival)
                       .Select(f => new FlightDto(f.Id, f.Origin, f.Destination, f.Departure, f.Arrival, f.Status))
                       .ToList();
            return Result<IReadOnlyList<FlightDto>>.Success(res);
        }
    }
}