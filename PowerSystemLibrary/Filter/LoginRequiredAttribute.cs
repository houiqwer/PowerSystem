using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PowerSystemLibrary.DBContext;
using PowerSystemLibrary.Entity;
using PowerSystemLibrary.Enum;
using PowerSystemLibrary.Util;

namespace PowerSystemLibrary.Filter
{
    public class LoginRequiredAttribute : System.Web.Http.AuthorizeAttribute
    {
        public string Role = string.Empty;
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string message = string.Empty;
            try
            {
                if (actionContext.Request.Headers.Authorization == null)
                {
                    throw new ExceptionUtil("没有找到相关用户信息，请重新登录");
                }

                actionContext.Request.GetRouteData();
                string token = actionContext.Request.Headers.Authorization.Scheme;

                //用户验证逻辑
                using (PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    User user = db.User.FirstOrDefault(t => t.Token == token);

                    if (user == null)
                    {
                        throw new ExceptionUtil("没有找到相关用户信息，请重新登录");
                    }

                    //可判断用户是否有权限
                    //string absolutePath = actionContext.Request.RequestUri.AbsolutePath;

                    if (!string.IsNullOrEmpty(Role))
                    {
                        List<Role> roleList = new List<Role>();
                        switch (Role)
                        {
                            case "ApplicationSheetAudit":
                                roleList = RoleUtil.GetApplicationSheetAuditRoleList();
                                break;
                            case "Dispatcher":
                                roleList = RoleUtil.GetDispatcherRoleList();
                                break;
                            case "WorkSheetAuditRoleList":
                                roleList = RoleUtil.GetWorkSheetAuditRoleList();
                                break;
                            default:
                                break;
                        }

                        UserRole userRole = db.UserRole.FirstOrDefault(m => roleList.Contains(m.Role) && m.UserID == user.ID);
                        if (userRole == null)
                        {
                            throw new ExceptionUtil("无权限使用该功能");
                        }
                    }

                    if (user.Expire < DateTime.Now)
                    {
                        throw new ExceptionUtil("登陆超时，请重新登陆");
                    }

                    user.Expire = DateTime.Now.AddDays(7);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (!string.IsNullOrEmpty(message))
            {
                ApiResult apiResult = new ApiResult
                {
                    code = ApiResultCodeType.Failure,
                    data = new { },
                    msg = message,
                };

                HttpResponseMessage response = actionContext.Response = actionContext.Response ?? new HttpResponseMessage();
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                //response.codeCode = System.Net.HttpcodeCode.Forbidden;
                response.Content = new StringContent(JsonConvert.SerializeObject(apiResult), Encoding.UTF8, "application/json");
                //HttpContext.Current.Response.Redirect("~/index.html");
            }
        }
    }
}
