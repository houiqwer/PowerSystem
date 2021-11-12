using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerSystemLibrary.DAO;
using PowerSystemLibrary.DBContext;
using PowerSystemLibrary.Entity;
using PowerSystemLibrary.Enum;
using PowerSystemLibrary.Util;

namespace PowerSystemLibrary.BLL
{
    public class UserBLL
    {

        //public ApiResult Add()
        //{
        //    ApiResult result = new ApiResult();
        //    string message = string.Empty;
        //    string ImagePath = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
        //    using (TransactionScope ts = new TransactionScope())
        //    {
        //        using (YJQYGHDDBContext db = new YJQYGHDDBContext())
        //        {
        //            try
        //            {
        //                SysUser currentUser = LoginHelper.CurrentUser;
        //                SysUser user = new SysUser();
        //                user.UserName = HttpContext.Current.Request["UserName"];//用户名
        //                user.RealName = HttpContext.Current.Request["RealName"];//姓名                                 
        //                user.Password = new BaseUtil().BuildPassword(user.UserName, "888888");//密码
        //                user.CellPhone = HttpContext.Current.Request["CellPhone"];//手机号
        //                user.DepartmentID = int.Parse(HttpContext.Current.Request["DepartmentID"]);
        //                user.Post = HttpContext.Current.Request["Post"];
        //                user.CellPhone = HttpContext.Current.Request["CellPhone"];//手机号
        //                if (db.SysUser.FirstOrDefault(t => t.UserName == user.UserName && t.IsDelete != true) != null)
        //                {
        //                    throw new ExceptionUtil("用户名重复");
        //                }
        //                if (db.SysUser.FirstOrDefault(t => t.CellPhone == user.CellPhone && t.IsDelete != true) != null)
        //                {
        //                    throw new ExceptionUtil("手机号重复");
        //                }
        //                user.CreateDate = DateTime.Now;//创建时间
        //                user.CreateUserID = currentUser.ID;//创建人
        //                user.IsDelete = false;//是否删除

        //                string Image = HttpContext.Current.Request["ProfilePicture"].ToString();//照片
        //                if (!string.IsNullOrEmpty(Image))
        //                {
        //                    Image = Image.Replace(Image.Split(',')[0] + ",", "");
        //                    new BaseUtil().Base64ToImage(Image, ParaUtil.ResourceImagePath + ImagePath);
        //                    ImagePath = ParaUtil.ResourceHtmlImagepath + ImagePath;
        //                    user.ProfilePicture = ImagePath;
        //                }
        //                if (!ClassUtil.Validate(user, ref message))
        //                {
        //                    throw new ExceptionUtil(message);
        //                }
        //                db.SysUser.Add(user);
        //                db.SaveChanges();
        //                string roles = HttpContext.Current.Request["Roles"].ToString();
        //                if (string.IsNullOrEmpty(roles))
        //                {
        //                    throw new ExceptionUtil("请选择用户角色");
        //                }
        //                string[] roleArray = roles.TrimEnd(',').Split(',');
        //                foreach (string role in roleArray)
        //                {
        //                    Role_User roleUser = new Role_User();
        //                    roleUser.RoleID = int.Parse(role);
        //                    roleUser.UserID = user.ID;

        //                    db.Role_User.Add(roleUser);
        //                }
        //                db.SaveChanges();
        //                new LogDAO().AddLog(LogCode.添加, "成功添加" + ClassUtil.GetEntityName(user) + ":" + user.UserName, db);
        //                result = ApiResult.NewSuccessJson("成功添加" + ClassUtil.GetEntityName(user));
        //                ts.Complete();
        //            }
        //            catch (Exception ex)
        //            {
        //                message = ex.Message;
        //            }
        //            finally
        //            {
        //                ts.Dispose();
        //            }

        //            if (!string.IsNullOrEmpty(message))
        //            {
        //                result = ApiResult.NewErrorJson(LogCode.添加错误, message, db);
        //            }

        //        }
        //    }
        //    return result;
        //}



