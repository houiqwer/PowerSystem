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
    [Table("Tb_OperationSheet")]
    [Description("操作票")]
    public class OperationSheet
    {
        [Key]
        public int ID { get; set; }
        
        public int OperationID { get; set; }

        public int ElectricalTaskID { get; set; }

       // public ElectricalTaskType ElectricalTaskType { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        //[Required(ErrorMessage = "请输入操作内容")]
        //[ExchangeType]
        //public string Content { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public int OperationUserID { get; set; }

        public DateTime OperationDate { get; set; }

        /// <summary>
        /// 监护人
        /// </summary>
        public Nullable<int> GuardianUserID { get; set; }

        public bool IsConfirm { get; set; } = false;

        public DateTime? FinishDate { get; set; }

        [NotMapped]
        public string OperationUserName { get; set; }
        [NotMapped]
        public string GuardianUserName { get; set; }

        [NotMapped]
        public string OperationDateString { get; set; }
        [NotMapped]
        public string FinishDateString { get; set; }

        [NotMapped]
        public List<int> OperationContentIDList { get; set; }

        [NotMapped]
        public List<OperationContent> OperationContentList { get; set; }
    }
}
