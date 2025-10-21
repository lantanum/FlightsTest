using Flights.Domain.Flights;


namespace Flights.Application.Flights.Dtos
{
    public record FlightDto(int Id, string Origin, string Destination, DateTimeOffset Departure, DateTimeOffset Arrival, FlightStatus Status);
}