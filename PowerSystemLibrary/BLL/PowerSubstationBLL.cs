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
    public class PowerSubstationBLL
    {
        public ApiResult List()
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    List<PowerSubstation> powerBubstationList = db.PowerSubstation.Where(t => t.IsDelete != true).ToList();
                    result = ApiResult.NewSuccessJson(powerBubstationList);
                }
                catch (Exception ex)
                {
                    message = ex.Message;
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
