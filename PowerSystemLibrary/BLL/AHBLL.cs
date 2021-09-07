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
    public class AHBLL
    {
        public ApiResult Add(AH aH)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (TransactionScope ts = new TransactionScope())
            {
                using (PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    try
                    {
                        if (!ClassUtil.Validate<AH>(aH, ref message))
                        {
                            throw new ExceptionUtil(message);
                        }
                        if (!System.Enum.IsDefined(typeof(VoltageType), aH.VoltageType))
                        {
                            throw new ExceptionUtil("请选择电压类型");
                        }
                        if (db.PowerSubstation.FirstOrDefault(t => t.IsDelete != true && t.ID == aH.PowerBubstationID) == null)
                        {
                            throw new ExceptionUtil("请选择所属变电所");
                        }
                        if (db.AH.FirstOrDefault(t => t.IsDelete != true && t.Name == aH.Name) != null)
                        {
                            throw new ExceptionUtil("开关柜名称重复");
                        }
                        aH.IsDelete = false;
                        aH.AHState = AHState.正常;
                        db.AH.Add(aH);
                        db.SaveChanges();
                        new LogDAO().AddLog(LogCode.添加, "成功添加" + ClassUtil.GetEntityName(aH) + ":" + aH.Name, db);
                        result = ApiResult.NewSuccessJson("成功添加" + ClassUtil.GetEntityName(aH) + ":" + aH.Name);
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

        public ApiResult Edit(AH aH)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (TransactionScope ts = new TransactionScope())
            {
                using (PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    try
                    {
                        AH oldAH = db.AH.FirstOrDefault(t => t.IsDelete != true && t.ID == aH.ID);
                        if (oldAH == null)
                        {
                            throw new ExceptionUtil("配电柜不存在");
                        }
                        if (!ClassUtil.Validate(aH, ref message))
                        {
                            throw new ExceptionUtil(message);
                        }
                        if (!System.Enum.IsDefined(typeof(VoltageType), aH.VoltageType))
                        {
                            throw new ExceptionUtil("请选择电压类型");
                        }
                        if (db.PowerSubstation.FirstOrDefault(t => t.IsDelete != true && t.ID == aH.PowerBubstationID) == null)
                        {
                            throw new ExceptionUtil("请选择所属变电所");
                        }
                        if (db.AH.FirstOrDefault(t => t.IsDelete != true && t.Name == aH.Name && t.ID != aH.ID) != null)
                        {
                            throw new ExceptionUtil("开关柜名称重复");
                        }
                        new ClassUtil().EditEntity(oldAH, aH);
                        db.SaveChanges();
                        new LogDAO().AddLog(LogCode.修改, "修改成功" + ClassUtil.GetEntityName(aH) + ":" + aH.Name, db);
                        result = ApiResult.NewSuccessJson("修改成功" + ClassUtil.GetEntityName(aH) + ":" + aH.Name);
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
                        result = ApiResult.NewErrorJson(LogCode.修改, message, db);
                    }
                }
            }
            return result;
        }

        public ApiResult Delete(List<int> IDList)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    List<AH> aHList = db.AH.Where(t => IDList.Contains(t.ID)).ToList();
                    if (aHList == null)
                    {
                        throw new ExceptionUtil("请至少选择一条" + ClassUtil.GetEntityName(new AH()));
                    }
                    aHList.ForEach(t => t.IsDelete = true);
                    db.SaveChanges();
                    new LogDAO().AddLog(LogCode.删除, "成功删除开关柜", db);
                    result = ApiResult.NewSuccessJson("成功删除开关柜");
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
                    AH oldAH = db.AH.FirstOrDefault(t => t.IsDelete != true && t.ID == id);
                    if(oldAH == null)
                    {
                        throw new ExceptionUtil("开关柜不存在");
                    }
                    result = ApiResult.NewSuccessJson(new
                    {
                        oldAH.ID,
                        oldAH.Name,
                        oldAH.PowerBubstationID,
                        PowerBubstationName = db.PowerSubstation.FirstOrDefault(t=>t.ID == oldAH.PowerBubstationID).Name,
                        oldAH.VoltageType,
                        VoltageTypeName = System.Enum.GetName(typeof(VoltageType), oldAH.VoltageType),
                        oldAH.AHState,
                        AHStateName = System.Enum.GetName(typeof(AHState), oldAH.AHState)

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

        public ApiResult List(string name="", VoltageType? voltageType = null, int page = 1, int limit = 10)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            name = name ?? string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    List<AH> aHList = db.AH.Where(t => t.Name.Contains(name) && (voltageType == null || t.VoltageType == voltageType)).ToList();
                    int total = aHList.Count;
                    aHList = aHList.Skip((page - 1) * limit).Take(limit).ToList();
                    List<int> powerBubstationIDList = aHList.Select(t => t.PowerBubstationID).Distinct().ToList();
                    List<PowerSubstation> powerBubstationList = db.PowerSubstation.Where(t => powerBubstationIDList.Contains(t.ID)).ToList();

                    List<object> returnList = new List<object>();
                    foreach (AH aH in aHList)
                    {
                        returnList.Add(new
                        {
                            aH.ID,
                            aH.Name,
                            VoltageTypeName = System.Enum.GetName(typeof(VoltageType), aH.VoltageType),
                            AHStateName = System.Enum.GetName(typeof(AHState), aH.AHState),
                            PowerBubstationName = powerBubstationList.FirstOrDefault(t => t.ID == aH.PowerBubstationID).Name
                        });
                    }
                    result = ApiResult.NewSuccessJson(returnList, total);
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
