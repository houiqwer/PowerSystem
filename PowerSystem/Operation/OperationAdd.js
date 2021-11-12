
$(function () {
    $('#workSheet').hide();
    InitWorkContent();
    InitAH();
    InitAudit();
    GetMonitorAuditUser();
    GetDeputyAuditUser();
    GetChiefAuditUser();
    GetLayui();
});

var VoltageType = 1;

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

    form.on('select(ah)', function (data) {
        GetAHType(data.value);

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

    if ($('#workContent').val() == null || $('#workContent').val() == "") {
        alert("请选择作业内容");
        $("#workContent").focus();
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
    //if ($('#workContent').val() == null || $('#workContent').val() == "") {
    //    alert("请输入作业内容");
    //    $("#workContent").focus();
    //    return;
    //}

    if ($('#auditUser').val() == null || $('#auditUser').val() == "") {
        alert("请选择审核人");
        $("#auditUser").focus();
        return;
    }

    if (VoltageType == 2) {
        if ($('#Influence').val() == null || $('#Influence').val() == "") {
            alert("请输入停电影响范围");
            $("#Influence").focus();
            return;
        }

        //if ($('#SafetyMeasures').val() == null || $('#SafetyMeasures').val() == "") {
        //    alert("请输入技术安全措施");
        //    $("#SafetyMeasures").focus();
        //    return;
        //}
        if ($('#MonitorAuditUser').val() == null || $('#MonitorAuditUser').val() == "") {
            alert("请选择部门班长审核人");
            $("#MonitorAuditUser").focus();
            return;
        }


        if ($('#DeputyAuditUser').val() == null || $('#DeputyAuditUser').val() == "") {
            alert("请选择部门副职审核人");
            $("#DeputyAuditUser").focus();
            return;
        }

        if ($('#ChiefAuditUser').val() == null || $('#ChiefAuditUser').val() == "") {
            alert("请选择部门正职审核人");
            $("#ChiefAuditUser").focus();
            return;
        }

        AddWorkSheet($('#ah').val(), $("#beginDate").val(), $("#endDate").val(), $('#workContent').val(), $('#auditUser').val(), $('#Influence').val(), $('#DeputyAuditUser').val(), $('#ChiefAuditUser').val(), $('#MonitorAuditUser').val());
        
    } else {
        Add($('#ah').val(), $("#beginDate").val(), $("#endDate").val(), $('#workContent').val(), $('#auditUser').val());
    }

    
}


function AddWorkSheet(ah, beginDate, endDate, workContent, auditUser, influence, deputyAuditUser, chiefAuditUser, monitorAuditUser) {
    var path = "/Operation/Add";
    var data = {
        "AHID": ah,
        "ApplicationSheet": {
            "BeginDate": beginDate,
            "EndDate": endDate,
            "WorkContentType": workContent,
            "AuditUserID": auditUser
        },
        "WorkSheet": {
            "Influence": influence,
            "MonitorAuditUserID": monitorAuditUser,
            "DeputyAuditUserID": deputyAuditUser,
            "ChiefAuditUserID": chiefAuditUser
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


function Add(ah, beginDate, endDate, workContent, auditUser) {
    
    var path = "/Operation/Add";
    var data = {
        "AHID": ah,
        "ApplicationSheet": {
            "BeginDate": beginDate,
            "EndDate": endDate,
            "WorkContentType": workContent,
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
        url: "/AH/List?limit=99",
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
                    if (i == 0 && data.data[i].VoltageType == 2) {
                        VoltageType = 2;
                        $('#Influence').val(data.data[i].Name);
                        $('#workSheet').show();
                    }
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

function GetAHType(id) {
    var data = {
        ID: id
    }
    $.ajax({
        url: "/AH/Get",
        type: "get",
        data: data,
        dataType: "json",
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function (data) {
            if (data.code == 0) {
                if (data.data.VoltageType == 2) {
                    VoltageType = 2;
                    $('#Influence').val(data.data.Name);
                    $('#workSheet').show();
                } else {
                    VoltageType = 1;
                    $('#workSheet').hide();
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

function GetDeputyAuditUser() {
    $.ajax({
        url: "/User/GetDeputyAuditUser",
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
                $("#DeputyAuditUser").html(html);
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

function GetChiefAuditUser() {
    $.ajax({
        url: "/User/GetChiefAuditUser",
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
                $("#ChiefAuditUser").html(html);
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

function GetMonitorAuditUser() {
    $.ajax({
        url: "/User/GetMonitorAuditUser",
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
                $("#MonitorAuditUser").html(html);
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

function InitWorkContent() {
    var data = {
        type: "WorkContentType"
    }
    $.ajax({
        url: "/Base/GetEnum",
        type: "get",
        data: data,
        dataType: "json",
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function (data) {
            if (data.code == 0) {
                var html = "";
                for (var i = 0; i < data.data.List.length; i++) {
                    html += "<option value=\"" + data.data.List[i].EnumValue + "\">" + data.data.List[i].EnumName + "</option>";
                }
                $("#workContent").html(html);
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



