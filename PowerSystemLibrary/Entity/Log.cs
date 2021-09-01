using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSystemLibrary.Enum;

namespace PowerSystemLibrary.Entity
{
    [Table("Tb_Log")]
    [Description("日志")]
    public class Log
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  //设置自增

        public int ID { get; set; }
        public LogCode LogCode { get; set; }
        public string Content { get; set; }
        public int? UserID { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;

    }
}
