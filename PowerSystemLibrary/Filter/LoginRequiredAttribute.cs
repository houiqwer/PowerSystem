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
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            bool hasAccess = false;
            string message = string.Empty;
            if (actionContext.Request.Headers.Authorization != null)
            {
                actionContext.Request.GetRouteData();
                string token = actionContext.Request.Headers.Authorization.Scheme;


                //用户验证逻辑
                using (PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    User user = db.User.FirstOrDefault(t => t.Token == token);

                    if (user != null)
                    {
                        //可判断用户是否有权限
                        string absolutePath = actionContext.Request.RequestUri.AbsolutePath;

                        // BaseUtil.GetFullRoute();

                        if (user.Expire > DateTime.Now)
                        {
                            hasAccess = true;
                            user.Expire = DateTime.Now.AddDays(7);
                            db.SaveChanges();
                        }
                        else
                        {
                            message = "登陆超时，请重新登陆";
                        }
                    }
                    else
                    {
                        message = "没有找到相关用户信息，请重新登录";
                    }
                }
            }
            else
            {
                message = "没有找到相关用户信息，请重新登录";
            }

            if (hasAccess == false)
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
