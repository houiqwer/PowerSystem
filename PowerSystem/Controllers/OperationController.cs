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
    /// 操作
    /// </summary>
    [RoutePrefix("Operation")]
    public class OperationController : ApiController
    {
        /// <summary>
        /// 发起停电申请
        /// </summary>
        /// <param name="operation">停电申请信息，需要根据高压和低压走不同流程</param>
        /// <returns></returns>
        [HttpPost, Route("Add"), LoginRequired]
        public ApiResult Add([FromBody] Operation operation)
        {
            return new OperationBLL().Add(operation);
        }

        /// <summary>
        /// 获取停电申请
        /// </summary>
        /// <param name="id">停电申请id</param>
        /// <returns></returns>
        [HttpGet, Route("Get"), LoginRequired]
        public ApiResult Get(int id)
        {
            return new OperationBLL().Get(id);
        }

        /// <summary>
        /// 停电申请列表
        /// </summary>
        /// <param name="departmentID">部门ID</param>
        /// <param name="voltageType">低压1 高压2</param>
        /// <param name="ahID">开关柜ID</param>
        /// <param name="beginDate">创建日期最早时间</param>
        /// <param name="endDate">创建日期最晚时间</param>
        /// <param name="page">页码</param>
        /// <param name="limit">单页条数</param>
        /// <returns></returns>
        [HttpGet, Route("List"), LoginRequired]
        public ApiResult List(int? departmentID = null, VoltageType? voltageType = null, int? ahID = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            return new OperationBLL().List(departmentID, voltageType, ahID, beginDate, endDate, page, limit);
        }

        /// <summary>
        /// 我的停电申请列表
        /// </summary>
        /// <param name="voltageType">低压1 高压2</param>
        /// <param name="ahID">开关柜ID</param>
        /// <param name="beginDate">创建日期最早时间</param>
        /// <param name="endDate">创建日期最晚时间</param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet, Route("MyList"), LoginRequired]
        public ApiResult MyList(VoltageType? voltageType = null, int? ahID = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            return new OperationBLL().MyList(voltageType, ahID, beginDate, endDate, page, limit);
        }

        /// <summary>
        /// 挂牌
        /// </summary>
        /// <param name="operation">需要ID</param>
        /// <returns></returns>
        [HttpPost, Route("Hang"), LoginRequired]
        public ApiResult Hang([FromBody] Operation operation)
        {
            return new OperationBLL().Hang(operation);
        }

        /// <summary>
        /// 摘牌
        /// </summary>
        /// <param name="operation">需要ID</param>
        /// <returns></returns>
        [HttpPost, Route("Pick"), LoginRequired]
        public ApiResult Pick([FromBody] Operation operation)
        {
            return new OperationBLL().Pick(operation);
        }


        /// <summary>
        /// 导出工作票
        /// </summary>
        /// <param name="ID">作业ID</param>
        /// <returns></returns>
        [HttpGet, Route("ExportWorkSheet"), LoginRequired]
        public ApiResult ExportWorkSheet(int ID)
        {
            return new OperationBLL().ExportWorkSheet(ID);
        }

        /// <summary>
        /// 导出列表
        /// </summary>
        /// <param name="departmentID"></param>
        /// <param name="voltageType"></param>
        /// <param name="ahID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet, Route("ExportByList"), LoginRequired]
        public ApiResult ExportByList(int? departmentID = null, VoltageType? voltageType = null, int? ahID = null, DateTime? beginDate = null, DateTime? endDate = null)
        {
            return new OperationBLL().ExportByList(departmentID, voltageType, ahID, beginDate, endDate);
        }

        /// <summary>
        /// 手机端列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet, Route("ListForApp"), LoginRequired]
        public ApiResult ListForApp(int page = 1, int limit = 10)
        {
            return new OperationBLL().ListForApp(page, limit);
        }
    }
}
