var id = unity.getURL('id');
var ElectricalTaskID = unity.getURL('ElectricalTaskID');

var sign = unity.getURL('sign');

$(function () {
    NewExtendToken();
    $(".finish").hide();
    $("#stopTaskUser").hide();
    $("#sendTaskUser").hide();
    $('#stopDispatcherAudit').hide();
    //$('#sendDispatcherAudit').hide();
    $('#workSheetAuditInfo').hide();
    if (id != null && id != '') {
        if (sign != null && sign != "") {
            $("#myAudit").show();
            $(".sh").show();
           
        }
        else {
            $("#myAudit").hide();
            $(".sh").hide();
            
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
               

                $('#BeginDate').html(data.data.ApplicationSheet.BeginDate);
                $('#EndDate').html(data.data.ApplicationSheet.EndDate);
                $('#Aduit').html(data.data.ApplicationSheet.Audit);
                $('#CreateDate').html(data.data.CreateDate);
                $("#WorkContent").html(data.data.ApplicationSheet.WorkContent);
                $('#DepartmentName').html(data.data.ApplicationSheet.DepartmentName);


                
                var item = "";
                item += "<tr ><th style='text-align:center'>审核人</th><th style='text-align:center'>审核状态</th><th style='text-align:center'>审核日期</th><th style='text-align:center'>审核说明</th></tr>";
                item += "<tr><td style='text-align:center'>" + data.data.ApplicationSheet.AuditUserName + "</td><td style='text-align:center'>" + data.data.ApplicationSheet.Audit + "</td><td style='text-align:center'>" + (data.data.ApplicationSheet.AuditDate == null ? "" : data.data.ApplicationSheet.AuditDate) + "</td><td style='text-align:center'>" + (data.data.ApplicationSheet.AuditMessage == null ? "" : data.data.ApplicationSheet.AuditMessage) + "</td></tr>";
                $("#tbody").append(item);

                if (data.data.VoltageType == '高压' && data.data.workSheet != null) {
                    $('#workSheetAuditInfo').show();
                    var workAudit = "";
                    workAudit += "<tr ><th style='text-align:center'>审核人</th><th style='text-align:center'>审核状态</th><th style='text-align:center'>审核日期</th><th style='text-align:center'>审核说明</th></tr>";
                    workAudit += "<tr><td style='text-align:center'>" + data.data.workSheet.MonitorAuditUserName + "</td><td style='text-align:center'>" + data.data.workSheet.MonitorAuditName + "</td><td style='text-align:center'>" + (data.data.workSheet.MonitorAuditDate == null ? "" : data.data.workSheet.MonitorAuditDateString) + "</td><td style='text-align:center'>" + (data.data.workSheet.MonitorAuditMessage == null ? "" : data.data.workSheet.MonitorAuditMessage) + "</td></tr>";
                    workAudit += "<tr><td style='text-align:center'>" + data.data.workSheet.DeputyAuditUserName + "</td><td style='text-align:center'>" + data.data.workSheet.DeputyAuditName + "</td><td style='text-align:center'>" + (data.data.workSheet.DeputyAuditDate == null ? "" : data.data.workSheet.DeputyAuditDateString) + "</td><td style='text-align:center'>" + (data.data.workSheet.DeputyAuditMessage == null ? "" : data.data.workSheet.DeputyAuditMessage) + "</td></tr>";
                    workAudit += "<tr><td style='text-align:center'>" + data.data.workSheet.ChiefAuditUserName + "</td><td style='text-align:center'>" + data.data.workSheet.ChiefAuditName + "</td><td style='text-align:center'>" + (data.data.workSheet.ChiefAuditDate == null ? "" : data.data.workSheet.ChiefAuditDateString) + "</td><td style='text-align:center'>" + (data.data.workSheet.ChiefAuditMessage == null ? "" : data.data.workSheet.ChiefAuditMessage) + "</td></tr>";
                    $("#workSheetAuditList").append(workAudit);
                }

                //停电
                if (data.data.stopElectricalTask != null && data.data.stopElectricalTask.DispatcherAudit != 1) {
                    var stopDispatcherList = "<tr ><th style='text-align:center'>作业类型</th><th style='text-align:center'>审核人</th><th style='text-align:center'>审核状态</th><th style='text-align:center'>审核日期</th><th style='text-align:center'>审核说明</th></tr>";
                    $('#stopDispatcherAudit').show();
                    stopDispatcherList += "<tr><td style='text-align:center'>" + data.data.stopElectricalTask.ElectricalTaskTypeName + "</td><td style='text-align:center'>" + data.data.stopElectricalTask.RealName + "</td><td style='text-align:center'>" + data.data.stopElectricalTask.AuditName + "</td><td style='text-align:center'>" + (data.data.stopElectricalTask.AuditDateString) + "</td><td style='text-align:center'>" + (data.data.stopElectricalTask.AuditMessage == null ? "" : data.data.stopElectricalTask.AuditMessage) + "</td></tr>";
                    $("#stopDispatcherList").append(stopDispatcherList);


                    $('#stopTaskUser').show();
                    var stop = "<tr ><th style='text-align:center'>停电电工</th><th style='text-align:center'>停电时间</th><th style='text-align:center'>是否确认</th></tr>";
                    for (var i = 0; i < data.data.stopElectricalTask.ElectricalTaskUserList.length; i++) {
                        stop += "<tr><td style='text-align:center'>" + data.data.stopElectricalTask.ElectricalTaskUserList[i].RealName + "</td><td style='text-align:center'>" + data.data.stopElectricalTask.ElectricalTaskUserList[i].CreateDate + "</td><td style='text-align:center'>" + (data.data.stopElectricalTask.ElectricalTaskUserList[i].IsConfirm ? '是' : '否') + "</td></tr>";
                    }

                    $("#stopElectricalTaskList").append(stop);
                }


                //送电
                if (data.data.sendElectricalTask != null) {
                    //$('#sendDispatcherAudit').show(); 
                    //var sendDispatcherList = "<tr ><th style='text-align:center'>作业类型</th><th style='text-align:center'>审核人</th><th style='text-align:center'>审核状态</th><th style='text-align:center'>审核日期</th><th style='text-align:center'>审核说明</th></tr>";
                    //sendDispatcherList += "<tr><td style='text-align:center'>" + data.data.sendElectricalTask.ElectricalTaskTypeName + "</td><td style='text-align:center'>" + data.data.sendElectricalTask.RealName + "</td><td style='text-align:center'>" + data.data.sendElectricalTask.AuditName + "</td><td style='text-align:center'>" + (data.data.sendElectricalTask.AuditDateString) + "</td><td style='text-align:center'>" + (data.data.sendElectricalTask.AuditMessage == null ? "" : data.data.sendElectricalTask.AuditMessage) + "</td></tr>";
                    //$("#sendDispatcherList").append(sendDispatcherList);

                    $('#sendTaskUser').show();
                    var send = "<tr ><th style='text-align:center'>送电电工</th><th style='text-align:center'>送电时间</th><th style='text-align:center'>是否确认</th></tr>";
                    for (var i = 0; i < data.data.sendElectricalTask.ElectricalTaskUserList.length; i++) {

                        send += "<tr><td style='text-align:center'>" + data.data.sendElectricalTask.ElectricalTaskUserList[i].RealName + "</td><td style='text-align:center'>" + data.data.sendElectricalTask.ElectricalTaskUserList[i].CreateDate + "</td><td style='text-align:center'>" + (data.data.sendElectricalTask.ElectricalTaskUserList[i].IsConfirm ? '是' : '否') + "</td></tr>";
                    }
                    $("#sendElectricalTaskList").append(send);
                }
               
               
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
    if (ElectricalTaskID != null && ElectricalTaskID != "") {
        Audit();
    }
}
function Audit() {

    var path = "/ElectricalTask/DispatcherAudit";
    var data = {
        ID: ElectricalTaskID,
        AuditMessage: $("#AuditDesc").val(),
        DispatcherAudit: $("#AuditState").val(),

    };
    if (basepost(data, path)) {
        layer.alert('提交成功！', {
            time: 0, //不自动关闭
            icon: 6,
            btn: ['确定'],
            title: "系统提示信息",
            yes: function (index) {
                layer.close(index);
                window.location.href = 'MyDispatcherAuditList.html';
            }
        });
    }
}

function cancle() {
    window.history.back();
}