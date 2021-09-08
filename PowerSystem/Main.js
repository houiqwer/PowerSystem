$(function () {
    layui.use('element', function () {
        var element = layui.element;
    });
    Menu();
    $("#UserName").html(localStorage.getItem("RealName"));
})



function Menu() {

    $.ajax({
        url: "/Menu/MRoleList",
        type: "GET",
        async: false,
        dataType: "json",
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        success: function (data) {
            if (data.code == 0) {
                if (data.data != null && data.data.length > 0) {
                    var html = "";
                    for (var i = 0; i < data.data.length; i++) {
                        html = html + "<li class=\"layui-nav-item\">";
                        var dataUrl = "javascript:;"
                        if (data.data[i].PModule.URL.indexOf('html') > 0)
                            dataUrl = data.data[i].PModule.URL;
                        html = html + " <a onclick=\"a('" + data.data[i].PModule.URL + "','" + data.data[i].PModule.Name + "')\" title='" + data.data[i].PModule.Name + "'    target=\"mainiframe\"><i class=\"" + data.data[i].PModule.ModuleIcon + "\"></i><span class=\"wext\">" + data.data[i].PModule.Name + "</span></a> ";
                        if (data.data[i].Module != null && data.data[i].Module.length > 0) {
                            html = html + "<dl class=\"layui-nav-child\">";
                            for (var j = 0; j < data.data[i].Module.length; j++) {
                                html = html + "<dd>";
                                html = html + "<a onclick=\"a('" + data.data[i].Module[j].Module.URL + "','" + data.data[i].PModule.Name + ">" + data.data[i].Module[j].Module.Name + "')\" target=\"mainiframe\">" + data.data[i].Module[j].Module.Name + "</a>";
                                if (data.data[i].Module[j].ChildModule != null && data.data[i].Module[j].ChildModule.length > 0) {
                                    html = html + "<dl class=\"layui-nav-child\">";
                                    for (var k = 0; k < data.data[i].Module[j].ChildModule.length; k++) {
                                        html = html + "<dd><a onclick=\"a('" + data.data[i].Module[j].ChildModule[k].URL + "','" + data.data[i].PModule.Name + ">" + data.data[i].Module[j].Module.Name + ">" + data.data[i].Module[j].ChildModule[k].Name + "')\" target=\"mainiframe\">" + data.data[i].Module[j].ChildModule[k].Name + "</a></dd>";
                                    }
                                    html = html + "</dl>";
                                }
                                html = html + "</dd>";
                            }
                            html = html + "</dl>";
                        }
                        html = html + "</li>";
                    }
                    $("#menu").html(html);
                    GetLayui();
                }
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
function a(e, t) {
    //$(e).attr('href', '//www.jb51.net');
    if (e != "#" && e != "")
        $("#mainiframe").attr("src", e);//根据id设置iframe的src，跳转到相应的iframe，即进行iframe局部刷新
    // alert(e);
    if (t != null && t != "") {
        localStorage.setItem("MName", t);
    }
}

function logout() {
    layer.confirm("确认退出系统", { title: "系统提示信息" }, function (index) {
        localStorage.clear();
        sessionStorage.clear();
        location.reload();
        window.location.href = "/Index.html";
    });
}