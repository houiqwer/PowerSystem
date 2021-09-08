/*自适应背景*/
var hei = $("body").height();
var wei = $("body").width();
if (wei / hei < 1.777) {
    $("body").css("background-size", "auto 100%");
}
window.onresize = function () {
    var hei = $("body").height();
    var wei = $("body").width();
    if (wei / hei < 1.777) {
        $("body").css("background-size", "auto 100%");
    } else {
        $("body").css("background-size", "100% auto");
    }
}

layui.use('element', function () {
    var element = layui.element;

});

layui.use('form', function () {
    var form = layui.form; //只有执行了这一步，部分表单元素才会自动修饰成功
});



$(function () {
    //var MName = localStorage.getItem("UserName");
    //var Pwd = localStorage.getItem("PassWord");
    //if (MName != null && MName != "" && Pwd != null && Pwd != "") {
    //    $("#username").val(MName);
    //    $("#password").val(Pwd);
    //    login();
    //}
});

function login() {
    var username = $("#username").val();
    var password = $("#password").val();
    //if ($("#CheckID").prop("checked") == true) {
    //    localStorage.setItem("UserName", $("#username").val());
    //    localStorage.setItem("PassWord", $("#password").val());
    //}
    if (username == null || username == "") {
        alert("请输入用户名");
        $("#username").focus();
        return false;
    }
    if (password == null || password == "") {
        alert("请输入密码");
        $("#password").focus();
        return false;
    }


    layer.alert("正在登陆，请稍后……");
    $.ajax({
        url: "/Account/Login",
        type: "POST",
        data: {
            "Username": username,
            "Password": password
        },
        dataType: "json",
        success: function (data) {
            if (data.code == 0) {
                ResetUserLogStatus(data);
                //localStorage.setItem("UserName", $("#username").val());
                //localStorage.setItem("PassWord", $("#password").val());

                window.location.href = "Main.html";

            } else {
                
                //alert(data.Status+":error");
                layer.ready(function () {
                    title: false
                    layer.alert(data.msg, {
                        title: false
                    });
                });
                $("#password").val("");
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

$("#username").keydown(function (e) {
    var curKey = e.which;
    if (curKey == 13) {
        login();
    }
});

$("#password").keydown(function (e) {
    var curKey = e.which;
    if (curKey == 13) {
        login();
    }
});