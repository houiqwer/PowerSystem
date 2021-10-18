var id = unity.getURL('id'); //任务id
var ElectricalTaskType = unity.getURL('ElectricalTaskType'); //停电 送电


var isOperationUser = true;

$(function () {
    $('#operationSheetAdd').hide();
    $('#operationSheetDetail').hide();
    $('#sendElectrialSheet').hide();
    InitOperationSheet(id);
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

layui.use('laydate', function () {
    var laydate = layui.laydate;
    //日期范围
    laydate.render({
        elem: '#SendElectricDate'
        , type: 'datetime'
        ,trigger: 'click'
    });

    
})




function Submit() {
    
    if (isOperationUser) {
        if ($('#Content').val() == null || $('#Content').val() == "") {
            alert("请输入高压操作内容");
            $("#Content").focus();
            return;
        }
        AddOperationSheet($('#Content').val());
    } else {
        if (ElectricalTaskType == 1) {//停电只要确认
            ConfirmOperationSheet();
        } else { //送电需要填写送电确认单
            if ($('#WorkFinishContent').val() == null || $('#WorkFinishContent').val() == "") {
                alert("请输入工作完成情况");
                $("#WorkFinishContent").focus();
                return;
            }

            if ($('#SendElectricDate').val() == null || $('#SendElectricDate').val() == "") {
                alert("请选择要求送电时间");
                $("#SendElectricDate").focus();
                return;
            }
            AddSendElectricalSheet($('#WorkFinishContent').val(), $('#SendElectricDate').val(), $('input[name="GroundLine"]:checked').val(), $('input[name="EvacuateAllPeople"]:checked').val())
        }
    }


}

function AddSendElectricalSheet(WorkFinishContent, SendElectricDate, GroundLine, EvacuateAllPeople) {
    var path = "/ElectricalTask/Confirm";
    var data = {
        'ID': id,
        'SendElectricalSheet': {
            'WorkFinishContent': WorkFinishContent,
            'SendElectricDate': SendElectricDate,
            'IsRemoveGroundLine': GroundLine == 1 ? true : false,
            'IsEvacuateAllPeople': EvacuateAllPeople == 1 ? true : false
        }
    };
    if (basepost(data, path)) {
        layer.alert("已提交送电确认单", {
            time: 0, //不自动关闭
            btn: ['确定'],
            title: "系统提示信息",
            yes: function (index) {
                parent.location.reload(true);
            }
        });
    }
}


function AddOperationSheet(content) {
    var path = "/ElectricalTask/Confirm";
    var data = {
        'ID': id,
        'OperationSheet': {
            'Content': content
        }
    };
    if (basepost(data, path)) {
        layer.alert("已提交操作单并确认", {
            time: 0, //不自动关闭
            btn: ['确定'],
            title: "系统提示信息",
            yes: function (index) {
                parent.location.reload(true);
            }
        });
    }
}

function ConfirmOperationSheet() {
    var path = "/ElectricalTask/Confirm";
    var data = {
        'ID': id
    };
    if (basepost(data, path)) {
        layer.alert("已确认完成", {
            time: 0, //不自动关闭
            btn: ['确定'],
            title: "系统提示信息",
            yes: function (index) {
                parent.location.reload(true);
            }
        });
    }
}




function Cancle() {
    var index = parent.layer.getFrameIndex(window.name);
    parent.layer.close(index);//关闭当前页
}

function InitOperationSheet(id) {
    var data = {
        "ID":id
    }
    $.ajax({
        url: "/OperationSheet/Get",
        type: "get",
        data: data,
        dataType: "json",
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function (data) {
            if (data.code == 0) {
                if (data.data == null) { //操作人填写操作票
                    $('#operationSheetAdd').show();
                    isOperationUser = true;
                   
                } else { // 监护人确认
                    isOperationUser = false;
                    $('#operationSheetDetail').show();
                    $("#Realname").html(data.data.Realname);
                    $("#OperationDate").html(data.data.OperationDate);
                    $("#WorkContent").html(data.data.Content);

                    if (ElectricalTaskType == 2) { //送电 监护人需要填写送电确认单
                        $('#sendElectrialSheet').show();
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











