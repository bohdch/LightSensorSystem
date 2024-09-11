using LightSensorBLL.DTOs;

namespace LightSensorBLL.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetAllClientsAsync(CancellationToken cancellationToken = default);
        Task AddClientAsync(NewClient newClient, CancellationToken cancellationToken = default);
        Task VerifyPassword(NewClient loginDto, CancellationToken cancellationToken = default);
    }
}
