using LightSensorDAL.Data;
using LightSensorDAL.Entities;
using LightSensorDAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ServerTests.DataAccessTests
{
    public class ClientRepositoryTests
    {
        [Fact]
        public async Task GetAllClientsAsync_ReturnsAllClients()
        {
            using var context = new LightSensorDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var repository = new ClientRepository(context);
            var clients = await repository.GetAllClientsAsync();
            Assert.Equal(3, clients.Count());
        }

        [Fact]
        public async Task GetClientByNameAsync_ReturnsCorrectClient()
        {
            using var context = new LightSensorDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var repository = new ClientRepository(context);
            var client = await repository.GetClientByNameAsync("TestClient1");
            Assert.NotNull(client);
            Assert.Equal("TestClient1", client.Name);
        }

        [Fact]
        public async Task AddClientAsync_AddsClientSuccessfully()
        {
            using var context = new LightSensorDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var repository = new ClientRepository(context);
            var newClient = new Client { Id = Guid.NewGuid(), Name = "NewClient", HashedPassword = "newhashedpassword" };
            await repository.AddClientAsync(newClient);

            var client = await context.Clients.FirstOrDefaultAsync(c => c.Name == "NewClient");
            Assert.NotNull(client);
            Assert.Equal("NewClient", client.Name);
        }
    }
}
