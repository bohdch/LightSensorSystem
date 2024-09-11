using LightSensorBLL.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;
using LightSensorBLL.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LightSensorBLL.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IClientService _clientService;
        private readonly IConfiguration _configuration;

        public JwtTokenService(IClientService clientService, IConfiguration configuration)
        {
            _clientService = clientService;
            _configuration = configuration;
        }

        public async Task<string> GetAuthToken(NewClient loginDto, CancellationToken cancellationToken = default)
        {
            await _clientService.VerifyPassword(loginDto, cancellationToken);

            return GenerateToken(loginDto.Login);
        }


        private string GenerateToken(string clientName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var (key, issuer, audience) = GetTokenParts();

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, clientName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private (byte[] SecretKey, string Issuer, string Audience) GetTokenParts()
        {
            var secretKey = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            return (secretKey, issuer, audience);
        }
    }
}
