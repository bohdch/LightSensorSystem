using System.Diagnostics.CodeAnalysis;

namespace LightSensorDAL.Entities
{
    [ExcludeFromCodeCoverage]
    public class Telemetry
    {
        public Guid Id { get; set; }
        public int DeviceId { get; set; }
        public double Illum { get; set; }
        public DateTime Time { get; set; }
    }
}
