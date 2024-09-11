using AutoMapper;
using LightSensorBLL.DTOs;
using LightSensorBLL.Interfaces;
using LightSensorDAL.Entities;
using LightSensorDAL.Interfaces;

namespace LightSensorBLL.Services
{
    public class TelemetryService : ITelemetryService
    {
        private readonly ITelemetryRepository _telemetryRepository;
        private readonly IMapper _mapper;

        public TelemetryService(ITelemetryRepository telemetryRepository, IMapper mapper)
        {
            _telemetryRepository = telemetryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<IlluminanceStatisticDto>> GetStatisticsAsync(int deviceId, CancellationToken cancellationToken = default)
        {
            var stats  = await _telemetryRepository.GetStatisticsAsync(deviceId, cancellationToken);
            var mappedStats = stats.Select(_mapper.Map<IlluminanceStatisticDto>);

            return mappedStats;
        }

        public async Task AddTelemetry(int deviceId, IEnumerable<TelemetryDto> telemetryDto, CancellationToken cancellationToken = default)
        {
            var telemetries = telemetryDto.Select(_mapper.Map<Telemetry>).ToList();

            foreach (var telemetry in telemetries)
            {
                telemetry.DeviceId = deviceId;
            }

            await _telemetryRepository.AddTelemetry(telemetries, cancellationToken);
        }
    }
}
