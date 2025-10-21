using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;


namespace Flights.Infrastructure.Persistence.Interceptors
{
    public class ChangeLoggingInterceptor : SaveChangesInterceptor
    {
        private readonly Func<string?> _usernameAccessor;
        public ChangeLoggingInterceptor(Func<string?> usernameAccessor)
        {
            _usernameAccessor = usernameAccessor;
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
        {
            var ctx = eventData.Context;
            if (ctx is null) return base.SavingChangesAsync(eventData, result, ct);
            var entries = ctx.ChangeTracker.Entries()
            .Where(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Added || e.State == Microsoft.EntityFrameworkCore.EntityState.Modified || e.State == Microsoft.EntityFrameworkCore.EntityState.Deleted)
            .Select(e => new { e.Entity.GetType().Name, e.State });


            if (entries.Any())
            {
                var user = _usernameAccessor() ?? "anonymous";
                foreach (var item in entries)
                    Log.Information("DB change by {User} at {Time}: {Entity} -> {State}", user, DateTimeOffset.UtcNow, item.Name, item.State);
            }
            return base.SavingChangesAsync(eventData, result, ct);
        }
    }
}