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
    public class ElectricalTaskBLL
    {
        private static object locker = new object();
        public ApiResult Accept(ElectricalTask electricalTask)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            lock (locker)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (PowerSystemDBContext db = new PowerSystemDBContext())
                    {
                        try
                        {
                            DateTime now = DateTime.Now;

                            User loginUser = LoginHelper.CurrentUser(db);

                            ElectricalTask selectedElectricalTask = db.ElectricalTask.FirstOrDefault(t => t.ID == electricalTask.ID);
                            if (selectedElectricalTask == null)
                            {
                                throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new ElectricalTask()));
                            }

                            if (selectedElectricalTask.ReciveCount >= ParaUtil.MaxReciveCount)
                            {
                                throw new ExceptionUtil(ClassUtil.GetEntityName(new ElectricalTask()) + "已有" + ParaUtil.MaxReciveCount + "人接收");
                            }

                            if (db.ElectricalTaskUser.FirstOrDefault(t => t.UserID == loginUser.ID && t.ElectricalTaskID == selectedElectricalTask.ID && !t.IsBack) != null)
                            {
                                throw new ExceptionUtil("您已接收该" + ClassUtil.GetEntityName(new ElectricalTask()));
                            }

                            Operation operation = db.Operation.FirstOrDefault(t => t.ID == selectedElectricalTask.OperationID);
                            AH ah = db.AH.FirstOrDefault(t => t.ID == selectedElectricalTask.AHID);

                            ElectricalTaskUser electricalTaskUser = new ElectricalTaskUser();
                            electricalTaskUser.ElectricalTaskID = selectedElectricalTask.ID;
                            electricalTaskUser.UserID = loginUser.ID;
                            electricalTaskUser.Date = now;
                            db.ElectricalTaskUser.Add(electricalTaskUser);

                            selectedElectricalTask.ReciveCount = selectedElectricalTask.ReciveCount + 1;

                            if (selectedElectricalTask.ElectricalTaskType == ElectricalTaskType.停电作业)
                            {
                                if (selectedElectricalTask.ReciveCount == ParaUtil.MaxReciveCount)
                                {
                                    operation.OperationFlow = ah.VoltageType == VoltageType.低压 ? OperationFlow.低压停电任务操作 : OperationFlow.高压停电任务操作;
                                }
                                else
                                {
                                    operation.OperationFlow = ah.VoltageType == VoltageType.低压 ? OperationFlow.低压停电任务领取 : OperationFlow.高压停电任务领取;
                                }
                            }


                            if (selectedElectricalTask.ElectricalTaskType == ElectricalTaskType.送电作业)
                            {
                                if (selectedElectricalTask.ReciveCount == ParaUtil.MaxReciveCount)
                                {
                                    operation.OperationFlow = ah.VoltageType == VoltageType.低压 ? OperationFlow.低压送电任务操作 : OperationFlow.高压送电任务操作;
                                }
                                else
                                {
                                    operation.OperationFlow = ah.VoltageType == VoltageType.低压 ? OperationFlow.低压送电任务领取 : OperationFlow.高压送电任务领取;
                                }
                            }

                            if (selectedElectricalTask.ElectricalTaskType == ElectricalTaskType.摘牌作业)
                            {
                                if (selectedElectricalTask.ReciveCount == ParaUtil.MaxReciveCount)
                                {
                                    operation.OperationFlow = ah.VoltageType == VoltageType.低压 ? OperationFlow.低压摘牌任务操作 : OperationFlow.高压摘牌任务操作;
                                }
                                else
                                {
                                    operation.OperationFlow = ah.VoltageType == VoltageType.低压 ? OperationFlow.低压摘牌任务领取 : OperationFlow.高压摘牌任务领取;
                                }
                            }


                            db.SaveChanges();

                            new LogDAO().AddLog(LogCode.接收任务, "已成功接收" + ah.Name + System.Enum.GetName(typeof(ElectricalTaskType), selectedElectricalTask.ElectricalTaskType) + "任务", db);
                            result = ApiResult.NewSuccessJson("已成功接收" + ah.Name + System.Enum.GetName(typeof(ElectricalTaskType), selectedElectricalTask.ElectricalTaskType) + "任务");
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
                            result = ApiResult.NewErrorJson(LogCode.接受任务错误, message, db);
                        }
                    }
                }
            }
            return result;
        }

        public ApiResult Confirm(ElectricalTask electricalTask)
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

                        ElectricalTask selectedElectricalTask = db.ElectricalTask.FirstOrDefault(t => t.ID == electricalTask.ID);
                        if (selectedElectricalTask == null)
                        {
                            throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new ElectricalTask()));
                        }

                        if (selectedElectricalTask.ReciveCount != ParaUtil.MaxReciveCount)
                        {
                            throw new ExceptionUtil("未达到开始" + ClassUtil.GetEntityName(new ElectricalTask()) + "的人数要求");
                        }

                        ElectricalTaskUser electricalTaskUser = db.ElectricalTaskUser.FirstOrDefault(t => t.ElectricalTaskID == selectedElectricalTask.ID && t.UserID == loginUser.ID && !t.IsBack);
                        if (electricalTaskUser == null)
                        {
                            throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new ElectricalTaskUser()));
                        }

                        if (db.ElectricalTaskUser.FirstOrDefault(t => t.UserID == loginUser.ID && t.ElectricalTaskID == selectedElectricalTask.ID && t.IsConfirm && !t.IsBack) != null)
                        {
                            throw new ExceptionUtil("您已完成该" + ClassUtil.GetEntityName(new ElectricalTask()));
                        }

                        Operation operation = db.Operation.FirstOrDefault(t => t.ID == selectedElectricalTask.OperationID);
                        AH ah = db.AH.FirstOrDefault(t => t.ID == operation.AHID);
                        string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.MessageSecret);
                        if (ah.VoltageType == VoltageType.高压 && selectedElectricalTask.ElectricalTaskType != ElectricalTaskType.摘牌作业)
                        {
                            OperationSheet oldOperationSheet = db.OperationSheet.FirstOrDefault(t => t.OperationID == operation.ID && t.ElectricalTaskID == selectedElectricalTask.ID);
                            if (oldOperationSheet == null)
                            {
                                //操作人填写
                                //if (!ClassUtil.Validate(electricalTask.OperationSheet, ref message))
                                //{
                                //    throw new ExceptionUtil(message);
                                //}
                                if (electricalTask.OperationSheet.OperationContentIDList == null || electricalTask.OperationSheet.OperationContentIDList.Count == 0)
                                {
                                    throw new ExceptionUtil("请选择高压操作内容");
                                }
                                OperationSheet operationSheet = new OperationSheet();
                                operationSheet.ElectricalTaskID = selectedElectricalTask.ID;
                                operationSheet.CreateDate = now;
                                operationSheet.OperationID = operation.ID;
                                //operationSheet.Content = electricalTask.OperationSheet.Content;
                                operationSheet.OperationUserID = loginUser.ID;
                                operationSheet.OperationDate = now;
                                db.OperationSheet.Add(operationSheet);
                                db.SaveChanges();
                                foreach (int contentID in electricalTask.OperationSheet.OperationContentIDList)
                                {
                                    OperationSheet_Content operationSheet_Content = new OperationSheet_Content();
                                    operationSheet_Content.OperationContentID = contentID;
                                    operationSheet_Content.OperationSheetID = operationSheet.ID;
                                    db.OperationSheet_Content.Add(operationSheet_Content);
                                }
                            }
                            else
                            {
                                //审核人填写
                                oldOperationSheet.GuardianUserID = loginUser.ID;
                                oldOperationSheet.FinishDate = now;
                                oldOperationSheet.IsConfirm = true;
                                db.SaveChanges();

                                //送电作业确认时审核人添加
                                if (selectedElectricalTask.ElectricalTaskType == ElectricalTaskType.送电作业)
                                {
                                    if (!ClassUtil.Validate(electricalTask.SendElectricalSheet, ref message))
                                    {
                                        throw new ExceptionUtil(message);
                                    }
                                    SendElectricalSheet sendElectricalSheet = new SendElectricalSheet();
                                    sendElectricalSheet.OperationID = operation.ID;
                                    sendElectricalSheet.ElectricalTaskID = selectedElectricalTask.ID;
                                    sendElectricalSheet.CreateDate = now;
                                    sendElectricalSheet.SendElectricDate = electricalTask.SendElectricalSheet.SendElectricDate;
                                    sendElectricalSheet.WorkFinishContent = electricalTask.SendElectricalSheet.WorkFinishContent;
                                    sendElectricalSheet.IsRemoveGroundLine = electricalTask.SendElectricalSheet.IsRemoveGroundLine;
                                    sendElectricalSheet.IsEvacuateAllPeople = electricalTask.SendElectricalSheet.IsEvacuateAllPeople;
                                    sendElectricalSheet.UserID = loginUser.ID;
                                    db.SendElectricalSheet.Add(sendElectricalSheet);
                                }

                            }
                        }

                        electricalTaskUser.Date = now;
                        electricalTaskUser.IsConfirm = true;
                        db.SaveChanges();

                        //判断是否都完成了
                        if (db.ElectricalTaskUser.Count(t => t.IsConfirm && t.ElectricalTaskID == selectedElectricalTask.ID && !t.IsBack) == ParaUtil.MaxReciveCount)
                        {
                            selectedElectricalTask.IsConfirm = true;
                            if (selectedElectricalTask.ElectricalTaskType == ElectricalTaskType.停电作业)
                            {
                                //通知发起人检修作业

                                //需要验证现场是否已停电
                                ah.AHState = AHState.停电;
                                operation.OperationFlow = ah.VoltageType == VoltageType.低压 ? OperationFlow.低压停电任务完成 : OperationFlow.高压停电任务完成;

                                //同时修改所有该设备未confirm的flow,这个不这么做了
                                //List<Operation> operationList = db.Operation.Where(t => t.AHID == ah.ID && !t.IsConfirm && !t.IsFinish).ToList();
                                //operationList.ForEach(t => t.OperationFlow = OperationFlow.低压停电任务完成);
                                string lampMessage = new LampUtil().OpenOrCloseLamp(ah.LampIP, AHState.停电);
                                if (lampMessage != string.Empty)
                                {
                                    throw new ExceptionUtil(lampMessage);
                                }

                                string resultMessage = WeChatAPI.SendMessage(accessToken, db.User.FirstOrDefault(t => t.ID == operation.UserID).WeChatID, ParaUtil.MessageAgentid, "您申请的" + ah.Name + ClassUtil.GetEntityName(operation) + System.Enum.GetName(typeof(ElectricalTaskType), selectedElectricalTask.ElectricalTaskType) + "已完成，请挂现场停电牌");

                            }
                            else if (selectedElectricalTask.ElectricalTaskType == ElectricalTaskType.送电作业)
                            {
                                //通知发起人和电工作业结束

                                //需要验证现场是否已送电
                                ah.AHState = AHState.正常;
                                operation.OperationFlow = ah.VoltageType == VoltageType.低压 ? OperationFlow.低压送电任务完成 : OperationFlow.高压送电任务完成;

                                operation.IsFinish = true;
                                operation.FinishDate = now;

                                //同时修改所有该设备已confirm的flow,这个不这么做了
                                //List<Operation> operationList = db.Operation.Where(t => t.AHID == ah.ID && t.IsConfirm && !t.IsFinish).ToList();
                                //operationList.ForEach(t => t.OperationFlow = OperationFlow.低压送电任务完成);


                                //string userWeChatString = db.User.FirstOrDefault(t => t.ID == operation.UserID).WeChatID + "|";
                                //List<ElectricalTaskUser> electricalTaskUserList = db.ElectricalTaskUser.Where(t => t.ElectricalTaskID == selectedElectricalTask.ID && t.IsBack!=true).ToList();
                                //List<int> userIDList = electricalTaskUserList.Select(t => t.UserID).ToList();
                                //List<string> weChatIDList = db.User.Where(t => userIDList.Contains(t.ID)).Select(t => t.WeChatID).ToList();
                                //foreach (string weChatID in weChatIDList)
                                //{
                                //    userWeChatString = userWeChatString + weChatID + "|";
                                //}
                                //string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.MessageSecret);
                                string lampMessage = new LampUtil().OpenOrCloseLamp(ah.LampIP, AHState.正常);
                                if (lampMessage != string.Empty)
                                {
                                    throw new ExceptionUtil(lampMessage);
                                }

                                string resultMessage = WeChatAPI.SendMessage(accessToken, db.User.FirstOrDefault(t => t.ID == operation.UserID).WeChatID, ParaUtil.MessageAgentid, "您申请的" + ah.Name + ClassUtil.GetEntityName(operation) + System.Enum.GetName(typeof(ElectricalTaskType), selectedElectricalTask.ElectricalTaskType) + "已完成");

                                //通知调度
                                List<Role> roleList = RoleUtil.GetDispatcherRoleList();
                                List<string> dispatcherWeChatIDList = db.User.Where(t => t.IsDelete != true && t.DepartmentID == loginUser.DepartmentID && db.UserRole.Where(m => roleList.Contains(m.Role)).Select(m => m.UserID).Contains(t.ID)).Select(t => t.WeChatID).ToList();
                                string dispatcherWeChatIDString = "";
                                foreach (string userWeChatID in dispatcherWeChatIDList)
                                {
                                    dispatcherWeChatIDString = dispatcherWeChatIDString + userWeChatID + "|";
                                }
                                dispatcherWeChatIDString.TrimEnd('|');
                                string dispatcherResultMessage = WeChatAPI.SendMessage(accessToken, dispatcherWeChatIDString, ParaUtil.MessageAgentid, ah.Name + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + "恢复送电");


                            }
                            else //摘牌任务
                            {
                                operation.IsPick = true;
                                //判断是否有并行任务
                                int surplusCount = db.Operation.Count(t => t.ID != operation.ID && t.AHID == operation.AHID && (t.IsPick != true && t.OperationFlow != OperationFlow.作业终止));
                                if (surplusCount == 0)//没有 创建送电任务给这两个电工去送电
                                {
                                    db.Operation.Where(t => t.AHID == ah.ID && (t.OperationFlow == OperationFlow.低压停电流程结束 || t.OperationFlow == OperationFlow.高压停电流程结束)).ToList().ForEach(t => t.IsSendElectric = true);
                                    ElectricalTask sendElectricalTask = new ElectricalTask();
                                    sendElectricalTask.DispatcherAudit = Enum.DispatcherAudit.无需审核;
                                    sendElectricalTask.OperationID = operation.ID;
                                    sendElectricalTask.AHID = operation.AHID;
                                    sendElectricalTask.CreateDate = now;
                                    sendElectricalTask.ElectricalTaskType = ElectricalTaskType.送电作业;
                                    db.ElectricalTask.Add(sendElectricalTask);
                                    db.SaveChanges();

                                    string ledMessage = new ShowLed().ShowLedMethod(ah.LedIP, true);
                                    if (ledMessage != string.Empty)
                                    {
                                        throw new ExceptionUtil(ledMessage);
                                    }
                                    //发送消息给电工 
                                    List<string> userWeChatIDList = db.User.Where(t => t.IsDelete != true && db.ElectricalTaskUser.Where(m => m.ElectricalTaskID == selectedElectricalTask.ID).Select(m => m.UserID).Contains(t.ID)).Select(t => t.WeChatID).ToList();
                                    string userWeChatIDString = "";
                                    foreach (string userWeChatID in userWeChatIDList)
                                    {
                                        userWeChatIDString = userWeChatIDString + userWeChatID + "|";
                                    }
                                    userWeChatIDString.TrimEnd('|');

                                    string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, "摘牌任务完成," + ah.Name + "剩余牌数为" + surplusCount + "," + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + "送电任务开始请及时处理");
                                    //db db.ElectricalTaskUser.Where(t=>t.ElectricalTaskID ==  selectedElectricalTask.ID).Select(t=>t.UserID)

                                    //发送消息给调度
                                    List<Role> roleList = RoleUtil.GetDispatcherRoleList();
                                    List<string> dispatcherWeChatIDList = db.User.Where(t => t.IsDelete != true && t.DepartmentID == loginUser.DepartmentID && db.UserRole.Where(m => roleList.Contains(m.Role)).Select(m => m.UserID).Contains(t.ID)).Select(t => t.WeChatID).ToList();
                                    string dispatcherWeChatIDString = "";
                                    foreach (string userWeChatID in dispatcherWeChatIDList)
                                    {
                                        dispatcherWeChatIDString = dispatcherWeChatIDString + userWeChatID + "|";
                                    }
                                    dispatcherWeChatIDString.TrimEnd('|');
                                    //string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.MessageSecret);
                                    string dispatcherResultMessage = WeChatAPI.SendMessage(accessToken, dispatcherWeChatIDString, ParaUtil.MessageAgentid, ah.Name + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + "送电任务开始");

                                }
                                else
                                {
                                    operation.FinishDate = now;
                                    operation.IsFinish = true;
                                    operation.OperationFlow = ah.VoltageType == VoltageType.低压 ? OperationFlow.低压停电流程结束 : OperationFlow.高压停电流程结束;

                                    string ledMessage = new ShowLed().ShowLedMethod(ah.LedIP, false, surplusCount);
                                    if (ledMessage != string.Empty)
                                    {
                                        throw new ExceptionUtil(ledMessage);
                                    }

                                    //发送消息给电工 
                                    List<Role> roleList = RoleUtil.GetElectricianRoleList();
                                    List<string> userWeChatIDList = db.User.Where(t => t.IsDelete != true && t.DepartmentID == loginUser.DepartmentID && db.UserRole.Where(m => roleList.Contains(m.Role)).Select(m => m.UserID).Contains(t.ID)).Select(t => t.WeChatID).ToList();
                                    string userWeChatIDString = "";
                                    foreach (string userWeChatID in userWeChatIDList)
                                    {
                                        userWeChatIDString = userWeChatIDString + userWeChatID + "|";
                                    }
                                    userWeChatIDString.TrimEnd('|');

                                    string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, ah.Name + "剩余牌数为" + surplusCount + ",牌未加完,禁止送电");
                                }
                            }
                            db.SaveChanges();
                        }

                        new LogDAO().AddLog(LogCode.完成任务, "已成功确认" + System.Enum.GetName(typeof(ElectricalTaskType), selectedElectricalTask.ElectricalTaskType) + "任务", db);
                        result = ApiResult.NewSuccessJson("已成功确认" + System.Enum.GetName(typeof(ElectricalTaskType), selectedElectricalTask.ElectricalTaskType) + "任务");
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
                        result = ApiResult.NewErrorJson(LogCode.完成任务错误, message, db);
                    }
                }
            }
            return result;
        }


        public ApiResult List(int? ahID = null, ElectricalTaskType? electricalTaskType = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
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

                    IQueryable<ElectricalTask> electricalTaskIQueryable = db.ElectricalTask.Where(t =>
                    (ahID == null || t.AHID == ahID) &&
                    (electricalTaskType == null || t.ElectricalTaskType == electricalTaskType) &&
                    (t.CreateDate >= beginDate && t.CreateDate <= endDate)
                    );
                    int total = electricalTaskIQueryable.Count();
                    List<ElectricalTask> electricalTaskList = electricalTaskIQueryable.OrderByDescending(t => t.CreateDate).Skip((page - 1) * limit).Take(limit).ToList();
                    List<int> ahIDList = electricalTaskList.Select(t => t.AHID).Distinct().ToList();


                    List<object> returnList = new List<object>();
                    List<AH> ahList = db.AH.Where(t => ahIDList.Contains(t.ID)).ToList();

                    foreach (ElectricalTask electricalTask in electricalTaskList)
                    {
                        returnList.Add(new
                        {
                            electricalTask.ID,
                            electricalTask.OperationID,
                            AHName = ahList.FirstOrDefault(t => t.ID == electricalTask.AHID).Name,
                            CreateDate = electricalTask.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                            electricalTask.ReciveCount,
                            electricalTask.IsConfirm,
                            ElectricalTaskTypeName = System.Enum.GetName(typeof(ElectricalTaskType), electricalTask.ElectricalTaskType)
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

        public ApiResult NotAcceptedList(int? ahID = null, ElectricalTaskType? electricalTaskType = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
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

                    IQueryable<ElectricalTask> electricalTaskIQueryable = db.ElectricalTask.Where(t =>
                    //t.IsConfirm != true && t.ReciveCount < 2 && t.Audit == Audit.通过 &&
                    t.IsConfirm != true && t.ReciveCount < 2 && (t.DispatcherAudit == Enum.DispatcherAudit.通过 || t.DispatcherAudit == Enum.DispatcherAudit.无需审核) &&
                    (ahID == null || t.AHID == ahID) &&
                     db.ElectricalTaskUser.FirstOrDefault(m => m.UserID == loginUser.ID && m.ElectricalTaskID == t.ID && !m.IsBack) == null &&
                    (electricalTaskType == null || t.ElectricalTaskType == electricalTaskType) &&
                    (t.CreateDate >= beginDate && t.CreateDate <= endDate)
                    );
                    int total = electricalTaskIQueryable.Count();
                    List<ElectricalTask> electricalTaskList = electricalTaskIQueryable.OrderByDescending(t => t.CreateDate).Skip((page - 1) * limit).Take(limit).ToList();
                    List<int> ahIDList = electricalTaskList.Select(t => t.AHID).Distinct().ToList();
                    List<int> electricalIDTaskList = electricalTaskList.Select(t => t.ID).ToList();

                    List<object> returnList = new List<object>();
                    List<AH> ahList = db.AH.Where(t => ahIDList.Contains(t.ID)).ToList();
                    //List<ElectricalTaskUser> electricalTaskUserList = db.ElectricalTaskUser.Where(t => electricalIDTaskList.Contains(t.ElectricalTaskID)).ToList();

                    foreach (ElectricalTask electricalTask in electricalTaskList)
                    {
                        AH ah = ahList.FirstOrDefault(t => t.ID == electricalTask.AHID);
                        returnList.Add(new
                        {
                            electricalTask.ID,
                            AHName = ah.Name,
                            VoltageTypeName = System.Enum.GetName(typeof(VoltageType), ah.VoltageType),
                            CreateDate = electricalTask.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                            electricalTask.ReciveCount,
                            ElectricalTaskTypeName = System.Enum.GetName(typeof(ElectricalTaskType), electricalTask.ElectricalTaskType),
                            //IsAccepted = electricalTaskUserList.Exists(t => t.ElectricalTaskID == electricalTask.ID && t.UserID == loginUser.ID)
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

        public ApiResult AcceptedList(int? ahID = null, ElectricalTaskType? electricalTaskType = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
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

                    IQueryable<ElectricalTask> electricalTaskIQueryable = db.ElectricalTask.Where(t =>
                    //(ahID == null || t.AHID == ahID) && t.Audit == Audit.通过 &&
                    (ahID == null || t.AHID == ahID) && (t.DispatcherAudit == Enum.DispatcherAudit.通过 || t.DispatcherAudit == Enum.DispatcherAudit.无需审核) &&
                     db.ElectricalTaskUser.FirstOrDefault(m => m.UserID == loginUser.ID && m.ElectricalTaskID == t.ID && !m.IsBack) != null &&
                     db.ElectricalTaskUser.FirstOrDefault(m => m.UserID == loginUser.ID && m.ElectricalTaskID == t.ID && m.IsConfirm) == null &&
                    (electricalTaskType == null || t.ElectricalTaskType == electricalTaskType) &&
                    (t.CreateDate >= beginDate && t.CreateDate <= endDate)
                    );
                    int total = electricalTaskIQueryable.Count();
                    List<ElectricalTask> electricalTaskList = electricalTaskIQueryable.OrderBy(t => t.IsConfirm).ThenByDescending(t => t.CreateDate).Skip((page - 1) * limit).Take(limit).ToList();
                    List<int> ahIDList = electricalTaskList.Select(t => t.AHID).Distinct().ToList();
                    List<int> electricalIDTaskList = electricalTaskList.Select(t => t.ID).ToList();

                    List<object> returnList = new List<object>();
                    List<AH> ahList = db.AH.Where(t => ahIDList.Contains(t.ID)).ToList();
                    //List<ElectricalTaskUser> electricalTaskUserList = db.ElectricalTaskUser.Where(t => electricalIDTaskList.Contains(t.ElectricalTaskID)).ToList();

                    //List< OperationSheet> operationSheetList = db.OperationSheet.Where(t => electricalIDTaskList.Contains(t.ElectricalTaskID)).ToList();

                    foreach (ElectricalTask electricalTask in electricalTaskList)
                    {
                        AH ah = ahList.FirstOrDefault(t => t.ID == electricalTask.AHID);
                        returnList.Add(new
                        {
                            electricalTask.ID,
                            AHName = ah.Name,
                            ah.VoltageType,
                            VoltageTypeName = System.Enum.GetName(typeof(VoltageType), ah.VoltageType),
                            CreateDate = electricalTask.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                            electricalTask.IsConfirm,
                            electricalTask.ReciveCount,
                            electricalTask.ElectricalTaskType,
                            ElectricalTaskTypeName = System.Enum.GetName(typeof(ElectricalTaskType), electricalTask.ElectricalTaskType),
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


        public ApiResult Back(ElectricalTask electricalTask)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (TransactionScope ts = new TransactionScope())
            {
                using (PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    try
                    {
                        User loginUser = LoginHelper.CurrentUser(db);

                        ElectricalTask selectedElectricalTask = db.ElectricalTask.FirstOrDefault(t => t.ID == electricalTask.ID);
                        if (selectedElectricalTask == null)
                        {
                            throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new ElectricalTask()));
                        }

                        ElectricalTaskUser electricalTaskUser = db.ElectricalTaskUser.FirstOrDefault(t => t.ElectricalTaskID == selectedElectricalTask.ID && t.UserID == loginUser.ID && !t.IsBack);
                        if (electricalTaskUser == null)
                        {
                            throw new ExceptionUtil("您未领取任务");
                        }
                        if (electricalTaskUser.IsConfirm)
                        {
                            throw new ExceptionUtil("任务已确认，无法退回");
                        }
                        if (electricalTaskUser.IsBack)
                        {
                            throw new ExceptionUtil("该任务已退回");
                        }
                        electricalTaskUser.IsBack = true;
                        selectedElectricalTask.ReciveCount -= 1;
                        db.SaveChanges();
                        new LogDAO().AddLog(LogCode.退回任务, "已成功退回" + System.Enum.GetName(typeof(ElectricalTaskType), selectedElectricalTask.ElectricalTaskType) + "任务", db);
                        result = ApiResult.NewSuccessJson("已成功退回" + System.Enum.GetName(typeof(ElectricalTaskType), selectedElectricalTask.ElectricalTaskType) + "任务");
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
                        result = ApiResult.NewErrorJson(LogCode.退回任务, message, db);
                    }
                }
            }
            return result;
        }


        public ApiResult DispatcherAuditList(int? ahID = null, ElectricalTaskType? electricalTaskType = null, DateTime? beginDate = null, DateTime? endDate = null, bool isAudit = false, int page = 1, int limit = 10)
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

                    IQueryable<ElectricalTask> electricalTaskIQueryable = db.ElectricalTask.Where(t => ((!isAudit && t.DispatcherAudit == Enum.DispatcherAudit.待审核) || (isAudit && t.AuditUserID == loginUser.ID && (t.DispatcherAudit == Enum.DispatcherAudit.通过 || t.DispatcherAudit == Enum.DispatcherAudit.驳回))) &&
                    (ahID == null || t.AHID == ahID) &&
                    (electricalTaskType == null || t.ElectricalTaskType == electricalTaskType) &&
                    (t.CreateDate >= beginDate && t.CreateDate <= endDate));
                    int total = electricalTaskIQueryable.Count();
                    List<ElectricalTask> electricalTaskList = electricalTaskIQueryable.OrderByDescending(t => t.CreateDate).Skip((page - 1) * limit).Take(limit).ToList();
                    List<int> ahIDList = electricalTaskList.Select(t => t.AHID).Distinct().ToList();

                    List<object> returnList = new List<object>();
                    List<AH> ahList = db.AH.Where(t => ahIDList.Contains(t.ID)).ToList();
                    foreach (ElectricalTask electricalTask in electricalTaskList)
                    {
                        AH ah = ahList.FirstOrDefault(t => t.ID == electricalTask.AHID);
                        returnList.Add(new
                        {
                            electricalTask.ID,
                            AHName = ah.Name,
                            VoltageTypeName = System.Enum.GetName(typeof(VoltageType), ah.VoltageType),
                            CreateDate = electricalTask.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                            //electricalTask.ReciveCount,
                            ElectricalTaskTypeName = System.Enum.GetName(typeof(ElectricalTaskType), electricalTask.ElectricalTaskType),
                            electricalTask.OperationID,
                            //Audit = System.Enum.GetName(typeof(Audit), electricalTask.Audit),
                            Audit = System.Enum.GetName(typeof(DispatcherAudit), electricalTask.DispatcherAudit)

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


        public ApiResult DispatcherAudit(ElectricalTask electricalTask)
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
                        ElectricalTask selectElectricalTask = db.ElectricalTask.FirstOrDefault(t => t.ID == electricalTask.ID);
                        if (selectElectricalTask == null)
                        {
                            throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new ElectricalTask()));
                        }
                        User loginUser = LoginHelper.CurrentUser(db);
                        Operation operation = db.Operation.FirstOrDefault(t => t.ID == selectElectricalTask.OperationID);
                        AH ah = db.AH.FirstOrDefault(t => t.ID == operation.AHID);
                        //if (electricalTask.Audit == Audit.通过)
                        if (electricalTask.DispatcherAudit == Enum.DispatcherAudit.通过)
                        {
                            //if (operation.VoltageType == VoltageType.低压)
                            //{

                            //}
                            //else
                            //{
                            //    //检查其他是否审核均通过，通过则发布停电任务并发送消息
                            //}

                            selectElectricalTask.AuditUserID = loginUser.ID;
                            //selectElectricalTask.Audit = Audit.通过;
                            selectElectricalTask.DispatcherAudit = Enum.DispatcherAudit.通过;
                            selectElectricalTask.AuditDate = now;
                            selectElectricalTask.AuditMessage = electricalTask.AuditMessage;
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
                            string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, "有新的" + ah.Name + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + System.Enum.GetName(typeof(ElectricalTaskType), selectElectricalTask.ElectricalTaskType) + "任务");
                            new LogDAO().AddLog(LogCode.审核成功, loginUser.Realname + "成功审核" + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + ClassUtil.GetEntityName(operation), db);
                        }
                        //else if (electricalTask.Audit == Enum.Audit.驳回)
                        else if (electricalTask.DispatcherAudit == Enum.DispatcherAudit.驳回)
                        {
                            selectElectricalTask.AuditUserID = loginUser.ID;

                            selectElectricalTask.DispatcherAudit = Enum.DispatcherAudit.驳回;
                            selectElectricalTask.AuditDate = now;
                            selectElectricalTask.AuditMessage = electricalTask.AuditMessage;

                            operation.OperationFlow = OperationFlow.作业终止;
                            operation.IsFinish = true;
                            operation.FinishDate = now;
                            db.SaveChanges();

                            //发消息给发起人
                            string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.MessageSecret);
                            string resultMessage = WeChatAPI.SendMessage(accessToken, db.User.FirstOrDefault(t => t.ID == operation.UserID).WeChatID, ParaUtil.MessageAgentid, "您的作业被驳回，原因为：" + electricalTask.AuditMessage);

                            if (selectElectricalTask.ElectricalTaskType == ElectricalTaskType.停电作业)
                            {
                                new SendDispatcherNoticeDAO().SendNotice(operation, ah, db);
                            }

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
    }
}
