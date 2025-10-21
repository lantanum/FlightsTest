namespace Flights.Infrastructure.Auth
{
    public static class PasswordHasher
    {
        public static bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}