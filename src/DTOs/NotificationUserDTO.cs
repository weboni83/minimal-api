using System.Text.Json.Serialization;

namespace MinimalAPIsDemo.DTOs
{
    public class NotificationUserDTO
    { 
        [JsonPropertyName("sender_id")]
        public string SenderUserId { get; set; }
        [JsonPropertyName("data")]
        public Dictionary<string, string> Data { get; set; }
    }
}
