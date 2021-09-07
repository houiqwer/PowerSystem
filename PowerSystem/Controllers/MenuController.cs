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
    /// 菜单
    /// </summary>
    [RoutePrefix("Menu")]
    public class MenuController : ApiController
    {
        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="menu">菜单实体</param>
        /// <returns></returns>
        [HttpPost, Route("Add"), LoginRequired]
        public ApiResult Add(Menu menu)
        {
            return new MenuBLL().Add(menu);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [HttpPost, Route("Edit"), LoginRequired]
        public ApiResult Edit(Menu menu)
        {
            return new MenuBLL().Edit(menu);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [HttpPost, Route("Delete"), LoginRequired]
        public ApiResult Delete(Menu menu)
        {
            return new MenuBLL().Delete(menu.ID);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet, Route("Get"), LoginRequired]
        public ApiResult Get(int ID)
        {
            return new MenuBLL().Get(ID);
        }

        /// <summary>
        /// 菜单列表 在角色菜单配置页以及菜单管理页显示
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpGet, Route("List"), LoginRequired]
        public ApiResult List(Role? role = null)
        {
            return new MenuBLL().List(role);
        }

        /// <summary>
        /// 角色菜单配置
        /// </summary>
        /// <param name="ajaxPost"></param>
        /// <returns></returns>
        [HttpPost, Route("RoleMenuAdd"), LoginRequired]
        public ApiResult RoleMenuAdd(AjaxPost ajaxPost)
        {
            return new MenuBLL().RoleMenuAdd(ajaxPost.Role,ajaxPost.IDList);
        }

        /// <summary>
        /// 登陆页菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("MRoleList"), LoginRequired]
        public ApiResult MRoleList()
        {
            return new MenuBLL().MRoleList();
        }
    }
}
