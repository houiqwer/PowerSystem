using PowerSystemLibrary.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemLibrary.Entity
{
    [Table("Tb_SendElectricalSheet")]
    [Description("送电关联表")]
    public class SendElectricalSheet
    {
        [Key]
        public int ID { get; set; }

        public int OperationID { get; set; }

        public int ElectricalTaskID { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "请选择送电时间")]
        [ExchangeType]
        public DateTime SendElectricDate { get; set; }

        [Required(ErrorMessage = "请输入工作完成情况")]
        [ExchangeType]
        public string WorkFinishContent { get; set; }

        [Required(ErrorMessage = "请选择地线是否拆除")]
        [ExchangeType]
        public bool IsRemoveGroundLine { get; set; }

        [Required(ErrorMessage = "请选择人员是否都撤离")]
        [ExchangeType]
        public bool IsEvacuateAllPeople { get; set; }

        public int UserID { get; set; }

        [NotMapped]
        public string SendElectricDateString { get; set; }

        [NotMapped]
        public string UserRealName { get; set; }

        [NotMapped]
        public string IsRemoveGroundLineName { get; set; }

        [NotMapped]
        public string IsEvacuateAllPeopleName { get; set; }
    }
}
