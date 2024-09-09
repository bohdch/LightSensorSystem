namespace LightSensorSimulator.Constants
{
    public static class LuxLevels
    {
        public const double NightLux = 0.0; // Minimal lux level at deep night
        public const double BaseLux = 350.0; // ~Lux level at sunrise and sunset on a clear sunny day
        public const double PeakLuxMin = 10000.0; // ~Minimum peak lux level on a clear sunny day
        public const double PeakLuxMax = 20000.0; // ~Maximum peak lux level on a clear sunny day
    }
}
