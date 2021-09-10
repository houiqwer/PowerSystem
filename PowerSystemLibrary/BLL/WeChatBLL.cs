using PowerSystemLibrary.DAO;
using PowerSystemLibrary.DBContext;
using PowerSystemLibrary.Entity;
using PowerSystemLibrary.Enum;
using PowerSystemLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PowerSystemLibrary.BLL
{
    public class WeChatBLL
    {
        public ApiResult OAuth(string code)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.PowerSecret);
                    string userId = WeChatAPI.GetUserInfo(accessToken, code);
                    string UserJson = WeChatAPI.GetUser(accessToken, userId);
                    if (RequestHelper.Json(UserJson, "errcode") == "0")
                    {
                        string weChatID = RequestHelper.Json(UserJson, "userid");
                        if (!string.IsNullOrEmpty(weChatID))
                        {
                            User loginUser = db.User.FirstOrDefault(t => t.WeChatID == weChatID);
                            if (loginUser != null)
                            {
                                loginUser.Token = !string.IsNullOrEmpty(loginUser.Token) ? loginUser.Token : Guid.NewGuid().ToString();
                                loginUser.Expire = DateTime.Now.AddHours(24 * 7);
                                loginUser.LastLoginDate = DateTime.Now;
                                db.SaveChanges();
                                new LogDAO().AddLog(LogCode.登陆, "微信登陆:" + loginUser.Username, db, loginUser);
                                result = ApiResult.NewSuccessJson(new
                                {
                                    UserID = loginUser.ID,
                                    loginUser.Username,
                                    loginUser.Realname,
                                    loginUser.Expire,
                                    loginUser.Token,
                                    RoleList = db.UserRole.Where(t => t.UserID == loginUser.ID).Select(t => t.Role).ToList()
                                });


                            }
                            else
                            {
                                throw new ExceptionUtil("用户已删除或未添加");
                            }
                        }
                        else
                        {
                            throw new ExceptionUtil("手机未绑定");
                        }
                    }
                    else
                    {
                        throw new ExceptionUtil("微信授权失败");
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message.ToString();
                }

                if (!string.IsNullOrEmpty(message))
                {
                    result = ApiResult.NewErrorJson(LogCode.系统错误, message, db);
                }
            }
            return result;
        }
    }
}
