using LightSensorDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace LightSensorDAL.Data
{
    public class LightSensorDbContext : DbContext
    {
        public DbSet<Telemetry> Telemetries { get; set; }

        public LightSensorDbContext(DbContextOptions<LightSensorDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
