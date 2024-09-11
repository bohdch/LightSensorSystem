using LightSensorBLL.DTOs;

namespace LightSensorBLL.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> GetAuthToken(NewClient loginDto, CancellationToken cancellationToken = default);
    }
}
