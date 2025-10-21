using Flights.Application.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace Flights.Infrastructure.Persistence.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _db;
        protected readonly DbSet<T> _set;
        public EfRepository(AppDbContext db) { _db = db; _set = db.Set<T>(); }


        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct) => await _set.FindAsync(new object?[] { id }, ct);


        public virtual async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate, CancellationToken ct)
        {
            IQueryable<T> q = _set.AsNoTracking();
            if (predicate is not null) q = q.Where(predicate);
            return await q.ToListAsync(ct);
        }


        public virtual async Task AddAsync(T entity, CancellationToken ct) => await _set.AddAsync(entity, ct);
        public virtual void Update(T entity) => _set.Update(entity);
        public virtual void Remove(T entity) => _set.Remove(entity);
    }
}