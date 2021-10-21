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
    [Table("Tb_OperationSheet_Content")]
    [Description("操作票内容关联表")]
    public class OperationSheet_Content
    {
        [Key]
        public int ID { get; set; }

        public int OperationSheetID { get; set; }

        public int OperationContentID { get; set; }
    }
}
