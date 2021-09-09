
$(function () {
    
    InitAH();
    InitAudit();
    GetLayui();
});


//JavaScript代码区域
layui.use('element', function () {
    var element = layui.element;

});

//表单启用
layui.use('form', function () {
    var form = layui.form;

    form.on('submit(submit)', function (data) {
        Submit();
    });
});

layui.use('laydate', function () {
    var laydate = layui.laydate;
    //日期范围
    laydate.render({
        elem: '#beginDate'
        ,type: 'datetime'
    });

    laydate.render({
        elem: '#endDate'
        , type: 'datetime'
    });
})




function Submit() {
    if ($('#ah').val() == null || $('#ah').val() == "") {
        alert("请选择停电设备");
        $("#ah").focus();
        return;
    }

    if ($("#beginDate").val() == null || $("#beginDate").val() == "") {
        alert("请输入开始时间");
        $("#beginDate").focus();
        return;
    }
    if ($("#endDate").val() == null || $("#endDate").val() == "") {
        alert("请输入结束时间");
        $("#endDate").focus();
        return;
    }
    if ($('#workContent').val() == null || $('#workContent').val() == "") {
        alert("请输入作业内容");
        $("#workContent").focus();
        return;
    }

    if ($('#auditUser').val() == null || $('#auditUser').val() == "") {
        alert("请选择审核人");
        $("#auditUser").focus();
        return;
    }

    Add($('#ah').val(), $("#beginDate").val(), $("#endDate").val(), $('#workContent').val(), $('#auditUser').val());
}


function Add(ah, beginDate, endDate, workContent, auditUser) {
    
    var path = "/Operation/Add";
    var data = {
        "AHID": ah,
        "ApplicationSheet": {
            "BeginDate": beginDate,
            "EndDate": endDate,
            "WorkContent": workContent,
            "AuditUserID": auditUser
        }
    }
    if (basepost(data, path)) {
        layer.alert('作业申请成功！', {
            time: 0, //不自动关闭
            btn: ['确定'],
            title: "系统提示信息",
            yes: function (index) {
                window.location.href = 'MyOperationList.html';
            }
        });
    }
}


function Cancle() {
    window.location.href = 'MyOperationList.html';
}

function InitAH() {
    $.ajax({
        url: "/AH/List?voltageType=1",
        type: "get",
        dataType: "json",
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function (data) {
            if (data.code == 0) {
                var html = "";
                for (var i = 0; i < data.data.length; i++) {
                    html += "<option value=\"" + data.data[i].ID + "\">" + data.data[i].Name + "</option>";
                }
                $("#ah").html(html);
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


function InitAudit() {
    $.ajax({
        url: "/User/GetUserListByRole",
        type: "get",
        dataType: "json",
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function (data) {
            if (data.code == 0) {
                var html = "";
                for (var i = 0; i < data.data.length; i++) {
                    html += "<option value=\"" + data.data[i].ID + "\">" + data.data[i].Realname + "</option>";
                }
                $("#auditUser").html(html);
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



