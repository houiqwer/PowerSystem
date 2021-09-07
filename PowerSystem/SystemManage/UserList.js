//JavaScript代码区域
layui.use('element', function () {
    var element = layui.element;
});
//表单启用
layui.use('form', function () {
    var form = layui.form; //只有执行了这一步，部分表单元素才会自动修饰成功
})
//日期

$(function () {
    NewExtendToken();
    list();
    $("#RealName").val(window.localStorage.getItem("SelRealName"));
    dopage($("#RealName").val(), $("#depIDs").val());
})

$("#cx").click(function () {
    window.localStorage.setItem("SelRealName", $("#RealName").val());

    dopage($("#RealName").val(), $("#depIDs").val());
})
function dopage(RealName, DepID) {

    layui.use('table', function () {
        var table = layui.table;
        var seahei = $('.search').height();
        var headhei = $('.layui-card-header').height() == undefined ? 0 : $('.layui-card-header').height();
        var hei = $('.safe-card1').height() - seahei - headhei;
        //$.ajaxSetup({
        //    headers: {
        //        "Authorization": store.userInfo.token
        //    }
        //});
        table.render({
            elem: '#demo'
            , url: '/User/List?realname=' + RealName + '&departmentID=' + DepID
            , page: true
            , cellMinWidth: 80 //全局定义常规单元格的最小宽度，layui 2.2.1 新增
            , headers: { "Authorization": store.userInfo.token }
            , cols: [[
                //{ type: 'checkbox' },
                { field: 'Username', align: 'center', title: '用户名' }
                , { field: 'Realname', align: 'center', title: '姓名', sort: true }

                , { field: 'DepartmentName', align: 'center', title: '部门' }
                , { field: 'Cellphone', align: 'center', title: '手机号' }

                //, { field: 'LastDate', align: 'center', title: '最近登录时间' }
                //, { field: 'FbiddenName', align: 'center', title: '启用状态' }
                //   , { field: 'UserStateName', align: 'center', title: '状态' }
                , { fixed: 'right', width: '20%', align: 'center', toolbar: '#barDemo', title: '操作' }
            ]],
            done: function (res, cur, count) {
                //var par = $(".layui-inline .layui-table-main table tr");
                //for (var i = 0; i < res.data.length; i++) {
                //    var child = $(par[i]).children().eq(7).children();
                //    if (res.data[i].UserState == 1) {
                //        child.css({ "color": "#0292d9" });
                //    }
                //    else if (res.data[i].UserState == 2) {
                //        child.css({ "color": "#52b378" });
                //    }
                //    else if (res.data[i].UserState == 3) {
                //        child.css({ "color": "#c91b24" });
                //    }
                //}
            }
        });
        $(".layui-table-view").height(hei);
    });
}
layui.use('table', function () {
    var table = layui.table;
    //监听工具条
    table.on('tool(demo)', function (obj) {
        var data = obj.data;
        if (obj.event === 'detail') {
            window.location.href = 'UserDetail.html?id=' + data.ID;
        } else if (obj.event === 'del') {

            del(data.ID);

        }
        else if (obj.event === 'edit') {
            window.location.href = 'UserEdit.html?id=' + data.ID;
        }
        else if (obj.event === 'reset') { Reset(data.ID); }
    });



    $('.demoTable .layui-btn').on('click', function () {
        var type = $(this).data('type');
        active[type] ? active[type].call(this) : '';
    });
});

function Add() {
    window.location.href = "UserEdit.html";
}

function del(id) {
    layer.confirm("确认删除？", { title: "系统提示信息" }, function (index) {
        var path = "/User/Delete";
        var data = {
            "IDList": [id],
        };
        if (basepost(data, path)) {
            layer.alert('删除成功！', {
                time: 0,//不自动关闭
                btn: ['确定'],
                title: "系统提示信息",
                yes: function (index) {
                    layer.close(index);
                    window.location.href = "UserList.html";
                }
            });
        }
    });
}


var zNodes = new Array();
function list() {

    $.ajax({
        url: "/Department/List",
        type: "GET",
        dataType: "json",
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        success: function (data) {
            if (data.code == 0) {
                //console.log(data.data.ChildList);
                //填充table的数据
                showall(data.data.ChildList);

                //setting.check.enable = false;
                //setting.view.dblClickExpand = false;
                var zTree = $.fn.zTree.init($("#treeDemo"), setting, zNodes);

                

            } else {
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
    });
}

function showall(menu_list) {
    //获取缓存值
    for (var menu in menu_list) {
        //如果有子节点，则遍历该子节点
        if (menu_list[menu].children.length > 0) {
            zNodes.push({ id: menu_list[menu].value, pId: (menu_list[menu].pid == null ? 0 : menu_list[menu].pid), name: menu_list[menu].name });
            showall(menu_list[menu].children);
        }
        //如果该节点没有子节点，则直接将该节点li以及文本创建好直接添加到父亲节点中
        else {
            zNodes.push({ id: menu_list[menu].value, pId: (menu_list[menu].pid == null ? 0 : menu_list[menu].pid), name: menu_list[menu].name });
        }
    }
}
var setting = {
    view: {
        dblClickExpand: true
    },
    data: {
        simpleData: {
            enable: true
        }
    },
    callback: {
        //beforeClick: beforeClick,
        onClick: onClick
    }
};

function onClick(e, treeId, treeNode) {
    var zTree = $.fn.zTree.getZTreeObj("treeDemo"),
        nodes = zTree.getSelectedNodes(),
        v = "";
    n = "";
    nodes.sort(function compare(a, b) { return a.id - b.id; });
    for (var i = 0, l = nodes.length; i < l; i++) {
        n += nodes[i].name + ",";
        v += nodes[i].id + ",";
    }

    if (n.length > 0) n = n.substring(0, n.length - 1);
    if (v.length > 0) v = v.substring(0, v.length - 1);
    var cityObj = $("#citySel");
    console.log(v);
    $("#depIDs").val(v);
    window.localStorage.setItem("seldep", v);
    cityObj.attr("value", n);
    hideMenu();
}

function showMenu() {
    var cityObj = $("#citySel");
    var cityOffset = $("#citySel").offset();
    $("#menuContent").css({ left: cityOffset.left + "px", top: cityOffset.top + cityObj.outerHeight() + "px" }).slideDown("fast");

    $("body").bind("mousedown", onBodyDown);
}
function hideMenu() {
    $("#menuContent").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}
function onBodyDown(event) {
    if (!(event.target.id == "menuBtn" || event.target.id == "menuContent" || $(event.target).parents("#menuContent").length > 0)) {
        hideMenu();
    }
}

function Reset(sysid) {
    layer.confirm("确认重置密码？", { title: "系统提示信息" }, function (index) {

        var path = "/User/ResetPassword";
        var data = {
            "ID": sysid,
        }
        if (basepost(data, path)) {
            window.location.href = "UserList.html";
            layer.closeAll();
        }
    })
}