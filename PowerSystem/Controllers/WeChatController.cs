using PowerSystemLibrary.BLL;
using PowerSystemLibrary.Entity;
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
    /// 微信登陆
    /// </summary>
    [RoutePrefix("WeChat")]
    public class WeChatController : ApiController
    {
        /// <summary>
        /// 微信验证
        /// </summary>
        /// <param name="ajaxPost">Code</param>
        /// <returns></returns>
        [HttpPost, Route("OAuth")]
        public ApiResult OAuth(AjaxPost ajaxPost)
        {
            return new WeChatBLL().OAuth(ajaxPost.Code);
        }
    }
}
