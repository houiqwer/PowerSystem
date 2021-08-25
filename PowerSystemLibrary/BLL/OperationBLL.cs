using PowerSystemLibrary.Enum;
using PowerSystemLibrary.Util;
using PowerSystemLibrary.Entity;
using PowerSystemLibrary.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using PowerSystemLibrary.DAO;

namespace PowerSystemLibrary.BLL
{
    public class OperationBLL
    {
        public ApiResult Add(Operation operation)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (TransactionScope ts = new TransactionScope())
            {
                using (PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    try
                    {

                        if (!ClassUtil.Validate(operation, ref message))
                        {
                            throw new ExceptionUtil(message);
                        }

                        User loginUser = LoginHelper.CurrentUser(db);
                        operation.UserID = loginUser.ID;

                        AH ah = db.AH.FirstOrDefault(t => t.ID == operation.AHID);
                        if (ah == null)
                        {
                            throw new ExceptionUtil("请选择" + ClassUtil.GetEntityName(new AH()));
                        }


                        operation.VoltageType = ah.VoltageType;

                        new LogDAO().AddLog(LogCode.添加, loginUser.Realname + "成功申请" + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + ClassUtil.GetEntityName(operation), db);
                        result = ApiResult.NewSuccessJson("成功申请" + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + ClassUtil.GetEntityName(operation));
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
    }
}
