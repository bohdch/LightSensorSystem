using System.Diagnostics.CodeAnalysis;

namespace LightSensorBLL.DTOs
{
    [ExcludeFromCodeCoverage]
    public class NewClient
    {
        public string Login { get; set; } // user name
        public string Password { get; set; }
    }
}
