layui.use('element', function () {
    var element = layui.element;
});

//表单启用
layui.use('form', function () {
    var form = layui.form; //只有执行了这一步，部分表单元素才会自动修饰成功
    form.on('submit(formDemo)', function (data) {
        tj();
    });
})
//监听提交

//$(document).ready(function () {

//});
var mid;
var pid = 0;
$(function () {
    NewExtendToken();
    $(".right-list").width($(".t-div").width() - 310);
    $(window).resize(function () { $(".right-list").width($(".t-div").width() - 310); });
    //InitHead();

    dopage();
    $(".file-tree").on("click", "a", function () {
        if ($(this).attr("name") != "child") {
            if (!$(this).parent().hasClass("folder-root closed")) {
                $(this).parent().removeClass("open");
                $(this).parent().addClass("folder-root closed");
            }
            else {
                $(this).parent().removeClass("closed");
                $(this).parent().addClass("open");
            }
        }
        var id = $(this).attr("id");
        mid = id;
        $(".bred").show();
        $(".bbl3").show();

        Init(id);
    });
})
function list() {

    $.ajax({
        url: "/Menu/List",
        type: "GET",
        dataType: "json",
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        success: function (data) {
            if (data.code == 0) {

                var tree = $(".file-tree");
                var html = "";
                
                for (var i = 0; i < data.data.length; i++) {
                    html += "<li class='folder-root ' ><a href='javascript:;'  id=" + data.data[i].PModule.ID + ">" + data.data[i].PModule.Name + "</a><ul>";
                    if (data.data[i].Module != null && data.data[i].Module.length > 0) {
                        for (var j = 0; j < data.data[i].Module.length; j++) {
                            if (data.data[i].Module[j].childlist != null && data.data[i].Module[j].childlist.length > 0) {
                                html += "<li class='folder-root '><a href='javascript:;' id='" + data.data[i].Module[j].ID + "'>" + data.data[i].Module[j].Name + "</a><ul>";
                                for (var k = 0; k < data.data[i].Module[j].childlist.length; k++) {
                                    html += "<li><a href='javascript:;' name='child' id='" + data.data[i].Module[j].childlist[k].ID + "'>" + data.data[i].Module[j].childlist[k].Name + "</a></li>";
                                }
                                html += "</ul></li>";
                            }
                            else
                                html += "<li><a href='javascript:;' name='child' id='" + data.data[i].Module[j].ID + "'>" + data.data[i].Module[j].Name + "</a></li>";
                        }
                    }

                    
                    html += "</ul></li>";

                }

                if (data.data.length > 0) {
                    mid = data.data[0].PModule.ID;
                    Init(mid)
                }
                tree.html(html);

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
var htmlChild = "";
var i = 1;
function showall(menu_list, parent) {
    for (var menu in menu_list) {
        //如果有子节点，则遍历该子节点
        if (menu_list[menu].children.length > 0) {
            //创建一个子节点li
            var li;
            if (i == 1)
                li = $("<li class=\"folder-root closed\" ></li>");// closed 初始折叠， open 初始展开
            else {
                i = 2;
                li = $("<li></li>");
            }

            //将li的文本设置好，并马上添加一个空白的ul子节点，并且将这个li添加到父亲节点中
            $(li).append("<a href='javascript:;'  id=" + menu_list[menu].value + ">" + menu_list[menu].name + "</a>").append("<ul></ul>").appendTo(parent);
            //将空白的ul作为下一个递归遍历的父亲节点传入
            showall(menu_list[menu].children, $(li).children().eq(1));
        }
        //如果该节点没有子节点，则直接将该节点li以及文本创建好直接添加到父亲节点中
        else {
            $("<li></li>").append("<a href='javascript:;' name=\"child\"  id=" + menu_list[menu].value + ">" + menu_list[menu].name + "</a>").appendTo(parent);
        }
    }
}


function dopage() {
    list();
    $(".file-tree").filetree({
        collapsed: false,
    });
}



function add(Name,url,icon,order) {
    var path = "/Menu/Add";
    var data = {
        "Name": Name,
        "URL": url,
        "Icon": icon,
        "Order":order,
        "ParentID": pid,
    };

    if (basepost(data, path)) {
        layer.alert('提交成功！', { icon: 6, title: "系统提示信息" });
        window.location.href = "MenuList.html";
        //dopage();
        //window.history.back(-1);
    }
}

function edit(id, Name, url, icon, order) {
    var path = "/Menu/Edit";

    var data = {
        "ID": id,
        "Name": Name,
        "URL": url,
        "Icon": icon,
        "Order": order,
        "ParentID": pid,
    };

    if (basepost(data, path)) {
        layer.alert('提交成功！', {
            time: 0 //不自动关闭
            , btn: ['确定']
            , title: "系统提示信息"
            , yes: function (index) {
                layer.close(index);
                window.location.href = "MenuList.html";
            }
        });

    }
}

function Init(id) {
    if (id != -2) {
        var data = new Array();
        data = {
            "ID": id
        };
        $.ajax({
            url: "/Menu/Get",
            type: "GET",
            data: data,
            dataType: "json",
            async: false,
            beforeSend: function (XHR) {
                XHR.setRequestHeader("Authorization", localStorage.getItem("Token"));
            },
            success: function (data) {
                if (data.code == 0) {
                    //填充数据
                    if (data.data.ParentName == null || data.data.ParentName == "") {
                        $("#parent").hide();
                    } else {
                        $("#parent").show();
                        $("#ParentName").val(data.data.ParentName);
                    }

                    $("#Name").val(data.data.Name);
                    $("#Order").val(data.data.Order);
                    $("#Url").val(data.data.URL);
                    $("#Icon").val(data.data.Icon);

                    //pid = data.data.ParentID;
                    //if (pid == 0 || pid == null)
                    //    $(".bred").hide();
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
}

function tj() {
    if ($("#Name").val() == "" || $("#Name").val() == null) {
        alert("请输入菜单名称");
        return false;
    }

    if ($("#Url").val() == "" || $("#Url").val() == null) {
        alert("请输入菜单链接地址");
        return false;
    }
    

    if (mid != null && mid != 0) {
        edit(mid, $("#Name").val(), $("#Url").val(), $('#Icon').val(),$('#Order').val());
    } else {
        add($("#Name").val(), $("#Url").val(), $('#Icon').val(), $('#Order').val());
    }
}
function addChild() {
    var a = $("#Name").val();
    $("#ParentName").val(a);
    $("#Name").val("");
    $('#Url').val("");
    $('#Icon').val("");
    $('#Order').val("");

    $(".bred").hide();
    $(".bbl3").hide();
    pid = mid;
    mid = 0;
}
function del() {
    if (mid != 0) {

        layer.confirm("确认删除部门？", { title: "系统提示信息" }, function (index) {

            var path = "/Menu/Delete";
            var data = {
                "ID": mid,
            };

            if (basepost(data, path)) {
                layer.alert('删除成功！', {
                    time: 0 //不自动关闭
                    , btn: ['确定']
                    , title: "系统提示信息"
                    , yes: function (index) {
                        layer.close(index);
                        window.location.href = "MenuList.html";
                    }
                });
                //dopage();
                //layer.closeAll();
            }
        })
    }
    else {
        layer.alert('提交失败', { icon: 5, title: "系统提示信息" });
    }
}


function AddParent() {
    pid = 0;
    $("#parent").hide();
    $("#Name").val("");
    $('#Url').val("");
    $('#Icon').val("");
    $('#Order').val("");
    mid = 0;
    GetLayui();
}