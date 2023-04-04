using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalAPIsDemo.Entities
{
    [Table("CM_COMMON")]
    public class CM_COMMON
    {
        public string PLANT_CD { get; set; }
        public string COMMON_CD { get; set; }
        public string COMMON_PART_CD { get; set; }
        public string COMMON_NM { get; set; }
        public string COMMON_PART_NM { get; set; }
        public int? COMMON_SEQ { get; set; }
        public string COMMON_SYS_CK { get; set; }
        public string COMMON_MODULE { get; set; }
        public string COMMON_ETC { get; set; }
        public string COMMON_REMARK { get; set; }
        public string COMMON_USE_CK { get; set; }
        public string INSERT_USER_ID { get; set; }
        public DateTime? INSERT_TIME { get; set; }
        public string UPDATE_USER_ID { get; set; }
        public DateTime? UPDATE_TIME { get; set; }
        public int AUDITTRAIL_ID { get; set; }
        public string AUDITTRAIL_REMARK { get; set; }
        public string AUDITTRAIL_STATUS { get; set; }
        public byte[]? COMMON_IMAGE { get; set; }

    }
}
