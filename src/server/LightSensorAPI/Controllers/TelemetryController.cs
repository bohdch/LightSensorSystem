using LightSensorBLL.DTOs;
using LightSensorBLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LightSensorAPI.Controllers
{
    [ApiController]
    [Route("devices/{deviceId}")]
    public class TelemetryController : ControllerBase
    {
        private readonly ITelemetryService _telemetryService;

        public TelemetryController(ITelemetryService telemetryService)
        {
            _telemetryService = telemetryService;
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatisticsAsync([FromRoute] int deviceId, CancellationToken cancellationToken = default)
        {
            var data = await _telemetryService.GetStatisticsAsync(deviceId, cancellationToken);
            return Ok(data);
        }

        [HttpPost("telemetry")]
        public async Task<IActionResult> PostTelemetry([FromRoute] int deviceId, [FromBody] IEnumerable<TelemetryDto> telemetryDto, CancellationToken cancellationToken = default)
        {
            await _telemetryService.AddTelemetry(deviceId ,telemetryDto, cancellationToken);
            return Ok();
        }
    }
}
