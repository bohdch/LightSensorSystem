using AutoMapper;
using LightSensorBLL.DTOs;
using LightSensorBLL.Interfaces;
using LightSensorDAL.Entities;
using LightSensorDAL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LightSensorBLL.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        public ClientService(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClientDto>> GetAllClientsAsync(CancellationToken cancellationToken = default)
        {
            var users = await _clientRepository.GetAllClientsAsync(cancellationToken);

            return _mapper.Map<IEnumerable<ClientDto>>(users);
        }

        public async Task AddClientAsync(NewClient newClient, CancellationToken cancellationToken = default)
        {
            var clientEntity = _mapper.Map<Client>(newClient);

            clientEntity.HashedPassword = HashPassword(clientEntity, newClient.Password);
            clientEntity.Name = newClient.Login;

            await _clientRepository.AddClientAsync(clientEntity, cancellationToken);
        }

        public async Task VerifyPassword(NewClient loginDto, CancellationToken cancellationToken = default)
        {
            var client = await _clientRepository.GetClientByNameAsync(loginDto.Login, cancellationToken);

            if (client is null)
                throw new ArgumentException("Invalid name");

            // Verify password
            var passwordHasher = new PasswordHasher<Client>();

            if (passwordHasher.VerifyHashedPassword(client, client.HashedPassword, loginDto.Password) ==
                PasswordVerificationResult.Failed)
            {
                throw new ArgumentException("Invalid password");
            }
        }


        private string HashPassword(Client client, string password)
        {
            var passwordHasher = new PasswordHasher<Client>();
            return passwordHasher.HashPassword(client, password);
        }
    }
}
