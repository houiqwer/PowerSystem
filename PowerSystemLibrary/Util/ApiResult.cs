using PowerSystemLibrary.DAO;
using PowerSystemLibrary.DBContext;
using PowerSystemLibrary.Entity;
using PowerSystemLibrary.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemLibrary.Util
{
    public class ApiResult
    {
        public ApiResultCodeType code { get; set; }
        public string msg { get; set; }
        public int count { get; set; }
        public int pageNumber { get; set; }
        public object data { get; set; }
        public object infor { get; set; }
        public ApiResult()
        {
            this.code = ApiResultCodeType.None;
            this.msg = "";
        }

       
        public static ApiResult NewErrorJson(LogCode logCode, string message, PowerSystemDBContext db, User user = null)
        {

            new LogDAO().AddLog(logCode, "发生错误:" + message, db);
            return new ApiResult()
            {
                code = ApiResultCodeType.Failure,
                msg = message,

            };
        }
        

        public static ApiResult NewSuccessJson(object Data, int count = 0, string msg = "")
        {
            return new ApiResult()
            {
                code = ApiResultCodeType.Success,
                msg = msg,
                count = count,
                pageNumber = count / 10 + 1,
                data = Data,
            };
        }

        public static ApiResult NewNoDataSuccessJson(object Data, string msg = "")
        {
            return new ApiResult()
            {
                code = ApiResultCodeType.Success,
                msg = msg,
                count = 0,
                pageNumber = 1,
                data = Data,

            };
        }
    }
}
