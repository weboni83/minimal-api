using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace MinimalAPIsDemo.DTOs
{
    public class NotificationDTO
    {
        [JsonPropertyName("device_id")]
        public string DeviceId { get; set; }
        [JsonPropertyName("is_androiod_device")]
        public bool IsAndroiodDevice { get; set; }
        [JsonPropertyName("data")]
        public Dictionary<string, string> Data { get; set; }
    }
}
