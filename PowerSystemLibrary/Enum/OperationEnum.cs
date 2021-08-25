using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemLibrary.Enum
{
    public enum VoltageType
    {
        低压 = 1,
        高压 = 2
    }

    public enum Audit
    {
        待审核 = 1,
        审核中 = 2,
        通过 = 3,
        驳回 = 4,
        //这个不要页面列举
        无需审核 = 5,
        //撤回
        撤回 = 6
    }

    //public enum OperationType
    //{
    //    停电 = 1,
    //    送电 = 2
    //}

    public enum OperationFlow
    {
        //低压
        低压停电作业申请 = 101,
        低压停电作业审核 = 102,
        低压停电任务领取 = 103,
        低压停电任务操作 = 104,
        低压停电任务完成 = 105,
        低压挂停电牌作业 = 106,
        低压检修作业完成 = 107,
        低压送电任务领取 = 108,
        低压送电任务操作 = 104,
        低压送电任务完成 = 105,
    }
}
