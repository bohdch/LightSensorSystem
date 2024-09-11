using System.Diagnostics.CodeAnalysis;

namespace LightSensorBLL.DTOs
{
    [ExcludeFromCodeCoverage]
    public class IlluminanceStatisticDto
    {
        public DateTime Date { get; set; }
        public double MaxIlluminance { get; set; }
    }
}
