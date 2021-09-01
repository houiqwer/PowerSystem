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
    [Table("Tb_AuditProcess")]
    [Description("审核流程")]
    public class AuditProcess
    {
        [Key]
        public int ID { get; set; }
        public SheetType SheetType { get; set; }
        public int SheetID { get; set; }
        public int UserID { get; set; }
        public int Level { get; set; }
        public bool IsBegin { get; set; } = false;
        public Audit Audit { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? AuditDate { get; set; }

    }
}
