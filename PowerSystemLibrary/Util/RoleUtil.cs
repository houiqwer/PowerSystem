﻿using PowerSystemLibrary.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemLibrary.Util
{
    public class RoleUtil
    {
        //申请单审核人
        public static List<Role> GetApplicationSheetAuditRoleList()
        {
            List<Role> roleList = new List<Role>();
            roleList.Add(Role.部门副职);
            roleList.Add(Role.部门正职);
            roleList.Add(Role.分管领导);

            return roleList;
        }

        //电工
        public static List<Role> GetElectricianRoleList()
        {
            List<Role> roleList = new List<Role>();
            roleList.Add(Role.电工);

            return roleList;
        }

        //系统管理员
        public static List<Role> GetAdminRoleList()
        {
            List<Role> roleList = new List<Role>();
            roleList.Add(Role.系统管理员);

            return roleList;
        }
    }
}