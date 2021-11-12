$(function () {
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
            SetTypeInfo(data);
        } else if (obj.event === 'delete') {
            Delete(data.ID);
        }
    });
});




function Page() {


    layui.use('table', function () {
        var table = layui.table;
        var ssq = $(".search").val() == undefined ? 0 : $(".search").height();
        var hei = $('.safe-card1').height() - 51 - ssq;
        table.render({
            elem: '#table'
            , url: '/OperationContent/List?electricalTaskType=1'
            , page: true
            , cellMinWidth: 80 //全局定义常规单元格的最小宽度，layui 2.2.1 新增
            , headers: { "Authorization": store.userInfo.token }
            , cols: [[
                { field: 'Content', align: 'center', title: '内容' }

                , { fixed: 'right', align: 'center', toolbar: '#bar', title: '操作' }
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




function Delete(id) {
    layer.confirm("确认删除？", { title: "系统提示信息" }, function (index) {
        var path = "/OperationContent/Delete";
        var data = {
            'id': id,
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

/*弹出层*/
//layer引用自定义弹出窗口
function Add() {
    $('#hidUserId').val();// 点击添加，清除隐藏的ID
    $('#content').val('');
    layer.open({
        type: 1,
        shade: false,
        area: ['800px', '350px'],
        title: '添加',
        content: $('#editUserInfo'),//data, //变量date=div内容
        cancel: function (index) { layer.close(index) }
    });
}

function SetTypeInfo(data) {
    $('#content').val(data.Content);
    $('#hidId').val(data.ID);
    layer.open({
        type: 1,
        shade: false,
        area: ['800px', '350px'],
        title: '修改',
        content: $('#editUserInfo'),//data, //变量date=div内容
        cancel: function (index) { layer.close(index) }
    });
    GetLayui();
}


function SubTypeInfo() {
    if ($('#content').val() == "" || $('#content').val() == null) {
        layer.alert("请输入操作内容");
        $('#content').focus();
        return;
    }
    
    if ($('#hidId').val() == "" || $('#hidId').val() == null)
        AddType($('#content').val());
    else
        EditType($('#hidId').val(), $('#content').val());
}

function AddType(content) {
    $.ajax({
        url: "/OperationContent/Add",
        headers: { "Authorization": store.userInfo.token },
        type: "POST",
        data: {
            'Content': content,
            'ElectricalTaskType':1
        },
        dataType: "json",
        success: function (data) {
            if (data.code == 0) {
                layer.closeAll();
                layer.alert('添加成功！', { icon: 6, title: "系统提示信息" });
                Page();
            } else {
                layer.ready(function () {
                    title: false
                    layer.alert(data.data, {
                        title: false
                    });
                });
            }
        },
        error: function () {
            layer.ready(function () {
                title: false
                layer.alert("当前网络可能有错误", {
                    title: false
                });
            });
        }
    });
}
function EditType(id, content) {
    $.ajax({
        url: "/OperationContent/Edit",
        headers: { "Authorization": store.userInfo.token },
        type: "POST",
        data: {
            'ID': id,
            'Content': content
        },
        dataType: "json",
        success: function (data) {
            if (data.code == 0) {
                $('#hidId').val('');
                layer.closeAll();
                layer.alert('修改成功！', { icon: 6, title: "系统提示信息" });
                Page();
            } else {
                layer.ready(function () {
                    title: false
                    layer.alert(data.data, {
                        title: false
                    });
                });
            }
        },
        error: function () {
            layer.ready(function () {
                title: false
                layer.alert("当前网络可能有错误", {
                    title: false
                });
            });
        }
    });
    $('#hidId').val('');
}
function ClearInfo() {
    $('#hidId').val('');
    $('#content').val('');
    layer.close(layer.index);
    GetLayui();
}