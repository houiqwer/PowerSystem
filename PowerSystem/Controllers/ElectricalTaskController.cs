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
    /// 任务
    /// </summary>
    [RoutePrefix("ElectricalTask")]
    public class ElectricalTaskController : ApiController
    {
        /// <summary>
        /// 接受任务
        /// </summary>
        /// <param name="electricalTask">需要ID</param>
        /// <returns></returns>
        [HttpPost, Route("Accept"), LoginRequired]
        public ApiResult Accept([FromBody] ElectricalTask electricalTask)
        {
            return new ElectricalTaskBLL().Accept(electricalTask);
        }

        /// <summary>
        /// 确认任务完成
        /// </summary>
        /// <param name="electricalTask">需要ID</param>
        /// <returns></returns>
        [HttpPost, Route("Confirm"), LoginRequired]
        public ApiResult Confirm([FromBody] ElectricalTask electricalTask)
        {
            return new ElectricalTaskBLL().Confirm(electricalTask);
        }

        /// <summary>
        /// 任务列表
        /// </summary>
        /// <param name="ahID">开关柜ID</param>
        /// <param name="electricalTaskType">停电作业1  送电作业2</param>
        /// <param name="beginDate">创建日期最早时间</param>
        /// <param name="endDate">创建日期最晚时间</param>
        /// <param name="page">页码</param>
        /// <param name="limit">单页条数</param>
        /// <returns></returns>
        [HttpGet, Route("List"), LoginRequired]
        public ApiResult List(int? ahID = null, ElectricalTaskType? electricalTaskType = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            return new ElectricalTaskBLL().List(ahID, electricalTaskType, beginDate, endDate, page, limit);
        }

        /// <summary>
        /// 未接收任务列表
        /// </summary>
        /// <param name="ahID">开关柜ID</param>
        /// <param name="electricalTaskType">停电作业1  送电作业2</param>
        /// <param name="beginDate">创建日期最早时间</param>
        /// <param name="endDate">创建日期最晚时间</param>
        /// <param name="page">页码</param>
        /// <param name="limit">单页条数</param>
        /// <returns></returns>
        [HttpGet, Route("NotAcceptedList"), LoginRequired]
        public ApiResult NotAcceptedList(int? ahID = null, ElectricalTaskType? electricalTaskType = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            return new ElectricalTaskBLL().NotAcceptedList(ahID, electricalTaskType, beginDate, endDate, page, limit);
        }

        /// <summary>
        /// 已接收任务列表
        /// </summary>
        /// <param name="ahID">开关柜ID</param>
        /// <param name="electricalTaskType">停电作业1  送电作业2</param>
        /// <param name="beginDate">创建日期最早时间</param>
        /// <param name="endDate">创建日期最晚时间</param>
        /// <param name="page">页码</param>
        /// <param name="limit">单页条数</param>
        /// <returns></returns>
        [HttpGet, Route("AcceptedList"), LoginRequired]
        public ApiResult AcceptedList(int? ahID = null, ElectricalTaskType? electricalTaskType = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            return new ElectricalTaskBLL().AcceptedList(ahID, electricalTaskType, beginDate, endDate, page, limit);
        }

        /// <summary>
        /// 退回任务
        /// </summary>
        /// <param name="electricalTask">需要ID</param>
        /// <returns></returns>
        [HttpPost, Route("Back"), LoginRequired]
        public ApiResult Back(ElectricalTask electricalTask)
        {
            return new ElectricalTaskBLL().Back(electricalTask);
        }


        /// <summary>
        /// 调度人审核列表
        /// </summary>
        /// <param name="ahID"></param>
        /// <param name="electricalTaskType"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isAudit"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet, Route("DispatcherAuditList"), LoginRequired]
        public ApiResult DispatcherAuditList(int? ahID = null, ElectricalTaskType? electricalTaskType = null, DateTime? beginDate = null, DateTime? endDate = null, bool isAudit = false, int page = 1, int limit = 10)
        {
            return new ElectricalTaskBLL().DispatcherAuditList(ahID, electricalTaskType,beginDate, endDate, isAudit, page, limit);
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="electricalTask"></param>
        /// <returns></returns>
        [HttpPost, Route("DispatcherAudit"), LoginRequired(Role = "Dispatcher")]
        public ApiResult DispatcherAudit(ElectricalTask electricalTask)
        {
            return new ElectricalTaskBLL().DispatcherAudit(electricalTask);
        }
    }
}
