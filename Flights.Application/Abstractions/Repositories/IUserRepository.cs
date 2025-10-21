using Flights.Domain.Users;


namespace Flights.Application.Abstractions.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetWithRoleByUsernameAsync(string username, CancellationToken ct);
    }
}