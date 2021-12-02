var urlID = unity.getURL('id');


$(function () {
    InitVoltageType();
    InitPowerSubstation();

    if (urlID != null && urlID != '') {
        Init(urlID);
    }

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


//初始化数据
function Init(id) {
    var data = {
        ID: id
    }
    $.ajax({
        url: "/AH/Get",
        type: "get",
        data: data,
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function (data) {
            if (data.code == 0) {
                $("#name").val(data.data.Name);
                $("#voltageType").val(data.data.VoltageType);
                $("#powerSubstation").val(data.data.PowerSubstationID);
                $("#lampIP").val(data.data.LampIP);
                $("#ledIP").val(data.data.LedIP);
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

function Submit() {

    if ($("#name").val() == null || $("#name").val() == "") {
        alert("请输入变电柜名称");
        $("#name").focus();
        return;
    }

    if ($("#ledIP").val() == null || $("#ledIP").val() == "") {
        alert("请输入led牌IP");
        $("#ledIP").focus();
        return;
    }

    if ($("#lampIP").val() == null || $("#lampIP").val() == "") {
        alert("请输入灯IP");
        $("#lampIP").focus();
        return;
    }

    if (urlID == null || urlID == '') {
        AddAH($('#name').val(), $("#voltageType").val(), $("#powerSubstation").val(), $("#ledIP").val(), $("#lampIP").val());
    }
    else {
        EditAH(urlID, $('#name').val(), $("#voltageType").val(), $("#powerSubstation").val(), $("#ledIP").val(), $("#lampIP").val());
    }

}


function AddAH(Name, VoltageType, PowerSubstationID,ledIP,lampIP) {
    var path = "/AH/Add";
    var data = {
        'Name': Name,
        'VoltageType': VoltageType,
        'PowerSubstationID': PowerSubstationID,
        'LedIP': ledIP,
        'LampIP': lampIP
    };
    if (basepost(data, path)) {
        layer.alert('添加成功！', {
            time: 0, //不自动关闭
            btn: ['确定'],
            title: "系统提示信息",
            yes: function (index) {
                window.location.href = 'AHList.html';
            }
        });
    }
}

function EditAH(ID, Name, VoltageType, PowerSubstationID, ledIP, lampIP) {
    var path = "/AH/Edit";
    var data = {
        'ID': ID,
        'Name': Name,
        'VoltageType': VoltageType,
        'PowerSubstationID': PowerSubstationID,
        'LedIP': ledIP,
        'LampIP': lampIP
    };
    if (basepost(data, path)) {
        layer.alert('修改成功！', {
            time: 0, //不自动关闭
            btn: ['确定'],
            title: "系统提示信息",
            yes: function (index) {
                window.location.href = 'AHList.html';
            }
        });
    }
}


function Cancle() {
    window.location.href = 'AHList.html';
}

function InitPowerSubstation() {
    $.ajax({
        url: "/PowerSubstation/List?limit=999",
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
                $("#powerSubstation").html(html);
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

function InitVoltageType() {
    var data = {
        type: "VoltageType"
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
                $("#voltageType").html(html);
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

