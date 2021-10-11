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
    public class PowerSubstationBLL
    {
        public ApiResult List(int page=1,int limit=10)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    List<PowerSubstation> powerSubstationList = db.PowerSubstation.Where(t => t.IsDelete != true).ToList();
                    int total = powerSubstationList.Count;
                    powerSubstationList = powerSubstationList.Skip((page - 1) * limit).Take(limit).ToList();
                    
                    result = ApiResult.NewSuccessJson(powerSubstationList,total);
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

        public ApiResult Add(PowerSubstation powerSubstation)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (TransactionScope ts = new TransactionScope())
            {
                using(PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    try
                    {
                        if (!ClassUtil.Validate<PowerSubstation>(powerSubstation, ref message))
                        {
                            throw new ExceptionUtil(message);
                        }
                        if(db.PowerSubstation.FirstOrDefault(t=>t.Name == powerSubstation.Name && t.IsDelete != true) != null)
                        {
                            throw new ExceptionUtil("变电所名称重复");
                        }
                        powerSubstation.IsDelete = false;
                        db.PowerSubstation.Add(powerSubstation);
                        db.SaveChanges();
                        new LogDAO().AddLog(LogCode.添加, "成功添加" + ClassUtil.GetEntityName(powerSubstation) + ":" + powerSubstation.Name, db);
                        result = ApiResult.NewSuccessJson("成功添加" + ClassUtil.GetEntityName(powerSubstation) + ":" + powerSubstation.Name);
                        ts.Complete();
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
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

        public ApiResult Edit(PowerSubstation powerSubstation)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (TransactionScope ts = new TransactionScope())
            {
                using(PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    try
                    {
                        PowerSubstation oldPowerSubstation = db.PowerSubstation.FirstOrDefault(t => t.IsDelete != true && t.ID == powerSubstation.ID);
                        if (oldPowerSubstation == null)
                        {
                            throw new ExceptionUtil(ClassUtil.GetEntityName(new PowerSubstation()) + "不存在");
                        }
                        if (!ClassUtil.Validate<PowerSubstation>(powerSubstation, ref message))
                        {
                            throw new ExceptionUtil(message);
                        }
                        if (db.PowerSubstation.FirstOrDefault(t => t.IsDelete != true && t.Name == powerSubstation.Name && t.ID != powerSubstation.ID) != null)
                        {
                            throw new ExceptionUtil(ClassUtil.GetEntityName(new PowerSubstation()) + "名称重复");
                        }
                        new ClassUtil().EditEntity(oldPowerSubstation, powerSubstation);
                        db.SaveChanges();
                        new LogDAO().AddLog(LogCode.修改, "修改成功" + ClassUtil.GetEntityName(powerSubstation) + ":" + powerSubstation.Name, db);
                        result = ApiResult.NewSuccessJson("修改成功" + ClassUtil.GetEntityName(powerSubstation) + ":" + powerSubstation.Name);
                        ts.Complete();
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
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
                    PowerSubstation powerSubstation = db.PowerSubstation.FirstOrDefault(t => t.ID == id);
                    if (powerSubstation == null)
                    {
                        throw new ExceptionUtil(ClassUtil.GetEntityName(new PowerSubstation()) + "不存在");
                    }
                    powerSubstation.IsDelete = true;
                    db.SaveChanges();
                    new LogDAO().AddLog(LogCode.删除, "成功删除" + ClassUtil.GetEntityName(new PowerSubstation()) + ":" + powerSubstation.Name, db);
                    result = ApiResult.NewSuccessJson("成功删除" + ClassUtil.GetEntityName(new PowerSubstation()));
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
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    PowerSubstation powerSubstation = db.PowerSubstation.FirstOrDefault(t => t.IsDelete != true && t.ID == id);
                    if (powerSubstation == null)
                    {
                        throw new ExceptionUtil(ClassUtil.GetEntityName(new PowerSubstation()) + "不存在");
                    }
                    result = ApiResult.NewSuccessJson(powerSubstation);
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
