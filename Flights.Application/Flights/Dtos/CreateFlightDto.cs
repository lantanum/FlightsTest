namespace Flights.Application.Flights.Dtos
{
    public class CreateFlightDto
    {
        public string Origin { get; set; } = default!;
        public string Destination { get; set; } = default!;
        public DateTimeOffset Departure { get; set; }
        public DateTimeOffset Arrival { get; set; }
    }
}