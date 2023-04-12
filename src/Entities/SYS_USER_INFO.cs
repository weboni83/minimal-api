using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MinimalAPIsDemo.Entities
{
    [Table("SYS_USER_INFO")]
    public class SYS_USER_INFO
    {
        [JsonPropertyName("plant_cd")]
        public string PLANT_CD { get; set; }
        [JsonPropertyName("user_id")]
        public string USER_ID { get; set; }
        [JsonPropertyName("user_name")]
        public string USER_NAME { get; set; }
        public string USER_PASSWORD { get; set; }
        public string USER_USE_YN { get; set; }
        public int? USER_FAILED_NUM { get; set; }
        public string USER_START_DATE { get; set; }
        public string USER_END_DATE { get; set; }
        public string NOW_USER_ID { get; set; }
        public string NEW_USER { get; set; }
        public int? USER_SECURITY_LEVEL { get; set; }
        public string EMPLOYEE_CD { get; set; }
        public string INSERT_USER_ID { get; set; }
        public DateTime? INSERT_TIME { get; set; }
        public string UPDATE_USER_ID { get; set; }
        public DateTime? UPDATE_TIME { get; set; }
        public int AUDITTRAIL_ID { get; set; }
        public string AUDITTRAIL_REMARK { get; set; }
        public string AUDITTRAIL_STATUS { get; set; }
        [JsonPropertyName("device_token")]
        public string DEVICE_TOKEN { get; set; }
    }


}
