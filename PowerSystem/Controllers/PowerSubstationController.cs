using PowerSystemLibrary.BLL;
using PowerSystemLibrary.Entity;
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
    [RoutePrefix("PowerSubstation")]
    public class PowerSubstationController : ApiController
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("List"), LoginRequired]
        public ApiResult List(int page = 1, int limit = 10)
        {
            return new PowerSubstationBLL().List(page,limit);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="powerSubstation"></param>
        /// <returns></returns>
        [HttpPost, Route("Add"), LoginRequired]
        public ApiResult Add(PowerSubstation powerSubstation)
        {
            return new PowerSubstationBLL().Add(powerSubstation);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="powerSubstation"></param>
        /// <returns></returns>
        [HttpPost, Route("Edit"), LoginRequired]
        public ApiResult Edit(PowerSubstation powerSubstation)
        {
            return new PowerSubstationBLL().Edit(powerSubstation);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="powerSubstation"></param>
        /// <returns></returns>
        [HttpPost, Route("Delete"), LoginRequired]
        public ApiResult Delete(PowerSubstation powerSubstation)
        {
            return new PowerSubstationBLL().Delete(powerSubstation.ID);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("Get"), LoginRequired]
        public ApiResult Get(int id)
        {
            return new PowerSubstationBLL().Get(id);
        }
    }
}
