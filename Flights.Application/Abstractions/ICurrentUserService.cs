namespace Flights.Application.Abstractions
{
    public interface ICurrentUserService
    {
        string? Username { get; }
        string? Role { get; }
    }
}