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
    [Table("Tb_ApplicationSheet")]
    [Description("申请单")]
    public class ApplicationSheet
    {
        [Key]
        public int ID { get; set; }
        public string NO { get; set; }
        public int OperationID { get; set; }
        public int UserID { get; set; }
        public int DepartmentID { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        //[Required(ErrorMessage = "请输入工作内容")]
        //[ExchangeType]
        //public string WorkContent { get; set; }

        [Required(ErrorMessage = "请选择工作内容")]
        [ExchangeType]
        public WorkContentType WorkContentType { get; set; }

        public Audit Audit { get; set; } = Audit.待审核;
        public int? AuditUserID { get; set; }
        public DateTime? AuditDate { get; set; }
        public string AuditMessage { get; set; }
        public bool? IsDelete { get; set; }        
    }
}
