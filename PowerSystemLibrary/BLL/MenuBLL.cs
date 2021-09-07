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
    public class MenuBLL
    {
        public ApiResult Add(Menu menu)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (TransactionScope ts = new TransactionScope())
            {
                using(PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    try
                    {
                        if (!ClassUtil.Validate<Menu>(menu, ref message))
                        {
                            throw new ExceptionUtil(message);
                        }
                        if (db.Menu.FirstOrDefault(t => t.Name == menu.Name && t.ParentID == menu.ParentID && t.IsDelete != true) != null)
                        {
                            throw new ExceptionUtil("菜单名称重复");
                        }

                        menu.IsDelete = false;
                        db.Menu.Add(menu);
                        db.SaveChanges();

                        new LogDAO().AddLog(LogCode.添加, "成功添加" + ClassUtil.GetEntityName(menu) + ":" + menu.Name, db);
                        result = ApiResult.NewSuccessJson("成功添加" + ClassUtil.GetEntityName(menu) + ":" + menu.Name);
                        ts.Complete();
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message.ToString();
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

        public ApiResult Edit(Menu menu)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (TransactionScope ts = new TransactionScope())
            {
                using (PowerSystemDBContext db = new PowerSystemDBContext())
                {
                    try
                    {

                        Menu oldMenu = db.Menu.FirstOrDefault(t => t.ID == menu.ID && t.IsDelete!=true);
                        if (oldMenu == null)
                        {
                            throw new ExceptionUtil("未找到原菜单");
                        }
                        if (!ClassUtil.Validate<Menu>(menu, ref message))
                        {
                            throw new ExceptionUtil(message);
                        }

                        if (db.Menu.FirstOrDefault(t => t.Name == menu.Name && t.ParentID == menu.ParentID && t.IsDelete != true && t.ID != menu.ID) != null)
                        {
                            throw new ExceptionUtil("菜单名称重复");
                        }

                        new ClassUtil().EditEntity(oldMenu, menu);
                        db.SaveChanges();

                        new LogDAO().AddLog(LogCode.修改, "成功修改" + ClassUtil.GetEntityName(menu) + ":" + menu.Name, db);
                        result = ApiResult.NewSuccessJson("成功修改" + ClassUtil.GetEntityName(menu) + ":" + menu.Name);
                        ts.Complete();
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message.ToString();

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

        public ApiResult Delete(int id)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {

                try
                {

                    Menu menu = db.Menu.FirstOrDefault(t => t.ID == id && t.IsDelete != true);
                    if (menu == null)
                    {
                        throw new ExceptionUtil("未找到原菜单");
                    }
                    menu.IsDelete = true;
                    db.SaveChanges();
                    new LogDAO().AddLog(LogCode.删除, "成功删除" + ClassUtil.GetEntityName(menu) + ":" + menu.Name, db);
                    result = ApiResult.NewSuccessJson("成功删除" + ClassUtil.GetEntityName(menu) + ":" + menu.Name);
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

        public ApiResult Get(int id)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {

                    Menu menu = db.Menu.FirstOrDefault(t => t.ID == id && t.IsDelete != true);

                    if (menu == null)
                    {
                        throw new ExceptionUtil("未找到该菜单");
                    }
                    result = ApiResult.NewSuccessJson(new
                    {
                        menu.ID,
                        menu.Name,
                        menu.URL,
                        menu.Order,
                        menu.ParentID,
                        menu.Icon,
                        ParentName = db.Menu.FirstOrDefault(t => t.ID == menu.ParentID) == null ? "" : db.Menu.FirstOrDefault(t => t.ID == menu.ParentID).Name,

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

        public ApiResult List(Role?role=null)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    User loginUser = LoginHelper.CurrentUser(db);
                    //菜单表
                    List<Menu> menuList = db.Menu.Where(t => t.IsDelete != true).ToList();
                    List<Role_Right> roleRightList = new List<Role_Right>();
                    List<Menu> parentMenuList = new List<Menu>();

                    parentMenuList = menuList.Where(t => (t.ParentID == null || t.ParentID == 0) && t.IsDelete !=true).OrderBy(t => t.Order).ToList();
                    if (role == null)
                    {
                        roleRightList = db.Role_Right.Where(t => db.UserRole.Where(r => r.UserID == loginUser.ID).Select(r => r.Role).Contains(t.Role)).ToList();

                    }
                    else
                    {
                        if (!System.Enum.IsDefined(typeof(Role), role))
                        {
                            throw new ExceptionUtil("角色不存在");
                        }
                        roleRightList = db.Role_Right.Where(t =>  t.Role == role).ToList();
                    }
                    List<object> returnList = new List<object>();
                    if (parentMenuList.Count != 0)
                    {
                        foreach (Menu menu in parentMenuList)
                        {

                            List<Menu> childrenList = menuList.Where(t => t.ParentID == menu.ID && t.IsDelete != true).OrderBy(t => t.Order).ToList();
                            List<object> olist = new List<object>();
                            foreach (Menu childMenu in childrenList)
                            {
                                List<object> childlist = new List<object>();
                                List<Menu> childMenuList = menuList.Where(t => t.ParentID == childMenu.ID).OrderBy(t => t.Order).ToList();
                                foreach (Menu childrenMenu in childMenuList)
                                {
                                    childlist.Add(new
                                    {
                                        ID = childrenMenu.ID,
                                        URL = childrenMenu.URL,
                                        ParentID = childrenMenu.ParentID,
                                        Order = childrenMenu.Order,
                                        Name = childrenMenu.Name,
                                        childrenMenu.Icon,
                                        CWhether = roleRightList.FirstOrDefault(t => t.MenuID == childrenMenu.ID) != null ? true : false
                                    });
                                }
                                olist.Add(new
                                {
                                    ID = childMenu.ID,
                                    URL = childMenu.URL,
                                    ParentID = childMenu.ParentID,
                                    Order = childMenu.Order,
                                    Name = childMenu.Name,
                                    childMenu.Icon,
                                    CWhether = roleRightList.FirstOrDefault(t => t.MenuID == childMenu.ID) != null ? true : false,
                                    childlist = childlist,
                                });
                            }

                            returnList.Add(new
                            {
                                PModule = menu,
                                PWhether = roleRightList.FirstOrDefault(t => t.MenuID == menu.ID) != null ? true : false,
                                Module = olist,

                            });
                        }
                        result = ApiResult.NewSuccessJson(returnList);
                    }
                    else
                    {
                        result = ApiResult.NewNoDataSuccessJson(returnList, "暂无菜单!");
                    }
                }
                catch (Exception exception)
                {
                    message = exception.Message;
                }

                if (!string.IsNullOrEmpty(message))
                {
                    result = ApiResult.NewErrorJson(LogCode.获取错误, message, db);
                }
            }
            return result;
        }

        public ApiResult RoleMenuAdd(Role role, List<int> menuIDList)
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using (PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    if (!System.Enum.IsDefined(typeof(Role), role))
                    {
                        throw new ExceptionUtil("角色不存在");
                    }
                    db.Role_Right.RemoveRange(db.Role_Right.Where(t => t.Role == role).ToList());
                    //遍历，添加数据
                    foreach (int menuID in menuIDList)
                    {
                        Role_Right roleRight = new Role_Right();
                        roleRight.Role = role;
                        roleRight.MenuID = menuID;
                        
                        db.Role_Right.Add(roleRight);
                    }
                    db.SaveChanges();
                    new LogDAO().AddLog(LogCode.添加, "成功添加角色菜单", db);
                    result = ApiResult.NewSuccessJson("成功添加角色菜单");
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                if (!string.IsNullOrEmpty(message))
                {
                    result = ApiResult.NewErrorJson(LogCode.添加错误, message, db);
                }
            }
            return result;
        }


        public ApiResult MRoleList()
        {
            ApiResult result = new ApiResult();
            string message = string.Empty;
            using(PowerSystemDBContext db = new PowerSystemDBContext())
            {
                try
                {
                    User loginUser = LoginHelper.CurrentUser(db);
                    List<UserRole> userRoleList = db.UserRole.Where(t => t.UserID == loginUser.ID).ToList();
                    List<Role> roleList = userRoleList.Select(t => t.Role).ToList();
                    List<int> menuIDList = db.Role_Right.Where(t => roleList.Contains(t.Role)).Select(t => t.MenuID).Distinct().ToList();
                    List<Menu> menuList = db.Menu.Where(t => t.IsDelete != true && menuIDList.Contains(t.ID)).ToList();
                    List<Menu> parentMenuList = menuList.Where(t => t.ParentID == 0 || t.ParentID == null).OrderBy(t=>t.Order).ToList();
                    List<object> returnList = new List<object>();
                    foreach (Menu menu in parentMenuList)
                    {
                        List<Menu> childMenuList = menuList.Where(t => t.ParentID == menu.ID).OrderBy(t=>t.Order).ToList();
                        List<object> childList = new List<object>();
                        foreach (Menu cMenu in childMenuList)
                        {
                            childList.Add(new
                            {
                                Module = cMenu,
                                ChildModule = menuList.Where(t => t.ParentID == cMenu.ID).ToList(),
                            });
                        }
                        returnList.Add(new
                        {
                            PModule = menu,
                            Module = childList,
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
