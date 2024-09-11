using LightSensorBLL.DTOs;
using LightSensorBLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LightSensorAPI.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> PostTelemetriesAsync([FromRoute] int deviceId, [FromBody] IEnumerable<TelemetryDto> telemetryDtos, CancellationToken cancellationToken = default)
        {
            await _telemetryService.AddTelemetriesAsync(deviceId ,telemetryDtos, cancellationToken);
            return Ok();
        }
    }
}
