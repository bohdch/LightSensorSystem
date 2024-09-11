using LightSensorDAL.Data;
using LightSensorDAL.Entities;
using LightSensorDAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LightSensorDAL.Repositories
{
    public class TelemetryRepository : ITelemetryRepository
    {
        private readonly LightSensorDbContext _lightSensorContext;

        public TelemetryRepository(LightSensorDbContext lightSensorContext)
        {
            _lightSensorContext = lightSensorContext;
        }

        public async Task<IEnumerable<Telemetry>> GetStatisticsAsync(int deviceId, CancellationToken cancellationToken = default)
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-30);

            return await _lightSensorContext.Telemetries
                .Where(t => t.DeviceId == deviceId && t.Time >= startDate)
                .GroupBy(t => t.Time.Date)
                .Select(grouped => new Telemetry
                {
                    Time = grouped.Key,
                    Illum = grouped.Max(t => t.Illum),
                    DeviceId = deviceId
                })
                .OrderBy(t => t.Time)
                .ToListAsync(cancellationToken);
        }

        public async Task AddTelemetry(IEnumerable<Telemetry> telemetry, CancellationToken cancellationToken = default)
        {
            await _lightSensorContext.Telemetries.AddRangeAsync(telemetry, cancellationToken);
            await _lightSensorContext.SaveChangesAsync(cancellationToken);
        }
    }
}
