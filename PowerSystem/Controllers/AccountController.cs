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
    /// 登陆
    /// </summary>
    [RoutePrefix("Account")]
    public class AccountController : ApiController
    {
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="user">需要Username Password</param>
        /// <returns></returns>
        [HttpPost, Route("Login")]
        public ApiResult Login([FromBody] User user)
        {
            return new AccountBLL().Login(user);
        }

        /// <summary>
        /// 身份验证
        /// </summary>
        /// <param name="user">需要Token</param>
        /// <returns></returns>
        [HttpPost, Route("ExtendToken")]
        public ApiResult ExtendToken([FromBody] User user)
        {
            return new AccountBLL().ExtendToken(user);
        }
    }
}
