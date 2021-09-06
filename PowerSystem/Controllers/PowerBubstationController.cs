using PowerSystemLibrary.BLL;
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
    /// 变电所
    /// </summary>
    [RoutePrefix("PowerBubstation")]
    public class PowerBubstationController : ApiController
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("List"), LoginRequired]
        public ApiResult List()
        {
            return new PowerBubstationBLL().List();
        }
    }
}
