using System.Text.Json.Serialization;

namespace MinimalAPIsDemo.DTOs
{
    public class AlarmUpdateDTO
    {
        [JsonPropertyName("read_yn")]
        public string READ_YN { get; set; }
    }
}
