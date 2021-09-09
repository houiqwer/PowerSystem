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

    [Table("Tb_Menu")]
    [Description("菜单")]
    public class Menu
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "请输入菜单名称")]
        [ExchangeType]
        public string Name { get; set; }
        [Required(ErrorMessage = "请输入菜单地址")]
        [ExchangeType]
        public string URL { get; set; }

        /// <summary>
        /// 图标  
        /// </summary>
        [ExchangeType]
        public string Icon { get; set; }

        public int? ParentID { get; set; }
        [ExchangeType]
        public int? Order { get; set; }
        public bool? IsDelete { get; set; }
    }
}
