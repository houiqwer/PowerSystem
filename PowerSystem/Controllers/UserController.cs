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
    /// 用户
    /// </summary>
    [RoutePrefix("User")]
    public class UserController : ApiController
    {
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <returns></returns>
        [HttpPost, Route("Add"), LoginRequired]
        public ApiResult Add([FromBody] User user)
        {
            return new UserBLL().Add(user);
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <returns></returns>
        [HttpPost, Route("Edit"), LoginRequired]
        public ApiResult Edit([FromBody] User user)
        {
            return new UserBLL().Edit(user);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="ajaxPost"></param>
        /// <returns></returns>
        [HttpPost, Route("Delete"), LoginRequired]
        public ApiResult Delete(AjaxPost ajaxPost)
        {
            return new UserBLL().Delete(ajaxPost.IDList);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="departmentID">部门id</param>
        /// <param name="realname">姓名</param>
        /// <param name="role">角色</param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet, Route("List"), LoginRequired]
        public ApiResult List(int? departmentID = null, string realname = null, Role? role = null, int page = 1, int limit = 10)
        {
            return new UserBLL().List(departmentID, realname, role, page, limit);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet, Route("Get"), LoginRequired]
        public ApiResult Get(int ID)
        {
            return new UserBLL().Get(ID);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost, Route("ResetPassword"), LoginRequired]
        public ApiResult ResetPassword(User user)
        {
            return new UserBLL().ResetPassword(user.ID);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost, Route("ChangePassword"), LoginRequired]
        public ApiResult ChangePassword(User user)
        {
            return new UserBLL().ChangePassword(user);
        }
    }
}
