using System.Text.Json.Serialization;

namespace MinimalAPIsDemo.DTOs
{
    public class AlarmDTO
    {
        [JsonPropertyName("plant_cd")]
        public string PLANT_CD { get; set; }
        [JsonPropertyName("alarm_id")]
        public long ALARM_ID { get; set; }
        [JsonPropertyName("alarm_title")]
        public string ALARM_TITLE { get; set; }
        [JsonPropertyName("alarm_content")]
        public string ALARM_CONTENT { get; set; }
        [JsonPropertyName("receive_emp_cd")]
        public string RECEIVE_EMP_CD { get; set; }
        [JsonPropertyName("read_yn")]
        public string READ_YN { get; set; }
        [JsonPropertyName("read_time")]
        public DateTime? READ_TIME { get; set; }
    }
}
