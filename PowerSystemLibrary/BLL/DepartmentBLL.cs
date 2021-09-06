using PowerSystemLibrary.DAO;
using PowerSystemLibrary.DBContext;
using PowerSystemLibrary.Entity;
using PowerSystemLibrary.Enum;
using PowerSystemLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PowerSystemLibrary.BLL
{
    public class DepartmentBLL
    {
        public ApiResult Add(Department department)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (TransactionScope ts = new TransactionScope())
            {
                using(PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    try
                    {
                        if (!ClassUtil.Validate<Department>(department, ref message))
                        {
                            throw new ExceptionUtil(message);
                        }
                        if (db.Department.FirstOrDefault(t => t.IsDelete != true && t.Name == department.Name && t.ParentID == department.ParentID) != null)
                        {
                            throw new ExceptionUtil("同级中已存在该部门名称，请检查部门名称");
                        }
                        if(db.Department.FirstOrDefault(t=>t.No == department.No && t.IsDelete!=true) != null)
                        {
                            throw new ExceptionUtil("部门编号重复");
                        }
                        department.IsDelete = false;
                        db.Department.Add(department);
                        db.SaveChanges();
                        new LogDAO().AddLog(LogCode.添加, "成功添加" + ClassUtil.GetEntityName(department) + ":" + department.Name, db);
                        result = ApiResult.NewSuccessJson("成功添加" + ClassUtil.GetEntityName(department) + ":" + department.Name);
                        ts.Complete();
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message.ToString();
                    }
                    finally
                    {
                        ts.Dispose();
                    }
                    if (!string.IsNullOrEmpty(message))
                    {
                        result = ApiResult.NewErrorJson(LogCode.添加错误, message, db);
                    }
                } 
            }
            return result;
        }

        public ApiResult Edit(Department department)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (TransactionScope ts = new TransactionScope())
            {
                using(PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    try
                    {
                        Department oldDepartment = db.Department.FirstOrDefault(t => t.ID == department.ID && t.IsDelete!=true);
                        if (oldDepartment == null)
                        {
                            throw new ExceptionUtil("未找到原部门");
                        }
                        if (!ClassUtil.Validate<Department>(department, ref message))
                        {
                            throw new ExceptionUtil(message);
                        }
                        if (db.Department.FirstOrDefault(t => t.IsDelete != true && t.Name == department.Name && t.ParentID == department.ParentID && t.ID != department.ID) != null)
                        {
                            throw new ExceptionUtil("同级中已存在该部门名称，请检查部门名称");
                        }
                        if (db.Department.FirstOrDefault(t => t.No == department.No && t.IsDelete != true && t.ID != department.ID) != null)
                        {
                            throw new ExceptionUtil("部门编号重复");
                        }

                        new ClassUtil().EditEntity(oldDepartment, department);
                        db.SaveChanges();
                        new LogDAO().AddLog(LogCode.修改, "成功修改" + ClassUtil.GetEntityName(department) + ":" + department.Name, db);
                        result = ApiResult.NewSuccessJson("成功修改" + ClassUtil.GetEntityName(department) + ":" + department.Name);
                        ts.Complete();
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message.ToString();
                    }
                    finally
                    {
                        ts.Dispose();
                    }
                    if (!string.IsNullOrEmpty(message))
                    {
                        result = ApiResult.NewErrorJson(LogCode.修改错误, message, db);
                    }
                }
            }
            return result;
        }

        public ApiResult Delete(int id)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    Department oldDepartment = db.Department.FirstOrDefault(t => t.ID == id && t.IsDelete != true);
                    if(oldDepartment == null)
                    {
                        throw new ExceptionUtil("该部门不存在");
                    }
                    Department childDepartment = db.Department.FirstOrDefault(t => t.ParentID == oldDepartment.ID && t.IsDelete !=true);
                    if(childDepartment != null)
                    {
                        throw new ExceptionUtil("该部门存在子部门，请先删除子部门");
                    }
                    oldDepartment.IsDelete = true;
                    db.SaveChanges();
                    new LogDAO().AddLog(LogCode.删除, "成功删除部门:" + oldDepartment.Name,db);
                    result = ApiResult.NewSuccessJson("成功删除部门:" + oldDepartment.Name);
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                if (!string.IsNullOrEmpty(message))
                {
                    result = ApiResult.NewErrorJson(LogCode.删除错误, message, db);
                }
            }
            return result;
        }

        public ApiResult Get(int id)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using(PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    Department department = db.Department.FirstOrDefault(t => t.ID == id && t.IsDelete != true);
                    if(department == null)
                    {
                        throw new ExceptionUtil("未找到该部门");
                    }
                    string parentName = string.Empty;
                    Department parentDepartment = db.Department.FirstOrDefault(t => t.ID == department.ParentID && t.IsDelete != true);
                    if (parentDepartment != null)
                    {
                        parentName = parentDepartment.Name;
                    }
                    result = ApiResult.NewSuccessJson(new
                    {
                        department.ID,
                        department.Name,
                        department.ParentID,
                        ParentName = parentName,
                        department.No,
                    });
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                if (!string.IsNullOrEmpty(message))
                {
                    result = ApiResult.NewErrorJson(LogCode.获取错误, message, db);
                }
            }
            return result;
        }

        public ApiResult List()
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using(PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    result = ApiResult.NewSuccessJson(
                    new
                    {
                        ChildList = new DepartmentDAO().List(db),
                    });
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                if (!string.IsNullOrEmpty(message))
                {
                    result = ApiResult.NewErrorJson(LogCode.获取错误, message, db);
                }
            }
            return result;
        }
    }
}
