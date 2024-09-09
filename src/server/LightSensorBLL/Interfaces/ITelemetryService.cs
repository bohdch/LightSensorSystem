using LightSensorBLL.DTOs;

namespace LightSensorBLL.Interfaces
{
    public interface ITelemetryService
    {
        Task AddTelemetry(int deviceId, IEnumerable<TelemetryDto> telemetryDto, CancellationToken cancellationToken = default);
        Task<IEnumerable<IlluminanceStatisticDto>> GetStatisticsAsync(int deviceId, CancellationToken cancellationToken = default);
    }
}
