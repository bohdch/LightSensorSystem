using LightSensorBLL.DTOs;

namespace LightSensorBLL.Interfaces
{
    public interface ITelemetryService
    {
        Task<IEnumerable<IlluminanceStatisticDto>> GetStatisticsAsync(int deviceId, CancellationToken cancellationToken = default);
        Task AddTelemetriesAsync(int deviceId, IEnumerable<TelemetryDto> telemetryDtos, CancellationToken cancellationToken = default);
    }
}
