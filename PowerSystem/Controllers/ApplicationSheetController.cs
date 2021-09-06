﻿using PowerSystemLibrary.Filter;
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
        [HttpPost, Route("Audit"), LoginRequired]
        public ApiResult Audit([FromBody] ApplicationSheet applicationSheet)
        {
            return new ApplicationSheetBLL().Audit(applicationSheet);
        }

        /// <summary>
        /// 申请单列表
        /// </summary>
        /// <param name="departmentID">部门ID</param>       
        /// <param name="no">申请单编号</param>
        /// <param name="voltageType">低压1 高压2</param>
        /// <param name="ahID">开关柜ID</param>
        /// <param name="beginDate">申请开始时间</param>
        /// <param name="endDate">申请结束时间</param>
        /// <param name="page">页码</param>
        /// <param name="limit">单页条数</param>
        /// <returns></returns>
        [HttpGet, Route("List"), LoginRequired]
        public ApiResult List(int? departmentID = null, string no = "", VoltageType? voltageType = null, int? ahID = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            return new ApplicationSheetBLL().List(departmentID, no, voltageType, ahID, beginDate, endDate, page, limit);
        }

    }
}