        //public ApiResult Edit(User user)
        //{
        //    ApiResult result = new ApiResult();
        //    string message = string.Empty;
        //    using (TransactionScope ts = new TransactionScope())
        //    {
        //        using (PowerSystemDBContext db = new PowerSystemDBContext())
        //        {
        //            try
        //            {
        //                User selectedUser = db.User.FirstOrDefault(t => t.ID == user.ID);
        //                if (user == null)
        //                {
        //                    throw new ExceptionUtil("找不到原" + ClassUtil.GetEntityName(user));
        //                }
        //                user.UserName = HttpContext.Current.Request["Username"];//用户名
        //                user.RealName = HttpContext.Current.Request["RealName"];//姓名                                 
        //                user.CellPhone = HttpContext.Current.Request["CellPhone"];//手机号
        //                user.DepartmentID = int.Parse(HttpContext.Current.Request["DepartmentID"]);
        //                user.Post = HttpContext.Current.Request["Post"];
        //                if (db.SysUser.FirstOrDefault(t => t.UserName == user.UserName && t.ID != user.ID && t.IsDelete != true) != null)
        //                {
        //                    throw new ExceptionUtil("用户名重复!");
        //                }
        //                if (db.SysUser.FirstOrDefault(t => t.CellPhone == user.CellPhone && t.ID != user.ID && t.IsDelete != true) != null)
        //                {
        //                    throw new ExceptionUtil("手机号重复!");
        //                }

        //                int isChangeImage = Convert.ToInt32(HttpContext.Current.Request["isChangeImage"]);//判断图片修改的标志
        //                if (isChangeImage == 1)//修改了图片
        //                {
        //                    string Image = HttpContext.Current.Request["ProfilePicture"].ToString();//照片
        //                    if (!string.IsNullOrEmpty(Image))
        //                    {
        //                        Image = Image.Replace(Image.Split(',')[0] + ",", "");
        //                        new BaseUtil().Base64ToImage(Image, ParaUtil.ResourceImagePath + ImagePath);
        //                        ImagePath = ParaUtil.ResourceHtmlImagepath + ImagePath;

        //                        if (!string.IsNullOrEmpty(user.ProfilePicture))
        //                        {
        //                            File.Delete(HttpContext.Current.Server.MapPath(user.ProfilePicture));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        ImagePath = "";
        //                    }
        //                    user.ProfilePicture = ImagePath;
        //                }

        //                string roles = HttpContext.Current.Request["Roles"].ToString();
        //                if (string.IsNullOrEmpty(roles))
        //                {
        //                    throw new ExceptionUtil("请选择用户角色");
        //                }
        //                string[] roleArray = roles.TrimEnd(',').Split(',');
        //                db.Role_User.RemoveRange(db.Role_User.Where(t => t.UserID == user.ID).ToList());
        //                foreach (string role in roleArray)
        //                {
        //                    Role_User roleUser = new Role_User();
        //                    roleUser.RoleID = int.Parse(role);
        //                    roleUser.UserID = user.ID;
        //                    db.Role_User.Add(roleUser);
        //                }
        //                db.SaveChanges();
        //                new LogDAO().AddLog(LogCode.修改, "成功修改" + ClassUtil.GetEntityName(user) + ":" + user.UserName, db);
        //                result = ApiResult.NewSuccessJson("成功修改" + ClassUtil.GetEntityName(user));
        //                ts.Complete();
        //            }
        //            catch (Exception ex)
        //            {
        //                message = ex.Message;
        //            }
        //            finally
        //            {
        //                ts.Dispose();
        //            }

        //            if (!string.IsNullOrEmpty(message))
        //            {
        //                result = ApiResult.NewErrorJson(LogCode.添加错误, message, db);
        //            }
        //        }
        //    }
        //    return result;
        //}

