using System.Diagnostics.CodeAnalysis;

namespace LightSensorSimulator.Models
{
    [ExcludeFromCodeCoverage]
    public class Measurement
    {
        public double Illuminance { get; set; }
        public long Timestamp { get; set; }
    }
}
