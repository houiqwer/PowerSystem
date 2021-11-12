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
    public class OperationContentBLL
    {
        public ApiResult Add(OperationContent operationContent)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (TransactionScope ts = new TransactionScope())
            {
                using (PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    try
                    {
                        if (!ClassUtil.Validate<OperationContent>(operationContent, ref message))
                        {
                            throw new ExceptionUtil(message);
                        }
                        db.OperationContent.Add(operationContent);
                        db.SaveChanges();
                        new LogDAO().AddLog(LogCode.添加, "成功添加" + ClassUtil.GetEntityName(operationContent), db);
                        result = ApiResult.NewSuccessJson("成功添加" + ClassUtil.GetEntityName(operationContent));
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

        public ApiResult Edit(OperationContent operationContent)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (TransactionScope ts = new TransactionScope())
            {
                using(PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    try
                    {
                        OperationContent oldOperationContent = db.OperationContent.FirstOrDefault(t => t.IsDelete != true && t.ID == operationContent.ID);
                        if (oldOperationContent == null)
                        {
                            throw new ExceptionUtil(ClassUtil.GetEntityName(new OperationContent()) + "不存在");
                        }
                        if (!ClassUtil.Validate(operationContent, ref message))
                        {
                            throw new ExceptionUtil(message);
                        }
                        new ClassUtil().EditEntity(oldOperationContent, operationContent);
                        db.SaveChanges();
                        new LogDAO().AddLog(LogCode.修改, "修改成功" + ClassUtil.GetEntityName(operationContent), db);
                        result = ApiResult.NewSuccessJson("修改成功" + ClassUtil.GetEntityName(operationContent));
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

        public ApiResult Get(int id)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    OperationContent operationContent = db.OperationContent.FirstOrDefault(t => t.IsDelete != true && t.ID == id);
                    if (operationContent == null)
                    {
                        throw new ExceptionUtil(ClassUtil.GetEntityName(new OperationContent()) + "不存在");
                    }
                    result = ApiResult.NewSuccessJson(operationContent);
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

        public ApiResult Delete(int id)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    OperationContent operationContent = db.OperationContent.FirstOrDefault(t => t.ID == id);
                    if (operationContent == null)
                    {
                        throw new ExceptionUtil(ClassUtil.GetEntityName(new OperationContent()) + "不存在");
                    }
                    operationContent.IsDelete = true;
                    db.SaveChanges();
                    new LogDAO().AddLog(LogCode.删除, "成功删除" + ClassUtil.GetEntityName(new OperationContent()), db);
                    result = ApiResult.NewSuccessJson("成功删除" + ClassUtil.GetEntityName(new OperationContent()));
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

        public ApiResult List(ElectricalTaskType electricalTaskType, int page=1,int limit=10)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    List<OperationContent> operationContentList = db.OperationContent.Where(t => t.IsDelete != true && t.ElectricalTaskType == electricalTaskType).ToList();
                    int total = operationContentList.Count;
                    operationContentList = operationContentList.Skip((page - 1) * limit).Take(limit).ToList();
                    result = ApiResult.NewSuccessJson(operationContentList, total);
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
