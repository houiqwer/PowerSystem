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
    /// log
    /// </summary>
    [RoutePrefix("Log")]
    public class LogController : ApiController
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="realName"></param>
        /// <param name="logCode"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet, Route("List"), LoginRequired]
        public ApiResult List(string realName, LogCode? logCode = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            return new LogBLL().List(realName, logCode, beginDate, endDate, page, limit);
        }
    }
}
