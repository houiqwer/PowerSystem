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
    [Description("配电柜")]
    public class AH
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "请输入配电柜名称")]
        [ExchangeType]
        public string Name { get; set; }
        public VoltageType VoltageType { get; set; }
        public AHState AHState { get; set; }
        [Required(ErrorMessage = "请选择变电所")]
        [ExchangeType]
        public int PowerSubstationID { get; set; }
        public bool? IsDelete { get; set; }

        /// <summary>
        /// 牌的IP
        /// </summary>
        [Required(ErrorMessage = "请输入牌的IP")]
        [ExchangeType]
        public string LedIP { get; set; }

        /// <summary>
        /// 灯的IP
        /// </summary>
        [Required(ErrorMessage = "请输入灯的IP")]
        [ExchangeType]
        public string LampIP { get; set; }

    }
}
