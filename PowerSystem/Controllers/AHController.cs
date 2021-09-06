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
    /// 开关柜
    /// </summary>
    [RoutePrefix("AH")]
    public class AHController : ApiController
    {
        /// <summary>
        /// 添加开关柜
        /// </summary>
        /// <param name="aH">开关柜实体</param>
        /// <returns></returns>
        [HttpPost, Route("Add"), LoginRequired]
        public ApiResult Add(AH aH)
        {
            return new AHBLL().Add(aH);
        }

        /// <summary>
        /// 编辑开关柜
        /// </summary>
        /// <param name="aH">开关柜实体</param>
        /// <returns></returns>
        [HttpPost, Route("Edit"), LoginRequired]
        public ApiResult Edit(AH aH)
        {
            return new AHBLL().Edit(aH);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ajaxPost"></param>
        /// <returns></returns>
        [HttpPost, Route("Delete"), LoginRequired]
        public ApiResult Delete(AjaxPost ajaxPost)
        {
            return new AHBLL().Delete(ajaxPost.IDList);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet, Route("Get"), LoginRequired]
        public ApiResult Get(int ID)
        {
            return new AHBLL().Get(ID);
        }


        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="voltageType"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet, Route("List"), LoginRequired]
        public ApiResult List(string name = "", VoltageType? voltageType = null, int page = 1, int limit = 10)
        {
            return new AHBLL().List(name,voltageType,page,limit);
        }

    }
}
