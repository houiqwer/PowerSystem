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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

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
                        if (ah.VoltageType == VoltageType.低压)
                        {
                            operation.OperationFlow = OperationFlow.低压停电作业申请;
                        }
                        else
                        {
                            operation.OperationFlow = OperationFlow.高压停电作业申请;
                        }

                        operation.CreateDate = now;
                        operation.OperationAudit = OperationAudit.待审核;
                        db.Operation.Add(operation);
                        db.SaveChanges();

                        ApplicationSheet applicationSheet = new ApplicationSheet();

                        string userWeChatIDString = "";
                        if (operation.ApplicationSheet.AuditUserID == null || db.User.FirstOrDefault(t => t.ID == operation.ApplicationSheet.AuditUserID && t.IsDelete != true) == null)
                        {
                            throw new ExceptionUtil("请选择审核人");
                        }
                        else
                        {
                            userWeChatIDString = db.User.FirstOrDefault(t => t.ID == operation.ApplicationSheet.AuditUserID && t.IsDelete != true).WeChatID;
                        }

                        applicationSheet.NO = Util.SheetUtil.BuildNO(ah.VoltageType, SheetType.申请单, db.ApplicationSheet.Count(t => t.CreateDate >= nowDate) + 1);
                        applicationSheet.OperationID = operation.ID;
                        applicationSheet.UserID = loginUser.ID;
                        applicationSheet.DepartmentID = loginUser.DepartmentID;
                        applicationSheet.BeginDate = operation.ApplicationSheet.BeginDate;
                        applicationSheet.EndDate = operation.ApplicationSheet.EndDate;
                        //applicationSheet.WorkContent = operation.ApplicationSheet.WorkContent;
                        applicationSheet.WorkContentType = operation.ApplicationSheet.WorkContentType;
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



                            string monitorUserWeChatIDString = "";

                            if (db.User.FirstOrDefault(t => t.ID == operation.WorkSheet.MonitorAuditUserID && t.IsDelete != true) == null)
                            {
                                throw new ExceptionUtil("请选择工作票班长审核人");
                            }
                            else
                            {
                                monitorUserWeChatIDString = db.User.FirstOrDefault(t => t.ID == operation.WorkSheet.MonitorAuditUserID && t.IsDelete != true).WeChatID;
                            }

                            if (db.User.FirstOrDefault(t => t.ID == operation.WorkSheet.DeputyAuditUserID && t.IsDelete != true) == null)
                            {
                                throw new ExceptionUtil("请选择工作票副职审核人");
                            }


                            if (db.User.FirstOrDefault(t => t.ID == operation.WorkSheet.ChiefAuditUserID && t.IsDelete != true) == null)
                            {
                                throw new ExceptionUtil("请选择工作票正职审核人");
                            }

                            workSheet.NO = Util.SheetUtil.BuildNO(ah.VoltageType, SheetType.工作票, db.WorkSheet.Count(t => t.CreateDate >= nowDate) + 1);
                            workSheet.OperationID = operation.ID;
                            workSheet.UserID = loginUser.ID;
                            workSheet.AuditLevel = AuditLevel.班长审核;
                            workSheet.DepartmentID = loginUser.DepartmentID;
                            workSheet.BeginDate = operation.ApplicationSheet.BeginDate;
                            workSheet.EndDate = operation.ApplicationSheet.EndDate;
                            //workSheet.WorkContent = operation.ApplicationSheet.WorkContent;
                            workSheet.WorkContentType = operation.ApplicationSheet.WorkContentType;
                            workSheet.CreateDate = now;
                            //workSheet.SafetyMeasures = operation.WorkSheet.SafetyMeasures;
                            //班长审核
                            workSheet.MonitorAuditUserID = operation.WorkSheet.MonitorAuditUserID;
                            workSheet.MonitorAudit = Audit.待审核;

                            workSheet.DeputyAuditUserID = operation.WorkSheet.DeputyAuditUserID;
                            workSheet.DeputyAudit = Audit.待审核;
                            workSheet.ChiefAuditUserID = operation.WorkSheet.ChiefAuditUserID;
                            workSheet.ChiefAudit = Audit.待审核;
                            workSheet.Influence = operation.WorkSheet.Influence;
                            db.WorkSheet.Add(workSheet);
                            db.SaveChanges();


                            string sendMessage = WeChatAPI.SendMessage(accessToken, monitorUserWeChatIDString, ParaUtil.MessageAgentid, loginUser.Realname + "于" + now.ToString("yyyy-MM-dd HH:mm") + "申请" + ah.Name + "位置的" + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + ClassUtil.GetEntityName(operation) + "工作票待审核");
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

                    ElectricalTask stopElectricalTask = db.ElectricalTask.FirstOrDefault(t => t.OperationID == operation.ID && t.ElectricalTaskType == ElectricalTaskType.停电作业 && t.DispatcherAudit != DispatcherAudit.待审核);
                    if (stopElectricalTask != null)
                    {
                        stopElectricalTask.RealName = userList.FirstOrDefault(u => u.ID == stopElectricalTask.AuditUserID).Realname;
                        stopElectricalTask.AuditDateString = stopElectricalTask.AuditDate.Value.ToString("yyyy-MM-dd HH:mm");
                        //stopElectricalTask.AuditName = System.Enum.GetName(typeof(Audit), stopElectricalTask.Audit);
                        stopElectricalTask.AuditName = System.Enum.GetName(typeof(DispatcherAudit), stopElectricalTask.DispatcherAudit);
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
                            stopOperationSheet.GuardianUserName = stopOperationSheet.GuardianUserID != null ? userList.FirstOrDefault(t => t.ID == stopOperationSheet.GuardianUserID).Realname : "";
                            stopOperationSheet.FinishDateString = stopOperationSheet.FinishDate.HasValue ? stopOperationSheet.FinishDate.Value.ToString("yyyy-MM-dd HH:ss") : "";
                            stopOperationSheet.OperationContentList = db.OperationContent.Where(o => db.OperationSheet_Content.Where(t => t.OperationSheetID == stopOperationSheet.ID).Select(t => t.OperationContentID).Contains(o.ID)).ToList();
                        }

                        stopElectricalTask.OperationSheet = stopOperationSheet;

                    }
                    //摘牌电工信息
                    ElectricalTask pickElectricalTask = db.ElectricalTask.FirstOrDefault(t => t.OperationID == operation.ID && t.ElectricalTaskType == ElectricalTaskType.摘牌作业);
                    if (pickElectricalTask != null)
                    {
                        pickElectricalTask.ElectricalTaskTypeName = System.Enum.GetName(typeof(ElectricalTaskType), pickElectricalTask.ElectricalTaskType);
                        List<ElectricalTaskUser> pickElectricalTaskUserList = db.ElectricalTaskUser.Where(t => t.ElectricalTaskID == pickElectricalTask.ID && t.IsBack != true).OrderByDescending(t => t.Date).ToList();
                        pickElectricalTaskUserList.ForEach(t => t.CreateDate = t.Date.ToString("yyyy-MM-dd HH:mm"));
                        pickElectricalTaskUserList.ForEach(t => t.RealName = userList.FirstOrDefault(u => u.ID == t.UserID).Realname);
                        pickElectricalTask.ElectricalTaskUserList = pickElectricalTaskUserList;
                    }

                    //送电电工信息
                    ElectricalTask sendElectricalTask = db.ElectricalTask.FirstOrDefault(t => t.OperationID == operation.ID && t.ElectricalTaskType == ElectricalTaskType.送电作业);
                    if (sendElectricalTask != null)
                    {
                        //sendElectricalTask.RealName = userList.FirstOrDefault(u => u.ID == stopElectricalTask.AuditUserID).Realname;
                        //sendElectricalTask.AuditDateString = sendElectricalTask.AuditDate.Value.ToString("yyyy-MM-dd HH:mm");
                        //sendElectricalTask.AuditName = System.Enum.GetName(typeof(Audit), sendElectricalTask.Audit);
                        sendElectricalTask.ElectricalTaskTypeName = System.Enum.GetName(typeof(ElectricalTaskType), sendElectricalTask.ElectricalTaskType);
                        List<ElectricalTaskUser> sendElectricalTaskUserList = db.ElectricalTaskUser.Where(t => t.ElectricalTaskID == sendElectricalTask.ID && t.IsBack != true).OrderByDescending(t => t.Date).ToList();
                        sendElectricalTaskUserList.ForEach(t => t.CreateDate = t.Date.ToString("yyyy-MM-dd HH:mm"));
                        sendElectricalTaskUserList.ForEach(t => t.RealName = userList.FirstOrDefault(u => u.ID == t.UserID).Realname);
                        sendElectricalTask.ElectricalTaskUserList = sendElectricalTaskUserList;

                        //操作票
                        OperationSheet sendOperationSheet = db.OperationSheet.FirstOrDefault(t => t.ElectricalTaskID == sendElectricalTask.ID);
                        if (sendOperationSheet != null)
                        {
                            sendOperationSheet.OperationUserName = userList.FirstOrDefault(t => t.ID == sendOperationSheet.OperationUserID).Realname;
                            sendOperationSheet.OperationDateString = sendOperationSheet.OperationDate.ToString("yyyy-MM-dd HH:ss");
                            sendOperationSheet.GuardianUserName = sendOperationSheet.GuardianUserID != null ? userList.FirstOrDefault(t => t.ID == sendOperationSheet.GuardianUserID).Realname : "";
                            sendOperationSheet.FinishDateString = sendOperationSheet.FinishDate.HasValue ? sendOperationSheet.FinishDate.Value.ToString("yyyy-MM-dd HH:ss") : "";
                            sendOperationSheet.OperationContentList = db.OperationContent.Where(o => db.OperationSheet_Content.Where(t => t.OperationSheetID == sendOperationSheet.ID).Select(t => t.OperationContentID).Contains(o.ID)).ToList();
                        }
                        //送电联票
                        SendElectricalSheet sendElectricalSheet = db.SendElectricalSheet.FirstOrDefault(t => t.ElectricalTaskID == sendElectricalTask.ID);
                        if (sendElectricalSheet != null)
                        {
                            sendElectricalSheet.SendElectricDateString = sendElectricalSheet.SendElectricDate.ToString("yyyy-MM-dd HH:ss");
                            sendElectricalSheet.UserRealName = userList.FirstOrDefault(t => t.ID == sendElectricalSheet.UserID).Realname;
                            sendElectricalSheet.IsEvacuateAllPeopleName = sendElectricalSheet.IsEvacuateAllPeople ? "是" : "否";
                            sendElectricalSheet.IsRemoveGroundLineName = sendElectricalSheet.IsRemoveGroundLine ? "是" : "否";
                        }

                        sendElectricalTask.SendElectricalSheet = sendElectricalSheet;
                        sendElectricalTask.OperationSheet = sendOperationSheet;
                    }

                    //高压工作票
                    WorkSheet workSheet = db.WorkSheet.FirstOrDefault(t => t.OperationID == operation.ID);
                    if (workSheet != null)
                    {
                        //班长审核信息
                        workSheet.MonitorAuditName = System.Enum.GetName(typeof(Audit), workSheet.MonitorAudit);
                        workSheet.MonitorAuditDateString = workSheet.MonitorAuditDate.HasValue ? workSheet.MonitorAuditDate.Value.ToString("yyyy-MM-dd HH:ss") : "";
                        workSheet.MonitorAuditUserName = userList.FirstOrDefault(t => t.ID == workSheet.MonitorAuditUserID).Realname;

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
                        operation.OperationAudit,
                        OperationAuditName = System.Enum.GetName(typeof(OperationAudit), operation.OperationAudit),
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
                            //applicationSheet.WorkContent,
                            WorkContent = System.Enum.GetName(typeof(WorkContentType), applicationSheet.WorkContentType),
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
                            DepartmentName = db.Department.FirstOrDefault(t => t.ID == applicationSheet.DepartmentID).Name,
                        },
                        stopElectricalTask,
                        pickElectricalTask,
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
                            OperationAuditName = System.Enum.GetName(typeof(OperationAudit), operation.OperationAudit),
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
                            OperationAuditName = System.Enum.GetName(typeof(OperationAudit), operation.OperationAudit),
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
                        selectedOperation.IsHang = true;
                        db.SaveChanges();

                        int surplusCount = db.Operation.Count(t => t.AHID == selectedOperation.AHID && (t.IsPick != true && t.OperationFlow != OperationFlow.作业终止));

                        string ledMessage = new ShowLed().ShowLedMethod(ah.LedIP, false, surplusCount);
                        if (ledMessage!=string.Empty)
                        {
                            throw new ExceptionUtil(ledMessage);
                        }

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
                        if (selectedOperation.OperationFlow == OperationFlow.低压挂停电牌作业 || selectedOperation.OperationFlow == OperationFlow.高压挂停电牌作业)
                        {

                        }
                        else
                        {
                            throw new ExceptionUtil("无法摘牌");
                        }

                        //selectedOperation.OperationFlow = ah.VoltageType == VoltageType.低压? OperationFlow.低压检修作业完成 : OperationFlow.高压检修作业完成;
                        selectedOperation.IsConfirm = true;


                        //增加摘牌任务
                        selectedOperation.OperationFlow = ah.VoltageType == VoltageType.低压 ? OperationFlow.低压摘牌任务领取 : OperationFlow.高压摘牌任务领取;

                        ElectricalTask electricalTask = new ElectricalTask();
                        electricalTask.DispatcherAudit = DispatcherAudit.无需审核;
                        electricalTask.OperationID = selectedOperation.ID;
                        electricalTask.AHID = selectedOperation.AHID;
                        electricalTask.CreateDate = now;
                        electricalTask.ElectricalTaskType = ElectricalTaskType.摘牌作业;
                        db.ElectricalTask.Add(electricalTask);
                        db.SaveChanges();

                        int surplusCount = db.Operation.Count(t => t.AHID == selectedOperation.AHID && (t.IsPick != true && t.OperationFlow != OperationFlow.作业终止));
                        string notice = ",剩余牌数为" + surplusCount + ",牌未加完,禁止送电";

                        string ledMessage = new ShowLed().ShowLedMethod(ah.LedIP, false, surplusCount);
                        if (ledMessage != string.Empty)
                        {
                            throw new ExceptionUtil(ledMessage);
                        }

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
                        string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, "有新的" + ah.Name + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + "摘牌任务" + notice);


                        //new ShowLed().ShowLedMethod(ah.LedIP, false, surplusCount);
                        //发消息给巡检通知剩余牌数，若无剩余牌数则需要确认送电任务
                        //int surplusCount = db.Operation.Count(t => t.ID != selectedOperation.ID && t.AHID == selectedOperation.AHID && (t.IsConfirm != true && t.OperationFlow!= OperationFlow.作业终止));
                        //List<Role> roleList = RoleUtil.GetDispatcherRoleList();
                        //List<string> userWeChatIDList = db.User.Where(t => t.IsDelete != true && t.DepartmentID == loginUser.DepartmentID && db.UserRole.Where(m => roleList.Contains(m.Role)).Select(m => m.UserID).Contains(t.ID)).Select(t => t.WeChatID).ToList();
                        //string userWeChatIDString = "";
                        //foreach (string userWeChatID in userWeChatIDList)
                        //{
                        //    userWeChatIDString = userWeChatIDString + userWeChatID + "|";
                        //}
                        //userWeChatIDString.TrimEnd('|');
                        //string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.MessageSecret);
                        //string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, loginUser.Realname + "成功摘牌,"+ah.Name+"剩余牌数为"+ surplusCount);

                        ////查看该设备是否有其他正在执行的作业和任务，若没有则申请送电
                        //if (surplusCount == 0)
                        //{
                        //    db.Operation.Where(t => t.AHID == ah.ID && (t.OperationFlow == OperationFlow.低压停电流程结束 || t.OperationFlow == OperationFlow.高压停电流程结束)).ToList().ForEach(t => t.IsSendElectric = true);

                        //    //增加送电任务
                        //    //selectedOperation.OperationFlow = OperationFlow.低压送电任务领取;
                        //    ElectricalTask electricalTask = new ElectricalTask();
                        //    electricalTask.OperationID = selectedOperation.ID;
                        //    electricalTask.AHID = selectedOperation.AHID;
                        //    electricalTask.CreateDate = now;
                        //    electricalTask.ElectricalTaskType = ElectricalTaskType.送电作业;
                        //    db.ElectricalTask.Add(electricalTask);
                        //    db.SaveChanges();

                        //    ////发消息给所有电工
                        //    //List<Role> roleList = RoleUtil.GetElectricianRoleList();
                        //    //List<string> userWeChatIDList = db.User.Where(t => t.IsDelete != true && t.DepartmentID == loginUser.DepartmentID && db.UserRole.Where(m => roleList.Contains(m.Role)).Select(m => m.UserID).Contains(t.ID)).Select(t => t.WeChatID).ToList();
                        //    //string userWeChatIDString = "";
                        //    //foreach (string userWeChatID in userWeChatIDList)
                        //    //{
                        //    //    userWeChatIDString = userWeChatIDString + userWeChatID + "|";
                        //    //}
                        //    //userWeChatIDString.TrimEnd('|');
                        //    //string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.MessageSecret);
                        //    //string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, "有新的" + ah.Name + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + "送电任务");
                        //}
                        //else
                        //{
                        //    selectedOperation.OperationFlow = ah.VoltageType == VoltageType.低压 ? OperationFlow.低压停电流程结束 : OperationFlow.高压停电流程结束;
                        //    selectedOperation.IsFinish = true;
                        //    selectedOperation.FinishDate = now;
                        //    db.SaveChanges();
                        //}

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
                    if (selectOperation == null)
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
                    //doc.Range.Replace("@content", workSheet.WorkContent, false, false);
                    doc.Range.Replace("@content", System.Enum.GetName(typeof(WorkContentType), workSheet.WorkContentType), false, false);
                    doc.Range.Replace("@ahName", aH.Name, false, false);
                    doc.Range.Replace("@influence", workSheet.Influence, false, false);

                    int hour = 0;
                    int minute = 0;
                    TimeSpan timeSpan = workSheet.EndDate - workSheet.BeginDate;
                    int totalMinute = Convert.ToInt32(timeSpan.TotalMinutes);
                    if (totalMinute > 60)
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



                    doc.Range.Replace("@monitorAuditUser", userList.FirstOrDefault(t => t.ID == workSheet.MonitorAuditUserID).Realname + "(" + System.Enum.GetName(typeof(Audit), workSheet.MonitorAudit) + ")", false, false);
                    doc.Range.Replace("@deputyAuditUser", userList.FirstOrDefault(t => t.ID == workSheet.DeputyAuditUserID).Realname + "(" + System.Enum.GetName(typeof(Audit), workSheet.DeputyAudit) + ")", false, false);
                    //doc.Range.Replace("@safetyMeasures", workSheet.SafetyMeasures, false, false);
                    doc.Range.Replace("@chiefAuditUser", userList.FirstOrDefault(t => t.ID == workSheet.ChiefAuditUserID).Realname + "(" + System.Enum.GetName(typeof(Audit), workSheet.ChiefAudit) + ")", false, false);

                    SendElectricalSheet sendElectricalSheet = db.SendElectricalSheet.FirstOrDefault(t => t.OperationID == selectOperation.ID);
                    if (sendElectricalSheet == null)
                    {
                        doc.Range.Replace("@workFinishContent", "", false, false);
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
                        doc.Range.Replace("@isRemoveGroundLine", sendElectricalSheet.IsRemoveGroundLine ? "是" : "否", false, false);
                        doc.Range.Replace("@isEvacuateAllPeople", sendElectricalSheet.IsEvacuateAllPeople ? "是" : "否", false, false);
                        doc.Range.Replace("@sendCreateDate", sendElectricalSheet.CreateDate.ToString("yyyy年MM月dd日HH时ss分"), false, false);
                        doc.Range.Replace("@sendElectricDate", sendElectricalSheet.SendElectricDate.ToString("yyyy年MM月dd日HH时ss分"), false, false);
                        doc.Range.Replace("@operationUser", userList.FirstOrDefault(t => t.ID == operationSheet.OperationUserID).Realname, false, false);
                        doc.Range.Replace("@sendCreateUser", createUser.Realname, false, false);
                        doc.Range.Replace("@guardianUser", userList.FirstOrDefault(t => t.ID == operationSheet.GuardianUserID).Realname, false, false);
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


        public ApiResult ExportByList(int? departmentID = null, VoltageType? voltageType = null, int? ahID = null, DateTime? beginDate = null, DateTime? endDate = null)
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

                    List<Operation> operationList = db.Operation.Where(t =>
                    (departmentID == null || db.User.Where(m => m.DepartmentID == departmentID).Select(m => m.ID).Contains(t.UserID)) &&
                    (ahID == null || t.AHID == ahID) &&
                    (voltageType == null || t.VoltageType == voltageType) &&
                    (t.CreateDate >= beginDate && t.CreateDate <= endDate)).OrderByDescending(t => t.CreateDate).ThenBy(t => t.AHID).ToList();

                    List<User> userList = db.User.ToList();
                    List<Department> departmentList = db.Department.ToList();

                    List<int> createUserIDList = operationList.Select(t => t.UserID).Distinct().ToList();
                    List<User> createUserList = userList.Where(t => createUserIDList.Contains(t.ID)).ToList();



                    List<int> operationIDList = operationList.Select(t => t.ID).ToList();

                    List<int> ahIDList = operationList.Select(t => t.AHID).Distinct().ToList();

                    //变电柜
                    List<AH> ahList = db.AH.Where(t => ahIDList.Contains(t.ID)).ToList();

                    //作业申请单
                    List<ApplicationSheet> applicationSheetList = db.ApplicationSheet.Where(t => operationIDList.Contains(t.OperationID)).ToList();

                    //工作票
                    List<WorkSheet> workSheetList = db.WorkSheet.Where(t => operationIDList.Contains(t.OperationID)).ToList();

                    //操作票
                    List<OperationSheet> operationSheetList = db.OperationSheet.Where(t => operationIDList.Contains(t.OperationID)).ToList();

                    //任务
                    List<ElectricalTask> electricalTaskList = db.ElectricalTask.Where(t => operationIDList.Contains(t.OperationID)).ToList();

                    List<int> electricalTaskIDList = electricalTaskList.Select(t => t.ID).ToList();
                    List<ElectricalTaskUser> electricalTaskUserList = db.ElectricalTaskUser.Where(t => electricalTaskIDList.Contains(t.ElectricalTaskID) && !t.IsBack).ToList();


                    #region 创建excel
                    HSSFWorkbook wb = new HSSFWorkbook();
                    ICellStyle cellStyle = wb.CreateCellStyle();
                    cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    //水平对齐
                    cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    //垂直对齐
                    cellStyle.VerticalAlignment = VerticalAlignment.Center;
                    //设置字体
                    IFont font = wb.CreateFont();
                    font.FontHeightInPoints = 11;
                    font.FontName = "宋体";

                    font.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    font.IsBold = false;
                    cellStyle.SetFont(font);

                    ISheet sh = wb.CreateSheet("sheet1");
                    sh.SetColumnWidth(0, 40 * 150);
                    sh.SetColumnWidth(1, 40 * 150);
                    sh.SetColumnWidth(2, 75 * 150);
                    sh.SetColumnWidth(3, 40 * 150);
                    sh.SetColumnWidth(4, 40 * 150);
                    sh.SetColumnWidth(5, 40 * 150);
                    sh.SetColumnWidth(6, 40 * 150);
                    sh.SetColumnWidth(7, 40 * 150);
                    sh.SetColumnWidth(8, 40 * 150);
                    sh.SetColumnWidth(9, 40 * 150);
                    sh.SetColumnWidth(10, 70 * 150);
                    sh.SetColumnWidth(11, 40 * 150);
                    sh.SetColumnWidth(12, 40 * 150);
                    sh.SetColumnWidth(13, 70 * 150);
                    //sh.SetColumnWidth(14, 70 * 150);

                    IRow row = sh.CreateRow(0);
                    row.Height = 400;

                    //创建15个单元格
                    ICell rowICell0 = row.CreateCell(0);
                    ICell rowICell1 = row.CreateCell(1);
                    ICell rowICell2 = row.CreateCell(2);
                    ICell rowICell3 = row.CreateCell(3);
                    ICell rowICell4 = row.CreateCell(4);
                    ICell rowICell5 = row.CreateCell(5);
                    ICell rowICell6 = row.CreateCell(6);
                    ICell rowICell7 = row.CreateCell(7);
                    ICell rowICell8 = row.CreateCell(8);
                    ICell rowICell9 = row.CreateCell(9);
                    ICell rowICell10 = row.CreateCell(10);
                    ICell rowICell11 = row.CreateCell(11);
                    ICell rowICell12 = row.CreateCell(12);
                    ICell rowICell13 = row.CreateCell(13);
                    //ICell rowICell14 = row.CreateCell(14);

                    //创建第一行
                    rowICell0.SetCellValue("停电");
                    rowICell0.CellStyle = cellStyle;

                    rowICell1.SetCellValue("");
                    rowICell1.CellStyle = cellStyle;
                    rowICell2.SetCellValue("");
                    rowICell2.CellStyle = cellStyle;

                    rowICell3.SetCellValue("");
                    rowICell3.CellStyle = cellStyle;
                    rowICell4.SetCellValue("");
                    rowICell4.CellStyle = cellStyle;
                    rowICell5.SetCellValue("");
                    rowICell5.CellStyle = cellStyle;
                    rowICell6.SetCellValue("");
                    rowICell6.CellStyle = cellStyle;
                    rowICell7.SetCellValue("");
                    rowICell7.CellStyle = cellStyle;



                    rowICell8.SetCellValue("送电");
                    rowICell8.CellStyle = cellStyle;
                    rowICell9.SetCellValue("");
                    rowICell9.CellStyle = cellStyle;
                    rowICell10.SetCellValue("");
                    rowICell10.CellStyle = cellStyle;
                    rowICell11.SetCellValue("");
                    rowICell11.CellStyle = cellStyle;
                    rowICell12.SetCellValue("");
                    rowICell12.CellStyle = cellStyle;
                    rowICell13.SetCellValue("");
                    rowICell13.CellStyle = cellStyle;
                    //rowICell14.SetCellValue("");
                    //rowICell14.CellStyle = cellStyle;
                    sh.AddMergedRegion(new CellRangeAddress(0, 0, 0, 7));
                    sh.AddMergedRegion(new CellRangeAddress(0, 0, 8, 13));

                    //创建第二行
                    IRow row0 = sh.CreateRow(1);
                    row0.Height = 400;
                    ICell row0ICell0 = row0.CreateCell(0);
                    ICell row0ICell1 = row0.CreateCell(1);
                    ICell row0ICell2 = row0.CreateCell(2);
                    ICell row0ICell3 = row0.CreateCell(3);
                    ICell row0ICell4 = row0.CreateCell(4);
                    ICell row0ICell5 = row0.CreateCell(5);
                    ICell row0ICell6 = row0.CreateCell(6);
                    ICell row0ICell7 = row0.CreateCell(7);
                    ICell row0ICell8 = row0.CreateCell(8);
                    ICell row0ICell9 = row0.CreateCell(9);
                    ICell row0ICell10 = row0.CreateCell(10);
                    ICell row0ICell11 = row0.CreateCell(11);
                    ICell row0ICell12 = row0.CreateCell(12);
                    ICell row0ICell13 = row0.CreateCell(13);
                    //ICell row0ICell14 = row0.CreateCell(14);

                    row0ICell0.SetCellValue("停电日期");
                    row0ICell0.CellStyle = cellStyle;

                    row0ICell1.SetCellValue("停电申请时间");
                    row0ICell1.CellStyle = cellStyle;
                    row0ICell2.SetCellValue("停电操作时间");
                    row0ICell2.CellStyle = cellStyle;
                    row0ICell3.SetCellValue("停电部门");
                    row0ICell3.CellStyle = cellStyle;
                    row0ICell4.SetCellValue("停电申请人");
                    row0ICell4.CellStyle = cellStyle;
                    row0ICell5.SetCellValue("停电原因");
                    row0ICell5.CellStyle = cellStyle;

                    row0ICell6.SetCellValue("停电操作人");
                    row0ICell6.CellStyle = cellStyle;

                    row0ICell7.SetCellValue("调度");
                    row0ICell7.CellStyle = cellStyle;
                    row0ICell8.SetCellValue("送电日期");
                    row0ICell8.CellStyle = cellStyle;
                    row0ICell9.SetCellValue("送电申请时间");
                    row0ICell9.CellStyle = cellStyle;
                    row0ICell10.SetCellValue("送电操作时间");
                    row0ICell10.CellStyle = cellStyle;
                    row0ICell11.SetCellValue("送电申请人");
                    row0ICell11.CellStyle = cellStyle;
                    row0ICell12.SetCellValue("送电操作人");
                    row0ICell12.CellStyle = cellStyle;
                    //row0ICell13.SetCellValue("调度");
                    //row0ICell13.CellStyle = cellStyle;
                    row0ICell13.SetCellValue("备注");
                    row0ICell13.CellStyle = cellStyle;
                    #endregion

                    int i = 0;
                    #region 表格赋值
                    foreach (Operation operation in operationList)
                    {
                        string desc = System.Enum.GetName(typeof(OperationFlow), operation.OperationFlow);
                        ApplicationSheet applicationSheet = applicationSheetList.FirstOrDefault(t => t.OperationID == operation.ID);

                        AH ah = ahList.FirstOrDefault(t => t.ID == operation.AHID);

                        User createUser = createUserList.FirstOrDefault(t => t.ID == operation.UserID);

                        //停电任务
                        ElectricalTask stopElectricalTask = electricalTaskList.FirstOrDefault(t => t.OperationID == operation.ID && t.ElectricalTaskType == ElectricalTaskType.停电作业 && t.DispatcherAudit != DispatcherAudit.待审核);

                        //送电
                        ElectricalTask sendElectricalTask = electricalTaskList.FirstOrDefault(t => t.OperationID == operation.ID && t.ElectricalTaskType == ElectricalTaskType.送电作业 && t.DispatcherAudit != DispatcherAudit.待审核);

                        //创建行
                        //创建13个单元格
                        IRow rowTemp = sh.CreateRow(i + 2);
                        i++;
                        rowTemp.Height = 400;
                        ICell Cell0 = rowTemp.CreateCell(0);
                        Cell0.CellStyle = cellStyle;
                        ICell Cell1 = rowTemp.CreateCell(1);
                        Cell1.CellStyle = cellStyle;
                        ICell Cell2 = rowTemp.CreateCell(2);
                        Cell2.CellStyle = cellStyle;
                        ICell Cell3 = rowTemp.CreateCell(3);
                        Cell3.CellStyle = cellStyle;
                        ICell Cell4 = rowTemp.CreateCell(4);
                        Cell4.CellStyle = cellStyle;
                        ICell Cell5 = rowTemp.CreateCell(5);
                        Cell5.CellStyle = cellStyle;
                        ICell Cell6 = rowTemp.CreateCell(6);
                        Cell6.CellStyle = cellStyle;
                        ICell Cell7 = rowTemp.CreateCell(7);
                        Cell7.CellStyle = cellStyle;
                        ICell Cell8 = rowTemp.CreateCell(8);
                        Cell8.CellStyle = cellStyle;
                        ICell Cell9 = rowTemp.CreateCell(9);
                        Cell9.CellStyle = cellStyle;
                        ICell Cell10 = rowTemp.CreateCell(10);
                        Cell10.CellStyle = cellStyle;
                        ICell Cell11 = rowTemp.CreateCell(11);
                        Cell11.CellStyle = cellStyle;
                        ICell Cell12 = rowTemp.CreateCell(12);
                        Cell12.CellStyle = cellStyle;
                        ICell Cell13 = rowTemp.CreateCell(13);
                        Cell13.CellStyle = cellStyle;
                        //ICell Cell14 = rowTemp.CreateCell(14);
                        //Cell14.CellStyle = cellStyle;

                        //备注
                        Cell13.SetCellValue(desc);



                        //停电申请时间
                        Cell1.SetCellValue(operation.CreateDate.ToString("yyyy-MM-dd HH:mm"));


                        //停电部门
                        Cell3.SetCellValue(departmentList.FirstOrDefault(t => t.ID == createUser.DepartmentID).Name);

                        //停电申请人
                        Cell4.SetCellValue(createUser.Realname);

                        //停电原因(变电柜+高低压)
                        Cell5.SetCellValue(ah.Name + "(" + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + ")");


                        //申请单
                        if (applicationSheet.Audit == Audit.驳回)
                        {
                            Cell13.SetCellValue("申请单审核驳回(" + userList.FirstOrDefault(t => t.ID == applicationSheet.AuditUserID).Realname + ")");
                            continue;
                        }

                        if (ah.VoltageType == VoltageType.高压)
                        {
                            WorkSheet workSheet = workSheetList.FirstOrDefault(t => t.OperationID == operation.ID);
                            if (workSheet.AuditLevel == AuditLevel.驳回)
                            {
                                desc = workSheet.MonitorAudit == Audit.驳回 ? ("高压工作票班长审核驳回(" + userList.FirstOrDefault(t => t.ID == workSheet.MonitorAuditUserID).Realname + ")") : (workSheet.ChiefAudit == Audit.驳回 ? ("高压工作票正职审核驳回(" + userList.FirstOrDefault(t => t.ID == workSheet.ChiefAuditUserID).Realname + ")") : ("高压工作票副职审核驳回(" + userList.FirstOrDefault(t => t.ID == workSheet.DeputyAuditUserID).Realname + ")"));
                                Cell13.SetCellValue(desc);
                                continue;
                            }
                        }

                        //没有停电任务
                        if (stopElectricalTask == null)
                        {
                            continue;
                        }
                        else
                        {
                            //调度
                            Cell7.SetCellValue(userList.FirstOrDefault(t => t.ID == stopElectricalTask.AuditUserID).Realname + "(" + System.Enum.GetName(typeof(DispatcherAudit), stopElectricalTask.DispatcherAudit) + ")");


                            if (stopElectricalTask.DispatcherAudit == DispatcherAudit.驳回)
                            {
                                Cell13.SetCellValue("停电调度审核驳回(" + userList.FirstOrDefault(t => t.ID == stopElectricalTask.AuditUserID).Realname + ")");
                                continue;
                            }

                            List<ElectricalTaskUser> stopElectricalTaskUserList = electricalTaskUserList.Where(t => t.ElectricalTaskID == stopElectricalTask.ID).OrderBy(t => t.Date).ToList();
                            stopElectricalTaskUserList.ForEach(t => t.CreateDate = t.Date.ToString("yyyy-MM-dd HH:mm"));
                            stopElectricalTaskUserList.ForEach(t => t.RealName = userList.FirstOrDefault(d => d.ID == t.UserID).Realname);
                            //停电操作时间
                            Cell2.SetCellValue(string.Join(",", stopElectricalTaskUserList.Select(t => t.CreateDate).ToArray()));

                            //停电操作人
                            Cell6.SetCellValue(string.Join(",", stopElectricalTaskUserList.Select(t => t.RealName).ToArray()));


                            //停电日期 取停电操作日期
                            Cell0.SetCellValue(stopElectricalTaskUserList.FirstOrDefault() != null ? stopElectricalTaskUserList.FirstOrDefault().Date.ToString("yyyy-MM-dd") : "");

                        }

                        //没有送电任务
                        if (sendElectricalTask == null)
                        {
                            //状态流程结束说明有并行作业
                            if (operation.OperationFlow == OperationFlow.低压停电流程结束 || operation.OperationFlow == OperationFlow.高压停电流程结束)
                            {
                                //Cell14.SetCellValue(ah.Name+"存在并行作业,停电任务结束");

                                //最近的送电完成的任务
                                ElectricalTask latestSendElectricalTask = electricalTaskList.Where(e => operationList.Where(t => t.AHID == ah.ID).Select(t => t.ID).Contains(e.OperationID) && e.ElectricalTaskType == ElectricalTaskType.送电作业 && e.CreateDate >= operation.FinishDate).OrderBy(t => t.CreateDate).FirstOrDefault();
                                if (latestSendElectricalTask != null)
                                {
                                    Cell13.SetCellValue(System.Enum.GetName(typeof(OperationFlow), operationList.FirstOrDefault(t => t.ID == latestSendElectricalTask.OperationID).OperationFlow) + "(" + "并行任务" + ")");

                                    Cell9.SetCellValue(latestSendElectricalTask.CreateDate.ToString("yyyy-MM-dd HH:mm"));


                                    //调度
                                    //Cell13.SetCellValue(userList.FirstOrDefault(t => t.ID == latestSendElectricalTask.AuditUserID).Realname + "(" + System.Enum.GetName(typeof(Audit), latestSendElectricalTask.Audit) + ")");


                                    //if (latestSendElectricalTask.Audit == Audit.驳回)
                                    //{
                                    //    Cell14.SetCellValue("送电调度审核驳回(" + userList.FirstOrDefault(t => t.ID == latestSendElectricalTask.AuditUserID).Realname + ")");
                                    //    continue;
                                    //}

                                    List<ElectricalTaskUser> latestSendElectricalTaskUserList = electricalTaskUserList.Where(t => t.ElectricalTaskID == latestSendElectricalTask.ID).OrderBy(t => t.Date).ToList();
                                    latestSendElectricalTaskUserList.ForEach(t => t.CreateDate = t.Date.ToString("yyyy-MM-dd HH:mm"));
                                    latestSendElectricalTaskUserList.ForEach(t => t.RealName = userList.FirstOrDefault(d => d.ID == t.UserID).Realname);
                                    //送电操作时间
                                    Cell10.SetCellValue(string.Join(",", latestSendElectricalTaskUserList.Select(t => t.CreateDate).ToArray()));

                                    //送电操作人
                                    Cell12.SetCellValue(string.Join(",", latestSendElectricalTaskUserList.Select(t => t.RealName).ToArray()));


                                    //送电申请人
                                    Cell11.SetCellValue(userList.FirstOrDefault(u => u.ID == operationList.FirstOrDefault(t => t.ID == latestSendElectricalTask.OperationID).UserID).Realname);


                                    //送电日期 取送电电操作日期
                                    Cell8.SetCellValue(latestSendElectricalTaskUserList.FirstOrDefault() != null ? latestSendElectricalTaskUserList.FirstOrDefault().Date.ToString("yyyy-MM-dd") : "");

                                }
                            }

                            continue;
                        }
                        else
                        {
                            //送电申请时间
                            Cell9.SetCellValue(sendElectricalTask.CreateDate.ToString("yyyy-MM-dd HH:mm"));


                            //调度
                            //Cell13.SetCellValue(userList.FirstOrDefault(t => t.ID == sendElectricalTask.AuditUserID).Realname + "(" + System.Enum.GetName(typeof(Audit), sendElectricalTask.Audit) + ")");


                            //if (sendElectricalTask.Audit == Audit.驳回)
                            //{
                            //    Cell14.SetCellValue("送电调度审核驳回(" + userList.FirstOrDefault(t => t.ID == sendElectricalTask.AuditUserID).Realname+")");
                            //    continue;
                            //}

                            List<ElectricalTaskUser> sendElectricalTaskUserList = electricalTaskUserList.Where(t => t.ElectricalTaskID == sendElectricalTask.ID).OrderBy(t => t.Date).ToList();
                            sendElectricalTaskUserList.ForEach(t => t.CreateDate = t.Date.ToString("yyyy-MM-dd HH:mm"));
                            sendElectricalTaskUserList.ForEach(t => t.RealName = userList.FirstOrDefault(d => d.ID == t.UserID).Realname);
                            //送电操作时间
                            Cell10.SetCellValue(string.Join(",", sendElectricalTaskUserList.Select(t => t.CreateDate).ToArray()));

                            //送电操作人
                            Cell12.SetCellValue(string.Join(",", sendElectricalTaskUserList.Select(t => t.RealName).ToArray()));


                            //送电申请人
                            Cell11.SetCellValue(createUser.Realname);


                            //送电日期 取送电电操作日期
                            Cell8.SetCellValue(sendElectricalTaskUserList.FirstOrDefault() != null ? sendElectricalTaskUserList.FirstOrDefault().Date.ToString("yyyy-MM-dd") : "");


                        }

                    }


                    string fileName = "停送电记录导出" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    BaseUtil.ExportByWeb(fileName, wb);
                    #endregion

                    new LogDAO().AddLog(LogCode.导出, "成功导出裕溪口停送电", db);
                    result = ApiResult.NewSuccessJson("成功导出裕溪口停送电");
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

        public ApiResult ListForApp(int page = 1, int limit = 10)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;

            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    DateTime currentDate = DateTime.Now.Date;
                    DateTime nextDate = currentDate.AddDays(1);
                    User loginUser = LoginHelper.CurrentUser(db);

                    IQueryable<Operation> operationIQueryable = db.Operation.Where(t => t.UserID == loginUser.ID && !t.IsFinish || (t.FinishDate.Value > currentDate && t.FinishDate.Value <= nextDate));
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
                            OperationAuditName = System.Enum.GetName(typeof(OperationAudit), operation.OperationAudit),
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
    }
}
