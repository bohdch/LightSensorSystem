using System.Diagnostics.CodeAnalysis;

namespace LightSensorSimulator.Constants
{
    [ExcludeFromCodeCoverage]
    public static class LuxLevels
    {
        public const double NightLux = 0.0; // Minimal lux level at deep night
        public const double TwilightLuxMin = 350.0; // ~Min Lux level at sunrise and sunset on a clear sunny day
        public const double TwilightLuxMax = 400.0; // ~Max Lux level at sunrise and sunset on a clear sunny day
        public const double PeakLuxMin = 10000.0; // ~Minimum peak lux level on a clear sunny day
        public const double PeakLuxMax = 20000.0; // ~Maximum peak lux level on a clear sunny day
    }
}
