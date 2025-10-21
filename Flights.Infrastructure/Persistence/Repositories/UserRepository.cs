using Flights.Application.Abstractions.Repositories;
using Flights.Domain.Users;
using Microsoft.EntityFrameworkCore;


namespace Flights.Infrastructure.Persistence.Repositories
{
    public class UserRepository : EfRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext db) : base(db) { }


        public async Task<User?> GetWithRoleByUsernameAsync(string username, CancellationToken ct)
        => await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == username, ct);
    }
}