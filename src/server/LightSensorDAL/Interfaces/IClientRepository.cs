using LightSensorDAL.Entities;

namespace LightSensorDAL.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllClientsAsync(CancellationToken cancellationToken = default);
        Task<Client> GetClientByNameAsync(string name, CancellationToken cancellationToken = default);
        Task AddClientAsync(Client entity, CancellationToken cancellationToken = default);
    }
}
