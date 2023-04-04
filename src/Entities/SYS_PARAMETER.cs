using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalAPIsDemo.Entities
{
    [Table("SYS_PARAMETER")]
    public class SYS_PARAMETER
    {
        public string PLANT_CD { get; set; }
        public string PARAMETER_CD { get; set; }
        public string PARAMETER_VALUE { get; set; }
        public string PARAMETER_REMARK { get; set; }
        public string PARAMETER_MODULE { get; set; }
        public string INSERT_USER_ID { get; set; }
        public DateTime? INSERT_TIME { get; set; }
        public string UPDATE_USER_ID { get; set; }
        public DateTime? UPDATE_TIME { get; set; }
        public int AUDITTRAIL_ID { get; set; }
        public string AUDITTRAIL_REMARK { get; set; }
        public string AUDITTRAIL_STATUS { get; set; }
    }


}
