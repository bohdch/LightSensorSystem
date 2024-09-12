using LightSensorBLL.DTOs;

namespace LightSensorBLL.Interfaces
{
    public interface IJwtService
    {
        Task<string> GetAuthToken(NewClient loginDto, CancellationToken cancellationToken = default);
    }
}
