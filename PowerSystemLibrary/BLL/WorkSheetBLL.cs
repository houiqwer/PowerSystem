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
    public class WorkSheetBLL
    {
        public ApiResult List(int? departmentID = null, string no = "", VoltageType? voltageType = null,int? ahID = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;

            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    User loginUser = LoginHelper.CurrentUser(db);
                    beginDate = beginDate ?? DateTime.MinValue;
                    endDate = endDate ?? DateTime.MaxValue;
                    no = no ?? string.Empty;

                    IQueryable<WorkSheet> workSheetIQueryable = db.WorkSheet.Where(t => t.IsDelete != true &&
                    (departmentID == null || t.DepartmentID == departmentID) &&
                    t.NO.Contains(no) &&
                    (ahID == null || db.Operation.Where(m => m.AHID == ahID).Select(m => m.ID).Contains(t.OperationID)) &&
                    (voltageType == null || db.Operation.Where(m => m.VoltageType == voltageType).Select(m => m.ID).Contains(t.OperationID)) &&
                    (t.BeginDate >= beginDate && t.EndDate <= endDate)
                    );

                    int total = workSheetIQueryable.Count();
                    List<WorkSheet> workSheetList = workSheetIQueryable.OrderByDescending(t => t.CreateDate).Skip((page - 1) * limit).Take(limit).ToList();
                    List<int> operationIDList = workSheetList.Select(t => t.OperationID).ToList();

                    List<Operation> operationList = db.Operation.ToList();
                    List<int> ahIDList = operationList.Select(t => t.AHID).Distinct().ToList();
                    List<int> userIDList = operationList.Select(t => t.UserID).Distinct().ToList();

                    List<object> returnList = new List<object>();
                    List<AH> ahList = db.AH.Where(t => ahIDList.Contains(t.ID)).ToList();
                    List<User> userList = db.User.Where(t => userIDList.Contains(t.ID)).ToList();

                    foreach (WorkSheet workSheet in workSheetList)
                    {
                        Operation operation = operationList.FirstOrDefault(t => t.ID == workSheet.OperationID);
                        returnList.Add(new
                        {
                            workSheet.ID,
                            userList.FirstOrDefault(t => t.ID == workSheet.UserID).Realname,
                            AHName = ahList.FirstOrDefault(t => t.ID == operation.AHID).Name,
                            CreateDate = workSheet.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                            BeginDate = workSheet.BeginDate.ToString("yyyy-MM-dd HH:mm"),
                            EndDate = workSheet.EndDate.ToString("yyyy-MM-dd HH:mm"),
                            VoltageType = System.Enum.GetName(typeof(VoltageType), operation.VoltageType),
                            OperationFlow = System.Enum.GetName(typeof(OperationFlow), operation.OperationFlow),
                            AuditLevel = System.Enum.GetName(typeof(AuditLevel), workSheet.AuditLevel),
                            //IsAuditUser = (loginUser.ID == workSheet.DeputyAuditUserID || loginUser.ID == workSheet.ChiefAuditUserID),
                            OperationID = operation.ID,
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


        public ApiResult MyAuditList(int? departmentID = null, string no = "", VoltageType? voltageType = null, int? ahID = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;

            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    User loginUser = LoginHelper.CurrentUser(db);
                    beginDate = beginDate ?? DateTime.MinValue;
                    endDate = endDate ?? DateTime.MaxValue;
                    no = no ?? string.Empty;

                    IQueryable<WorkSheet> workSheetIQueryable = db.WorkSheet.Where(t => t.IsDelete != true &&
                    (departmentID == null || t.DepartmentID == departmentID) &&
                    (db.Operation.Where(o => o.OperationFlow != OperationFlow.作业终止).Select(o => o.ID).Contains(t.OperationID)) &&
                    t.NO.Contains(no) &&
                    ((t.AuditLevel == AuditLevel.班长审核 && t.MonitorAudit == Enum.Audit.待审核 && t.MonitorAuditUserID == loginUser.ID) || (t.AuditLevel == AuditLevel.副职审核 &&  t.DeputyAudit == Enum.Audit.待审核 && t.DeputyAuditUserID == loginUser.ID) || (t.AuditLevel == AuditLevel.正职审核 && t.ChiefAudit == Enum.Audit.待审核 && t.ChiefAuditUserID == loginUser.ID))  &&
                    (ahID == null || db.Operation.Where(m => m.AHID == ahID).Select(m => m.ID).Contains(t.OperationID)) &&
                    (voltageType == null || db.Operation.Where(m => m.VoltageType == voltageType).Select(m => m.ID).Contains(t.OperationID)) &&
                    (t.BeginDate >= beginDate && t.EndDate <= endDate)
                    );

                    int total = workSheetIQueryable.Count();
                    List<WorkSheet> workSheetList = workSheetIQueryable.OrderByDescending(t => t.CreateDate).Skip((page - 1) * limit).Take(limit).ToList();
                    List<int> operationIDList = workSheetList.Select(t => t.OperationID).ToList();

                    List<Operation> operationList = db.Operation.ToList();
                    List<int> ahIDList = operationList.Select(t => t.AHID).Distinct().ToList();
                    List<int> userIDList = operationList.Select(t => t.UserID).Distinct().ToList();

                    List<object> returnList = new List<object>();
                    List<AH> ahList = db.AH.Where(t => ahIDList.Contains(t.ID)).ToList();
                    List<User> userList = db.User.Where(t => userIDList.Contains(t.ID)).ToList();

                    foreach (WorkSheet workSheet in workSheetList)
                    {
                        Operation operation = operationList.FirstOrDefault(t => t.ID == workSheet.OperationID);
                        returnList.Add(new
                        {
                            workSheet.ID,
                            userList.FirstOrDefault(t => t.ID == workSheet.UserID).Realname,
                            AHName = ahList.FirstOrDefault(t => t.ID == operation.AHID).Name,
                            CreateDate = workSheet.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                            BeginDate = workSheet.BeginDate.ToString("yyyy-MM-dd HH:mm"),
                            EndDate = workSheet.EndDate.ToString("yyyy-MM-dd HH:mm"),
                            VoltageType = System.Enum.GetName(typeof(VoltageType), operation.VoltageType),
                            OperationFlow = System.Enum.GetName(typeof(OperationFlow), operation.OperationFlow),
                            AuditLevel = System.Enum.GetName(typeof(AuditLevel), workSheet.AuditLevel),
                            //IsAuditUser = loginUser.ID == applicationSheet.AuditUserID
                            OperationID = operation.ID,
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


        public ApiResult Audit(WorkSheet workSheet)
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
                        WorkSheet selectedWorkSheetSheet = db.WorkSheet.FirstOrDefault(t => t.ID == workSheet.ID);
                        if (selectedWorkSheetSheet == null)
                        {
                            throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new WorkSheet()));
                        }
                        User loginUser = LoginHelper.CurrentUser(db);

                        if(selectedWorkSheetSheet.AuditLevel == AuditLevel.通过 || selectedWorkSheetSheet.AuditLevel == AuditLevel.驳回)
                        {
                            throw new ExceptionUtil("工作票审核已结束");
                        }
                        Operation operation = db.Operation.FirstOrDefault(t => t.ID == selectedWorkSheetSheet.OperationID);

                        AH ah = db.AH.FirstOrDefault(t => t.ID == operation.AHID);

                        string userWeChatIDString = string.Empty;
                        string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.MessageSecret);

                        if (workSheet.AuditLevel == Enum.AuditLevel.通过)
                        {

                            if(selectedWorkSheetSheet.AuditLevel == AuditLevel.班长审核)
                            {
                                operation.OperationAudit = OperationAudit.审核中;
                                selectedWorkSheetSheet.AuditLevel = AuditLevel.副职审核;
                                selectedWorkSheetSheet.MonitorAuditDate = now;
                                selectedWorkSheetSheet.MonitorAudit = Enum.Audit.通过;
                                selectedWorkSheetSheet.MonitorAuditMessage = workSheet.AuditMessage;
                                //发消息给副职审核
                                userWeChatIDString = db.User.FirstOrDefault(t => t.ID == selectedWorkSheetSheet.DeputyAuditUserID && t.IsDelete != true).WeChatID;
                                string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, loginUser.Realname + "于" + now.ToString("yyyy-MM-dd HH:mm") + "申请" + ah.Name + "位置的" + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + ClassUtil.GetEntityName(operation) + "工作票待审核");
                            }
                            else if(selectedWorkSheetSheet.AuditLevel == AuditLevel.副职审核)
                            {
                                //operation.OperationAudit = OperationAudit.审核中;
                                selectedWorkSheetSheet.AuditLevel = AuditLevel.正职审核;
                                
                                selectedWorkSheetSheet.DeputyAuditDate = now;
                                selectedWorkSheetSheet.DeputyAudit = Enum.Audit.通过;
                                selectedWorkSheetSheet.DeputyAuditMessage = workSheet.AuditMessage;

                                //发消息给正职审核 
                                userWeChatIDString = db.User.FirstOrDefault(t => t.ID == selectedWorkSheetSheet.ChiefAuditUserID && t.IsDelete != true).WeChatID;
                                string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, loginUser.Realname + "于" + now.ToString("yyyy-MM-dd HH:mm") + "申请" + ah.Name + "位置的" + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + ClassUtil.GetEntityName(operation) + "工作票待审核");

                            }
                            else
                            {
                                selectedWorkSheetSheet.AuditLevel = AuditLevel.通过;
                                
                                selectedWorkSheetSheet.ChiefAuditDate = now;
                                selectedWorkSheetSheet.ChiefAudit = Enum.Audit.通过;
                                selectedWorkSheetSheet.ChiefAuditMessage = workSheet.AuditMessage;

                                //判断申请单是否审核通过 
                                if(db.ApplicationSheet.FirstOrDefault(t=>t.OperationID == operation.ID).Audit  == Enum.Audit.通过)
                                {
                                    operation.OperationAudit = OperationAudit.通过;
                                    //发布停电任务
                                    ElectricalTask electricalTask = new ElectricalTask();
                                    electricalTask.OperationID = operation.ID;
                                    electricalTask.AHID = operation.AHID;
                                    electricalTask.CreateDate = now;
                                    electricalTask.DispatcherAudit = DispatcherAudit.待审核;
                                    electricalTask.ElectricalTaskType = ElectricalTaskType.停电作业;
                                    db.ElectricalTask.Add(electricalTask);

                                    //发消息给所有调度
                                    List<Role> roleList = RoleUtil.GetDispatcherRoleList();
                                    List<string> userWeChatIDList = db.User.Where(t => t.IsDelete != true && t.DepartmentID == loginUser.DepartmentID && db.UserRole.Where(m => roleList.Contains(m.Role)).Select(m => m.UserID).Contains(t.ID)).Select(t => t.WeChatID).ToList();
                                    
                                    foreach (string userWeChatID in userWeChatIDList)
                                    {
                                        userWeChatIDString = userWeChatIDString + userWeChatID + "|";
                                    }
                                    userWeChatIDString.TrimEnd('|');
                                    
                                    string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, "有新的" + ah.Name + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + "停电任务等待调度");
                                }
                            }
                            operation.OperationFlow = OperationFlow.高压停电作业审核;
                            db.SaveChanges();

                            new LogDAO().AddLog(LogCode.审核成功, loginUser.Realname + "成功审核" + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + ClassUtil.GetEntityName(operation)+"工作票", db);


                        }
                        else if (workSheet.AuditLevel == Enum.AuditLevel.驳回)
                        {
                            operation.OperationAudit = OperationAudit.驳回;
                            if(selectedWorkSheetSheet.AuditLevel == AuditLevel.班长审核)
                            {
                                selectedWorkSheetSheet.MonitorAuditDate = now;
                                selectedWorkSheetSheet.MonitorAudit = Enum.Audit.驳回;
                                selectedWorkSheetSheet.MonitorAuditMessage = workSheet.AuditMessage;
                            }
                            else if (selectedWorkSheetSheet.AuditLevel == AuditLevel.副职审核)
                            {
                                selectedWorkSheetSheet.DeputyAuditDate = now;
                                selectedWorkSheetSheet.DeputyAudit = Enum.Audit.驳回;
                                selectedWorkSheetSheet.DeputyAuditMessage = workSheet.AuditMessage;
                            }
                            else
                            {
                                selectedWorkSheetSheet.ChiefAuditDate = now;
                                selectedWorkSheetSheet.ChiefAudit = Enum.Audit.驳回;
                                selectedWorkSheetSheet.ChiefAuditMessage = workSheet.AuditMessage;
                            }
                            selectedWorkSheetSheet.AuditLevel = AuditLevel.驳回;


                            operation.OperationFlow = OperationFlow.作业终止;
                            operation.IsFinish = true;
                            operation.FinishDate = now;
                            db.SaveChanges();

                            //发消息给发起人
                            string resultMessage = WeChatAPI.SendMessage(accessToken, db.User.FirstOrDefault(t => t.ID == operation.UserID).WeChatID, ParaUtil.MessageAgentid, "您的申请的高压工作票被驳回，原因为：" + workSheet.AuditMessage);

                            new SendDispatcherNoticeDAO().SendNotice(operation, ah, db);

                            new LogDAO().AddLog(LogCode.审核驳回, loginUser.Realname + "成功审核" + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + ClassUtil.GetEntityName(operation), db);
                        }
                        else
                        {
                            throw new ExceptionUtil("请选择正确的审核意见");
                        }

                        result = ApiResult.NewSuccessJson("成功审核" + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + ClassUtil.GetEntityName(operation));
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
                        result = ApiResult.NewErrorJson(LogCode.审核错误, message, db);
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

                    WorkSheet workSheet = db.WorkSheet.FirstOrDefault(t => t.ID == id);
                    if (workSheet == null)
                    {
                        throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new WorkSheet()));
                    }
                    User createUser = db.User.FirstOrDefault(t => t.ID == workSheet.UserID);
                    Operation operation = db.Operation.FirstOrDefault(t => t.ID == workSheet.OperationID);
                    result = ApiResult.NewSuccessJson(new
                    {
                        workSheet.ID,
                        //workSheet.WorkContent,
                        WorkContent = System.Enum.GetName(typeof(WorkContentType),workSheet.WorkContentType),
                        workSheet.Influence,
                        //workSheet.SafetyMeasures,
                        DepartmentName = db.Department.FirstOrDefault(t => t.ID == createUser.DepartmentID).Name,
                        createUser.Realname,
                        AHName = db.AH.FirstOrDefault(t => t.ID == operation.AHID).Name,
                        CreateDate = workSheet.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                        BeginDate = workSheet.BeginDate.ToString("yyyy-MM-dd HH:mm"),
                        EndDate = workSheet.EndDate.ToString("yyyy-MM-dd HH:mm"),
                        VoltageType = System.Enum.GetName(typeof(VoltageType), operation.VoltageType),
                        OperationFlow = System.Enum.GetName(typeof(OperationFlow), operation.OperationFlow),

                        AuditLevel = System.Enum.GetName(typeof(AuditLevel), workSheet.AuditLevel),

                        MonitorAuditUserName = db.User.FirstOrDefault(t=>t.ID == workSheet.MonitorAuditUserID).Realname,
                        MonitorAudit = System.Enum.GetName(typeof(Audit), workSheet.MonitorAudit),
                        MonitorAuditDate = workSheet.MonitorAuditDate.HasValue ? workSheet.MonitorAuditDate.Value.ToString("yyyy-MM-dd HH:mm") : "",
                        workSheet.MonitorAuditMessage,

                        DeputyAuditUserName =db.User.FirstOrDefault(t=>t.ID == workSheet.DeputyAuditUserID).Realname,
                        DeputyAudit = workSheet.MonitorAudit == Enum.Audit.驳回 ? "无需审核" : System.Enum.GetName(typeof(Audit), workSheet.DeputyAudit),
                        DeputyAuditDate = workSheet.DeputyAuditDate.HasValue ? workSheet.DeputyAuditDate.Value.ToString("yyyy-MM-dd HH:mm"):"",
                        workSheet.DeputyAuditMessage,

                        ChiefAuditUserName = db.User.FirstOrDefault(t=>t.ID == workSheet.ChiefAuditUserID).Realname,
                        ChiefAudit = workSheet.DeputyAudit == Enum.Audit.驳回?"无需审核":System.Enum.GetName(typeof(Audit), workSheet.ChiefAudit),
                        ChiefAuditDate = workSheet.ChiefAuditDate.HasValue? workSheet.ChiefAuditDate.Value.ToString("yyyy-MM-dd HH:mm"):"",
                        workSheet.ChiefAuditMessage,
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
    }
}
