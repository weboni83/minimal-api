using System.Text.Json.Serialization;

namespace MinimalAPIsDemo.DTOs
{
    public class ResponseDTO
    {
        [JsonPropertyName("is_success")]
        public bool IsSuccess { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
