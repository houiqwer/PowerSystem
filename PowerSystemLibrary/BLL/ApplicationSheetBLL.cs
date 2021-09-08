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

                        if (applicationSheet.Audit == Enum.Audit.通过)
                        {
                            selectedApplicationSheet.Audit = Enum.Audit.通过;
                            selectedApplicationSheet.AuditDate = now;
                            selectedApplicationSheet.AuditUserID = loginUser.ID;
                            selectedApplicationSheet.AuditMessage = applicationSheet.AuditMessage;


                            operation.OperationFlow = OperationFlow.低压停电作业审核;
                            db.SaveChanges();

                            if (operation.VoltageType == VoltageType.低压)
                            {
                                //发布停电任务
                                ElectricalTask electricalTask = new ElectricalTask();
                                electricalTask.OperationID = operation.ID;
                                electricalTask.AHID = operation.AHID;
                                electricalTask.CreateDate = now;
                                electricalTask.ElectricalTaskType = ElectricalTaskType.停电作业;
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
                                string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, "有新的" + ah.Name + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + "停电任务");

                            }
                            else
                            {
                                //检查其他是否审核均通过，通过则发布停电任务并发送消息
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
                            db.SaveChanges();
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

        public ApiResult List(int? departmentID = null, string no = "", VoltageType? voltageType = null, Audit? audit = null, int? ahID = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
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

        public ApiResult MyAuditList(int? departmentID = null, string no = "", VoltageType? voltageType = null, Audit? audit = null, int? ahID = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
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
