using LightSensorDAL.Data;
using LightSensorDAL.Entities;
using LightSensorDAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LightSensorDAL.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly LightSensorDbContext _lightSensorContext;

        public ClientRepository(LightSensorDbContext lightSensorContext)
        {
            _lightSensorContext = lightSensorContext;
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync(CancellationToken cancellationToken = default)
        {
            return await _lightSensorContext.Clients
                .ToListAsync(cancellationToken);
        }

        public Task<Client> GetClientByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return _lightSensorContext.Clients
                .FirstOrDefaultAsync(u => u.Name == name, cancellationToken);
        }

        public async Task AddClientAsync(Client entity, CancellationToken cancellationToken = default)
        {
            await _lightSensorContext.Clients.AddAsync(entity, cancellationToken);
            await _lightSensorContext.SaveChangesAsync(cancellationToken);
        }
    }
}
