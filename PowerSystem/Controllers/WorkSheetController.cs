using PowerSystemLibrary.BLL;
using PowerSystemLibrary.Entity;
using PowerSystemLibrary.Enum;
using PowerSystemLibrary.Filter;
using PowerSystemLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PowerSystem.Controllers
{
    /// <summary>
    /// 工作票
    /// </summary>
    [RoutePrefix("WorkSheet")]
    public class WorkSheetController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="departmentID"></param>
        /// <param name="no"></param>
        /// <param name="voltageType"></param>
        /// <param name="ahID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet, Route("List"), LoginRequired]
        public ApiResult List(int? departmentID = null, string no = "", VoltageType? voltageType = null, int? ahID = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            return new WorkSheetBLL().List(departmentID, no, voltageType, ahID, beginDate, endDate, page, limit);
        }

        /// <summary>
        /// 我的审核列表
        /// </summary>
        /// <param name="departmentID"></param>
        /// <param name="no"></param>
        /// <param name="voltageType"></param>
        /// <param name="ahID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet, Route("MyAuditList"),LoginRequired]
        public ApiResult MyAuditList(int? departmentID = null, string no = "", VoltageType? voltageType = null, int? ahID = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            return new WorkSheetBLL().MyAuditList(departmentID, no, voltageType, ahID, beginDate, endDate, page, limit);
        }

        /// <summary>
        /// 工作票审核
        /// </summary>
        /// <param name="workSheet">需要ID、AuditLevel(通过3 驳回4)、AuditMessage</param>
        /// <returns></returns>
        [HttpPost, Route("Audit"), LoginRequired(Role = "WorkSheetAuditRoleList")]
        public ApiResult Audit(WorkSheet workSheet)
        {
            return new WorkSheetBLL().Audit(workSheet);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("Get"), LoginRequired]
        public ApiResult Get(int id)
        {
            return new WorkSheetBLL().Get(id);
        }

    }
}
