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
    [Table("Tb_UserRole")]
    [Description("用户权限")]
    public class UserRole
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public Role Role { get; set; }
        [NotMapped]
        public string RoleName { get; set; }
    }
}
