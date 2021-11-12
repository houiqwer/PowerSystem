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
    [Table("Tb_OperationContent")]
    [Description("操作内容")]
    public class OperationContent
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "请输入操作内容")]
        [ExchangeType]
        public string Content { get; set; }

        public ElectricalTaskType ElectricalTaskType { get; set; }

        public bool IsDelete { get; set; } = false;
    }
}
