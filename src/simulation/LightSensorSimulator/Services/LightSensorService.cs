using System.Text;
using System.Text.Json;
using LightSensorSimulator.Constants;
using LightSensorSimulator.Models;
using Timer = System.Timers.Timer;

namespace LightSensorSimulator.Services
{
    public class LightSensorService
    {
        private readonly DeviceConfiguration _deviceConfiguration;
        private readonly HttpClient _httpClient;
        private List<Measurement> _measurements;
        private Timer _measurementTimer;

        public LightSensorService(DeviceConfiguration deviceConfiguration ,HttpClient httpClient)
        {
            _deviceConfiguration = deviceConfiguration;
            _httpClient = httpClient;
            _measurements = new List<Measurement>();

            _measurementTimer = new Timer(_deviceConfiguration.MeasurementInterval);

            _measurementTimer.Elapsed += (sender, e) =>
            {
                TakeMeasurement();
            };

            _measurementTimer.Start();
        }

        private void TakeMeasurement()
        {
            var currentDate = DateTime.UtcNow.Date;
            var currentDateTime = DateTime.UtcNow;

            //Normalizes time into a range 0 - 24
            var timeFraction = currentDateTime.Hour + (currentDateTime.Minute / 60.0);  

            double peakLux = GetRandomPeakLux(currentDate);
            double illuminance = CalculateIlluminance(timeFraction, peakLux);
            illuminance = Math.Round(illuminance * 2) / 2.0;  // 0.5 Lux resolution
            
            var measurement = new Measurement
            {
                Illuminance = illuminance,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            _measurements.Add(measurement);

            if (_measurements.Count >= _deviceConfiguration.MeasurementsToSend)
            {
                SendDataToServer(_measurements).Wait();
                _measurements.Clear();
            }
        }

        private double GetRandomPeakLux(DateTime currentDate)
        {
            return LuxLevels.PeakLuxMin + new Random(currentDate.DayOfYear).Next(0, (int)(LuxLevels.PeakLuxMax - LuxLevels.PeakLuxMin + 1));
        }

        private double CalculateIlluminance(double timeFraction, double peakLux)
        {
            double illuminance;
            double noonTime = (SunTimes.Sunrise + SunTimes.Sunset) / 2;

            if (timeFraction >= SunTimes.Sunrise && timeFraction <= SunTimes.Sunset)
            {
                // Daylight: Calculate illuminance between sunrise and sunset
                double mu = (timeFraction - SunTimes.Sunrise) / (noonTime - SunTimes.Sunrise);
                illuminance = timeFraction <= noonTime
                    ? ExponentialInterpolate(LuxLevels.BaseLux, peakLux, mu)
                    : ExponentialInterpolate(peakLux, LuxLevels.BaseLux, (timeFraction - noonTime) / (SunTimes.Sunset - noonTime));
            }
            else if (timeFraction > SunTimes.Sunset)
            {
                // Post-sunset: Calculate illuminance after sunset towards night
                double mu = (timeFraction - SunTimes.Sunset) / (24 - SunTimes.Sunset);
                illuminance = ExponentialInterpolateAggressive(LuxLevels.BaseLux, LuxLevels.NightLux, mu);
            }
            else
            {
                // Pre-sunrise: Calculate illuminance before sunrise
                double mu = timeFraction / SunTimes.Sunrise;
                illuminance = ExponentialInterpolateAggressive(LuxLevels.NightLux, LuxLevels.BaseLux, mu);
            }

            return illuminance;
        }

        // Smoothly transition between y1 and y2
        private double ExponentialInterpolate(double y1, double y2, double mu)
        {
            (y1, y2) = ValidateInputs(y1, y2);
            return y1 * Math.Pow(y2 / y1, mu);
        }

        // Smoothly transition between y1 and y2 using a more aggressive formula
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

        private async Task SendDataToServer(IEnumerable<Measurement> measurements)
        {
            var serializedMeasurements = JsonSerializer.Serialize(measurements.Select(m => new { illum = m.Illuminance, time = m.Timestamp }));
            var content = new StringContent(serializedMeasurements, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_deviceConfiguration.ServerUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine($"Response Code: {response.StatusCode}, Response Content: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Error sending data to API");
            }
        }
    }
}
