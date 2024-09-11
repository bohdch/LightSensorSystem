using System.Diagnostics.CodeAnalysis;

namespace LightSensorBLL.DTOs
{
    [ExcludeFromCodeCoverage]
    public class ClientDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
