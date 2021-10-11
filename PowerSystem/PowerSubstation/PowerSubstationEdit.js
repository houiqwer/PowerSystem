var urlID = unity.getURL('id');


$(function () {
    
    if (urlID != null && urlID != '') {
        Init(urlID);
    }

    GetLayui();
});


//JavaScript代码区域
layui.use('element', function () {
    var element = layui.element;

});

//表单启用
layui.use('form', function () {
    var form = layui.form;

    form.on('submit(submit)', function (data) {
        Submit();
    });
});


//初始化数据
function Init(id) {
    var data = {
        ID: id
    }
    $.ajax({
        url: "/PowerSubstation/Get",
        type: "get",
        data: data,
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function (data) {
            if (data.code == 0) {
                $("#name").val(data.data.Name);
                
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

function Submit() {

    if ($("#name").val() == null || $("#name").val() == "") {
        alert("请输入变电所名称");
        $("#name").focus();
        return;
    }

    if (urlID == null || urlID == '') {
        Add($('#name').val());
    }
    else {
        Edit(urlID, $('#name').val());
    }

}


function Add(Name) {
    var path = "/PowerSubstation/Add";
    var data = {
        'Name': Name,
        
    };
    if (basepost(data, path)) {
        layer.alert('添加成功！', {
            time: 0, //不自动关闭
            btn: ['确定'],
            title: "系统提示信息",
            yes: function (index) {
                window.location.href = 'PowerSubstationList.html';
            }
        });
    }
}

function Edit(ID, Name) {
    var path = "/PowerSubstation/Edit";
    var data = {
        'ID': ID,
        'Name': Name,
       
    };
    if (basepost(data, path)) {
        layer.alert('修改成功！', {
            time: 0, //不自动关闭
            btn: ['确定'],
            title: "系统提示信息",
            yes: function (index) {
                window.location.href = 'PowerSubstationList.html';
            }
        });
    }
}





function Cancle() {
    window.location.href = 'PowerSubstationList.html';
}




