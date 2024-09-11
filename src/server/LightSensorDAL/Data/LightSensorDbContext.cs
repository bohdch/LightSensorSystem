using LightSensorDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace LightSensorDAL.Data
{
    public class LightSensorDbContext : DbContext
    {
        public DbSet<Telemetry> Telemetries { get; set; }
        public DbSet<Client> Clients { get; set; }

        public LightSensorDbContext(DbContextOptions<LightSensorDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Client>()
                .HasIndex(u => u.Name)
                .IsUnique();

            // Predefined client
            modelBuilder.Entity<Client>().HasData(
                new
                {
                    Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                    Name = "LightSensorClient",
                    HashedPassword = "AQAAAAIAAYagAAAAEKOFhy//Me/EsaBC93qxqEKuJOhGOJuJh0nZioQyHMdk12rsmryoml7hbvp/uP7MAw==",
                });
        }
    }
}
