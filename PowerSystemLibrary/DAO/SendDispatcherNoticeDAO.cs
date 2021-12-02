using PowerSystemLibrary.DBContext;
using PowerSystemLibrary.Entity;
using PowerSystemLibrary.Enum;
using PowerSystemLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemLibrary.DAO
{
    public class SendDispatcherNoticeDAO
    {
        public void SendNotice(Operation selectedOperation, AH ah, PowerSystemDBContext db)
        {
            string message = string.Empty;
            DateTime now = DateTime.Now;
            try
            {
                //没用并行任务
                int surplusCount = db.Operation.Count(t => t.ID != selectedOperation.ID && t.AHID == selectedOperation.AHID && (t.IsPick != true && t.OperationFlow != OperationFlow.作业终止));
                
                string ledMessage = new ShowLed().ShowLedMethod(ah.LedIP, surplusCount == 0, surplusCount);
                if (ledMessage != string.Empty)
                {
                    throw new ExceptionUtil(ledMessage);
                }

                List<Operation> operationList = db.Operation.Where(t => !t.IsSendElectric && t.AHID == selectedOperation.AHID && (t.OperationFlow == OperationFlow.低压停电流程结束 || t.OperationFlow == OperationFlow.高压停电流程结束)).ToList();

                //Operation operation = db.Operation.Where(t => t.AHID == selectedOperation.AHID && (t.OperationFlow == OperationFlow.低压停电流程结束 || t.OperationFlow == OperationFlow.高压停电流程结束)).OrderByDescending(t => t.CreateDate).FirstOrDefault();
                if (surplusCount == 0 && operationList.Count > 0 && ah.AHState == AHState.停电)
                {
                    Operation operation = operationList.OrderByDescending(t => t.CreateDate).FirstOrDefault();
                    operationList.ForEach(t => t.IsSendElectric = true);
                    ElectricalTask electricalTask = new ElectricalTask();
                    electricalTask.OperationID = operation.ID;
                    electricalTask.AHID = operation.AHID;
                    electricalTask.CreateDate = now;
                    electricalTask.DispatcherAudit = DispatcherAudit.无需审核;
                    electricalTask.ElectricalTaskType = ElectricalTaskType.送电作业;
                    db.ElectricalTask.Add(electricalTask);
                    db.SaveChanges();

                    User createUser = db.User.FirstOrDefault(t => t.ID == operation.UserID);
                    string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.MessageSecret);
                    //发送消息给调度
                    List<Role> dispatcherRoleList = RoleUtil.GetDispatcherRoleList();
                    List<string> dispatcherWeChatIDList = db.User.Where(t => t.IsDelete != true && t.DepartmentID == createUser.DepartmentID && db.UserRole.Where(m => dispatcherRoleList.Contains(m.Role)).Select(m => m.UserID).Contains(t.ID)).Select(t => t.WeChatID).ToList();
                    string dispatcherWeChatIDString = "";
                    foreach (string userWeChatID in dispatcherWeChatIDList)
                    {
                        dispatcherWeChatIDString = dispatcherWeChatIDString + userWeChatID + "|";
                    }
                    dispatcherWeChatIDString.TrimEnd('|');
                    //string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.MessageSecret);
                    string dispatcherResultMessage = WeChatAPI.SendMessage(accessToken, dispatcherWeChatIDString, ParaUtil.MessageAgentid, ah.Name + System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + "送电任务开始");

                    //发给电工
                    List<Role> roleList = RoleUtil.GetElectricianRoleList();
                    List<string> userWeChatIDList = db.User.Where(t => t.IsDelete != true && t.DepartmentID == createUser.DepartmentID && db.UserRole.Where(m => roleList.Contains(m.Role)).Select(m => m.UserID).Contains(t.ID)).Select(t => t.WeChatID).ToList();
                    string userWeChatIDString = "";
                    foreach (string userWeChatID in userWeChatIDList)
                    {
                        userWeChatIDString = userWeChatIDString + userWeChatID + "|";
                    }
                    userWeChatIDString.TrimEnd('|');

                    string resultMessage = WeChatAPI.SendMessage(accessToken, userWeChatIDString, ParaUtil.MessageAgentid, ah.Name + "有新的送电任务待领取");


                    operation.OperationFlow = ah.VoltageType == VoltageType.低压 ? OperationFlow.低压送电任务领取 : OperationFlow.高压送电任务领取;
                    operation.IsFinish = false;
                    operation.FinishDate = null;
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                message = ex.Message.ToString();
            }
            if (!string.IsNullOrEmpty(message))
            {
                new LogDAO().AddLog(LogCode.系统日志, message, db);
            }

        }
    }
}
