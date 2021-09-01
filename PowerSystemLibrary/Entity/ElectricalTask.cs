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
    [Table("Tb_ElectricalTask")]
    [Description("任务")]
    public class ElectricalTask
    {
        [Key]
        public int ID { get; set; }
        public int OperationID { get; set; }
        public int AHID { get; set; }
        public ElectricalTaskType ElectricalTaskType { get; set; }
        public int ReciveCount { get; set; } = 0;
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public bool IsConfirm { get; set; } = false;

    }
}
