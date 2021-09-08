//JavaScript代码区域
layui.use('element', function () {
    var element = layui.element;
});

//表单启用
layui.use('form', function () {
    var form = layui.form; //只有执行了这一步，部分表单元素才会自动修饰成功
})


$(function () {
    NewExtendToken();

    $('#UserName').html(localStorage.getItem("UserName"));
    $('#RealName').html(localStorage.getItem("RealName"))
    GetLayui();
})
function tj() {
    // alert($("#depIDs").val());
    if ($("#oldPwd").val() == "" || $("#oldPwd").val() == null) {
        layer.msg('请输入旧密码', { icon: 5 });
        $("#newPwdCon").focus();
        return;
    }
    if ($("#newPwd").val() == "" || $("#newPwd").val() == null) {
        layer.msg('请输入新密码', { icon: 5 });
        $("#newPwdCon").focus();
        return;
    }

    if ($("#newPwdCon").val() == "" || $("#newPwdCon").val() == null) {
        layer.msg('请输入新密码确认', { icon: 5 });
        $("#newPwdCon").focus();
        return;
    }
    if ($("#newPwdCon").val() != $("#newPwd").val()) {
        layer.msg('两次输入的密码不一致', { icon: 5 });
        return;
    }
    Edit($('#oldPwd').val(), $('#newPwd').val());

}
function Edit(oldPwd, newPwd) {
    var path = "/User/ChangePassword";
    var data = {
        "Password": oldPwd,
        "NewPassword": newPwd
    };

    if (basepost(data, path)) {
        layer.alert('密码修改成功！', {
            time: 0,//不自动关闭
            btn: ['确定'],
            title: "系统提示信息",
            yes: function (index) {
                layer.close(index);
                window.location.href = "UserList.html";
            }
        });

    }

}
