using Flights.Domain.Flights;
using Flights.Domain.Users;
using Microsoft.EntityFrameworkCore;


namespace Flights.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Flight> Flights => Set<Flight>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Flight>(e =>
            {
                e.ToTable("Flights");
                e.HasKey(x => x.Id);
                e.Property(x => x.Origin).HasMaxLength(256).IsRequired();
                e.Property(x => x.Destination).HasMaxLength(256).IsRequired();
                e.Property(x => x.Status).HasConversion<int>();
                e.HasIndex(x => new { x.Origin, x.Destination });
            });


            b.Entity<Role>(e =>
            {
                e.ToTable("Roles");
                e.HasKey(x => x.Id);
                e.Property(x => x.Code).HasMaxLength(256).IsRequired();
                e.HasIndex(x => x.Code).IsUnique();
            });


            b.Entity<User>(e =>
            {
                e.ToTable("Users");
                e.HasKey(x => x.Id);
                e.Property(x => x.Username).HasMaxLength(256).IsRequired();
                e.Property(x => x.Password).HasMaxLength(256).IsRequired();
                e.HasIndex(x => x.Username).IsUnique();
                e.HasOne(x => x.Role).WithMany(r => r.Users).HasForeignKey(x => x.RoleId);
            });
        }
    }
}