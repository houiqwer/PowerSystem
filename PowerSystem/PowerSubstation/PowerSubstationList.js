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
            Edit(data.ID);
        } else if (obj.event === 'delete') {
            Delete(data.ID);
        }
    });
});

function Add() {
    window.location.href = "PowerSubstationEdit.html";
}

function Edit(id) {
    window.location.href = "PowerSubstationEdit.html?id=" + id;
}

function Page() {
    

    layui.use('table', function () {
        var table = layui.table;
        var ssq = $(".search").val() == undefined ? 0 : $(".search").height();
        var hei = $('.safe-card1').height() - 51 - ssq;
        table.render({
            elem: '#table'
            , url: '/PowerSubstation/List'
            , page: true
            , cellMinWidth: 80 //全局定义常规单元格的最小宽度，layui 2.2.1 新增
            , headers: { "Authorization": store.userInfo.token }
            , cols: [[
                { field: 'Name', align: 'center', title: '变电所' }
               
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
        var path = "/PowerSubstation/Delete";
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