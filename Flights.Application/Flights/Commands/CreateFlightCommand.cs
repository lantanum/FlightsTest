using Flights.Application.Common;
using Flights.Application.Flights.Dtos;
using Flights.Domain.Flights;
using MediatR;
using Flights.Application.Abstractions;
using Flights.Application.Abstractions.Repositories;

namespace Flights.Application.Flights.Commands
{
    public record CreateFlightCommand(CreateFlightDto Dto) : IRequest<Result<int>>;

    public class CreateFlightCommandHandler
        : IRequestHandler<CreateFlightCommand, Result<int>>
    {
        private readonly IFlightRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IFlightCache _cache;

        public CreateFlightCommandHandler(
            IFlightRepository repo,
            IUnitOfWork uow,
            IFlightCache cache)
        {
            _repo = repo;
            _uow = uow;
            _cache = cache;
        }

        public async Task<Result<int>> Handle(CreateFlightCommand request, CancellationToken ct)
        {
            var entity = new Flight
            {
                Origin = request.Dto.Origin,
                Destination = request.Dto.Destination,
                Departure = request.Dto.Departure,
                Arrival = request.Dto.Arrival,
                Status = FlightStatus.InTime
            };

            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            await _cache.RefreshAsync(ct);

            return Result<int>.Success(entity.Id);
        }
    }
}
