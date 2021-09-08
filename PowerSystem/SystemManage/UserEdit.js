//JavaScript代码区域
layui.use('element', function () {
    var element = layui.element;
});

//表单启用
layui.use('form', function () {
    var form = layui.form; //只有执行了这一步，部分表单元素才会自动修饰成功
})
layui.use('upload', function () {
    var $ = layui.jquery,
        upload = layui.upload;
    //执行实例
    var uploadInst = upload.render({
        elem: '#test4', //绑定元素

        choose: function (obj) {
            //预读本地文件示例，不支持ie8
            obj.preview(function (index, file, result) {
                $('#demo1').attr('src', result); //图片链接（base64）
            });
        }, auto: false,

    });
});
var id = unity.getURL("id");
$(function () {
    NewExtendToken();
    list();
    InitRole();
    if (id != null && id != "") {
        Init(id);
    }
    GetLayui();
})
function tj() {
    // alert($("#depIDs").val());
    if ($("#SysUserName").val() == "" || $("#SysUserName").val() == null) {
        alert("请输入用户名");
        return;
    }
    if ($("#RealName").val() == "" || $("#RealName").val() == null) {
        alert("请输入姓名");
        return;
    }

    if ($("#CellPhone").val() == "" || $("#CellPhone").val() == null) {
        alert("请输入手机号");
        return;
    }

    if ($("#depIDs").val() == "" || $("#depIDs").val() == null) {
        alert("请选择部门");
        return;
    }


    var roles = GetChecked("Role", "角色");
    if (roles == "") {
        return false;
    }
    if (id != null && id != "") {
        Edit(id, $("#SysUserName").val(), $("#RealName").val(), $("#CellPhone").val(), $("#depIDs").val(), roles);
    }
    else {
        Add($("#SysUserName").val(), $("#RealName").val(), $("#CellPhone").val(), $("#depIDs").val(), roles);
    }
}

function Add(SysUserName, RealName, CellPhone, DepID, Roles) {

    var path = "/User/Add";
    var data = {
        'Username': SysUserName,
        "Realname": RealName,
        'Cellphone': CellPhone,
        'DepartmentID': DepID,
        'RoleIDList': Roles
    };
    $.ajax({
        url: path,
        type: "POST",
        data: data,
        dataType: "json",
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        success: function (data) {
            if (data.code == 0) {
                layer.alert('添加成功！', {
                    closeBtn: 0, time: 0, //不自动关闭
                    icon: 6,
                    btn: ['确定'],
                    Title: "系统提示信息",
                    yes: function (index) {
                        layer.close(index);
                        window.location.href = "UserList.html";
                    }
                });
            } else {
                $("#submit").attr("disabled", false);
                layer.closeAll();
                layer.ready(function () {
                    Title: false
                    layer.alert(data.msg, {
                        Title: false
                    });
                });
            }
        },
        error: function (data) {
            $("#submit").attr("disabled", false);
            layer.closeAll();
            layer.ready(function () {
                layer.alert(data.msg, {
                    closeBtn: 0, time: 0, //不自动关闭
                    icon: 5,
                    btn: ['确定'],
                    Title: "系统提示信息",
                    yes: function (index) {
                        layer.close(index);
                        window.location.href = document.referrer;
                    }
                });
            });

        }
    });

}

function Edit(id, SysUserName, RealName, CellPhone, DepID, Roles) {
    var path = "/User/Edit";
    var data = {
        'ID': id,
        'Username': SysUserName,
        'Realname': RealName,
        'Cellphone': CellPhone,
        'DepartmentID': DepID,
        'RoleIDList': Roles
    };
    $.ajax({
        url: path,
        type: "POST",
        data: data,
        dataType: "json",
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        success: function (data) {
            if (data.code == 0) {
                layer.alert('添加成功！', {
                    closeBtn: 0, time: 0, //不自动关闭
                    icon: 6,
                    btn: ['确定'],
                    Title: "系统提示信息",
                    yes: function (index) {
                        layer.close(index);
                        window.location.href = "UserList.html";
                    }
                });
            } else {
                $("#submit").attr("disabled", false);
                layer.closeAll();
                layer.ready(function () {
                    Title: false
                    layer.alert(data.msg, {
                        Title: false
                    });
                });
            }
        },
        error: function (data) {
            $("#submit").attr("disabled", false);
            layer.closeAll();
            layer.ready(function () {
                layer.alert(data.msg, {
                    closeBtn: 0, time: 0, //不自动关闭
                    icon: 5,
                    btn: ['确定'],
                    Title: "系统提示信息",
                    yes: function (index) {
                        layer.close(index);
                        window.location.href = document.referrer;
                    }
                });
            });

        }
    });
}


function Init(id) {
    var data = {
        ID: id
    }
    $.ajax({
        url: "/User/Get",
        type: "get",
        data: data,
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        success: function (data) {
            if (data.code == 0) {
                $("#SysUserName").val(data.data.Username);
                $("#RealName").val(data.data.Realname);

                $("#CellPhone").val(data.data.Cellphone);
                var zTree = $.fn.zTree.getZTreeObj("treeDemo");
                //var deps = data.data.DepartmentID.split(',');
                //for (var i = 0; i < deps.length; i++) {
                //    //zTree.checkNode(node, true, true);

                //}
                var node = zTree.getNodeByParam("id", data.data.DepartmentID);
                if (node != null) {
                    zTree.checkNode(node, true)
                }

                var node = zTree.getNodeByParam("id", data.data.DepartmentID);
                if (node != null) {
                    zTree.checkNode(node, true)
                }
                var cityObj = $("#citySel");
                $("#depIDs").val(data.data.DepartmentID);
                cityObj.attr("value", data.data.DepartmentName);



                // $("#DepID").val(data.data[0].Depart.DepIDs);//DepIDs


                if (data.data.UserRoleList.length > 0) {
                    for (var i = 0; i < data.data.UserRoleList.length; i++) {
                        $("#Role-" + data.data.UserRoleList[i].Role).attr("checked", true);

                    }
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
                $.fn.zTree.init($("#treeDemo"), setting, zNodes);
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
    for (var menu in menu_list) {
        //如果有子节点，则遍历该子节点
        if (menu_list[menu].children.length > 0) {
            //创建一个子节点li{ id: 1, pId: 0, name: "北京" },
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

function InitRole() {
    var data = new Array();
    $.ajax({
        url: "/Base/RoleList",
        type: "GET",

        dataType: "json",
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        success: function (data) {
            if (data.code == 0) {
                var html = "";
                for (var i = 0; i < data.data.length; i++) {

                    html = html + "<input type=\"checkbox\"  id=\"Role-" + data.data[i].EnumValue + "\" value='" + data.data[i].EnumValue + "' lay-skin=\"primary\" title=\"" + data.data[i].EnumName + "\" >";


                }
                $("#Role").html(html);

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