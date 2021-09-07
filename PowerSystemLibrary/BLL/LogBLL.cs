using PowerSystemLibrary.DBContext;
using PowerSystemLibrary.Entity;
using PowerSystemLibrary.Enum;
using PowerSystemLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemLibrary.BLL
{
    public class LogBLL
    {
        public ApiResult List(string realName, LogCode? logCode = null, DateTime? beginDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            realName = realName ?? "";
            using(PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    beginDate = beginDate ?? DateTime.MinValue;
                    endDate = endDate == null ? DateTime.MaxValue : endDate.Value.AddDays(1);
                    IQueryable<Log> logIQueryable = db.Log.Where(t => db.User.Where(u => u.Realname.Contains(realName)).Select(u => u.ID).Contains((int)t.UserID) && t.CreateDate >= beginDate && t.CreateDate < endDate && (logCode == null || t.LogCode == logCode));
                    int total = logIQueryable.Count();
                    List<Log> logList = logIQueryable.OrderByDescending(t => t.CreateDate).Skip((page - 1) * limit).Take(limit).ToList();
                    List<int?> userIDList = logList.Select(t => t.UserID).Distinct().ToList();
                    List<User> userList = db.User.Where(t => userIDList.Contains(t.ID)).ToList();
                    List<object> returnList = new List<object>();
                    foreach (Log log in logList)
                    {
                        returnList.Add(new
                        {
                            log.ID,
                            log.Content,
                            LogCode = System.Enum.GetName(typeof(LogCode), log.LogCode),
                            CreateDate = log.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                            CreateUser = userList.FirstOrDefault(t=>t.ID == log.UserID).Realname
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
                    result = ApiResult.NewErrorJson(LogCode.系统日志, message, db);
                }
            }
            return result;
        }
    }
}
