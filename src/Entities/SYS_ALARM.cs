using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MinimalAPIsDemo.Entities
{
    [Table("SYS_ALARM")]
    public class SYS_ALARM
    {
        [JsonPropertyName("plant_cd")]
        public string PLANT_CD { get; set; }
        [JsonPropertyName("alarm_id")]
        public long ALARM_ID { get; set; }
        [JsonPropertyName("alarm_content")]
        public string ALARM_CONTENT { get; set; }
        [JsonPropertyName("receive_emp_cd")]
        public string RECEIVE_EMP_CD { get; set; }
        [JsonPropertyName("read_yn")]
        public string READ_YN { get; set; }
        [JsonPropertyName("read_time")]
        public DateTime? READ_TIME { get; set; }
        public string REMARK { get; set; }
        public string FORM_NAME { get; set; }
        public string REF_KEY1 { get; set; }
        public string REF_KEY2 { get; set; }
        public string REF_KEY3 { get; set; }
        public string REF_KEY4 { get; set; }
        public string REF_KEY5 { get; set; }
        public string ALIMTALK_YN { get; set; }
        public string INSERT_USER_ID { get; set; }
        public DateTime? INSERT_TIME { get; set; }
        public string UPDATE_USER_ID { get; set; }
        public DateTime? UPDATE_TIME { get; set; }
        public string AUDITTRAIL_REMARK { get; set; }
        public string AUDITTRAIL_STATUS { get; set; }
        public int AUDITTRAIL_ID { get; set; }
        public string REF_KEY6 { get; set; }
        public string REF_KEY7 { get; set; }
        public string SIGN_ENTITY { get; set; }
        public string SIGN_ENTITY_STATUS { get; set; }
    }

}
