using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemLibrary.Enum
{
    public enum LogCode
    {
        成功 = 0,
        登陆 = 1,
        添加 = 2,
        修改 = 3,
        删除 = 4,
        导入 = 5,
        导出 = 6,
        获取 = 7,
        下载 = 8,
        审核成功 = 9,
        审核驳回 = 10,
        接收任务 = 11,
        完成任务 = 12,
        挂牌 = 13,
        摘牌 = 14,
        退回任务=15,

        登陆错误 = 10001,
        添加错误 = 10002,
        修改错误 = 10003,
        删除错误 = 10004,
        导入错误 = 10005,
        导出错误 = 10006,
        获取错误 = 10007,
        下载错误 = 10008,
        审核错误 = 10009,
        接受任务错误 = 10011,
        完成任务错误 = 10012,
        挂牌错误 = 10013,
        摘牌错误 = 10014,

        //登陆
        账号或密码错误 = 20101,
        用户被禁用 = 20102,
        用户登陆凭证过期 = 20103,

        //权限
        无权限操作 = 30001,

        //其他
        系统日志 = 90000,
        系统错误 = 90001,
        系统测试 = 90002
    }
}
