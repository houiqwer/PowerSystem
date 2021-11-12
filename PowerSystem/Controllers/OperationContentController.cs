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
    /// 操作内容
    /// </summary>
    [RoutePrefix("OperationContent")]
    public class OperationContentController : ApiController
    {
        /// <summary>
        /// 添加操作内容
        /// </summary>
        /// <param name="operationContent">操作内容实体</param>
        /// <returns></returns>
        [HttpPost, Route("Add"), LoginRequired]
        public ApiResult Add(OperationContent operationContent)
        {
            return new OperationContentBLL().Add(operationContent);
        }


        /// <summary>
        /// 编辑操作内容
        /// </summary>
        /// <param name="operationContent">操作内容实体</param>
        /// <returns></returns>
        [HttpPost, Route("Edit"), LoginRequired]
        public ApiResult Edit(OperationContent operationContent)
        {
            return new OperationContentBLL().Edit(operationContent);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("Get"), LoginRequired]
        public ApiResult Get(int id)
        {
            return new OperationContentBLL().Get(id);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="operationContent"></param>
        /// <returns></returns>
        [HttpPost, Route("Delete"), LoginRequired]
        public ApiResult Delete(OperationContent operationContent)
        {
            return new OperationContentBLL().Delete(operationContent.ID);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="electricalTaskType">作业类型</param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet, Route("List"), LoginRequired]
        public ApiResult List(ElectricalTaskType electricalTaskType, int page = 1, int limit = 10)
        {
            return new OperationContentBLL().List(electricalTaskType, page, limit);
        }
    }
}
