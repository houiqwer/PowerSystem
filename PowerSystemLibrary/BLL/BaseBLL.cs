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
    public class BaseBLL
    {
        public ApiResult GetEnum(string type)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            List<EnumEntity> enumList = new List<EnumEntity>();
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    switch (type)
                    {
                        case "LogCode":
                            enumList = BaseUtil.EnumToList<LogCode>();
                            break;
                        case "Role":
                            enumList = BaseUtil.EnumToList<Role>();
                            break;
                        case "ElectricalTaskType":
                            enumList = BaseUtil.EnumToList<ElectricalTaskType>();
                            break;
                        case "VoltageType":
                            enumList = BaseUtil.EnumToList<VoltageType>();
                            break;
                        case "WorkContentType":
                            enumList = BaseUtil.EnumToList<WorkContentType>();
                            break;
                        default:
                            message = "不存在该枚举数据";
                            break;
                    }
                    result = ApiResult.NewSuccessJson(new
                    {
                        List = enumList,
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
                return result;
            }
        }

        public ApiResult RoleList()
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            result = ApiResult.NewSuccessJson(BaseUtil.EnumToList<Role>(), System.Enum.GetValues(typeof(Role)).Length);
            return result;
        }
    }
}
