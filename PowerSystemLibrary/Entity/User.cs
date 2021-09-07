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
    [Table("Tb_User")]
    [Description("用户")]
    public class User
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "请输入用户名")]
        [ExchangeType]
        public string Username { get; set; }
        [Required(ErrorMessage = "请输入姓名")]
        [ExchangeType]
        public string Realname { get; set; }
        public string Password { get; set; }

        [ExchangeType]
        public string Cellphone { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string Token { get; set; }
        public DateTime? Expire { get; set; }
        public bool? IsDelete { get; set; }
        public string WeChatID { get; set; }
        [ExchangeType]
        public int DepartmentID { get; set; }

        [NotMapped]
        public string NewPassword { get; set; }

        [NotMapped]
        public List<int> RoleIDList { get; set; }
    }
}
