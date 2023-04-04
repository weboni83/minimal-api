using System.Text.Json.Serialization;

namespace MinimalAPIsDemo.DTOs
{
    public class UserDTO
{
        [JsonPropertyName("plant_cd")]
        public string PLANT_CD { get; set; }
        [JsonPropertyName("user_id")]
        public string USER_ID { get; set; }
        [JsonPropertyName("user_name")]
        public string USER_NAME { get; set; }
    }
}
