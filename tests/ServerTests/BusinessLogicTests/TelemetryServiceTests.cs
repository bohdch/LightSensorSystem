using AutoMapper;
using FluentAssertions;
using LightSensorBLL.DTOs;
using LightSensorBLL.Services;
using LightSensorDAL.Entities;
using LightSensorDAL.Interfaces;
using Moq;

namespace ServerTests.BusinessLogicTests;

public class TelemetryServiceTests
{
    private Mock<ITelemetryRepository> _telemetryRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private TelemetryService _telemetryService;

    public TelemetryServiceTests()
    {
        _telemetryRepositoryMock = new Mock<ITelemetryRepository>();
        _mapperMock = new Mock<IMapper>();
        _telemetryService = new TelemetryService(_telemetryRepositoryMock.Object, _mapperMock.Object);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetStatisticsAsync_ReturnsMappedStatistics(int deviceId)
    {
        // Arrange
        var telemetryStats = new List<Telemetry>
        {
            new() { Id = Guid.NewGuid(), DeviceId = deviceId, Illum = 1500.0, Time = DateTime.UtcNow.AddDays(-1) },
            new() { Id = Guid.NewGuid(), DeviceId = deviceId, Illum = 2000.0, Time = DateTime.UtcNow.AddDays(-2) }
        };

        var telemetryStatsDto = new List<IlluminanceStatisticDto>
        {
            new() { Date = telemetryStats[0].Time.Date, MaxIlluminance = telemetryStats[0].Illum },
            new() { Date = telemetryStats[1].Time.Date, MaxIlluminance = telemetryStats[1].Illum }
        };

        _telemetryRepositoryMock.Setup(r => r.GetStatisticsAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(telemetryStats);

        _mapperMock.Setup(m => m.Map<IlluminanceStatisticDto>(It.IsAny<Telemetry>()))
            .Returns<Telemetry>(t => new IlluminanceStatisticDto { Date = t.Time.Date, MaxIlluminance = t.Illum });

        // Act
        var result = await _telemetryService.GetStatisticsAsync(deviceId, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeEquivalentTo(telemetryStatsDto);
        _telemetryRepositoryMock.Verify(r => r.GetStatisticsAsync(deviceId, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<IlluminanceStatisticDto>(It.IsAny<Telemetry>()), Times.Exactly(telemetryStats.Count));
    }

    [Theory]
    [InlineData(3)]
    public async Task AddTelemetry_AddsDataCorrectly(int deviceId)
    {
        // Arrange
        var telemetryDtos = new List<TelemetryDto>
        {
            new() { Illum = 75.5, Time = DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeMilliseconds() },
            new() { Illum = 100.5, Time = DateTimeOffset.UtcNow.AddMinutes(-45).ToUnixTimeMilliseconds() },
            new() { Illum = 150.2, Time = DateTimeOffset.UtcNow.AddDays(-30).ToUnixTimeMilliseconds() },
            new() { Illum = 250.3, Time = DateTimeOffset.UtcNow.AddDays(-15).ToUnixTimeMilliseconds() },
            new() { Illum = 333.4, Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }
        };

        var telemetries = new List<Telemetry>
        {
            new() {Id = Guid.NewGuid(), DeviceId = deviceId, Illum = telemetryDtos[0].Illum, Time = DateTimeOffset.FromUnixTimeMilliseconds(telemetryDtos[0].Time).UtcDateTime },
            new() {Id = Guid.NewGuid(), DeviceId = deviceId, Illum = telemetryDtos[1].Illum, Time = DateTimeOffset.FromUnixTimeMilliseconds(telemetryDtos[1].Time).UtcDateTime },
            new() {Id = Guid.NewGuid(), DeviceId = deviceId, Illum = telemetryDtos[2].Illum, Time = DateTimeOffset.FromUnixTimeMilliseconds(telemetryDtos[2].Time).UtcDateTime },
            new() {Id = Guid.NewGuid(), DeviceId = deviceId, Illum = telemetryDtos[3].Illum, Time = DateTimeOffset.FromUnixTimeMilliseconds(telemetryDtos[3].Time).UtcDateTime },
            new() {Id = Guid.NewGuid(), DeviceId = deviceId, Illum = telemetryDtos[4].Illum, Time = DateTimeOffset.FromUnixTimeMilliseconds(telemetryDtos[4].Time).UtcDateTime }
        };

        _mapperMock.Setup(m => m.Map<Telemetry>(It.IsAny<TelemetryDto>()))
            .Returns<TelemetryDto>(dto => new Telemetry { DeviceId = deviceId, Illum = dto.Illum });

        _telemetryRepositoryMock.Setup(r => r.AddTelemetry(It.IsAny<IEnumerable<Telemetry>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _telemetryService.AddTelemetry(deviceId, telemetryDtos, It.IsAny<CancellationToken>());

        // Assert
        telemetries.All(t => t.DeviceId == deviceId).Should().BeTrue("All telemetry entries should have the correct DeviceId");
        telemetryDtos.ForEach(dto => _mapperMock.Verify(m => m.Map<Telemetry>(dto), Times.Once));
        _telemetryRepositoryMock.Verify(
            r => r.AddTelemetry(It.Is<IEnumerable<Telemetry>>(t => t.All(te => te.DeviceId == deviceId)),
                It.IsAny<CancellationToken>()), Times.Once);
    }
}
