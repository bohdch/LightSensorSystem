using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace LightSensorSimulator.Models
{
    [ExcludeFromCodeCoverage]
    public class TokenResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
