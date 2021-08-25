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
    [Table("Tb_Role")]
    [Description("权限")]
    public class Role
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "请输入权限名称")]
        [ExchangeType]
        public string Name { get; set; }
        public int Level { get; set; }
        public bool? IsDelete { get; set; }
    }
}
