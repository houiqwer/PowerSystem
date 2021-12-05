$(function () {
    InitVoltageType();
    InitPowerSubstation();
    Page();
    GetLayui();
})


layui.use('element', function () {
    var element = layui.element;
});

layui.use('form', function () {
    var form = layui.form;
})

var table = layui.table;

layui.use('table', function () {
    var table = layui.table;

    table.on('tool(table)', function (obj) {
        var data = obj.data;
        if (obj.event === 'edit') {
            Edit(data.ID);
        } else if (obj.event === 'delete') {
            Delete(data.ID);
        }
    });
});

function Add() {
    window.location.href = "AHEdit.html";
}

function Edit(id) {
    window.location.href = "AHEdit.html?id=" + id;
}

function Page() {
    var name = $("#name").val();
    var voltageType = $("#voltageType").val();
    var powerSubstation = $("#powerSubstation").val();

    layui.use('table', function () {
        var table = layui.table;
        var ssq = $(".search").val() == undefined ? 0 : $(".search").height();
        var hei = $('.safe-card1').height() - 51 - ssq;
        table.render({
            elem: '#table'
            , url: '/AH/List?name=' + name + '&voltageType=' + voltageType + '&powerSubstationID=' + powerSubstation

            , page: true
            , cellMinWidth: 80 //全局定义常规单元格的最小宽度，layui 2.2.1 新增
            , headers: { "Authorization": store.userInfo.token }
            , cols: [[
                { field: 'Name', align: 'center', title: '变电柜' }
                , { field: 'VoltageTypeName', align: 'center', title: '电压' }
                , { field: 'PowerSubstationName', align: 'center', title: '变电所' }
                , { field: 'AHStateName', align: 'center', title: '当前状态' }
                , { fixed: 'right', align: 'center', toolbar: '#bar', title: '操作', width: 160 }
            ]]
            , done: function (res, cur, count) {
                if (cur > 1 && res.data.length === 0) {
                    window.localStorage.setItem(window.location.pathname + "PageIndex", cur - 1);
                    Page();
                }
                else {
                    window.localStorage.setItem(window.location.pathname + "PageIndex", cur);
                }
            }
        });
        $(".layui-table-view").height(hei);
    });
}

$("#search").click(function () {
    localStorage.setItem(window.location.pathname + "PageIndex", 1);
    Page();
})

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
                var html = "<option>高压和低压</option>";
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

function InitPowerSubstation() {
    var data = {
        limit: 100
    }
    $.ajax({
        url: "/PowerSubstation/List",
        type: "get",
        data: data,
        dataType: "json",
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function (data) {
            if (data.code == 0) {
                var html = "<option>所有变电所</option>";
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


function Delete(id) {
    layer.confirm("确认删除？", { title: "系统提示信息" }, function (index) {
        var path = "/AH/Delete";
        var data = {
            'ID': id,
        };
        if (basepost(data, path)) {
            layer.alert("已删除", {
                time: 0, //不自动关闭
                btn: ['确定'],
                title: "系统提示信息",
                yes: function (index) {
                    layer.close(index);
                    Page();
                }
            });
        }
    });

}