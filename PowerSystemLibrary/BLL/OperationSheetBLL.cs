using Aspose.Words;
using Aspose.Words.Tables;
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
                            //operationSheet.Content,
                            OperationDate = operationSheet.OperationDate.ToString("yyyy-MM-dd HH:mm"),
                            db.User.FirstOrDefault(t => t.ID == operationSheet.OperationUserID).Realname,
                            OperationContentList = db.OperationContent.Where(o=>db.OperationSheet_Content.Where(t=>t.OperationSheetID == operationSheet.ID).Select(t=>t.OperationContentID).Contains(o.ID)).ToList()

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

                    operationSheet.OperationContentList = db.OperationContent.Where(o => db.OperationSheet_Content.Where(t => t.OperationSheetID == operationSheet.ID).Select(t => t.OperationContentID).Contains(o.ID)).ToList();

                    List<User> userList = db.User.ToList();

                    WorkSheet workSheet = db.WorkSheet.FirstOrDefault(t => t.OperationID == operation.ID);
                    AH ah = db.AH.FirstOrDefault(t => t.ID == operation.AHID);
                    ElectricalTask electricalTask = db.ElectricalTask.FirstOrDefault(t => t.ID == operationSheet.ElectricalTaskID);

                    User createUser = userList.FirstOrDefault(t => t.ID == operation.UserID);

                    Document doc = new Document();
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    builder.MoveToBookmark("Title");
                    builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                    builder.Font.Name = "黑体";
                    builder.Font.Size = 18;
                    builder.Write("停送电操作票");

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
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Size = 9;
                    builder.Font.Name = "黑体";
                    builder.Write("申请停电单位");

                    builder.CellFormat.Width = width;
                    builder.CellFormat.PreferredWidth = PreferredWidth.FromPoints(width);
                    builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;//垂直居中对齐
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;//水平居中对齐
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;


                    //第一行第二列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Name = "等线";
                    builder.Write(db.Department.FirstOrDefault(t => t.ID == createUser.DepartmentID).Name);
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;


                    //第一行第三列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Size = 9;
                    builder.Font.Name = "黑体";
                    builder.Write("工作票号");

                    //第一行第四列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Name = "等线";
                    builder.Write(workSheet.NO);
                    builder.EndRow();


                    //第二行第一列
                    builder.InsertCell();
                    builder.RowFormat.Height = 30;
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Name = "黑体";
                    builder.Write("申请人");
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                    //第二行第二列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Name = "等线";
                    builder.Write(createUser.Realname);
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                    //第二行第三列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Name = "黑体";
                    builder.Write("联系方式");
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;


                    //第二行第四列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Name = "等线";
                    builder.Write(createUser.Cellphone);
                    builder.EndRow();

                    //第三行第一列
                    builder.InsertCell();
                    builder.RowFormat.Height = 30;
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Name = "黑体";
                    builder.Write("操作任务");
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                    //第三行第二列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Name = "等线";
                    builder.Write(System.Enum.GetName(typeof(VoltageType), ah.VoltageType) + System.Enum.GetName(typeof(ElectricalTaskType), electricalTask.ElectricalTaskType));
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                    //第三行第三列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Name = "黑体";
                    builder.Write("停电或送电");
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;


                    //第三行第四列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Name = "等线";
                    builder.Write(electricalTask.ElectricalTaskType == ElectricalTaskType.停电作业 ? "停电" : "送电");
                    builder.EndRow();


                    //第四行第一列
                    builder.InsertCell();
                    builder.RowFormat.Height = 30;
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Name = "黑体";
                    builder.Write("操作人");
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                    //第四行第二列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Name = "等线";
                    builder.Write(userList.FirstOrDefault(t => t.ID == operationSheet.OperationUserID).Realname);
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                    //第四行第三列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Name = "黑体";
                    builder.Write("监护人");
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;


                    //第四行第四列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Name = "等线";
                    builder.Write(operationSheet.GuardianUserID != null ? userList.FirstOrDefault(t => t.ID == operationSheet.GuardianUserID).Realname : "");
                    builder.EndRow();

                    //第五行第一列
                    builder.InsertCell();
                    builder.RowFormat.Height = 30;
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Name = "黑体";
                    builder.Write("审票人签名");
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                    //第五行第二列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.First;
                    builder.Font.Name = "等线";
                    builder.Write(operationSheet.GuardianUserID != null ? userList.FirstOrDefault(t => t.ID == operationSheet.GuardianUserID).Realname : "");
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                    //第五行第三列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                    
                    //第五行第四列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                    builder.EndRow();

                    //第六行第一列
                    builder.InsertCell();
                    builder.RowFormat.Height = 30;
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Font.Name = "黑体";
                    builder.Write("顺序号");
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                    //第六行第二列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.First;

                    //第六行第三列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;

                    //第六行第四列
                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                    builder.EndRow();

                    int order = 1;
                    foreach (OperationContent operationContent in operationSheet.OperationContentList)
                    {
                        
                        builder.InsertCell();
                        builder.RowFormat.Height = 30;
                        builder.CellFormat.VerticalMerge = CellMerge.None;
                        builder.CellFormat.HorizontalMerge = CellMerge.None;
                        builder.Font.Name = "黑体";
                        builder.Write(order.ToString());
                        builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                        
                        builder.InsertCell();
                        builder.CellFormat.VerticalMerge = CellMerge.None;
                        builder.CellFormat.HorizontalMerge = CellMerge.First;
                        builder.Font.Name = "等线";
                        builder.Write(operationContent.Content);

                        
                        builder.InsertCell();
                        builder.CellFormat.VerticalMerge = CellMerge.None;
                        builder.CellFormat.HorizontalMerge = CellMerge.Previous;

                        
                        builder.InsertCell();
                        builder.CellFormat.VerticalMerge = CellMerge.None;
                        builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                        builder.EndRow();
                        order++;
                    }

                    builder.InsertCell();
                    builder.RowFormat.Height = 30;
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.First;
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;
                    builder.Font.Name = "黑体";
                    builder.Write("操作完毕时间:   "+ (operationSheet.FinishDate.HasValue ? operationSheet.FinishDate.Value.ToString("yyyy 年 MM 月 dd 日 HH 时 mm 分"):""));
                   


                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                    


                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;


                    builder.InsertCell();
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                    builder.EndRow();


                    string FilePath = "停送电操作票" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".doc";
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
