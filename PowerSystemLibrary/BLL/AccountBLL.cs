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
    public class AccountBLL
    {
        public ApiResult Login(User user)
        {
            string message = string.Empty;
            ApiResult result = new ApiResult();
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                    {
                        throw new ExceptionUtil("请填写用户名和密码");
                    }
                    string username = user.Username.Trim();

                    string md5Password = new BaseUtil().BuildPassword(username, user.Password.Trim());
                    //判断用户名错误
                    if (db.User.FirstOrDefault(t => t.Username == user.Username) == null)
                    {
                        new LogDAO().AddLog(LogCode.账号或密码错误, "登陆失败:" + user.Username, db);
                        throw new ExceptionUtil("登陆失败，请检查用户名");
                    }
                    User loginUser = db.User.FirstOrDefault(t => t.Username == user.Username && t.Password == md5Password);
                    //判断密码错误
                    if (loginUser == null)
                    {
                        new LogDAO().AddLog(LogCode.账号或密码错误, "登陆失败:" + user.Username, db);
                        throw new ExceptionUtil("登陆失败，请检查密码");
                    }
                    //判断是否被禁用或删除
                    if (loginUser.IsDelete == true)
                    {
                        new LogDAO().AddLog(LogCode.用户被禁用, "登陆失败:" + user.Username, db);
                        throw new ExceptionUtil("用户被删除，请联系系统管理员");
                    }

                    loginUser.Token = !string.IsNullOrEmpty(loginUser.Token) ? loginUser.Token : Guid.NewGuid().ToString();
                    loginUser.Expire = DateTime.Now.AddHours(24 * 7);
                    loginUser.LastLoginDate = DateTime.Now;
                    db.SaveChanges();
                    new LogDAO().AddLog(LogCode.登陆, "登陆:" + user.Username, db, loginUser);
                    result = ApiResult.NewSuccessJson(new
                    {
                        UserID = loginUser.ID,
                        loginUser.Username,
                        loginUser.Realname,
                        loginUser.Expire,
                        loginUser.Token
                    });
                }
                catch (Exception ex)
                {
                    message = ex.Message.ToString();
                }

                if (!string.IsNullOrEmpty(message))
                {
                    result = ApiResult.NewErrorJson(LogCode.登陆错误, message, db);
                }
            }
            return result;
        }

        public ApiResult ExtendToken(User user)
        {
            string message = string.Empty;
            ApiResult result = new ApiResult();
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    User loginUser = db.User.FirstOrDefault(t => t.Token == user.Token);
                    if (loginUser == null)
                    {
                        throw new ExceptionUtil("用户未找到");
                    }

                    if (loginUser.Expire <= DateTime.Now)
                    {
                        new LogDAO().AddLog(LogCode.用户登陆凭证过期, "登陆凭证过期:" + user.Username, db);
                        throw new ExceptionUtil("用户登陆超时，请重新登陆");
                    }

                    loginUser.Expire = loginUser.Expire == null ? DateTime.Now.AddHours(24 * 7) : loginUser.Expire.Value.AddHours(24 * 7);
                    db.SaveChanges();

                    result = ApiResult.NewSuccessJson("success");
                }
                catch (Exception ex)
                {
                    message = ex.Message.ToString();
                }

                if (!string.IsNullOrEmpty(message))
                {
                    result = ApiResult.NewErrorJson(LogCode.登陆错误, message, db);
                }
            }
            return result;
        }

    }
}
