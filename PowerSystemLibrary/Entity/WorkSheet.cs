﻿using PowerSystemLibrary.Enum;
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
    [Table("Tb_WorkSheet")]
    [Description("工作票")]
    public class WorkSheet
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
        
        public string WorkContent { get; set; }

        //public Audit Audit { get; set; } = Audit.待审核;

        [Required(ErrorMessage = "请输入影响范围")]
        [ExchangeType]
        public string Influence { get; set; }

        public AuditLevel AuditLevel { get; set; }

        //部门副职审核
        public int DeputyAuditUserID { get; set; }
        public Audit DeputyAudit { get; set; } = Audit.待审核;
        public DateTime? DeputyAuditDate { get; set; }
        public string DeputyAuditMessage { get; set; }

        [Required(ErrorMessage = "请填写技术安全措施")]
        [ExchangeType]
        public string SafetyMeasures { get; set; }


        //部门正职审核
        public int ChiefAuditUserID { get; set; }
        public Audit ChiefAudit { get; set; } = Audit.待审核;

        public DateTime? ChiefAuditDate { get; set; }
        public string ChiefAuditMessage { get; set; }

        public bool? IsDelete { get; set; }

        [NotMapped]
        public string AuditMessage { get; set; }

    }
}
