using PowerSystemLibrary.Enum;
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
    [Table("Tb_Operation")]
    [Description("作业")]
    public class Operation
    {
        [Key]
        public int ID { get; set; }
        //申请人
        public int UserID { get; set; }
        public int AHID { get; set; }
        //申请时间
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public VoltageType VoltageType { get; set; }
        public OperationFlow OperationFlow { get; set; }
        public bool IsConfirm { get; set; } = false;
        public bool IsFinish { get; set; } = false;
        public DateTime? FinishDate { get; set; }
        public bool IsSendElectric { get; set; } = false;
        public OperationAudit OperationAudit { get; set; }
        public bool IsPick { get; set; } = false;

        public bool IsHang { get; set; } = false;
        [NotMapped]
        public ApplicationSheet ApplicationSheet { get; set; }

        [NotMapped]
        public WorkSheet WorkSheet { get; set; }

        [NotMapped]
        public OperationSheet OperationSheet { get; set; }
    }
}
