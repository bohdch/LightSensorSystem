using LightSensorDAL.Entities;

namespace LightSensorDAL.Interfaces
{
    public interface ITelemetryRepository
    {
        Task<IEnumerable<Telemetry>> GetStatisticsAsync(int deviceId, CancellationToken cancellationToken = default);
        Task AddTelemetriesAsync(IEnumerable<Telemetry> telemetry, CancellationToken cancellationToken = default);
    }
}
