using PowerSystemLibrary.BLL;
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
    /// 操作票
    /// </summary>
    [RoutePrefix("OperationSheet")]
    public class OperationSheetController : ApiController
    {
       /// <summary>
       /// 根据任务获取操作票信息
       /// </summary>
       /// <param name="id">任务ID</param>
       /// <returns></returns>
        [HttpGet, Route("Get"), LoginRequired]
        public ApiResult Get(int id)
        {
            return new OperationSheetBLL().Get(id);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="ahID"></param>
        /// <param name="electricalTaskType"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet, Route("List"), LoginRequired]
        public ApiResult List(int? ahID = null, ElectricalTaskType? electricalTaskType = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            return new OperationSheetBLL().List(ahID,electricalTaskType,beginDate,endDate,page,limit);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("Export"), LoginRequired]
        public ApiResult Export(int id)
        {
            return new OperationSheetBLL().Export(id);
        }
    }
}
