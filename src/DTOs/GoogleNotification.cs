using System.Text.Json.Serialization;

namespace MinimalAPIsDemo.DTOs
{
    public class GoogleNotification
    {
        public class DataPayload
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }
            [JsonPropertyName("body")]
            public string Body { get; set; }
        }
        [JsonPropertyName("priority")]
        public string Priority { get; set; } = "high";
        [JsonPropertyName("data")]
        public DataPayload Data { get; set; }
        [JsonPropertyName("notification")]
        public DataPayload Notification { get; set; }
    }
}
