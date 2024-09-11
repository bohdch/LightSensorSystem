using System.Diagnostics.CodeAnalysis;

namespace LightSensorBLL.DTOs
{
    [ExcludeFromCodeCoverage]
    public class TelemetryDto
    {
        public double Illum { get; set; }

        public long Time { get; set; }
    }
}
