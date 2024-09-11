﻿using LightSensorDAL.Entities;

namespace LightSensorDAL.Interfaces
{
    public interface ITelemetryRepository
    {
        Task<IEnumerable<Telemetry>> GetStatisticsAsync(int deviceId, CancellationToken cancellationToken = default);
        Task AddTelemetry(IEnumerable<Telemetry> telemetry, CancellationToken cancellationToken = default);
    }
}