using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemLibrary.Enum
{
    public enum SheetType
    {
        [Description("A")]
        申请单 = 1,
        [Description("W")]
        工作票 = 2,
        [Description("O")]
        操作票 = 3
    }

    public enum Audit
    {
        待审核 = 1,
        //审核中 = 2,
        通过 = 3,
        驳回 = 4,
        //这个不要页面列举
        //无需审核 = 5,
        //撤回
        //撤回 = 6
    }

    public enum ElectricalTaskType
    {
        停电作业 = 1,
        送电作业 = 2,
        摘牌作业 = 3,
    }

    public enum OperationFlow
    {
        //无作业 = 0,
        //低压
        低压停电作业申请 = 101,
        低压停电作业审核 = 102,
        低压停电任务领取 = 103,
        低压停电任务操作 = 104,
        低压停电任务完成 = 105,
        低压挂停电牌作业 = 106,
        低压检修作业完成 = 107,

        低压摘牌任务领取 = 108,
        低压摘牌任务操作 = 109,
        低压摘牌任务完成 = 110,
        低压停电流程结束 = 111,


        //低压送电任务领取 = 108,
        //低压送电任务操作 = 109,
        //低压送电任务完成 = 110,
        //低压停电流程结束 = 111,

        低压送电任务领取 = 112,
        低压送电任务操作 = 113,
        低压送电任务完成 = 114,
        

        高压停电作业申请 = 201,
        高压停电作业审核 = 202,
        高压停电任务领取 = 203,
        高压停电任务操作 = 204,
        高压停电任务完成 = 205,
        高压挂停电牌作业 = 206,
        高压检修作业完成 = 207,

        高压摘牌任务领取 = 208,
        高压摘牌任务操作 = 209,
        高压摘牌任务完成 = 210,
        高压停电流程结束 = 211,

        高压送电任务领取 = 212,
        高压送电任务操作 = 213,
        高压送电任务完成 = 214,

        //高压送电任务领取 = 208,
        //高压送电任务操作 = 209,
        //高压送电任务完成 = 210,
        //高压停电流程结束 = 211,

        作业终止 = 999,

    }

    public enum AuditLevel
    {
        班长审核=0,
        副职审核=1,
        正职审核=2,
        通过=3,
        驳回=4
    }

    public enum OperationAudit
    {
        待审核=1,
        审核中=2,
        通过=3,
        驳回=4
    }

    public enum WorkContentType
    {
        检修=1,
        保养=2,
        保洁=3,
        其他=99
    }


    public enum DispatcherAudit
    {
        待审核=1,
        无需审核 = 2,
        通过=3,
        驳回=4
    }
}
