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
using Aspose.Words;
using Aspose.Words.Tables;
using System.IO;
using System.Web;

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
                        if(ah.VoltageType == VoltageType.低压)
                        {
                            operation.OperationFlow = OperationFlow.低压停电作业申请;
                        }
                        else
                        {
                            operation.OperationFlow = OperationFlow.高压停电作业申请;
                        }
                        
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



                        //判断是否是高压，高压需要同时填写工作票和操作票
                        if (operation.VoltageType == VoltageType.高压)
                        {
                            if (operation.WorkSheet == null)
                            {
                                throw new ExceptionUtil("请填写" + ClassUtil.GetEntityName(new WorkSheet()));
                            }

                            if (!ClassUtil.Validate(operation.WorkSheet, ref message))
                            {
                                throw new ExceptionUtil(message);
                            }
                            WorkSheet workSheet = new WorkSheet();

                            string deputyUserWeChatIDString = "";
                            if (db.User.FirstOrDefault(t => t.ID == operation.WorkSheet.DeputyAuditUserID && t.IsDelete != true) == null)
                            {
                                throw new ExceptionUtil("请选择工作票副职审核人");
                            }
                            else
                            {
                                deputyUserWeChatIDString = db.User.FirstOrDefault(t => t.ID == operation.WorkSheet.DeputyAuditUserID && t.IsDelete != true).WeChatID;
                            }

                            if (db.User.FirstOrDefault(t => t.ID == operation.WorkSheet.ChiefAuditUserID && t.IsDelete != true) == null)
                            {
                                throw new ExceptionUtil("请选择工作票正职审核人");
                            }
                            var a = db.WorkSheet.Count(t => t.CreateDate >= nowDate) + 1;
                            workSheet.NO = SheetUtil.BuildNO(ah.VoltageType, SheetType.工作票, db.WorkSheet.Count(t => t.CreateDate >= nowDate) + 1);
                            workSheet.OperationID = operation.ID;
                            workSheet.UserID = loginUser.ID;
                            workSheet.AuditLevel = AuditLevel.副职审核;
                            workSheet.DepartmentID = loginUser.DepartmentID;
                            workSheet.BeginDate = operation.ApplicationSheet.BeginDate;
                            workSheet.EndDate = operation.ApplicationSheet.EndDate;
                            workSheet.WorkContent = operation.ApplicationSheet.WorkContent;
                            workSheet.CreateDate = now;
                            workSheet.SafetyMeasures = operation.WorkSheet.SafetyMeasures;
                            workSheet.DeputyAuditUserID = operation.WorkSheet.DeputyAuditUserID;
                            workSheet.DeputyAudit = Audit.待审核;
                            workSheet.ChiefAuditUserID = operation.WorkSheet.ChiefAuditUserID;
                            workSheet.ChiefAudit = Audit.待审核;
                            workSheet.Influence = operation.WorkSheet.Influence;
                            db.WorkSheet.Add(workSheet);
                            db.SaveChanges();


                            string sendMessage = WeChatAPI.SendMessage(accessToken, deputyUserWeChatIDString, ParaUtil.MessageAgentid, loginUser.Realname + "于" + now.ToString("yyyy-MM-dd HH:mm") + "申请" + ah.Name + "位置的" + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + ClassUtil.GetEntityName(operation)+"工作票待审核");
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

                    List<User> userList = db.User.ToList();

                    User user = userList.FirstOrDefault(t => t.ID == operation.UserID);
                    AH ah = db.AH.FirstOrDefault(t => t.ID == operation.AHID);

                    ApplicationSheet applicationSheet = db.ApplicationSheet.FirstOrDefault(t => t.OperationID == operation.ID);

                    //停电电工信息
                    
                    ElectricalTask stopElectricalTask = db.ElectricalTask.FirstOrDefault(t => t.OperationID == operation.ID && t.ElectricalTaskType == ElectricalTaskType.停电作业 && t.Audit!=Audit.待审核);
                    if (stopElectricalTask != null)
                    {
                        stopElectricalTask.RealName = userList.FirstOrDefault(u => u.ID == stopElectricalTask.AuditUserID).Realname;
                        stopElectricalTask.AuditDateString = stopElectricalTask.AuditDate.Value.ToString("yyyy-MM-dd HH:mm");
                        stopElectricalTask.AuditName = System.Enum.GetName(typeof(Audit), stopElectricalTask.Audit);
                        stopElectricalTask.ElectricalTaskTypeName = System.Enum.GetName(typeof(ElectricalTaskType), stopElectricalTask.ElectricalTaskType);
                        List<ElectricalTaskUser> stopElectricalTaskUserList = db.ElectricalTaskUser.Where(t => t.ElectricalTaskID == stopElectricalTask.ID && t.IsBack != true).OrderByDescending(t => t.Date).ToList();
                        stopElectricalTaskUserList.ForEach(t => t.CreateDate = t.Date.ToString("yyyy-MM-dd HH:mm"));
                        stopElectricalTaskUserList.ForEach(t => t.RealName = userList.FirstOrDefault(u => u.ID == t.UserID).Realname);
                        
                        stopElectricalTask.ElectricalTaskUserList = stopElectricalTaskUserList;

                        //操作票
                        OperationSheet stopOperationSheet = db.OperationSheet.FirstOrDefault(t => t.ElectricalTaskID == stopElectricalTask.ID);
                        if (stopOperationSheet != null)
                        {
                            stopOperationSheet.OperationUserName = userList.FirstOrDefault(t => t.ID == stopOperationSheet.OperationUserID).Realname;
                            stopOperationSheet.OperationDateString = stopOperationSheet.OperationDate.ToString("yyyy-MM-dd HH:ss");
                            stopOperationSheet.GuardianUserName = userList.FirstOrDefault(t => t.ID == stopOperationSheet.GuardianUserID).Realname;
                            stopOperationSheet.FinishDateString = stopOperationSheet.FinishDate.HasValue ? stopOperationSheet.FinishDate.Value.ToString("yyyy-MM-dd HH:ss") : "";
                        }

                        stopElectricalTask.OperationSheet = stopOperationSheet;

                    }

                    //送电电工信息
                    ElectricalTask sendElectricalTask = db.ElectricalTask.FirstOrDefault(t => t.OperationID == operation.ID && t.ElectricalTaskType == ElectricalTaskType.送电作业 && t.Audit != Audit.待审核);
                    if (sendElectricalTask != null)
                    {
                        sendElectricalTask.RealName = userList.FirstOrDefault(u => u.ID == stopElectricalTask.AuditUserID).Realname;
                        sendElectricalTask.AuditDateString = sendElectricalTask.AuditDate.Value.ToString("yyyy-MM-dd HH:mm");
                        sendElectricalTask.AuditName = System.Enum.GetName(typeof(Audit), sendElectricalTask.Audit);
                        sendElectricalTask.ElectricalTaskTypeName = System.Enum.GetName(typeof(ElectricalTaskType), sendElectricalTask.ElectricalTaskType);
                        List<ElectricalTaskUser> sendElectricalTaskUserList = db.ElectricalTaskUser.Where(t => t.ElectricalTaskID == sendElectricalTask.ID && t.IsBack != true).OrderByDescending(t => t.Date).ToList();
                        sendElectricalTaskUserList.ForEach(t => t.CreateDate = t.Date.ToString("yyyy-MM-dd HH:mm"));
                        sendElectricalTaskUserList.ForEach(t => t.RealName = userList.FirstOrDefault(u => u.ID == t.UserID).Realname);
                        sendElectricalTask.ElectricalTaskUserList = sendElectricalTaskUserList;

                        //操作票
                        OperationSheet sendOperationSheet = db.OperationSheet.FirstOrDefault(t => t.ElectricalTaskID == sendElectricalTask.ID);
                        if(sendOperationSheet != null)
                        {
                            sendOperationSheet.OperationUserName = userList.FirstOrDefault(t => t.ID == sendOperationSheet.OperationUserID).Realname;
                            sendOperationSheet.OperationDateString = sendOperationSheet.OperationDate.ToString("yyyy-MM-dd HH:ss");
                            sendOperationSheet.GuardianUserName = userList.FirstOrDefault(t => t.ID == sendOperationSheet.GuardianUserID).Realname;
                            sendOperationSheet.FinishDateString = sendOperationSheet.FinishDate.HasValue ? sendOperationSheet.FinishDate.Value.ToString("yyyy-MM-dd HH:ss") : "";
                        }

                        sendElectricalTask.OperationSheet = sendOperationSheet;
                    }

                    //高压工作票
                    WorkSheet workSheet = db.WorkSheet.FirstOrDefault(t => t.OperationID == operation.ID);
                    if(workSheet != null)
                    {
                        //副职审核信息
                        workSheet.DeputyAuditName = System.Enum.GetName(typeof(Audit), workSheet.DeputyAudit);
                        workSheet.DeputyAuditDateString = workSheet.DeputyAuditDate.HasValue ? workSheet.DeputyAuditDate.Value.ToString("yyyy-MM-dd HH:ss") : "";
                        workSheet.DeputyAuditUserName = userList.FirstOrDefault(t => t.ID == workSheet.DeputyAuditUserID).Realname;

                        //正职审核信息
                        workSheet.ChiefAuditName = System.Enum.GetName(typeof(Audit), workSheet.ChiefAudit);
                        workSheet.ChiefAuditDateString = workSheet.ChiefAuditDate.HasValue ? workSheet.ChiefAuditDate.Value.ToString("yyyy-MM-dd HH:ss") : "";
                        workSheet.ChiefAuditUserName = userList.FirstOrDefault(t => t.ID == workSheet.ChiefAuditUserID).Realname;
                    }

                    


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
                            AuditUserName = db.User.FirstOrDefault(t => t.ID == applicationSheet.AuditUserID).Realname,
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
                        },
                        stopElectricalTask,
                        sendElectricalTask,
                        workSheet
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
                        if (selectedOperation.OperationFlow == OperationFlow.低压停电任务完成 || selectedOperation.OperationFlow == OperationFlow.高压停电任务完成)
                        {

                        }
                        else
                        {
                            throw new ExceptionUtil("无法挂牌");
                        }
                        
                        AH ah = db.AH.FirstOrDefault(t => t.ID == selectedOperation.AHID);
                        
                        selectedOperation.OperationFlow = ah.VoltageType == VoltageType.低压 ? OperationFlow.低压挂停电牌作业 : OperationFlow.高压挂停电牌作业;
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
                        if (selectedOperation.OperationFlow == OperationFlow.低压挂停电牌作业 || selectedOperation.OperationFlow==OperationFlow.高压挂停电牌作业)
                        {

                        }
                        else
                        {
                            throw new ExceptionUtil("无法摘牌");
                        }

                        selectedOperation.OperationFlow = ah.VoltageType == VoltageType.低压? OperationFlow.低压检修作业完成 : OperationFlow.高压检修作业完成;
                        selectedOperation.IsConfirm = true;
                        db.SaveChanges();

                        //发消息给巡检通知剩余牌数，若无剩余牌数则需要确认送电任务
                        int surplusCount = db.Operation.Count(t => t.ID != selectedOperation.ID && t.AHID == selectedOperation.AHID && (t.IsConfirm != true && t.OperationFlow!= OperationFlow.作业终止));
                        List<Role> roleList = RoleUtil.GetDispatcherRoleList();
                        List<string> userWeChatIDList = db.User.Where(t => t.IsDelete != true && t.DepartmentID == loginUser.DepartmentID && db.UserRole.Where(m => roleList.Contains(m.Role)).Select(m => m.UserID).Contains(t.ID)).Select(t => t.WeChatID).ToList();
                        string userWeChatIDString = "";
                        foreach (string userWeChatID in userWeChatIDList)
                        {
                            userWeChatIDString = userWeChatIDString + userWeChatID + "|";
                        }
                        userWeChatIDString.TrimEnd('|');
                        string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.MessageSecret);
                        string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, loginUser.Realname + "成功摘牌,"+ah.Name+"剩余牌数为"+ surplusCount);

                        //查看该设备是否有其他正在执行的作业和任务，若没有则申请送电
                        if (surplusCount == 0)
                        {
                            //增加送电任务
                            //selectedOperation.OperationFlow = OperationFlow.低压送电任务领取;
                            ElectricalTask electricalTask = new ElectricalTask();
                            electricalTask.OperationID = selectedOperation.ID;
                            electricalTask.AHID = selectedOperation.AHID;
                            electricalTask.CreateDate = now;
                            electricalTask.ElectricalTaskType = ElectricalTaskType.送电作业;
                            db.ElectricalTask.Add(electricalTask);
                            db.SaveChanges();

                            ////发消息给所有电工
                            //List<Role> roleList = RoleUtil.GetElectricianRoleList();
                            //List<string> userWeChatIDList = db.User.Where(t => t.IsDelete != true && t.DepartmentID == loginUser.DepartmentID && db.UserRole.Where(m => roleList.Contains(m.Role)).Select(m => m.UserID).Contains(t.ID)).Select(t => t.WeChatID).ToList();
                            //string userWeChatIDString = "";
                            //foreach (string userWeChatID in userWeChatIDList)
                            //{
                            //    userWeChatIDString = userWeChatIDString + userWeChatID + "|";
                            //}
                            //userWeChatIDString.TrimEnd('|');
                            //string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.MessageSecret);
                            //string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, "有新的" + ah.Name + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + "送电任务");
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

        public ApiResult ExportWorkSheet(int id)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    Operation selectOperation = db.Operation.FirstOrDefault(t => t.ID == id);
                    if(selectOperation == null)
                    {
                        throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new Operation()));
                    }
                    AH aH = db.AH.FirstOrDefault(t => t.ID == selectOperation.AHID);
                    
                    WorkSheet workSheet = db.WorkSheet.FirstOrDefault(t => t.OperationID == selectOperation.ID);
                    if (workSheet == null)
                    {
                        throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new WorkSheet()));
                    }
                    List<User> userList = db.User.ToList();
                    Department department = db.Department.FirstOrDefault(t => t.ID == workSheet.DepartmentID);
                    User createUser = userList.FirstOrDefault(t => t.ID == workSheet.UserID);


                    ElectricalTask electricalTask = db.ElectricalTask.FirstOrDefault(t => t.OperationID == selectOperation.ID && t.ElectricalTaskType == ElectricalTaskType.送电作业);

                    string tempFile = ParaUtil.TempleteFileHtmlpath + "申请停送电工作票.docx";
                    string fielname = System.Web.HttpContext.Current.Server.MapPath(tempFile);
                    Aspose.Words.Document doc = new Aspose.Words.Document(fielname);
                    doc.Range.Replace("@NO", workSheet.NO, false, false);
                    doc.Range.Replace("@department", department.Name, false, false);
                    doc.Range.Replace("@createUser", createUser.Realname, false, false);
                    doc.Range.Replace("@content", workSheet.WorkContent, false, false);
                    doc.Range.Replace("@ahName", aH.Name, false, false);
                    doc.Range.Replace("@influence", workSheet.Influence, false, false);

                    int hour = 0;
                    int minute = 0;
                    TimeSpan timeSpan = workSheet.EndDate - workSheet.BeginDate;
                    int totalMinute = Convert.ToInt32(timeSpan.TotalMinutes);
                    if(totalMinute > 60)
                    {
                        hour = totalMinute / 60;
                        minute = totalMinute % 60;
                    }
                    else
                    {
                        minute = totalMinute;
                    }
                    doc.Range.Replace("@beginY", workSheet.BeginDate.Year.ToString(), false, false);
                    doc.Range.Replace("@beginMon", workSheet.BeginDate.Month.ToString(), false, false);
                    doc.Range.Replace("@beginD", workSheet.BeginDate.Day.ToString(), false, false);
                    doc.Range.Replace("@beginH", workSheet.BeginDate.Hour.ToString(), false, false);
                    doc.Range.Replace("@beginMinute", workSheet.BeginDate.Minute.ToString(), false, false);


                    doc.Range.Replace("@endY", workSheet.EndDate.Year.ToString(), false, false);
                    doc.Range.Replace("@endMon", workSheet.EndDate.Month.ToString(), false, false);
                    doc.Range.Replace("@endD", workSheet.EndDate.Day.ToString(), false, false);
                    doc.Range.Replace("@endH", workSheet.EndDate.Hour.ToString(), false, false);
                    doc.Range.Replace("@endMinute", workSheet.EndDate.Minute.ToString(), false, false);

                    doc.Range.Replace("@totalH", hour.ToString(), false, false);
                    doc.Range.Replace("@totalMinute", minute.ToString(), false, false);




                    doc.Range.Replace("@deputyAuditUser", userList.FirstOrDefault(t=>t.ID == workSheet.DeputyAuditUserID).Realname+"("+System.Enum.GetName(typeof(Audit), workSheet.DeputyAudit)+")", false, false);
                    doc.Range.Replace("@safetyMeasures", workSheet.SafetyMeasures, false, false);
                    doc.Range.Replace("@chiefAuditUser", userList.FirstOrDefault(t=>t.ID == workSheet.ChiefAuditUserID).Realname+"("+System.Enum.GetName(typeof(Audit), workSheet.ChiefAudit)+")", false, false);

                    SendElectricalSheet sendElectricalSheet = db.SendElectricalSheet.FirstOrDefault(t => t.OperationID == selectOperation.ID);
                    if(sendElectricalSheet == null)
                    {
                        doc.Range.Replace("@workFinishContent","", false, false);
                        doc.Range.Replace("@isRemoveGroundLine", "", false, false);
                        doc.Range.Replace("@isEvacuateAllPeople", "", false, false);
                        doc.Range.Replace("@sendCreateDate", "", false, false);
                        doc.Range.Replace("@sendElectricDate", "", false, false);
                        doc.Range.Replace("@operationUser", "", false, false);
                        doc.Range.Replace("@sendCreateUser", "", false, false);
                        doc.Range.Replace("@guardianUser", "", false, false);
                        doc.Range.Replace("@finishDate", "", false, false);
                    }
                    else
                    {
                        ElectricalTask sendElectricalTask = db.ElectricalTask.FirstOrDefault(t => t.OperationID == selectOperation.ID && t.ElectricalTaskType == ElectricalTaskType.送电作业);
                        OperationSheet operationSheet = db.OperationSheet.FirstOrDefault(t => t.ElectricalTaskID == sendElectricalTask.ID);
                        doc.Range.Replace("@workFinishContent", sendElectricalSheet.WorkFinishContent, false, false);
                        doc.Range.Replace("@isRemoveGroundLine", sendElectricalSheet.IsRemoveGroundLine?"是":"否", false, false);
                        doc.Range.Replace("@isEvacuateAllPeople", sendElectricalSheet.IsEvacuateAllPeople?"是":"否", false, false);
                        doc.Range.Replace("@sendCreateDate", sendElectricalSheet.CreateDate.ToString("yyyy年MM月dd日HH时ss分"), false, false);
                        doc.Range.Replace("@sendElectricDate", sendElectricalSheet.SendElectricDate.ToString("yyyy年MM月dd日HH时ss分"), false, false);
                        doc.Range.Replace("@operationUser", userList.FirstOrDefault(t=>t.ID == operationSheet.OperationUserID).Realname, false, false);
                        doc.Range.Replace("@sendCreateUser", createUser.Realname, false, false);
                        doc.Range.Replace("@guardianUser", userList.FirstOrDefault(t=>t.ID == operationSheet.GuardianUserID).Realname, false, false);
                        doc.Range.Replace("@finishDate", operationSheet.FinishDate.Value.ToString("yyyy年MM月dd日HH时ss分"), false, false);
                    }


                    string fileName = "申请停送电工作票" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".doc";
                    doc.Save(ParaUtil.ResourcePath + fileName);

                    using (FileStream fs = new FileStream(ParaUtil.ResourcePath + fileName, FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[(int)fs.Length];
                        fs.Read(bytes, 0, bytes.Length);
                        fs.Close();
                        HttpContext.Current.Response.ContentType = "application/octet-stream";
                        HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8));
                        HttpContext.Current.Response.BinaryWrite(bytes);
                        HttpContext.Current.Response.Flush();
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                    new LogDAO().AddLog(LogCode.导出, "成功导出停送电作业全流程表单", db);
                    result = ApiResult.NewSuccessJson("成功导出停送电作业全流程表单");
                }
                catch (Exception ex)
                {
                    message = ex.Message.ToString();
                }
                if (!string.IsNullOrEmpty(message))
                {
                    result = ApiResult.NewErrorJson(LogCode.导出错误, message, db);
                }
            }
            return result;
        }

    }
}
