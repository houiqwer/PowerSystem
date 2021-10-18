using PowerSystemLibrary.DAO;
using PowerSystemLibrary.DBContext;
using PowerSystemLibrary.Entity;
using PowerSystemLibrary.Enum;
using PowerSystemLibrary.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PowerSystemLibrary.BLL
{
    public class OperationSheetBLL
    {
        public ApiResult Get(int id)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;

            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    ElectricalTask selectElectricalTask = db.ElectricalTask.FirstOrDefault(t => t.ID == id);
                    if (selectElectricalTask == null)
                    {
                        throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new ElectricalTask()));
                    }
                    OperationSheet operationSheet = db.OperationSheet.FirstOrDefault(t => t.ElectricalTaskID == selectElectricalTask.ID);
                    if(operationSheet == null)
                    {
                        result = ApiResult.NewSuccessJson(null);
                    }
                    else
                    {
                        result = ApiResult.NewSuccessJson(new
                        {
                            operationSheet.ID,
                            operationSheet.Content,
                            OperationDate = operationSheet.OperationDate.ToString("yyyy-MM-dd HH:mm"),
                            db.User.FirstOrDefault(t => t.ID == operationSheet.OperationUserID).Realname

                        });
                    }
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

        public ApiResult List(int? ahID = null, ElectricalTaskType? electricalTaskType = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
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

                    IQueryable<OperationSheet> operationSheetIQueryable = db.OperationSheet.Where(t => (ahID == null || db.Operation.Where(m => m.AHID == ahID).Select(m => m.ID).Contains(t.OperationID)) &&
                    (electricalTaskType == null || db.ElectricalTask.Where(e => e.ElectricalTaskType == electricalTaskType).Select(e => e.ID).Contains(t.ElectricalTaskID))
                    && (t.CreateDate >= beginDate && t.CreateDate <= endDate));
                    int total = operationSheetIQueryable.Count();
                    List<OperationSheet> operationSheetList = operationSheetIQueryable.OrderByDescending(t => t.CreateDate).Skip((page - 1) * limit).Take(limit).ToList();

                    List<int> operationIDList = operationSheetList.Select(t => t.OperationID).Distinct().ToList();
                    List<Operation> operationList = db.Operation.Where(t => operationIDList.Contains(t.ID)).ToList();

                    List<int> ahIDList = operationList.Select(t => t.AHID).Distinct().ToList();
                    List<AH> ahList = db.AH.Where(t => ahIDList.Contains(t.ID)).ToList();

                    List<int> electricalTaskIDList = operationSheetList.Select(t => t.ElectricalTaskID).ToList();
                    List<ElectricalTask> electricalTaskList = db.ElectricalTask.Where(t => electricalTaskIDList.Contains(t.ID)).ToList();


                    List<object> returnList = new List<object>();

                    foreach (OperationSheet operationSheet in operationSheetList)
                    {
                        Operation operation = operationList.FirstOrDefault(t => t.ID == operationSheet.OperationID);
                        AH ah = ahList.FirstOrDefault(t => t.ID == operation.AHID);
                        ElectricalTask electricalTask = electricalTaskList.FirstOrDefault(t => t.ID == operationSheet.ElectricalTaskID);
                        returnList.Add(new
                        {
                            operationSheet.ID,
                            operationSheet.OperationID,
                            CreateDate = operationSheet.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                            ah.Name,
                            VoltageType = System.Enum.GetName(typeof(VoltageType), ah.VoltageType),
                            ElectricalTaskType = System.Enum.GetName(typeof(ElectricalTaskType), electricalTask.ElectricalTaskType)
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
                    OperationSheet operationSheet = db.OperationSheet.FirstOrDefault(t => t.ID == id);
                    if(operationSheet == null)
                    {
                        throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new OperationSheet()));
                    }
                    Operation operation = db.Operation.FirstOrDefault(t => t.ID == operationSheet.OperationID);
                    if(operation == null)
                    {
                        throw new ExceptionUtil("未找到" + ClassUtil.GetEntityName(new Operation()));
                    }

                    List<User> userList = db.User.ToList();

                    WorkSheet workSheet = db.WorkSheet.FirstOrDefault(t => t.OperationID == operation.ID);
                    AH ah = db.AH.FirstOrDefault(t => t.ID == operation.AHID);
                    ElectricalTask electricalTask = db.ElectricalTask.FirstOrDefault(t => t.ID == operationSheet.ElectricalTaskID);

                    User createUser = userList.FirstOrDefault(t => t.ID == operation.UserID);

                    string tempFile = ParaUtil.TempleteFileHtmlpath + "停送电操作票.docx";
                    string fielname = System.Web.HttpContext.Current.Server.MapPath(tempFile);
                    Aspose.Words.Document doc = new Aspose.Words.Document(fielname);
                    doc.Range.Replace("@no", workSheet.NO, false, false);
                    doc.Range.Replace("@department", db.Department.FirstOrDefault(t=>t.ID == createUser.DepartmentID).Name, false, false);
                    doc.Range.Replace("@createUser", createUser.Realname, false, false);
                    doc.Range.Replace("@cellphone", createUser.Cellphone, false, false);
                    doc.Range.Replace("@voltageType", System.Enum.GetName(typeof(VoltageType),ah.VoltageType)+ System.Enum.GetName(typeof(ElectricalTaskType), electricalTask.ElectricalTaskType), false, false);
                    doc.Range.Replace("@electricalTaskType", electricalTask.ElectricalTaskType == ElectricalTaskType.停电作业 ?"停电":"送电", false, false);
                    doc.Range.Replace("@operationUser", userList.FirstOrDefault(t=>t.ID == operationSheet.OperationUserID).Realname, false, false);
                    doc.Range.Replace("@guardianUser", operationSheet.GuardianUserID!=null? userList.FirstOrDefault(t => t.ID == operationSheet.GuardianUserID).Realname:"", false, false);
                    doc.Range.Replace("@content", operationSheet.Content, false, false);
                    if (operationSheet.FinishDate.HasValue)
                    {
                        doc.Range.Replace("@year", operationSheet.FinishDate.Value.Year.ToString(), false, false);
                        doc.Range.Replace("@mon", operationSheet.FinishDate.Value.Month.ToString(), false, false);
                        doc.Range.Replace("@day", operationSheet.FinishDate.Value.Day.ToString(), false, false);
                        doc.Range.Replace("@hour", operationSheet.FinishDate.Value.Hour.ToString(), false, false);
                        doc.Range.Replace("@minute", operationSheet.FinishDate.Value.Minute.ToString(), false, false);
                    }
                    else
                    {
                        doc.Range.Replace("@year", "", false, false);
                        doc.Range.Replace("@mon", "", false, false);
                        doc.Range.Replace("@day", "", false, false);
                        doc.Range.Replace("@hour", "", false, false);
                        doc.Range.Replace("@minute", "", false, false);
                    }
                    string fileName = "停送电操作票" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".doc";
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
                    new LogDAO().AddLog(LogCode.导出, "成功导出停送电操作票", db);
                    result = ApiResult.NewSuccessJson("成功导出停送电操作票");

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
