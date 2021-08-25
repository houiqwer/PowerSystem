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
    [Table("Tb_Department")]
    [Description("部门")]
    public class Department
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "请输入部门名称")]
        [ExchangeType]
        public string Name { get; set; }
        public bool? IsDelete { get; set; }
    }
}
