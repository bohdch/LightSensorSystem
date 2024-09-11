using System.Text.Json.Serialization;

namespace LightSensorSimulator.Models
{
    public class TokenResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
