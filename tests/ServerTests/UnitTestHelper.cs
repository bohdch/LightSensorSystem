using LightSensorDAL.Data;
using LightSensorDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ServerTests
{
    internal static class UnitTestHelper
    {
        public static DbContextOptions<LightSensorDbContext> GetUnitTestDbOptions()
        {
            var options = new DbContextOptionsBuilder<LightSensorDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new LightSensorDbContext(options))
            {
                SeedData(context);
            }

            return options;
        }

        public static void SeedData(LightSensorDbContext context)
        {
            var baseDate = DateTime.UtcNow.Date;

            // Data for Device 1
            context.Telemetries.AddRange(
                new Telemetry
                {
                    Id = Guid.NewGuid(),
                    DeviceId = 1,
                    Illum = 100.0,
                    Time = baseDate.AddDays(-1) // Yesterday
                },
                new Telemetry
                {
                    Id = Guid.NewGuid(),
                    DeviceId = 1,
                    Illum = 150.0,
                    Time = baseDate.AddDays(-1) // Yesterday, different time
                },
                new Telemetry
                {
                    Id = Guid.NewGuid(),
                    DeviceId = 1,
                    Illum = 200.0,
                    Time = baseDate.AddDays(-2) // 2 days ago
                },
                new Telemetry
                {
                    Id = Guid.NewGuid(),
                    DeviceId = 1,
                    Illum = 250.0,
                    Time = baseDate.AddDays(-30) // 30 days ago
                },
                new Telemetry
                {
                    Id = Guid.NewGuid(),
                    DeviceId = 1,
                    Illum = 300.0,
                    Time = baseDate.AddDays(-15) // 15 days ago
                }
            );

            // Data for Device 2
            context.Telemetries.AddRange(
                new Telemetry
                {
                    Id = Guid.NewGuid(),
                    DeviceId = 2,
                    Illum = 50.0,
                    Time = baseDate.AddDays(-1) // Yesterday
                },
                new Telemetry
                {
                    Id = Guid.NewGuid(),
                    DeviceId = 2,
                    Illum = 75.0,
                    Time = baseDate.AddDays(-2) // 2 days ago
                },
                new Telemetry
                {
                    Id = Guid.NewGuid(),
                    DeviceId = 2,
                    Illum = 85.0,
                    Time = baseDate.AddDays(-3) // 3 days ago
                },
                new Telemetry
                {
                    Id = Guid.NewGuid(),
                    DeviceId = 2,
                    Illum = 95.0,
                    Time = baseDate.AddDays(-30) // 30 days ago
                },
                new Telemetry
                {
                    Id = Guid.NewGuid(),
                    DeviceId = 2,
                    Illum = 105.0,
                    Time = baseDate.AddDays(-15) // 15 days ago
                }
            );

            // Seed clients
            context.Clients.AddRange(
                new Client
                {
                    Id = Guid.NewGuid(),
                    Name = "TestClient1",
                    HashedPassword = "hashedpassword1"
                },
                new Client
                {
                    Id = Guid.NewGuid(),
                    Name = "TestClient2",
                    HashedPassword = "hashedpassword2"
                }
            );

            context.SaveChanges();
        }
    }
}
