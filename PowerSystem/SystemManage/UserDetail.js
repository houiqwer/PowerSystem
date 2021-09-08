//JavaScript代码区域
layui.use('element', function () {
    var element = layui.element;
});

//表单启用
layui.use('form', function () {
    var form = layui.form; //只有执行了这一步，部分表单元素才会自动修饰成功
})

//日期

var id = unity.getURL("id");
$(function () {
    NewExtendToken
    if (id != null && id != "") {
        Init(id);
    }
})
function Init(id) {
    var data = {
        ID: id
    }
    $.ajax({
        url: "/User/Get",
        type: "get",
        data: data,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        success: function (data) {
            if (data.code == 0) {
                $("#SysUserName").text(data.data.Username);
                $("#RealName").text(data.data.Realname);

                $("#CellPhone").text(data.data.Cellphone);
                $("#DepID").text(data.data.DepartmentName);
                var role = "";
                if (data.data.UserRoleList.length > 0) {
                    for (var i = 0; i < data.data.UserRoleList.length; i++) {
                        role += data.data.UserRoleList[i].RoleName +",";
                    }
                }
                role = role.substring(0, role.length - 1);
                $('#Role').text(role);
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