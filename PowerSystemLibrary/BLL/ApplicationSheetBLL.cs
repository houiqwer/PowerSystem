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
using Aspose.Words.Tables;
using Aspose.Words;
using System.IO;
using System.Web;

namespace PowerSystemLibrary.BLL
{
    public class ApplicationSheetBLL
    {
        public ApiResult Audit(ApplicationSheet applicationSheet)
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
                        ApplicationSheet selectedApplicationSheet = db.ApplicationSheet.FirstOrDefault(t => t.ID == applicationSheet.ID);
                        if (selectedApplicationSheet == null)
                        {
                            throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new ApplicationSheet()));
                        }
                        User loginUser = LoginHelper.CurrentUser(db);

                        Operation operation = db.Operation.FirstOrDefault(t => t.ID == selectedApplicationSheet.OperationID);
                        AH ah = db.AH.FirstOrDefault(t => t.ID == operation.AHID);
                        string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.MessageSecret);
                        if (applicationSheet.Audit == Enum.Audit.通过)
                        {
                            selectedApplicationSheet.Audit = Enum.Audit.通过;
                            selectedApplicationSheet.AuditDate = now;
                            selectedApplicationSheet.AuditUserID = loginUser.ID;
                            selectedApplicationSheet.AuditMessage = applicationSheet.AuditMessage;


                            operation.OperationFlow = operation.VoltageType == VoltageType.低压? OperationFlow.低压停电作业审核:OperationFlow.高压停电作业审核;
                            db.SaveChanges();

                            if (operation.VoltageType == VoltageType.低压)
                            {
                                operation.OperationAudit = OperationAudit.通过;
                                //发布停电任务
                                ElectricalTask electricalTask = new ElectricalTask();
                                electricalTask.OperationID = operation.ID;
                                electricalTask.AHID = operation.AHID;
                                electricalTask.CreateDate = now;
                                electricalTask.DispatcherAudit = Enum.DispatcherAudit.待审核;
                                electricalTask.ElectricalTaskType = ElectricalTaskType.停电作业;
                                db.ElectricalTask.Add(electricalTask);


                                db.SaveChanges();
                                //发消息给所有调度
                                List<Role> roleList = RoleUtil.GetDispatcherRoleList();
                                List<string> userWeChatIDList = db.User.Where(t => t.IsDelete != true && t.DepartmentID == loginUser.DepartmentID && db.UserRole.Where(m => roleList.Contains(m.Role)).Select(m => m.UserID).Contains(t.ID)).Select(t => t.WeChatID).ToList();
                                string userWeChatIDString = "";
                                foreach (string userWeChatID in userWeChatIDList)
                                {
                                    userWeChatIDString = userWeChatIDString + userWeChatID + "|";
                                }
                                userWeChatIDString.TrimEnd('|');
                                
                                string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, "有新的" + ah.Name + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + "停电任务等待调度");

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
                                //string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, "有新的" + ah.Name + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + "停电任务");

                            }
                            else
                            {
                                //检查其他是否审核均通过，通过则发布停电任务并发送消息
                                if(db.WorkSheet.FirstOrDefault(t=>t.OperationID == operation.ID).AuditLevel == AuditLevel.通过)
                                {
                                    operation.OperationAudit = OperationAudit.通过;
                                    //发布停电任务
                                    ElectricalTask electricalTask = new ElectricalTask();
                                    electricalTask.OperationID = operation.ID;
                                    electricalTask.DispatcherAudit = Enum.DispatcherAudit.待审核;
                                    electricalTask.AHID = operation.AHID;
                                    electricalTask.CreateDate = now;
                                    electricalTask.ElectricalTaskType = ElectricalTaskType.停电作业;
                                    db.ElectricalTask.Add(electricalTask);

                                    db.SaveChanges();

                                    //发消息给所有调度
                                    List<Role> roleList = RoleUtil.GetDispatcherRoleList();
                                    List<string> userWeChatIDList = db.User.Where(t => t.IsDelete != true && t.DepartmentID == loginUser.DepartmentID && db.UserRole.Where(m => roleList.Contains(m.Role)).Select(m => m.UserID).Contains(t.ID)).Select(t => t.WeChatID).ToList();
                                    string userWeChatIDString = "";
                                    foreach (string userWeChatID in userWeChatIDList)
                                    {
                                        userWeChatIDString = userWeChatIDString + userWeChatID + "|";
                                    }
                                    userWeChatIDString.TrimEnd('|');

                                    string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, "有新的" + ah.Name + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + "停电任务等待调度");
                                }
                                else
                                {
                                    operation.OperationAudit = OperationAudit.审核中;
                                    db.SaveChanges();
                                }
                            }
                            new LogDAO().AddLog(LogCode.审核成功, loginUser.Realname + "成功审核" + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + ClassUtil.GetEntityName(operation), db);


                        }
                        else if (applicationSheet.Audit == Enum.Audit.驳回)
                        {
                            selectedApplicationSheet.Audit = Enum.Audit.驳回;
                            selectedApplicationSheet.AuditDate = now;
                            selectedApplicationSheet.AuditUserID = loginUser.ID;
                            selectedApplicationSheet.AuditMessage = applicationSheet.AuditMessage;


                            operation.OperationFlow = OperationFlow.作业终止;
                            operation.IsFinish = true;
                            operation.FinishDate = now;
                            operation.OperationAudit = OperationAudit.驳回;
                            db.SaveChanges();

                            new SendDispatcherNoticeDAO().SendNotice(operation, ah, db);

                            //发消息给发起人
                            string resultMessage = WeChatAPI.SendMessage(accessToken, db.User.FirstOrDefault(t => t.ID == operation.UserID).WeChatID, ParaUtil.MessageAgentid, "您的作业被驳回，原因为：" + applicationSheet.AuditMessage);


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


        public ApiResult DispatcherAudit(ApplicationSheet applicationSheet)
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
                        ApplicationSheet selectedApplicationSheet = db.ApplicationSheet.FirstOrDefault(t => t.ID == applicationSheet.ID);
                        if (selectedApplicationSheet == null)
                        {
                            throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new ApplicationSheet()));
                        }
                        User loginUser = LoginHelper.CurrentUser(db);
                        Operation operation = db.Operation.FirstOrDefault(t => t.ID == selectedApplicationSheet.OperationID);
                        AH ah = db.AH.FirstOrDefault(t => t.ID == operation.AHID);

                        if (applicationSheet.Audit == Enum.Audit.通过)
                        {
                            if (operation.VoltageType == VoltageType.低压)
                            {
                                //发布停电任务
                                //ElectricalTask electricalTask = new ElectricalTask();
                                //electricalTask.OperationID = operation.ID;
                                //electricalTask.AHID = operation.AHID;
                                //electricalTask.CreateDate = now;
                                //electricalTask.ElectricalTaskType = ElectricalTaskType.停电作业;
                                //electricalTask.AuditUserID = loginUser.ID;
                                //db.ElectricalTask.Add(electricalTask);
                                ElectricalTaskType electricalTaskType = new ElectricalTaskType();
                                if(operation.OperationFlow == OperationFlow.低压停电作业审核)
                                {
                                    electricalTaskType = ElectricalTaskType.停电作业;
                                }
                                //else
                                //{
                                //    electricalTaskType = ElectricalTaskType.送电作业;
                                //}
                                
                                ElectricalTask electricalTask = db.ElectricalTask.FirstOrDefault(t => t.OperationID == operation.ID && t.ElectricalTaskType == electricalTaskType);
                                electricalTask.AuditUserID = loginUser.ID;
                                //electricalTask.Audit = Enum.Audit.通过;
                                electricalTask.DispatcherAudit = Enum.DispatcherAudit.通过;
                                electricalTask.AuditDate = now;
                                electricalTask.AuditMessage = applicationSheet.AuditMessage;
                                db.SaveChanges();

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
                                string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, "有新的" + ah.Name + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + System.Enum.GetName(typeof(ElectricalTaskType), electricalTaskType)+"任务");

                            }
                            else
                            {
                                //检查其他是否审核均通过，通过则发布停电任务并发送消息
                            }
                            new LogDAO().AddLog(LogCode.审核成功, loginUser.Realname + "成功审核" + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + ClassUtil.GetEntityName(operation), db);


                        }
                        else if (applicationSheet.Audit == Enum.Audit.驳回)
                        {
                            ElectricalTaskType electricalTaskType = new ElectricalTaskType();
                            if (operation.OperationFlow == OperationFlow.低压停电作业审核)
                            {
                                electricalTaskType = ElectricalTaskType.停电作业;
                            }
                            //else
                            //{
                            //    electricalTaskType = ElectricalTaskType.送电作业;
                            //}
                            //selectedApplicationSheet.Audit = Enum.Audit.驳回;
                            //selectedApplicationSheet.AuditDate = now;
                            //selectedApplicationSheet.AuditUserID = loginUser.ID;
                            //selectedApplicationSheet.AuditMessage = applicationSheet.AuditMessage;
                            ElectricalTask electricalTask = db.ElectricalTask.FirstOrDefault(t => t.OperationID == operation.ID && t.ElectricalTaskType == electricalTaskType);
                            electricalTask.AuditUserID = loginUser.ID;
                            //electricalTask.Audit = Enum.Audit.驳回;
                            electricalTask.DispatcherAudit = Enum.DispatcherAudit.驳回;
                            electricalTask.AuditDate = now;
                            electricalTask.AuditMessage = applicationSheet.AuditMessage;
                            

                            operation.OperationFlow = OperationFlow.作业终止;
                            operation.IsFinish = true;
                            operation.FinishDate = now;
                            db.SaveChanges();

                            //发消息给发起人

                            string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.MessageSecret);
                            string resultMessage = WeChatAPI.SendMessage(accessToken, db.User.FirstOrDefault(t => t.ID == operation.UserID).WeChatID, ParaUtil.MessageAgentid, "您的作业被驳回，原因为：" + applicationSheet.AuditMessage);


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

                    ApplicationSheet applicationSheet = db.ApplicationSheet.FirstOrDefault(t => t.ID == id);
                    if (applicationSheet == null)
                    {
                        throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new ApplicationSheet()));
                    }

                    Operation operation = db.Operation.FirstOrDefault(t => t.ID == applicationSheet.OperationID);
                    result = ApiResult.NewSuccessJson(new
                    {
                        applicationSheet.ID,
                        //applicationSheet.WorkContent,
                        WorkContent = System.Enum.GetName(typeof(WorkContentType),applicationSheet.WorkContentType),
                        AuditUserName = db.User.FirstOrDefault(t => t.ID == applicationSheet.UserID).Realname,
                        applicationSheet.AuditMessage,
                        AuditDate = applicationSheet.AuditDate.HasValue ? applicationSheet.AuditDate.Value.ToString("yyyy-MM-dd HH:mm") : null,
                        db.User.FirstOrDefault(t => t.ID == applicationSheet.UserID).Realname,
                        AHName = db.AH.FirstOrDefault(t => t.ID == operation.AHID).Name,
                        CreateDate = applicationSheet.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                        BeginDate = applicationSheet.BeginDate.ToString("yyyy-MM-dd HH:mm"),
                        EndDate = applicationSheet.EndDate.ToString("yyyy-MM-dd HH:mm"),
                        VoltageType = System.Enum.GetName(typeof(VoltageType), operation.VoltageType),
                        OperationFlow = System.Enum.GetName(typeof(OperationFlow), operation.OperationFlow),
                        Audit = System.Enum.GetName(typeof(Audit), applicationSheet.Audit),
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

        public ApiResult List(int? departmentID = null, string no = "", VoltageType? voltageType = null, Audit? audit = null, int? ahID = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
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

                    IQueryable<ApplicationSheet> applicationSheetIQueryable = db.ApplicationSheet.Where(t => t.IsDelete != true &&
                    (departmentID == null || t.DepartmentID == departmentID) &&
                    (audit == null || t.Audit == audit) &&
                    t.NO.Contains(no) &&
                    (ahID == null || db.Operation.Where(m => m.AHID == ahID).Select(m => m.ID).Contains(t.OperationID)) &&
                    (voltageType == null || db.Operation.Where(m => m.VoltageType == voltageType).Select(m => m.ID).Contains(t.OperationID)) &&
                    (t.BeginDate >= beginDate && t.EndDate <= endDate)
                    );

                    int total = applicationSheetIQueryable.Count();
                    List<ApplicationSheet> applicationSheetList = applicationSheetIQueryable.OrderByDescending(t => t.CreateDate).Skip((page - 1) * limit).Take(limit).ToList();
                    List<int> operationIDList = applicationSheetList.Select(t => t.OperationID).ToList();

                    List<Operation> operationList = db.Operation.ToList();
                    List<int> ahIDList = operationList.Select(t => t.AHID).Distinct().ToList();
                    List<int> userIDList = operationList.Select(t => t.UserID).Distinct().ToList();

                    List<object> returnList = new List<object>();
                    List<AH> ahList = db.AH.Where(t => ahIDList.Contains(t.ID)).ToList();
                    List<User> userList = db.User.Where(t => userIDList.Contains(t.ID)).ToList();

                    foreach (ApplicationSheet applicationSheet in applicationSheetList)
                    {
                        Operation operation = operationList.FirstOrDefault(t => t.ID == applicationSheet.OperationID);
                        returnList.Add(new
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

        public ApiResult MyAuditList(int? departmentID = null, string no = "", VoltageType? voltageType = null, bool isAudit = false, int? ahID = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;

            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    beginDate = beginDate ?? DateTime.MinValue;
                    endDate = endDate ?? DateTime.MaxValue;
                    no = no ?? string.Empty;
                    User loginUser = LoginHelper.CurrentUser(db);
                    IQueryable<ApplicationSheet> applicationSheetIQueryable = db.ApplicationSheet.Where(t => t.IsDelete != true &&
                    t.AuditUserID == loginUser.ID &&
                    (departmentID == null || t.DepartmentID == departmentID) &&
                    ((!isAudit && t.Audit == Enum.Audit.待审核 && db.Operation.Where(o=>o.OperationFlow!= OperationFlow.作业终止).Select(o=>o.ID).Contains(t.OperationID)) || (isAudit && (t.Audit == Enum.Audit.通过 || t.Audit == Enum.Audit.驳回))) &&
                    t.NO.Contains(no) &&
                    (ahID == null || db.Operation.Where(m => m.AHID == ahID).Select(m => m.ID).Contains(t.OperationID)) &&
                    (voltageType == null || db.Operation.Where(m => m.VoltageType == voltageType).Select(m => m.ID).Contains(t.OperationID)) &&
                    (t.BeginDate >= beginDate && t.EndDate <= endDate)
                    );

                    int total = applicationSheetIQueryable.Count();
                    List<ApplicationSheet> applicationSheetList = applicationSheetIQueryable.OrderByDescending(t => t.CreateDate).Skip((page - 1) * limit).Take(limit).ToList();
                    List<int> operationIDList = applicationSheetList.Select(t => t.OperationID).ToList();

                    List<Operation> operationList = db.Operation.ToList();
                    List<int> ahIDList = operationList.Select(t => t.AHID).Distinct().ToList();
                    List<int> userIDList = operationList.Select(t => t.UserID).Distinct().ToList();

                    List<object> returnList = new List<object>();
                    List<AH> ahList = db.AH.Where(t => ahIDList.Contains(t.ID)).ToList();
                    List<User> userList = db.User.Where(t => userIDList.Contains(t.ID)).ToList();

                    foreach (ApplicationSheet applicationSheet in applicationSheetList)
                    {
                        Operation operation = operationList.FirstOrDefault(t => t.ID == applicationSheet.OperationID);
                        returnList.Add(new
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

        public ApiResult Export(int id)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    ApplicationSheet applicationSheet = db.ApplicationSheet.FirstOrDefault(t => t.IsDelete != true && t.ID == id);
                    if (applicationSheet == null)
                    {
                        throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new ApplicationSheet()));
                    }
                    Operation operation = db.Operation.FirstOrDefault(t => t.ID == applicationSheet.OperationID);
                    Department department = db.Department.FirstOrDefault(t => t.ID == applicationSheet.DepartmentID);
                    User createUser = db.User.FirstOrDefault(t => t.ID == applicationSheet.UserID);
                    User auditUser = db.User.FirstOrDefault(t => t.ID == applicationSheet.AuditUserID);

                    AH aH = db.AH.FirstOrDefault(t => t.ID == operation.AHID);
                    Document doc = new Document();
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    builder.MoveToBookmark("Title");
                    builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                    builder.Font.Name = "黑体";
                    builder.Font.Size = 18;
                    builder.Write("裕溪口分公司停电作业申请单");

                    builder.MoveToBookmark("Table");
                    double width = 70;
                    builder.CellFormat.Width = width;
                    builder.CellFormat.PreferredWidth = PreferredWidth.FromPoints(width);
                    builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;//垂直居中对齐
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;//水平居中对齐
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;


                    //第一行第一列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Size = 9;
                    builder.Font.Name = "黑体";
                    builder.Write("申请停电单位:" + department.Name);

                    builder.CellFormat.Width = width;
                    builder.CellFormat.PreferredWidth = PreferredWidth.FromPoints(width);
                    builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;//垂直居中对齐
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;//水平居中对齐
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                    //第一行第二列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Write("申请人:" + createUser.Realname);
                    builder.EndRow();

                    //第二行第一列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.First;
                    builder.Write("计划停电时间:" + applicationSheet.BeginDate.ToString("yyyy年MM月dd日HH时mm分") + "至" + applicationSheet.EndDate.ToString("yyyy年MM月dd日HH时mm分"));

                    //第二行第二列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                    builder.EndRow();

                    //第三行第一列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.First;
                    builder.RowFormat.Height = 30;
                    builder.Write("停电设备:" + aH.Name);

                    //第三行第二列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                    builder.EndRow();

                    //第四行第一列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.First;
                    //builder.Write("作业内容:" + applicationSheet.WorkContent);
                    builder.Write("作业内容:" + System.Enum.GetName(typeof(WorkContentType),applicationSheet.WorkContentType));

                    //第四行第二列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                    builder.EndRow();

                    //第五行第一列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.First;
                    builder.RowFormat.Height = 12;
                    builder.Write("安全技术措施:" + "\r\n" + "1、将相关设备停电挂牌并验电；2、执行公司安全规程；3、执行登高作业的相关规定；4、执行起重作业的相关规定；5、执行烧焊的相关规定；6、作业完毕所有的防护均恢复后送电试车。");

                    //第五行第二列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                    builder.EndRow();


                    //第六行第一列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.First;
                    builder.Write("批准人:" + auditUser.Realname + "(" + System.Enum.GetName(typeof(Audit), applicationSheet.Audit) + ")");

                    //第六行第二列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                    builder.EndRow();

                    string FilePath = "停电作业申请单" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".doc";
                    doc.Save(ParaUtil.ResourcePath + FilePath, Aspose.Words.SaveFormat.Doc);

                    using (FileStream fs = new FileStream(ParaUtil.ResourcePath + FilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[(int)fs.Length];
                        fs.Read(bytes, 0, bytes.Length);
                        fs.Close();
                        HttpContext.Current.Response.ContentType = "application/octet-stream";
                        HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(FilePath, Encoding.UTF8));
                        HttpContext.Current.Response.BinaryWrite(bytes);
                        HttpContext.Current.Response.Flush();
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                    new LogDAO().AddLog(LogCode.导出, "成功导出停电作业申请单", db);
                    result = ApiResult.NewSuccessJson("成功导出停电作业申请单");

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

        public ApiResult ExportAllSheet(int id)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    ApplicationSheet applicationSheet = db.ApplicationSheet.FirstOrDefault(t => t.IsDelete != true && t.ID == id);
                    if (applicationSheet == null)
                    {
                        throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new ApplicationSheet()));
                    }
                    Operation operation = db.Operation.FirstOrDefault(t => t.ID == applicationSheet.OperationID);
                    Department department = db.Department.FirstOrDefault(t => t.ID == applicationSheet.DepartmentID);
                    List<User> userList = db.User.ToList();
                    User createUser = userList.FirstOrDefault(t => t.ID == applicationSheet.UserID);
                    User auditUser = userList.FirstOrDefault(t => t.ID == applicationSheet.AuditUserID);

                    AH aH = db.AH.FirstOrDefault(t => t.ID == operation.AHID);

                    
                    //停电电工信息
                    //ElectricalTask stopElectricalTask = db.ElectricalTask.FirstOrDefault(t => t.OperationID == operation.ID && t.ElectricalTaskType == ElectricalTaskType.停电作业 && t.Audit != Enum.Audit.待审核);
                    ElectricalTask stopElectricalTask = db.ElectricalTask.FirstOrDefault(t => t.OperationID == operation.ID && t.ElectricalTaskType == ElectricalTaskType.停电作业 && t.DispatcherAudit != Enum.DispatcherAudit.待审核);
                    if(stopElectricalTask != null)
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
                    }

                    //摘牌电工信息
                    ElectricalTask pickElectricalTask = db.ElectricalTask.FirstOrDefault(t => t.OperationID == operation.ID && t.ElectricalTaskType == ElectricalTaskType.摘牌作业);
                    if(pickElectricalTask != null)
                    {
                        pickElectricalTask.ElectricalTaskTypeName = System.Enum.GetName(typeof(ElectricalTaskType), pickElectricalTask.ElectricalTaskType);
                        List<ElectricalTaskUser> sendElectricalTaskUserList = db.ElectricalTaskUser.Where(t => t.ElectricalTaskID == pickElectricalTask.ID && t.IsBack != true).OrderByDescending(t => t.Date).ToList();
                        sendElectricalTaskUserList.ForEach(t => t.CreateDate = t.Date.ToString("yyyy-MM-dd HH:mm"));
                        sendElectricalTaskUserList.ForEach(t => t.RealName = userList.FirstOrDefault(u => u.ID == t.UserID).Realname);

                        pickElectricalTask.ElectricalTaskUserList = sendElectricalTaskUserList;
                    }

                    //送电电工信息
                    //ElectricalTask sendElectricalTask = db.ElectricalTask.FirstOrDefault(t => t.OperationID == operation.ID && t.ElectricalTaskType == ElectricalTaskType.送电作业 && t.Audit != Enum.Audit.待审核);
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
                    }

                    Document doc = new Document();
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    builder.MoveToBookmark("Title");
                    builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                    builder.Font.Name = "黑体";
                    builder.Font.Size = 18;
                    builder.Write("裕溪口分公司停送电作业全流程表单");

                    builder.MoveToBookmark("Table");
                    double width = 70;
                    builder.CellFormat.Width = width;
                    builder.CellFormat.PreferredWidth = PreferredWidth.FromPoints(width);
                    builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;//垂直居中对齐
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;//水平居中对齐
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;


                    //第一行第一列
                    builder.InsertCell();
                    builder.RowFormat.Height = 30;
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.First;
                    builder.Font.Size = 9;
                    builder.Font.Name = "黑体";
                    builder.Write("申请停电单位:" + department.Name);

                    builder.CellFormat.Width = width;
                    builder.CellFormat.PreferredWidth = PreferredWidth.FromPoints(width);
                    builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;//垂直居中对齐
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;//水平居中对齐

                    //第一行第二列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous ;
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                    //第一行第三列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.First;
                    builder.Write("申请人:" + createUser.Realname);

                    //第一行第四列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;

                    builder.EndRow();

                    //第二行第一列
                    builder.InsertCell();
                    builder.RowFormat.Height = 30;
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.First;
                    builder.Write("计划停电时间:" + applicationSheet.BeginDate.ToString("yyyy年MM月dd日HH时mm分") + "至" + applicationSheet.EndDate.ToString("yyyy年MM月dd日HH时mm分"));

                    //第二行第二列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;

                    //第二行第三列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;

                    //第二行第四列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                    builder.EndRow();

                    //第三行第一列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.First;
                    builder.RowFormat.Height = 30;
                    builder.Write("停电设备:" + aH.Name);

                    //第三行第二列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                    //第三行第三列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.First;
                    builder.Write("作业状态:" + System.Enum.GetName(typeof(OperationFlow), operation.OperationFlow));

                    //第三行第四列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                    builder.EndRow();

                    //第四行第一列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.First;
                    //builder.Write("作业内容:" + applicationSheet.WorkContent);
                    builder.Write("作业内容:" + System.Enum.GetName(typeof(WorkContentType),applicationSheet.WorkContentType));

                    //第四行第二列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;

                    //第四行第三列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;

                    //第四行第四列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                    builder.EndRow();

                    //第五行第一列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.First;
                    builder.Write("批准人:" + auditUser.Realname + "(" + applicationSheet.AuditDate.Value.ToString("yyyy-MM-dd HH:mm") + System.Enum.GetName(typeof(Audit), applicationSheet.Audit) + ")");

                    //第五行第二列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                    //第五行第三列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.First;
                    builder.Write("审核理由:" + applicationSheet.AuditMessage);

                    //第五行第四列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                    builder.EndRow();


                    

                    //通过了才会有调度审核
                    if (applicationSheet.Audit == Enum.Audit.通过)
                    {
                        //停电作业
                        //if (stopElectricalTask != null && stopElectricalTask.Audit!=Enum.Audit.待审核)
                        if (stopElectricalTask != null && stopElectricalTask.DispatcherAudit!=Enum.DispatcherAudit.待审核)
                        {
                            builder.InsertCell();
                            builder.CellFormat.VerticalMerge = CellMerge.None;
                            builder.CellFormat.HorizontalMerge = CellMerge.First;

                            builder.Write("停电调度审核:" + stopElectricalTask.RealName + "("+stopElectricalTask.AuditDateString + stopElectricalTask.AuditName + ")");

                            builder.InsertCell();
                            builder.CellFormat.VerticalMerge = CellMerge.None;
                            builder.CellFormat.HorizontalMerge = CellMerge.Previous;

                            builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                            builder.InsertCell();
                            builder.CellFormat.VerticalMerge = CellMerge.None;
                            builder.CellFormat.HorizontalMerge = CellMerge.First;
                            builder.Write("审核理由:" + stopElectricalTask.AuditMessage);

                            builder.InsertCell();
                            builder.CellFormat.VerticalMerge = CellMerge.None;
                            builder.CellFormat.HorizontalMerge = CellMerge.Previous;

                            builder.EndRow();

                            
                            foreach (ElectricalTaskUser stopTaskUser in stopElectricalTask.ElectricalTaskUserList)
                            {
                                builder.InsertCell();
                                builder.CellFormat.VerticalMerge = CellMerge.None;
                                builder.CellFormat.HorizontalMerge = CellMerge.None;

                                builder.Write("停电电工:");
                                builder.CellFormat.Borders.LineStyle = LineStyle.Single;


                                builder.InsertCell();
                                builder.CellFormat.VerticalMerge = CellMerge.None;
                                builder.CellFormat.HorizontalMerge = CellMerge.None;
                                builder.Write(stopTaskUser.RealName);
                                builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                                builder.InsertCell();
                                builder.CellFormat.VerticalMerge = CellMerge.None;
                                builder.CellFormat.HorizontalMerge = CellMerge.None;
                                builder.Write(stopTaskUser.CreateDate);
                                builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                                builder.InsertCell();
                                builder.CellFormat.VerticalMerge = CellMerge.None;
                                builder.CellFormat.HorizontalMerge = CellMerge.None;
                                builder.Write(stopTaskUser.IsConfirm?"已确认":"未确认");

                                builder.EndRow();
                            }


                        }

                        //电工摘牌
                        if(pickElectricalTask != null)
                        {
                            foreach (ElectricalTaskUser pickTaskUser in pickElectricalTask.ElectricalTaskUserList)
                            {
                                builder.InsertCell();
                                builder.CellFormat.VerticalMerge = CellMerge.None;
                                builder.CellFormat.HorizontalMerge = CellMerge.None;

                                builder.Write("摘牌电工:");
                                builder.CellFormat.Borders.LineStyle = LineStyle.Single;


                                builder.InsertCell();
                                builder.CellFormat.VerticalMerge = CellMerge.None;
                                builder.CellFormat.HorizontalMerge = CellMerge.None;
                                builder.Write(pickTaskUser.RealName);
                                builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                                builder.InsertCell();
                                builder.CellFormat.VerticalMerge = CellMerge.None;
                                builder.CellFormat.HorizontalMerge = CellMerge.None;
                                builder.Write(pickTaskUser.CreateDate);
                                builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                                builder.InsertCell();
                                builder.CellFormat.VerticalMerge = CellMerge.None;
                                builder.CellFormat.HorizontalMerge = CellMerge.None;
                                builder.Write(pickTaskUser.IsConfirm ? "已确认" : "未确认");

                                builder.EndRow();
                            }
                        }

                        //送电作业
                        if (sendElectricalTask != null)
                        {
                            //builder.InsertCell();
                            //builder.CellFormat.VerticalMerge = CellMerge.None;
                            //builder.CellFormat.HorizontalMerge = CellMerge.First;

                            //builder.Write("送电调度审核:" + sendElectricalTask.RealName + "("+ sendElectricalTask.AuditDateString + sendElectricalTask.AuditName + ")");

                            //builder.InsertCell();
                            //builder.CellFormat.VerticalMerge = CellMerge.None;
                            //builder.CellFormat.HorizontalMerge = CellMerge.Previous;

                            //builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                            //builder.InsertCell();
                            //builder.CellFormat.VerticalMerge = CellMerge.None;
                            //builder.CellFormat.HorizontalMerge = CellMerge.First;
                            //builder.Write("审核理由:" + sendElectricalTask.AuditMessage);

                            //builder.InsertCell();
                            //builder.CellFormat.VerticalMerge = CellMerge.None;
                            //builder.CellFormat.HorizontalMerge = CellMerge.Previous;

                            //builder.EndRow();


                            foreach (ElectricalTaskUser sendTaskUser in sendElectricalTask.ElectricalTaskUserList)
                            {
                                builder.InsertCell();
                                builder.CellFormat.VerticalMerge = CellMerge.None;
                                builder.CellFormat.HorizontalMerge = CellMerge.None;

                                builder.Write("送电电工:");
                                builder.CellFormat.Borders.LineStyle = LineStyle.Single;


                                builder.InsertCell();
                                builder.CellFormat.VerticalMerge = CellMerge.None;
                                builder.CellFormat.HorizontalMerge = CellMerge.None;
                                builder.Write(sendTaskUser.RealName);
                                builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                                builder.InsertCell();
                                builder.CellFormat.VerticalMerge = CellMerge.None;
                                builder.CellFormat.HorizontalMerge = CellMerge.None;
                                builder.Write(sendTaskUser.CreateDate);
                                builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                                builder.InsertCell();
                                builder.CellFormat.VerticalMerge = CellMerge.None;
                                builder.CellFormat.HorizontalMerge = CellMerge.None;
                                builder.Write(sendTaskUser.IsConfirm ? "已确认" : "未确认");

                                builder.EndRow();
                            }


                        }
                    }


                    string FilePath = "停送电作业全流程表单" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".doc";
                    doc.Save(ParaUtil.ResourcePath + FilePath, Aspose.Words.SaveFormat.Doc);

                    using (FileStream fs = new FileStream(ParaUtil.ResourcePath + FilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[(int)fs.Length];
                        fs.Read(bytes, 0, bytes.Length);
                        fs.Close();
                        HttpContext.Current.Response.ContentType = "application/octet-stream";
                        HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(FilePath, Encoding.UTF8));
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
