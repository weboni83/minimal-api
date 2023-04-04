using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalAPIsDemo.Entities
{
    [Table("CM_PLANT")]
    public class CM_PLANT
    {
        public string PLANT_CD { get; set; }
        public string PLANT_NM { get; set; }
        public string PLANT_GS_CD { get; set; }
        public string PLANT_REMARK { get; set; }
        public string PLANT_USE_CK { get; set; }
        public string PLANT_BUSINESS_NO { get; set; }
        public string PLANT_CATEGORY { get; set; }
        public string PLANT_CATEGORY_GB { get; set; }
        public string PLANT_TEL_NO { get; set; }
        public string PLANT_NATION { get; set; }
        public string PLANT_POST_NO { get; set; }
        public string PLANT_ADDRESS1 { get; set; }
        public string PLANT_ADDRESS2 { get; set; }
        public string INSERT_USER_ID { get; set; }
        public DateTime? INSERT_TIME { get; set; }
        public string UPDATE_USER_ID { get; set; }
        public DateTime? UPDATE_TIME { get; set; }
        public int AUDITTRAIL_ID { get; set; }
        public string AUDITTRAIL_REMARK { get; set; }
        public string AUDITTRAIL_STATUS { get; set; }
    }
}
