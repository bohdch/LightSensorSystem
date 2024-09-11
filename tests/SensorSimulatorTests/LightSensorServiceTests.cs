using Microsoft.Extensions.Configuration;
using LightSensorSimulator.Constants;
using LightSensorSimulator.Models;
using LightSensorSimulator.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace SensorSimulatorTests;

public class LightSensorServiceTests
{
    private Mock<ILogger<LightSensorService>> _mockLogger;
    private Mock<IConfiguration> _mockConfiguration;
    private DeviceConfiguration _deviceConfiguration;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private HttpClient _httpClient;
    private LightSensorService _service;

    public LightSensorServiceTests()
    {
        _mockLogger = new Mock<ILogger<LightSensorService>>();
        _mockConfiguration = new Mock<IConfiguration>();
        _deviceConfiguration = new DeviceConfiguration { MeasurementInterval = 900000, MeasurementsToSend = 5, ServerUrl = "https://localhost:7048/devices/1/telemetry" };
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object) { BaseAddress = new Uri(_deviceConfiguration.ServerUrl) };
        _service = new LightSensorService(_mockLogger.Object, _mockConfiguration.Object, _deviceConfiguration, _httpClient);
    }

    [Theory]
    [MemberData(nameof(GetTimeFractionsForTesting))]
    public void ValidateIlluminanceBehavior(double timeAtSunrise, double midMorning, double timeAtNoon, double midAfternoon, double timeAfterSunset)
    {
        // Arrange
        DateTime testDate = new DateTime(2022, 1, 1);
        var (peakLux, twilightLux) = _service.ReturnDynamicValues(testDate);

        // Act
        double illuminanceAtSunrise = _service.CalculateIlluminance(timeAtSunrise, peakLux, twilightLux);
        double illuminanceMidMorning = _service.CalculateIlluminance(midMorning, peakLux, twilightLux);
        double illuminanceAtNoon = _service.CalculateIlluminance(timeAtNoon, peakLux, twilightLux);
        double illuminanceMidAfternoon = _service.CalculateIlluminance(midAfternoon, peakLux, twilightLux);
        double illuminanceAfterSunset = _service.CalculateIlluminance(timeAfterSunset, peakLux, twilightLux);

        // Assert
        Assert.True(illuminanceAtSunrise < illuminanceMidMorning, "Illuminance mid-morning should be higher than at sunrise");
        Assert.True(illuminanceMidMorning < illuminanceAtNoon, "Illuminance at noon should be the highest during the day");
        Assert.True(illuminanceAtNoon > illuminanceMidAfternoon, "Illuminance mid-afternoon should start to decline from noon");
        Assert.True(illuminanceAfterSunset < illuminanceMidAfternoon, "Illuminance after sunset should be lower than during the day");
    }

    [Theory]
    [InlineData(2024, 1, 1, 2023, 2, 2)]
    public void Illuminance_IsDifferent_OnDifferentDays_AtVariousTimes(int year1, int month1, int day1, int year2, int month2, int day2)
    {
        // Define the times to test: 8 PM (20.0), 4 AM (4.0), 10 AM (10.0), 3 PM (15.0)
        double[] timesToTest = [20.0, 4.0, 10.0, 15.0];

        // Arrange
        var date1 = new DateTime(year1, month1, day1);
        var date2 = new DateTime(year2, month2, day2);

        foreach (var time in timesToTest)
        {
            var (peak1, twilightLux1) = _service.ReturnDynamicValues(date1);
            var (peak2, twilightLux2) = _service.ReturnDynamicValues(date2);

            // Act
            double illuminanceDay1 = _service.CalculateIlluminance(time, peak1, twilightLux1);
            double illuminanceDay2 = _service.CalculateIlluminance(time, peak2, twilightLux2);

            // Assert
            Assert.NotEqual(illuminanceDay1, illuminanceDay2);
        }
    }

    [Fact]
    public void ReturnDynamicValues_ReturnsValuesWithinExpectedRange()
    {
        // Act
        var values = _service.ReturnDynamicValues(new DateTime(2024, 6, 10));

        // Assert
        Assert.InRange(values.Item1, LuxLevels.PeakLuxMin, LuxLevels.PeakLuxMax);
        Assert.InRange(values.Item2, LuxLevels.TwilightLuxMin, LuxLevels.TwilightLuxMax);
    }

    public static IEnumerable<object[]> GetTimeFractionsForTesting()
    {
        double sunrise = SunTimes.Sunrise;
        double sunset = SunTimes.Sunset;
        double noon = (sunrise + sunset) / 2;
        double midMorning = (sunrise + noon) / 2;
        double midAfternoon = (noon + sunset) / 2;

        yield return new object[]
        {
            sunrise - 1,    // Before Sunrise
            midMorning,     // Mid-morning
            noon,           // Noon peak
            midAfternoon,   // Mid-afternoon
            sunset + 1      // After Sunset
        };
    }
}