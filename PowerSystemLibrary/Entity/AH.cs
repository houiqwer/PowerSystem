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
    [Table("Tb_AH")]
    [Description("开关柜")]
    public class AH
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "请输入开关柜名称")]
        [ExchangeType]
        public string Name { get; set; }
        public VoltageType VoltageType { get; set; }
        public AHState AHState { get; set; }
        public bool? IsDelete { get; set; }
    }
}
