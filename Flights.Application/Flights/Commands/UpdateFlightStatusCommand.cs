using Flights.Application.Common;
using Flights.Application.Flights.Dtos;
using MediatR;
using Flights.Application.Abstractions;
using Flights.Application.Abstractions.Repositories;


namespace Flights.Application.Flights.Commands
{
    public record UpdateFlightStatusCommand(int FlightId, UpdateFlightStatusDto Dto) : IRequest<Result<bool>>;


    public class UpdateFlightStatusCommandHandler : IRequestHandler<UpdateFlightStatusCommand, Result<bool>>
    {
        private readonly IFlightRepository _repo; 
        private readonly IUnitOfWork _uow; 
        private readonly IFlightCache _cache;
        public UpdateFlightStatusCommandHandler(IFlightRepository repo, IUnitOfWork uow, IFlightCache cache)
        { 
            _repo = repo; 
            _uow = uow; 
            _cache = cache; }


        public async Task<Result<bool>> Handle(UpdateFlightStatusCommand request, CancellationToken ct)
        {
            var f = await _repo.GetByIdAsync(request.FlightId, ct);
            if (f is null) return Result<bool>.Fail("Flight not found");
            f.Status = request.Dto.Status;
            _repo.Update(f);
            await _uow.SaveChangesAsync(ct);
            await _cache.RefreshAsync(ct);
            return Result<bool>.Success(true);
        }
    }
}