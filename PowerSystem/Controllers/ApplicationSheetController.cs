using PowerSystemLibrary.Filter;
using PowerSystemLibrary.Util;
using PowerSystemLibrary.BLL;
using PowerSystemLibrary.Entity;
using PowerSystemLibrary.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace PowerSystem.Controllers
{
    /// <summary>
    /// 申请单
    /// </summary>
    [RoutePrefix("ApplicationSheet")]
    public class ApplicationSheetController : ApiController
    {
        /// <summary>
        /// 申请单审核
        /// </summary>
        /// <param name="applicationSheet">需要ID、Audit(通过3 驳回4)、AuditMessage</param>
        /// <returns></returns>
        [HttpPost, Route("Audit"), LoginRequired(Role = "ApplicationSheetAudit")]
        public ApiResult Audit([FromBody] ApplicationSheet applicationSheet)
        {
            return new ApplicationSheetBLL().Audit(applicationSheet);
        }

        /// <summary>
        /// 调度申请单审核
        /// </summary>
        /// <param name="applicationSheet">需要ID、Audit(通过3 驳回4)、AuditMessage</param>
        /// <returns></returns>
        [HttpPost, Route("DispatcherAudit"), LoginRequired(Role = "Dispatcher")]
        public ApiResult DispatcherAudit([FromBody] ApplicationSheet applicationSheet)
        {
            return new ApplicationSheetBLL().DispatcherAudit(applicationSheet);
        }

        /// <summary>
        /// 获取单条申请单信息
        /// </summary>
        /// <param name="id">申请单id</param>
        /// <returns></returns>
        [HttpGet, Route("Get"), LoginRequired]
        public ApiResult Get(int id)
        {
            return new ApplicationSheetBLL().Get(id);
        }

        /// <summary>
        /// 申请单列表
        /// </summary>
        /// <param name="departmentID">部门ID</param>       
        /// <param name="no">申请单编号</param>
        /// <param name="voltageType">低压1 高压2</param>
        /// <param name="audit">待审核1 通过3 驳回4</param>
        /// <param name="ahID">开关柜ID</param>
        /// <param name="beginDate">申请开始时间</param>
        /// <param name="endDate">申请结束时间</param>
        /// <param name="page">页码</param>
        /// <param name="limit">单页条数</param>
        /// <returns></returns>
        [HttpGet, Route("List"), LoginRequired]
        public ApiResult List(int? departmentID = null, string no = "", VoltageType? voltageType = null, Audit? audit = null, int? ahID = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            return new ApplicationSheetBLL().List(departmentID, no, voltageType, audit, ahID, beginDate, endDate, page, limit);
        }

        /// <summary>
        /// 需要审核的申请单列表
        /// </summary>
        /// <param name="departmentID">部门ID</param>       
        /// <param name="no">申请单编号</param>
        /// <param name="voltageType">低压1 高压2</param>
        /// <param name="isAudit">是否已审核</param>
        /// <param name="ahID">开关柜ID</param>
        /// <param name="beginDate">申请开始时间</param>
        /// <param name="endDate">申请结束时间</param>
        /// <param name="page">页码</param>
        /// <param name="limit">单页条数</param>
        /// <returns></returns>
        [HttpGet, Route("MyAuditList"), LoginRequired]
        public ApiResult MyAuditList(int? departmentID = null, string no = "", VoltageType? voltageType = null, bool isAudit = false, int? ahID = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            return new ApplicationSheetBLL().MyAuditList(departmentID, no, voltageType, isAudit, ahID, beginDate, endDate, page, limit);
        }

        /// <summary>
        /// 导出申请单
        /// </summary>
        /// <param name="ID">申请单ID</param>
        /// <returns></returns>
        [HttpGet, Route("Export"), LoginRequired]
        public ApiResult Export(int ID)
        {
            return new ApplicationSheetBLL().Export(ID);
        }

        /// <summary>
        /// 导出全流程表单
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet, Route("ExportAllSheet"), LoginRequired]
        public ApiResult ExportAllSheet(int ID)
        {
            return new ApplicationSheetBLL().ExportAllSheet(ID);
        }

    }
}
