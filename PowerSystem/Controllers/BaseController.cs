using PowerSystemLibrary.BLL;
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
    /// 枚举
    /// </summary>
    [RoutePrefix("Base")]
    public class BaseController : ApiController
    {
        /// <summary>
        /// 获取枚举数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet, Route("GetEnum")]
        public ApiResult GetEnum(string type)
        {
            return new BaseBLL().GetEnum(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("RoleList")]
        public ApiResult RoleList()
        {
            return new BaseBLL().RoleList();
        }
    }
}
