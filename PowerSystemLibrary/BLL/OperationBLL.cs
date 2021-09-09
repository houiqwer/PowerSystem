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
                        DateTime now = DateTime.Now;
                        DateTime nowDate = now.Date;

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

                        if (operation.ApplicationSheet == null)
                        {
                            throw new ExceptionUtil("请填写" + ClassUtil.GetEntityName(new ApplicationSheet()));
                        }

                        if (!ClassUtil.Validate(operation.ApplicationSheet, ref message))
                        {
                            throw new ExceptionUtil(message);
                        }


                        operation.VoltageType = ah.VoltageType;
                        operation.OperationFlow = OperationFlow.低压停电作业申请;
                        operation.CreateDate = now;
                        db.Operation.Add(operation);
                        db.SaveChanges();

                        ApplicationSheet applicationSheet = new ApplicationSheet();

                        string userWeChatIDString = "";
                        if (operation.ApplicationSheet.AuditUserID == null || db.User.FirstOrDefault(t=>t.ID == operation.ApplicationSheet.AuditUserID && t.IsDelete!=true) == null)
                        {
                            throw new ExceptionUtil("请选择审核人");
                        }
                        else
                        {
                            userWeChatIDString = db.User.FirstOrDefault(t => t.ID == operation.ApplicationSheet.AuditUserID && t.IsDelete != true).WeChatID;
                        }

                        applicationSheet.NO = SheetUtil.BuildNO(ah.VoltageType, SheetType.申请单, db.ApplicationSheet.Count(t => t.CreateDate >= nowDate) + 1);
                        applicationSheet.OperationID = operation.ID;
                        applicationSheet.UserID = loginUser.ID;
                        applicationSheet.DepartmentID = loginUser.DepartmentID;
                        applicationSheet.BeginDate = operation.ApplicationSheet.BeginDate;
                        applicationSheet.EndDate = operation.ApplicationSheet.EndDate;
                        applicationSheet.WorkContent = operation.ApplicationSheet.WorkContent;
                        applicationSheet.CreateDate = now;
                        applicationSheet.AuditUserID = operation.ApplicationSheet.AuditUserID;
                        db.ApplicationSheet.Add(applicationSheet);
                        db.SaveChanges();

                        //发送审核消息-部门副职及以上
                        //List<Role> roleList = RoleUtil.GetApplicationSheetAuditRoleList();
                        //List<UserRole> userRoleList = db.UserRole.Where(m => roleList.Contains(m.Role)).ToList();

                        //List<string> userWeChatIDList = db.User.Where(t => t.IsDelete != true && t.DepartmentID == loginUser.DepartmentID && db.UserRole.Where(m => roleList.Contains(m.Role)).Select(m => m.UserID).Contains(t.ID)).Select(t => t.WeChatID).ToList();
                        //string userWeChatIDString = "";
                        //foreach (string userWeChatID in userWeChatIDList)
                        //{
                        //    userWeChatIDString = userWeChatIDString + userWeChatID + "|";
                        //}
                        //userWeChatIDString.TrimEnd('|');
                        

                        string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.MessageSecret);
                        string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, loginUser.Realname + "于" + now.ToString("yyyy-MM-dd HH:mm") + "申请" + ah.Name + "位置的" + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + ClassUtil.GetEntityName(operation));



                        //判断是否因为高压，高压需要同时填写工作票和操作票
                        if (operation.VoltageType == VoltageType.高压)
                        {

                        }


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

        public ApiResult Get(int id)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;

            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    User loginUser = LoginHelper.CurrentUser(db);
                    Operation operation = db.Operation.FirstOrDefault(t => t.ID == id);

                    if (operation == null)
                    {
                        throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new Operation()));
                    }

                    User user = db.User.FirstOrDefault(t => t.ID == operation.UserID);
                    AH ah = db.AH.FirstOrDefault(t => t.ID == operation.AHID);

                    ApplicationSheet applicationSheet = db.ApplicationSheet.FirstOrDefault(t => t.OperationID == operation.ID);
                    result = ApiResult.NewSuccessJson(new
                    {
                        operation.ID,
                        user.Realname,
                        AHName = ah.Name,
                        CreateDate = operation.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                        VoltageType = System.Enum.GetName(typeof(VoltageType), operation.VoltageType),
                        OperationFlow = System.Enum.GetName(typeof(OperationFlow), operation.OperationFlow),
                        OperationFlowID = operation.OperationFlow,
                        operation.IsFinish,
                        operation.IsConfirm,
                        IsUser = user.ID == loginUser.ID,
                        ApplicationSheet = new
                        {
                            applicationSheet.ID,
                            applicationSheet.WorkContent,
                            AuditUserName = db.User.FirstOrDefault(t => t.ID == applicationSheet.UserID).Realname,
                            applicationSheet.AuditMessage,
                            AuditDate = applicationSheet.AuditDate.HasValue ? applicationSheet.AuditDate.Value.ToString("yyyy-MM-dd HH:mm") : null,
                            user.Realname,
                            AHName = ah.Name,
                            CreateDate = applicationSheet.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                            BeginDate = applicationSheet.BeginDate.ToString("yyyy-MM-dd HH:mm"),
                            EndDate = applicationSheet.EndDate.ToString("yyyy-MM-dd HH:mm"),
                            VoltageType = System.Enum.GetName(typeof(VoltageType), operation.VoltageType),
                            OperationFlow = System.Enum.GetName(typeof(OperationFlow), operation.OperationFlow),
                            Audit = System.Enum.GetName(typeof(Audit), applicationSheet.Audit),
                            DepartmentName =db.Department.FirstOrDefault(t => t.ID == applicationSheet.DepartmentID).Name,
                        }
                    });

                }
                catch (Exception ex)
                {
                    message = ex.Message.ToString();
                }

                if (!string.IsNullOrEmpty(message))
                {
                    result = ApiResult.NewErrorJson(LogCode.获取错误, message, db);
                }
            }
            return result;
        }

        public ApiResult List(int? departmentID = null, VoltageType? voltageType = null, int? ahID = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;

            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    beginDate = beginDate ?? DateTime.MinValue;
                    endDate = endDate ?? DateTime.MaxValue;
                    User loginUser = LoginHelper.CurrentUser(db);

                    IQueryable<Operation> operationIQueryable = db.Operation.Where(t =>
                    (departmentID == null || db.User.Where(m => m.DepartmentID == departmentID).Select(m => m.ID).Contains(t.UserID)) &&
                    (ahID == null || t.AHID == ahID) &&
                    (voltageType == null || t.VoltageType == voltageType) &&
                    (t.CreateDate >= beginDate && t.CreateDate <= endDate));
                    int total = operationIQueryable.Count();
                    List<Operation> operationList = operationIQueryable.OrderByDescending(t => t.CreateDate).Skip((page - 1) * limit).Take(limit).ToList();
                    List<int> ahIDList = operationList.Select(t => t.AHID).Distinct().ToList();
                    List<int> userIDList = operationList.Select(t => t.UserID).Distinct().ToList();
                    List<int> operationIDList = operationList.Select(t => t.ID).ToList();

                    List<object> returnList = new List<object>();
                    List<AH> ahList = db.AH.Where(t => ahIDList.Contains(t.ID)).ToList();
                    List<User> userList = db.User.Where(t => userIDList.Contains(t.ID)).ToList();
                    List<ApplicationSheet> applicationSheetList = db.ApplicationSheet.Where(t => operationIDList.Contains(t.OperationID)).ToList();

                    foreach (Operation operation in operationList)
                    {
                        ApplicationSheet applicationSheet = applicationSheetList.FirstOrDefault(t => t.OperationID == operation.ID);

                        //高压需要增加其他表单

                        returnList.Add(new
                        {
                            operation.ID,
                            userList.FirstOrDefault(t => t.ID == operation.UserID).Realname,
                            AHName = ahList.FirstOrDefault(t => t.ID == operation.AHID).Name,
                            CreateDate = operation.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                            VoltageType = System.Enum.GetName(typeof(VoltageType), operation.VoltageType),
                            OperationFlow = System.Enum.GetName(typeof(OperationFlow), operation.OperationFlow),
                            operation.IsFinish,
                            operation.IsConfirm,
                            ApplicationSheet = new
                            {
                                applicationSheet.ID,
                                userList.FirstOrDefault(t => t.ID == applicationSheet.UserID).Realname,
                                AHName = ahList.FirstOrDefault(t => t.ID == operation.AHID).Name,
                                CreateDate = applicationSheet.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                                BeginDate = applicationSheet.BeginDate.ToString("yyyy-MM-dd HH:mm"),
                                EndDate = applicationSheet.EndDate.ToString("yyyy-MM-dd HH:mm"),
                                VoltageType = System.Enum.GetName(typeof(VoltageType), operation.VoltageType),
                                OperationFlow = System.Enum.GetName(typeof(OperationFlow), operation.OperationFlow),
                                Audit = System.Enum.GetName(typeof(Audit), applicationSheet.Audit),
                            }
                        });
                    }

                    result = ApiResult.NewSuccessJson(returnList, total);

                }
                catch (Exception ex)
                {
                    message = ex.Message.ToString();
                }

                if (!string.IsNullOrEmpty(message))
                {
                    result = ApiResult.NewErrorJson(LogCode.获取错误, message, db);
                }
            }
            return result;
        }


        public ApiResult MyList(VoltageType? voltageType = null, int? ahID = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;

            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    beginDate = beginDate ?? DateTime.MinValue;
                    endDate = endDate ?? DateTime.MaxValue;
                    User loginUser = LoginHelper.CurrentUser(db);

                    IQueryable<Operation> operationIQueryable = db.Operation.Where(t =>
                    t.UserID == loginUser.ID &&
                    (ahID == null || t.AHID == ahID) &&
                    (voltageType == null || t.VoltageType == voltageType) &&
                    (t.CreateDate >= beginDate && t.CreateDate <= endDate));
                    int total = operationIQueryable.Count();
                    List<Operation> operationList = operationIQueryable.OrderByDescending(t => t.CreateDate).Skip((page - 1) * limit).Take(limit).ToList();
                    List<int> ahIDList = operationList.Select(t => t.AHID).Distinct().ToList();
                    List<int> userIDList = operationList.Select(t => t.UserID).Distinct().ToList();
                    List<int> operationIDList = operationList.Select(t => t.ID).ToList();

                    List<object> returnList = new List<object>();
                    List<AH> ahList = db.AH.Where(t => ahIDList.Contains(t.ID)).ToList();
                    List<User> userList = db.User.Where(t => userIDList.Contains(t.ID)).ToList();
                    List<ApplicationSheet> applicationSheetList = db.ApplicationSheet.Where(t => operationIDList.Contains(t.OperationID)).ToList();

                    foreach (Operation operation in operationList)
                    {
                        ApplicationSheet applicationSheet = applicationSheetList.FirstOrDefault(t => t.OperationID == operation.ID);

                        //高压需要增加其他表单

                        returnList.Add(new
                        {
                            operation.ID,
                            userList.FirstOrDefault(t => t.ID == operation.UserID).Realname,
                            AHName = ahList.FirstOrDefault(t => t.ID == operation.AHID).Name,
                            CreateDate = operation.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                            VoltageType = System.Enum.GetName(typeof(VoltageType), operation.VoltageType),
                            OperationFlow = System.Enum.GetName(typeof(OperationFlow), operation.OperationFlow),
                            operation.IsFinish,
                            operation.IsConfirm,
                            ApplicationSheet = new
                            {
                                applicationSheet.ID,
                                userList.FirstOrDefault(t => t.ID == applicationSheet.UserID).Realname,
                                AHName = ahList.FirstOrDefault(t => t.ID == operation.AHID).Name,
                                CreateDate = applicationSheet.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                                BeginDate = applicationSheet.BeginDate.ToString("yyyy-MM-dd HH:mm"),
                                EndDate = applicationSheet.EndDate.ToString("yyyy-MM-dd HH:mm"),
                                VoltageType = System.Enum.GetName(typeof(VoltageType), operation.VoltageType),
                                OperationFlow = System.Enum.GetName(typeof(OperationFlow), operation.OperationFlow),
                                Audit = System.Enum.GetName(typeof(Audit), applicationSheet.Audit),
                            }
                        });
                    }

                    result = ApiResult.NewSuccessJson(returnList, total);

                }
                catch (Exception ex)
                {
                    message = ex.Message.ToString();
                }

                if (!string.IsNullOrEmpty(message))
                {
                    result = ApiResult.NewErrorJson(LogCode.获取错误, message, db);
                }
            }
            return result;
        }


        public ApiResult Hang(Operation operation)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (TransactionScope ts = new TransactionScope())
            {
                using (PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    try
                    {
                        DateTime now = DateTime.Now;
                        User loginUser = LoginHelper.CurrentUser(db);

                        Operation selectedOperation = db.Operation.FirstOrDefault(t => t.ID == operation.ID && t.UserID == loginUser.ID);

                        if (selectedOperation == null)
                        {
                            throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new Operation()));
                        }

                        //这里需要补充所有情况
                        if (selectedOperation.OperationFlow != OperationFlow.低压停电任务完成)
                        {
                            throw new ExceptionUtil("无法挂牌");
                        }

                        selectedOperation.OperationFlow = OperationFlow.低压挂停电牌作业;
                        db.SaveChanges();

                        new LogDAO().AddLog(LogCode.挂牌, loginUser.Realname + "成功挂牌", db);
                        result = ApiResult.NewSuccessJson("成功挂牌");
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
                        result = ApiResult.NewErrorJson(LogCode.挂牌错误, message, db);
                    }
                }
            }
            return result;
        }


        public ApiResult Pick(Operation operation)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (TransactionScope ts = new TransactionScope())
            {
                using (PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    try
                    {
                        DateTime now = DateTime.Now;
                        User loginUser = LoginHelper.CurrentUser(db);

                        Operation selectedOperation = db.Operation.FirstOrDefault(t => t.ID == operation.ID && t.UserID == loginUser.ID);
                        AH ah = db.AH.FirstOrDefault(t => t.ID == selectedOperation.AHID);
                        if (selectedOperation == null)
                        {
                            throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new Operation()));
                        }

                        //这里需要补充所有情况
                        if (selectedOperation.OperationFlow != OperationFlow.低压挂停电牌作业)
                        {
                            throw new ExceptionUtil("无法摘牌");
                        }

                        selectedOperation.OperationFlow = OperationFlow.低压检修作业完成;
                        selectedOperation.IsConfirm = true;
                        db.SaveChanges();

                        //查看该设备是否有其他正在执行的作业和任务，若没有则申请送电
                        if (db.Operation.Count(t => t.ID != selectedOperation.ID && t.AHID == selectedOperation.AHID && t.IsConfirm != true) == 0)
                        {
                            //增加送电任务
                            //selectedOperation.OperationFlow = OperationFlow.低压送电任务领取;
                            ElectricalTask electricalTask = new ElectricalTask();
                            electricalTask.OperationID = selectedOperation.ID;
                            electricalTask.AHID = selectedOperation.AHID;
                            electricalTask.CreateDate = now;
                            electricalTask.ElectricalTaskType = ElectricalTaskType.送电作业;
                            db.ElectricalTask.Add(electricalTask);

                            //发消息给所有电工
                            List<Role> roleList = RoleUtil.GetElectricianRoleList();
                            List<string> userWeChatIDList = db.User.Where(t => t.IsDelete != true && t.DepartmentID == loginUser.DepartmentID && db.UserRole.Where(m => roleList.Contains(m.Role)).Select(m => m.UserID).Contains(t.ID)).Select(t => t.WeChatID).ToList();
                            string userWeChatIDString = "";
                            foreach (string userWeChatID in userWeChatIDList)
                            {
                                userWeChatIDString = userWeChatIDString + userWeChatID + "|";
                            }
                            userWeChatIDString.TrimEnd('|');
                            string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.MessageSecret);
                            string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, "有新的" + ah.Name + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + "送电任务");
                        }

                        new LogDAO().AddLog(LogCode.摘牌, loginUser.Realname + "成功摘牌", db);
                        result = ApiResult.NewSuccessJson("成功摘牌");
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
                        result = ApiResult.NewErrorJson(LogCode.摘牌错误, message, db);
                    }
                }
            }
            return result;
        }

    }
}
