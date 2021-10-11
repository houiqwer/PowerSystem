﻿var id = unity.getURL('id');
var sign = unity.getURL('sign');

$(function () {
    NewExtendToken();
    $(".finish").hide();

    if (id != null && id != '') {
        if (sign != null && sign != "") {
            $("#myAudit").show();
            $(".sh").show();
            $("#TaskUser").hide();
        }
        else {
            $("#myAudit").hide();
            $(".sh").hide();
            $("#TaskUser").show();
        }
        Init(id);
    }
    GetLayui();
});
//JavaScript代码区域
layui.use('element', function () {
    var element = layui.element;

});

//表单启用
layui.use('form', function () {
    var form = layui.form; //只有执行了这一步，部分表单元素才会自动修饰成功
})

var applicationSheetID;
//初始化数据
function Init(id) {

    var data = {
        ID: id
    }
    $.ajax({
        url: "/Operation/Get",
        // headers: { Authorization: store.userInfo.token },
        type: "get",
        data: data,
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function (data) {
            // 成功获取数据
            if (data.code == 0) {
                $('#Realname').html(data.data.Realname);
                $("#AHName").html(data.data.AHName);
                $("#VoltageType").html(data.data.VoltageType);
                $("#OperationFlow").html(data.data.OperationFlow);

                $("#IsFinish").html(data.data.IsFinish ? "是" : "否");
                $("#IsConfirm").html(data.data.IsConfirm ? "已确认" : "未确认");
                applicationSheetID = data.data.ApplicationSheet.ID;

                $('#BeginDate').html(data.data.ApplicationSheet.BeginDate);
                $('#EndDate').html(data.data.ApplicationSheet.EndDate);
                $('#Aduit').html(data.data.ApplicationSheet.Audit);
                $('#CreateDate').html(data.data.CreateDate);
                $("#WorkContent").html(data.data.ApplicationSheet.WorkContent);
                $('#DepartmentName').html(data.data.ApplicationSheet.DepartmentName);


                //if (data.data.ApplicationSheet.Audit == "通过" && data.data.IsConfirm == false && data.data.Realname == window.localStorage.getItem("RealName")) {
                //    $(".finish").show();
                //}
                var item = "";
                item += "<tr ><th style='text-align:center'>审核人</th><th style='text-align:center'>审核状态</th><th style='text-align:center'>审核日期</th><th style='text-align:center'>审核说明</th></tr>";
                item += "<tr><td style='text-align:center'>" + data.data.ApplicationSheet.AuditUserName + "</td><td style='text-align:center'>" + data.data.ApplicationSheet.Audit + "</td><td style='text-align:center'>" + (data.data.ApplicationSheet.AuditDate == null ? "" : data.data.ApplicationSheet.AuditDate) + "</td><td style='text-align:center'>" + (data.data.ApplicationSheet.AuditMessage == null ? "" : data.data.ApplicationSheet.AuditMessage) + "</td></tr>";
                $("#tbody").append(item);


                var stop = "<tr ><th style='text-align:center'>停电电工</th><th style='text-align:center'>停电时间</th><th style='text-align:center'>是否确认</th></tr>";
                if (data.data.stopElectricalTaskList.length > 0) {
                    
                    for (var i = 0; i < data.data.stopElectricalTaskList[0].ElectricalTaskUserList.length; i++) {
                        stop += "<tr><td style='text-align:center'>" + data.data.stopElectricalTaskList[0].ElectricalTaskUserList[i].RealName + "</td><td style='text-align:center'>" + data.data.stopElectricalTaskList[0].ElectricalTaskUserList[i].CreateDate + "</td><td style='text-align:center'>" + (data.data.stopElectricalTaskList[0].ElectricalTaskUserList[i].IsConfirm ? '是' : '否') + "</td></tr>";
                    }
                }
                
                $("#stopElectricalTaskList").append(stop);

                var send = "<tr ><th style='text-align:center'>送电电工</th><th style='text-align:center'>送电时间</th><th style='text-align:center'>是否确认</th></tr>";
                if (data.data.sendElectricalTaskList.length > 0) {

                    for (var i = 0; i < data.data.sendElectricalTaskList[0].ElectricalTaskUserList.length; i++) {
                        
                        send += "<tr><td style='text-align:center'>" + data.data.sendElectricalTaskList[0].ElectricalTaskUserList[i].RealName + "</td><td style='text-align:center'>" + data.data.sendElectricalTaskList[0].ElectricalTaskUserList[i].CreateDate + "</td><td style='text-align:center'>" + (data.data.sendElectricalTaskList[0].ElectricalTaskUserList[i].IsConfirm ? '是' : '否') + "</td></tr>";
                    }
                }

                $("#sendElectricalTaskList").append(send);
            }
            else {
                Failure(data);
            }
        },
        error: function () {
            layer.ready(function () {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    })
}



function tj() {
    if ($("#AuditDesc").val() == null || $("#AuditDesc").val() == "") {
        layer.msg("请输入审批事由", { icon: 5 });
        return;
    }
    if (applicationSheetID != null && applicationSheetID != "") {
        Audit();
    }
}
function Audit() {

    var path = "/ApplicationSheet/Audit";
    var data = {
        ID: applicationSheetID,
        AuditMessage: $("#AuditDesc").val(),
        Audit: $("#AuditState").val(),

    };
    if (basepost(data, path)) {
        layer.alert('提交成功！', {
            time: 0, //不自动关闭
            icon: 6,
            btn: ['确定'],
            title: "系统提示信息",
            yes: function (index) {
                layer.close(index);
                window.location.href = '../ApplicationSheet/MyApplicationSheetAuditList.html';
            }
        });
    }
}

function cancle() {
    window.history.back();
}