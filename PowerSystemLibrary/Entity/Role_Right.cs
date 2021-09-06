using PowerSystemLibrary.Enum;
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
    [Table("Tb_Role_Right")]
    [Description("角色权限")]
    public class Role_Right
    {
        [Key]
        public int ID { get; set; }
        public int MenuID { get; set; }
        public Role Role { get; set; }
    }
}
