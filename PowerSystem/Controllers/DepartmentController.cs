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
    /// 部门
    /// </summary>
    [RoutePrefix("Department")]
    public class DepartmentController : ApiController
    {
        /// <summary>
        /// 添加部门
        /// </summary>
        /// <param name="department">部门实体</param>
        /// <returns></returns>
        [HttpPost, Route("Add"), LoginRequired]
        public ApiResult Add([FromBody] Department department)
        {
            return new DepartmentBLL().Add(department);
        }

        /// <summary>
        /// 修改部门
        /// </summary>
        /// <param name="department">部门实体</param>
        /// <returns></returns>
        [HttpPost, Route("Edit"), LoginRequired]
        public ApiResult Edit([FromBody] Department department)
        {
            return new DepartmentBLL().Edit(department);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        [HttpPost, Route("Delete"), LoginRequired]
        public ApiResult Delete(Department department)
        {
            return new DepartmentBLL().Delete(department.ID);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet, Route("Get"), LoginRequired]
        public ApiResult Get(int ID)
        {
            return new DepartmentBLL().Get(ID);
        }

        /// <summary>
        /// 部门树
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("List"), LoginRequired]
        public ApiResult List()
        {
            return new DepartmentBLL().List();
        }
    }
}
