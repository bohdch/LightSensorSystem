using LightSensorBLL.DTOs;
using LightSensorBLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LightSensorAPI.Controllers
{
    [ApiController]
    [Route("clients")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IJwtService _jwtService;

        public ClientController(IClientService clientService, IJwtService jwtService)
        {
            _clientService = clientService;
            _jwtService = jwtService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllClients(CancellationToken cancellationToken = default)
        {
            var clients = await _clientService.GetAllClientsAsync(cancellationToken);
            return Ok(clients);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginClient([FromBody] NewClient loginDto, CancellationToken cancellationToken = default)
        {
            return Ok(new { Token = await _jwtService.GetAuthToken(loginDto, cancellationToken) });
        }

        [HttpPost("new")]
        public async Task<IActionResult> AddClient([FromBody] NewClient newClient, CancellationToken cancellationToken = default)
        {
            await _clientService.AddClientAsync(newClient, cancellationToken);
            return Ok();
        }
    }
}