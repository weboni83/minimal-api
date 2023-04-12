using System.Text.Json.Serialization;

namespace MinimalAPIsDemo.DTOs
{
    public class UserTokenDTO
    {
        [JsonPropertyName("device_token")]
        public string DEVICE_TOKEN { get; set; }
    }
}
