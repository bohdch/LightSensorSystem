using LightSensorDAL.Data;
using LightSensorDAL.Entities;
using LightSensorDAL.Repositories;

namespace ServerTests.DataAccessTests;

public class TelemetryRepositoryTests
{
    [Theory]
    [InlineData(1)]
    public async Task GetStatisticsAsync_ReturnsAggregatedData(int deviceId)
    {
        // Arrange
        using var context = new LightSensorDbContext(UnitTestHelper.GetUnitTestDbOptions());
        var repository = new TelemetryRepository(context);
        var specificDate = DateTime.UtcNow.AddDays(-2).Date; // 2 days ago

        // Act
        var statistics = await repository.GetStatisticsAsync(deviceId);

        // Assert
        var telemetryForSpecificDate = statistics.FirstOrDefault(t => t.Time.Date == specificDate);

        Assert.NotNull(statistics);
        Assert.Equal(200.0, telemetryForSpecificDate.Illum); // Maximum illumination from 2 days ago
        Assert.Equal(context.Telemetries.Count(t => t.DeviceId == deviceId && t.Time > DateTime.Now.AddDays(-30)),
            statistics.Count()); // 4 days of data within the last 30 days
    }

    [Theory]
    [InlineData(3)]
    public async Task AddTelemetry_AddsDataCorrectly(int deviceId)
    {
        // Arrange
        using var context = new LightSensorDbContext(UnitTestHelper.GetUnitTestDbOptions());
        var repository = new TelemetryRepository(context);
        var newTelemetry = new List<Telemetry>
        {
            new() { Id = Guid.NewGuid(), DeviceId = deviceId, Illum = 100.4, Time = DateTime.UtcNow.AddHours(-1) },
            new() { Id = Guid.NewGuid(), DeviceId = deviceId, Illum = 123.4, Time = DateTime.UtcNow.AddMinutes(-45) },
            new() { Id = Guid.NewGuid(), DeviceId = deviceId, Illum = 150.0, Time = DateTime.UtcNow.AddMinutes(-30) },
            new() { Id = Guid.NewGuid(), DeviceId = deviceId, Illum = 200.0, Time = DateTime.UtcNow.AddMinutes(-15) },
            new() { Id = Guid.NewGuid(), DeviceId = deviceId, Illum = 250.0, Time = DateTime.UtcNow }
        };

        // Act
        await repository.AddTelemetry(newTelemetry);

        // Assert
        var allItemsExist = newTelemetry.All(item => context.Telemetries.Contains(item));

        Assert.Equal(15, context.Telemetries.Count()); // Including the seeded data
        Assert.True(allItemsExist);
    }
}