        public ApiResult Add(User user)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using(TransactionScope ts = new TransactionScope())
            {
                using(PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    try
                    {
                       
                        if (!ClassUtil.Validate<User>(user, ref message))
                        {
                            throw new ExceptionUtil(message);
                        }
                        if(db.User.FirstOrDefault(t=>t.IsDelete!=true && t.Username == user.Username) != null)
                        {
                            throw new ExceptionUtil("用户名存在");
                        }
                        user.Password = new BaseUtil().BuildPassword(user.Username, "888888");
                        user.WeChatID = user.Cellphone;
                        Department department = db.Department.FirstOrDefault(t => t.IsDelete != true && t.ID == user.DepartmentID);
                        if(department == null)
                        {
                            throw new ExceptionUtil("请选择部门");
                        }
                        if (user.RoleIDList == null || user.RoleIDList.Count == 0)
                        {
                            throw new ExceptionUtil("请选择所属角色");
                        }

                        user.DepartmentID = department.ID;
                        db.User.Add(user);
                        db.SaveChanges();

                        foreach (int roleID in user.RoleIDList)
                        {
                            UserRole userRole = new UserRole();
                            userRole.Role = (Role)roleID;
                            userRole.UserID = user.ID;
                            db.UserRole.Add(userRole);
                        }
                        db.SaveChanges();

                        string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.UserSecret);
                        JObject resultJObject = WeChatAPI.BlindUser(accessToken, user);
                        if (Convert.ToInt32(resultJObject["errcode"]) != 0)
                        {
                            throw new ExceptionUtil("同步企业微信错误：" + resultJObject["errmsg"]);
                        }

                        new LogDAO().AddLog(LogCode.添加, "成功添加" + ClassUtil.GetEntityName(user) + ":" + user.Username, db);
                        result = ApiResult.NewSuccessJson("成功添加" + ClassUtil.GetEntityName(user) + ":" + user.Username);
                        ts.Complete();

                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                    }
                    finally
                    {
                        ts.Dispose();
                    }
                    if (!string.IsNullOrEmpty(message))
                    {
                        result = ApiResult.NewErrorJson(LogCode.添加错误, message, db);
                    }
                }
            }
            return result;
        }

        public ApiResult Edit(User user)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (TransactionScope ts = new TransactionScope())
            {
                using(PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    try
                    {
                        User oldUser = db.User.FirstOrDefault(t => t.IsDelete != true && t.ID == user.ID);
                        if(oldUser == null)
                        {
                            throw new ExceptionUtil("用户不存在");
                        }
                        if (!ClassUtil.Validate(user, ref message))
                        {
                            throw new ExceptionUtil(message);
                        }
                        if (db.User.FirstOrDefault(t => t.IsDelete != true && t.Username == user.Username && t.ID != user.ID) != null)
                        {
                            throw new ExceptionUtil("用户名存在");
                        }
                        Department department = db.Department.FirstOrDefault(t => t.IsDelete != true && t.ID == user.DepartmentID);
                        if (department == null)
                        {
                            throw new ExceptionUtil("请选择部门");
                        }
                        if (user.RoleIDList == null || user.RoleIDList.Count == 0)
                        {
                            throw new ExceptionUtil("请选择所属角色");
                        }

                        new ClassUtil().EditEntity(oldUser, user);
                        db.SaveChanges();

                        db.UserRole.RemoveRange(db.UserRole.Where(t => t.UserID == user.ID).ToList());
                        foreach (int roleID in user.RoleIDList)
                        {
                            UserRole userRole = new UserRole();
                            userRole.Role = (Role)roleID;
                            userRole.UserID = user.ID;
                            db.UserRole.Add(userRole);
                        }
                        db.SaveChanges();
                        new LogDAO().AddLog(LogCode.修改, "修改成功" + ClassUtil.GetEntityName(user) + ":" + user.Username, db);
                        result = ApiResult.NewSuccessJson("修改成功" + ClassUtil.GetEntityName(user) + ":" + user.Username);
                        ts.Complete();

                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                    }
                    finally
                    {
                        ts.Dispose();
                    }
                    if (!string.IsNullOrEmpty(message))
                    {
                        result = ApiResult.NewErrorJson(LogCode.修改错误, message, db);
                    }
                }
            }
            return result;
        }

        public ApiResult Delete(List<int> IDList)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    List<User> userList = db.User.Where(t => IDList.Contains(t.ID)).ToList();
                    if (userList == null)
                    {
                        throw new ExceptionUtil("请至少选择一条" + ClassUtil.GetEntityName(new User()));
                    }
                    userList.ForEach(t => t.IsDelete = true);
                    db.SaveChanges();

                    //企业微信也同步删除
                    string accessToken = WeChatAPI.GetToken(ParaUtil.CorpID, ParaUtil.UserSecret);
                    JObject resultJObject = WeChatAPI.DeleteUser(accessToken, userList);
                    if (Convert.ToInt32(resultJObject["errcode"]) != 0)
                    {
                        throw new ExceptionUtil("同步企业微信错误：" + resultJObject["errmsg"]);
                    }

                    new LogDAO().AddLog(LogCode.删除, "成功删除用户", db);
                    result = ApiResult.NewSuccessJson("成功删除用户");

                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                if (!string.IsNullOrEmpty(message))
                {
                    result = ApiResult.NewErrorJson(LogCode.删除错误, message, db);
                }
            }
            return result;
        }

        public ApiResult List(int? departmentID = null, string realname = null, Role? role = null, int page = 1, int limit = 10)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            realname = realname ?? string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    User loginUser = LoginHelper.CurrentUser(db);
                    IQueryable<User> userIQueryable = db.User.Where(t => t.IsDelete != true
                    && t.Realname.Contains(realname)
                    && (departmentID == null || t.DepartmentID == departmentID)
                    && (role == null || db.UserRole.Where(m => m.Role == role).Select(m => m.UserID).Contains(t.ID))
                    );
                    int total = userIQueryable.Count();
                    List<User> userList = userIQueryable.OrderBy(t => t.Realname).Skip((page - 1) * limit).Take(limit).ToList();
                    List<object> returnList = new List<object>();
                    List<int> userIDList = userList.Select(t => t.ID).ToList();
                    List<int> departmentIDList = userList.Select(t => t.DepartmentID).Distinct().ToList();

                    List<Department> departmentList = db.Department.Where(t => departmentIDList.Contains(t.ID)).ToList();
                    List<UserRole> userRoleList = db.UserRole.Where(t => userIDList.Contains(t.UserID)).ToList();
                    userRoleList.ForEach(t => t.RoleName = System.Enum.GetName(typeof(Role), t.Role));

                    foreach (User user in userList)
                    {
                        returnList.Add(new
                        {
                            user.ID,
                            user.Username,
                            user.Realname,
                            user.Cellphone,
                            user.DepartmentID,
                            UserRoleList = userRoleList.Where(t => t.UserID == user.ID).ToList(),
                            DepartmentName = departmentList.FirstOrDefault(t => t.ID == user.DepartmentID).Name,
                        });
                    }

                    result = ApiResult.NewSuccessJson(returnList, total);

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

        public ApiResult Get(int ID)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    User user = db.User.FirstOrDefault(t => t.IsDelete != true && t.ID == ID);
                    if (user == null)
                    {
                        throw new ExceptionUtil("未找到该数据");
                    }
                    List<UserRole> userRoleList = db.UserRole.Where(t => t.UserID == user.ID).ToList();
                    userRoleList.ForEach(t => t.RoleName = System.Enum.GetName(typeof(Role), t.Role));
                    result = ApiResult.NewSuccessJson(new
                    {
                        user.ID,
                        user.Username,
                        user.Realname,
                        user.Cellphone,
                        user.DepartmentID,
                        DepartmentName = db.Department.FirstOrDefault(t=>t.ID == user.DepartmentID).Name,
                        UserRoleList = userRoleList,
                        user.WeChatID,

                    });

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

        public ApiResult ResetPassword(int ID)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;

            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    User user = db.User.FirstOrDefault(t => t.ID == ID && t.IsDelete != true);
                    if (user == null)
                    {
                        throw new ExceptionUtil("找不到原" + ClassUtil.GetEntityName(new User()));
                    }
                    user.Password = new BaseUtil().BuildPassword(user.Username, "888888");
                    db.SaveChanges();
                    new LogDAO().AddLog(LogCode.修改, "成功重置" + ClassUtil.GetEntityName(user) + user.Username + "密码", db);
                    result = ApiResult.NewSuccessJson("成功重置" + ClassUtil.GetEntityName(user) + user.Username + "密码");
                }
                catch (Exception ex)
                {
                    message = ex.Message.ToString();

                }
                if (!string.IsNullOrEmpty(message))
                {
                    result = ApiResult.NewErrorJson(LogCode.修改错误, message, db);
                }
            }
            return result;
        }

        public ApiResult ChangePassword(User user)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;

            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    User loginUser = LoginHelper.CurrentUser(db);

                    if (string.IsNullOrEmpty(user.Password))
                    {
                        throw new ExceptionUtil("请输入原密码");
                    }

                    if (string.IsNullOrEmpty(user.NewPassword))
                    {
                        throw new ExceptionUtil("请输入新密码");
                    }

                    string md5Password = new BaseUtil().BuildPassword(loginUser.Username, user.Password);
                    if (md5Password != loginUser.Password)
                    {
                        throw new ExceptionUtil("原密码错误，请重新输入");
                    }

                    loginUser.Password = new BaseUtil().BuildPassword(loginUser.Username, user.NewPassword);

                    db.SaveChanges();
                    new LogDAO().AddLog(LogCode.修改, "成功修改" + ClassUtil.GetEntityName(user) + user.Username + "登陆密码", db);
                    result = ApiResult.NewSuccessJson("成功修改" + ClassUtil.GetEntityName(user) + "登陆密码");

                }
                catch (Exception ex)
                {
                    message = ex.Message.ToString();

                }
                if (!string.IsNullOrEmpty(message))
                {
                    result = ApiResult.NewErrorJson(LogCode.修改错误, message, db);
                }
            }

            return result;
        }

        //获取部门副职及以上
        public ApiResult GetUserListByRole()
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using(PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    User loginUser = LoginHelper.CurrentUser(db);
                    List<Role> roleList = RoleUtil.GetApplicationSheetAuditRoleList();
                    List<int> userIDList = db.UserRole.Where(m => roleList.Contains(m.Role)).Select(t=>t.UserID).Distinct().ToList();
                    List<User> userList = db.User.Where(t => t.IsDelete != true && userIDList.Contains(t.ID) && t.DepartmentID == loginUser.DepartmentID).ToList();
                    List<object> returnList = new List<object>();
                    foreach (User user in userList)
                    {
                        returnList.Add(new
                        {
                            user.ID,
                            user.Realname
                        });
                    }
                    result = ApiResult.NewSuccessJson(returnList);
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

        //获取部门副职
        public ApiResult GetDeputyAuditUser()
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    User loginUser = LoginHelper.CurrentUser(db);
                    List<Role> roleList = RoleUtil.GetWorkSheetDeputyAuditRoleList();
                    List<int> userIDList = db.UserRole.Where(m => roleList.Contains(m.Role)).Select(t => t.UserID).Distinct().ToList();
                    List<User> userList = db.User.Where(t => t.IsDelete != true && userIDList.Contains(t.ID) && t.DepartmentID == loginUser.DepartmentID).ToList();
                    List<object> returnList = new List<object>();
                    foreach (User user in userList)
                    {
                        returnList.Add(new
                        {
                            user.ID,
                            user.Realname
                        });
                    }
                    result = ApiResult.NewSuccessJson(returnList);
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

        //获取部门正职
        public ApiResult GetChiefAuditUser()
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    User loginUser = LoginHelper.CurrentUser(db);
                    List<Role> roleList = RoleUtil.GetWorkSheetChiefAuditRoleList();
                    List<int> userIDList = db.UserRole.Where(m => roleList.Contains(m.Role)).Select(t => t.UserID).Distinct().ToList();
                    List<User> userList = db.User.Where(t => t.IsDelete != true && userIDList.Contains(t.ID) && t.DepartmentID == loginUser.DepartmentID).ToList();
                    List<object> returnList = new List<object>();
                    foreach (User user in userList)
                    {
                        returnList.Add(new
                        {
                            user.ID,
                            user.Realname
                        });
                    }
                    result = ApiResult.NewSuccessJson(returnList);
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

        //获取部门班长
        public ApiResult GetMonitorAuditUser()
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    User loginUser = LoginHelper.CurrentUser(db);
                    List<Role> roleList = RoleUtil.GetMonitorRoleList();
                    List<int> userIDList = db.UserRole.Where(m => roleList.Contains(m.Role)).Select(t => t.UserID).Distinct().ToList();
                    List<User> userList = db.User.Where(t => t.IsDelete != true && userIDList.Contains(t.ID) && t.DepartmentID == loginUser.DepartmentID).ToList();
                    List<object> returnList = new List<object>();
                    foreach (User user in userList)
                    {
                        returnList.Add(new
                        {
                            user.ID,
                            user.Realname
                        });
                    }
                    result = ApiResult.NewSuccessJson(returnList);
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
