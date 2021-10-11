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
    [Table("Tb_ElectricalTaskUser")]
    [Description("任务接收人")]
    public class ElectricalTaskUser
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int ElectricalTaskID { get; set; }
        public DateTime Date { get; set; }
        public bool IsConfirm { get; set; } = false;

        public bool IsBack { get; set; } = false;

        [NotMapped]
        public string RealName { get; set; }
        [NotMapped]
        public string CreateDate { get; set; }
    }
}
