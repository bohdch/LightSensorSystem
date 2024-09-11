using AutoMapper;
using LightSensorBLL.DTOs;
using LightSensorBLL.Services;
using LightSensorDAL.Entities;
using LightSensorDAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ServerTests.BusinessLogicTests
{
    public class ClientServiceTests
    {
        private Mock<IClientRepository> _clientRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private ClientService _clientService;

        public ClientServiceTests()
        {
            _clientRepositoryMock = new Mock<IClientRepository>();
            _mapperMock = new Mock<IMapper>();
            _clientService = new ClientService(_clientRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllClientsAsync_ReturnsAllClientsMapped()
        {
            // Arrange
            var clients = new List<Client>
            {
                new() { Id = Guid.NewGuid(), Name = "User1", HashedPassword = "Password1" },
                new() { Id = Guid.NewGuid(), Name = "User2", HashedPassword = "Password2" }
            };
            var clientDtos = clients.Select(c => new ClientDto { Name = c.Name }).ToList();

            _clientRepositoryMock.Setup(repo => repo.GetAllClientsAsync(default))
                .ReturnsAsync(clients);
            _mapperMock.Setup(m => m.Map<IEnumerable<ClientDto>>(clients))
                .Returns(clientDtos);

            // Act
            var result = await _clientService.GetAllClientsAsync();

            // Assert
            Assert.Equal(clientDtos.Count, result.Count());
            _mapperMock.Verify(m => m.Map<IEnumerable<ClientDto>>(clients), Times.Once);
        }

        [Fact]
        public async Task AddClientAsync_ThrowsException_WhenClientExists()
        {
            // Arrange
            var newClient = new NewClient { Login = "ExistingUser", Password = "Password123" };
            var client = new Client { Name = newClient.Login, HashedPassword = "ExistingHash" };

            _clientRepositoryMock.Setup(repo => repo.GetClientByNameAsync(newClient.Login, default))
                .ReturnsAsync(client);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _clientService.AddClientAsync(newClient));
        }

        [Fact]
        public async Task VerifyPassword_ThrowsArgumentException_WhenInvalidName()
        {
            // Arrange
            var loginDto = new NewClient { Login = "NonExistentUser", Password = "Password123" };

            _clientRepositoryMock.Setup(repo => repo.GetClientByNameAsync(loginDto.Login, default))
                .ReturnsAsync((Client)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _clientService.VerifyPassword(loginDto));
        }

        [Fact]
        public async Task VerifyPassword_ThrowsArgumentException_WhenInvalidPassword()
        {
            // Arrange
            var client = new Client { Name = "User", HashedPassword = "HashedPass" };
            var loginDto = new NewClient { Login = "User", Password = "WrongPassword" };

            _clientRepositoryMock.Setup(repo => repo.GetClientByNameAsync(loginDto.Login, default))
                .ReturnsAsync(client);
            var passwordHasher = new PasswordHasher<Client>();
            var hashedPassword = passwordHasher.HashPassword(client, "CorrectPassword");
            client.HashedPassword = hashedPassword;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _clientService.VerifyPassword(loginDto));
        }
    }
}
