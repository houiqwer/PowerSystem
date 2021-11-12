var id = unity.getURL('id');
var sign = unity.getURL('sign');

$(function () {
    NewExtendToken();
    
    
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

var applicationSheetID;
//初始化数据
function Init(id) {

    var data = {
        ID: id
    }
    $.ajax({
        url: "/WorkSheet/Get",
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
                $('#DepartmentName').html(data.data.DepartmentName);
                $("#AHName").html(data.data.AHName);
                $("#VoltageType").html(data.data.VoltageType);
                $("#OperationFlow").html(data.data.OperationFlow);
                $('#AuditLevel').html(data.data.AuditLevel);

                $('#BeginDate').html(data.data.BeginDate);
                $('#EndDate').html(data.data.EndDate);
                $("#WorkContent").html(data.data.WorkContent);
                $("#Influence").html(data.data.Influence);
                $("#SafetyMeasures").html(data.data.SafetyMeasures);
                
                var item = "";
                item += "<tr ><th style='text-align:center'>审核人</th><th style='text-align:center'>审核状态</th><th style='text-align:center'>审核日期</th><th style='text-align:center'>审核说明</th></tr>";
                item += "<tr><td style='text-align:center'>" + data.data.MonitorAuditUserName + "</td><td style='text-align:center'>" + data.data.MonitorAudit + "</td><td style='text-align:center'>" + (data.data.MonitorAuditDate == null ? "" : data.data.MonitorAuditDate) + "</td><td style='text-align:center'>" + (data.data.MonitorAuditMessage == null ? "" : data.data.MonitorAuditMessage) + "</td></tr>";
                item += "<tr><td style='text-align:center'>" + data.data.DeputyAuditUserName + "</td><td style='text-align:center'>" + data.data.DeputyAudit + "</td><td style='text-align:center'>" + (data.data.DeputyAuditDate == null ? "" : data.data.DeputyAuditDate) + "</td><td style='text-align:center'>" + (data.data.DeputyAuditMessage == null ? "" : data.data.DeputyAuditMessage) + "</td></tr>";
                item += "<tr><td style='text-align:center'>" + data.data.ChiefAuditUserName + "</td><td style='text-align:center'>" + data.data.ChiefAudit + "</td><td style='text-align:center'>" + (data.data.ChiefAuditDate == null ? "" : data.data.ChiefAuditDate) + "</td><td style='text-align:center'>" + (data.data.ChiefAuditMessage == null ? "" : data.data.ChiefAuditMessage) + "</td></tr>";
                $("#tbody").append(item);

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
    if (sign != null && sign != "") {
        Audit();
    }
}
function Audit() {

    var path = "/WorkSheet/Audit";
    var data = {
        ID: id,
        AuditMessage: $("#AuditDesc").val(),
        AuditLevel: $("#AuditState").val(),

    };
    if (basepost(data, path)) {
        layer.alert('提交成功！', {
            time: 0, //不自动关闭
            icon: 6,
            btn: ['确定'],
            title: "系统提示信息",
            yes: function (index) {
                layer.close(index);
                window.location.href = 'MyWorkSheetAuditList.html';
            }
        });
    }
}

function cancle() {
    window.history.back();
}