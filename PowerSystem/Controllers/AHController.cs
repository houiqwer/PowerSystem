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
        /// 添加变电柜
        /// </summary>
        /// <param name="aH">变电柜实体</param>
        /// <returns></returns>
        [HttpPost, Route("Add"), LoginRequired]
        public ApiResult Add(AH aH)
        {
            return new AHBLL().Add(aH);
        }

        /// <summary>
        /// 编辑变电柜
        /// </summary>
        /// <param name="aH">变电柜实体</param>
        /// <returns></returns>
        [HttpPost, Route("Edit"), LoginRequired]
        public ApiResult Edit(AH aH)
        {
            return new AHBLL().Edit(aH);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ah">需要ID</param>
        /// <returns></returns>
        [HttpPost, Route("Delete"), LoginRequired]
        public ApiResult Delete(AH ah)
        {
            return new AHBLL().Delete(ah.ID);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="ID">变电柜ID</param>
        /// <returns></returns>
        [HttpGet, Route("Get"), LoginRequired]
        public ApiResult Get(int ID)
        {
            return new AHBLL().Get(ID);
        }


        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="name">变电柜名称</param>
        /// <param name="voltageType">电压类型 低压1 高压2</param>
        /// <param name="powerSubstationID">变电所ID</param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet, Route("List"), LoginRequired]
        public ApiResult List(string name = "", VoltageType? voltageType = null, int? powerSubstationID = null, int page = 1, int limit = 10)
        {
            return new AHBLL().List(name, voltageType, powerSubstationID, page, limit);
        }

    }
}
