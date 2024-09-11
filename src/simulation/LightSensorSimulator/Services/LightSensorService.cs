using System.Text;
using System.Text.Json;
using LightSensorSimulator.Constants;
using LightSensorSimulator.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace LightSensorSimulator.Services
{
    public class LightSensorService
    {
        private readonly ILogger<LightSensorService> _logger;
        private readonly IConfiguration _configuration;
        private readonly DeviceConfiguration _deviceConfiguration;
        private readonly HttpClient _httpClient;
        private List<Measurement> _measurements;
        private Timer _measurementTimer;

        public LightSensorService(ILogger<LightSensorService> logger, IConfiguration configuration,
            DeviceConfiguration deviceConfiguration, HttpClient httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _deviceConfiguration = deviceConfiguration;
            _httpClient = httpClient;

            _measurements = new List<Measurement>();
            _measurementTimer = new Timer(_deviceConfiguration.MeasurementInterval);
            _measurementTimer.Elapsed += async (_, _) => await TakeMeasurementAsync();

            // Start the timer
            _measurementTimer.AutoReset = true;
            _measurementTimer.Enabled = true;

            _logger.LogInformation("Measurement timer started with an interval of {Interval} minutes", _measurementTimer.Interval / 60000);

            TakeMeasurementAsync().Wait(); // Take the first measurement immediately
        }

        public async Task TakeMeasurementAsync()
        {
            var currentDate = DateTime.UtcNow.Date;
            var currentDateTime = DateTime.UtcNow;

            // Normalize time into a range 0 - 24
            var timeFraction = currentDateTime.Hour + (currentDateTime.Minute / 60.0);

            var (peakLux, twilightLux) = ReturnDynamicValues(currentDate);
            double illuminance = CalculateIlluminance(timeFraction, peakLux, twilightLux);
            illuminance = Math.Round(illuminance * 2) / 2.0;  // 0.5 Lux resolution
            
            var measurement = new Measurement
            {
                Illuminance = illuminance,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            _measurements.Add(measurement);
            _logger.LogInformation("Measurement taken at {Timestamp}: {Illuminance} Lux", currentDateTime, illuminance);

            if (_measurements.Count >= _deviceConfiguration.MeasurementsToSend)
            {
                await SendDataToServer(_measurements);
                _measurements.Clear();
                _logger.LogInformation("Data batch sent successfully");
            }
        }

        public async Task SendDataToServer(IEnumerable<Measurement> measurements)
        {
            try
            {
                var apiToken = await GetApiTokenAsync();
                if (string.IsNullOrEmpty(apiToken))
                {
                    _logger.LogError("No API token available. Cannot send data.");
                    return;
                }

                var serializedMeasurements = JsonSerializer.Serialize(measurements.Select(m => new { illum = m.Illuminance, time = m.Timestamp }));
                var content = new StringContent(serializedMeasurements, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);

                await _httpClient.PostAsync(_deviceConfiguration.ServerUrl, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending data to the server.");
            }
        }

        public double CalculateIlluminance(double timeFraction, double peakLux, double twilightLux)
        {
            double illuminance;
            double noonTime = (SunTimes.Sunrise + SunTimes.Sunset) / 2;

            if (timeFraction >= SunTimes.Sunrise && timeFraction <= SunTimes.Sunset)
            {
                // Daylight: Calculate illuminance between sunrise and sunset
                double mu = CalculateProportionalDistance(timeFraction, SunTimes.Sunrise,
                    timeFraction <= noonTime ? noonTime : SunTimes.Sunset);

                illuminance = timeFraction <= noonTime
                    ? ExponentialInterpolate(twilightLux, peakLux, mu)
                    : ExponentialInterpolate(peakLux, twilightLux, (timeFraction - noonTime) / (SunTimes.Sunset - noonTime));
            }
            else if (timeFraction > SunTimes.Sunset)
            {
                // Post-sunset: Calculate illuminance after sunset towards night
                double mu = CalculateProportionalDistance(timeFraction, SunTimes.Sunset, 24.0);
                illuminance = ExponentialInterpolateAggressive(twilightLux, LuxLevels.NightLux, mu);
            }
            else
            {
                // Pre-sunrise: Calculate illuminance before sunrise
                double mu = CalculateProportionalDistance(timeFraction, 0.0, SunTimes.Sunrise);
                illuminance = ExponentialInterpolateAggressive(LuxLevels.NightLux, twilightLux, mu);
            }

            return illuminance;
        }

        public (double, double) ReturnDynamicValues(DateTime currentDate)
        {
            double peakLux = LuxLevels.PeakLuxMin + new Random(currentDate.DayOfYear).Next(0, (int)(LuxLevels.PeakLuxMax - LuxLevels.PeakLuxMin + 1));
            double baseLux = LuxLevels.TwilightLuxMin + new Random(currentDate.DayOfYear).Next(0, (int)(LuxLevels.TwilightLuxMax - LuxLevels.TwilightLuxMin + 1));

            return (peakLux, baseLux);
        }

        // Computes the proportion of the distance covered from periodStart to periodEnd
        private double CalculateProportionalDistance(double timeFraction, double periodStart, double periodEnd)
        {
            return (timeFraction - periodStart) / (periodEnd - periodStart);
        }

        // Gradual transition between y1 and y2
        private double ExponentialInterpolate(double y1, double y2, double mu)
        {
            (y1, y2) = ValidateInputs(y1, y2);
            return y1 * Math.Pow(y2 / y1, mu);
        }

        // Rapid transition between y1 and y2 using a more aggressive formula
        private double ExponentialInterpolateAggressive(double y1, double y2, double mu)
        {
            (y1, y2) = ValidateInputs(y1, y2);
            return y1 * Math.Pow(y2 / y1, Math.Pow(mu, Math.E));
        }

        private (double, double) ValidateInputs(double y1, double y2)
        {
            y1 = y1 <= 0 ? 0.1 : y1;
            y2 = y2 <= 0 ? 0.1 : y2;

            return (y1, y2);
        }

        private async Task<string> GetApiTokenAsync()
        {
            try
            {
                var loginRequest = new
                {
                    Login = _configuration["Auth:BaseClient"],
                    Password = _configuration["Auth:Password"]
                };

                var json = JsonSerializer.Serialize(loginRequest);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var loginResponse = await _httpClient.PostAsync(_configuration["Auth:LoginUrl"], data);
                if (loginResponse.IsSuccessStatusCode)
                {
                    var loginContent = await loginResponse.Content.ReadAsStringAsync();
                    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(loginContent);
                    return tokenResponse?.Token;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve API token due to an exception.");
            }
            return null;
        }
    }
}
