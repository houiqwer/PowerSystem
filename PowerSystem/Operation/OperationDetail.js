var id = unity.getURL('id');
var sign = unity.getURL('sign');

$(function () {
    NewExtendToken();
    $(".applicationSheet").hide();
    $("#workSheet").hide();
    $(".workSheet").hide();
    $("#stopOperationSheet").hide();
    $(".stopOperationSheet").hide();
    $("#sendOperationSheet").hide();
    $(".sendOperationSheet").hide();
    $(".finish").hide();
    $("#stopTaskUser").hide();
    $("#sendTaskUser").hide();
    $('#stopDispatcherAudit').hide();
    $('#sendElectricalSheet').hide();
    //$('#sendDispatcherAudit').hide();
    $('#pickTaskUser').hide();
    if (id != null && id != '') {
        if (sign != null && sign != "") {
            $("#myAudit").show();
            $(".sh").show();
            //$("#TaskUser").hide();
        }
        else {
            $("#myAudit").hide();
            $(".sh").hide();
            //$("#TaskUser").show();
        }
        Init(id);
    }
    GetLayui();
});
//JavaScript代码区域
layui.use('element', function () {
    var element = layui.element;

});

//表单启用
layui.use('form', function () {
    var form = layui.form; //只有执行了这一步，部分表单元素才会自动修饰成功
})

var applicationSheetID;
var stopOperationSheetID;
var sendOperationSheetID;
//初始化数据
function Init(id) {

    var data = {
        ID: id
    }
    $.ajax({
        url: "/Operation/Get",
        // headers: { Authorization: store.userInfo.token },
        type: "get",
        data: data,
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function (data) {
            // 成功获取数据
            if (data.code == 0) {
                $('#Realname').html(data.data.Realname);
                $("#AHName").html(data.data.AHName);
                $("#VoltageType").html(data.data.VoltageType);
                $("#OperationFlow").html(data.data.OperationFlow);

                $("#IsFinish").html(data.data.IsFinish ? "是" : "否");
                $("#IsConfirm").html(data.data.IsConfirm ? "已确认" : "未确认");
                
                applicationSheetID = data.data.ApplicationSheet.ID;

                $('#BeginDate').html(data.data.ApplicationSheet.BeginDate);
                $('#EndDate').html(data.data.ApplicationSheet.EndDate);
                $('#Aduit').html(data.data.OperationAuditName);
                $('#CreateDate').html(data.data.CreateDate);
                $("#WorkContent").html(data.data.ApplicationSheet.WorkContent);
                $('#DepartmentName').html(data.data.ApplicationSheet.DepartmentName);


                
                var item = "";
                item += "<tr ><th style='text-align:center'>审核人</th><th style='text-align:center'>审核状态</th><th style='text-align:center'>审核日期</th><th style='text-align:center'>审核说明</th></tr>";
                item += "<tr><td style='text-align:center'>" + data.data.ApplicationSheet.AuditUserName + "</td><td style='text-align:center'>" + data.data.ApplicationSheet.Audit + "</td><td style='text-align:center'>" + (data.data.ApplicationSheet.AuditDate == null ? "" : data.data.ApplicationSheet.AuditDate) + "</td><td style='text-align:center'>" + (data.data.ApplicationSheet.AuditMessage == null ? "" : data.data.ApplicationSheet.AuditMessage) + "</td></tr>";
                $("#tbody").append(item);


                
                if (data.data.stopElectricalTask != null && data.data.stopElectricalTask.DispatcherAudit != 1) {
                    var stopDispatcherList = "<tr ><th style='text-align:center'>作业类型</th><th style='text-align:center'>审核人</th><th style='text-align:center'>审核状态</th><th style='text-align:center'>审核日期</th><th style='text-align:center'>审核说明</th></tr>";
                    $('#stopDispatcherAudit').show();
                    stopDispatcherList += "<tr><td style='text-align:center'>" + data.data.stopElectricalTask.ElectricalTaskTypeName + "</td><td style='text-align:center'>" + data.data.stopElectricalTask.RealName + "</td><td style='text-align:center'>" + data.data.stopElectricalTask.AuditName + "</td><td style='text-align:center'>" + (data.data.stopElectricalTask.AuditDateString) + "</td><td style='text-align:center'>" + (data.data.stopElectricalTask.AuditMessage == null ? "" : data.data.stopElectricalTask.AuditMessage) + "</td></tr>";
                    $("#stopDispatcherList").append(stopDispatcherList);

                    $('#stopTaskUser').show();
                    var stop = "<tr ><th style='text-align:center'>停电电工</th><th style='text-align:center'>停电时间</th><th style='text-align:center'>是否确认</th></tr>";
                    for (var i = 0; i < data.data.stopElectricalTask.ElectricalTaskUserList.length; i++) {
                        stop += "<tr><td style='text-align:center'>" + data.data.stopElectricalTask.ElectricalTaskUserList[i].RealName + "</td><td style='text-align:center'>" + data.data.stopElectricalTask.ElectricalTaskUserList[i].CreateDate + "</td><td style='text-align:center'>" + (data.data.stopElectricalTask.ElectricalTaskUserList[i].IsConfirm ? '是' : '否') + "</td></tr>";
                    }
                    $("#stopElectricalTaskList").append(stop);


                    //停电操作单
                    if (data.data.stopElectricalTask.OperationSheet != null) {
                        stopOperationSheetID = data.data.stopElectricalTask.OperationSheet.ID;
                        $("#stopOperationSheet").show();
                        $(".stopOperationSheet").show();
                        $('#stopOperationUserName').html(data.data.stopElectricalTask.OperationSheet.OperationUserName);
                        $('#stopOperationDate').html(data.data.stopElectricalTask.OperationSheet.OperationDateString);
                        $('#stopFinishDate').html(data.data.stopElectricalTask.OperationSheet.FinishDateString);
                        $('#stopGuardianUserName').html(data.data.stopElectricalTask.OperationSheet.GuardianUserName);
                        var html = "";
                        for (var i = 0; i < data.data.stopElectricalTask.OperationSheet.OperationContentList.length; i++) {
                            if (i == 0)
                                html += " <tr class='anquan'><th  rowspan='" + data.data.stopElectricalTask.OperationSheet.OperationContentList.length + 1 + "'>停电操作内容</th> <td colspan='4'><div class='safe-grey'>" + data.data.stopElectricalTask.OperationSheet.OperationContentList[i].Content + "</div></td></tr>";
                            else
                                html += " <tr class='anquan'> <td colspan='4'><div class='safe-grey'>" + data.data.stopElectricalTask.OperationSheet.OperationContentList[i].Content + "</div></td></tr>";
                        }
                        $('#stopOperationContent').after(html);
                    }

                }

                if (data.data.pickElectricalTask != null) {
                    $('#pickTaskUser').show();
                    var send = "<tr ><th style='text-align:center'>摘牌电工</th><th style='text-align:center'>摘牌时间</th><th style='text-align:center'>是否确认</th></tr>";
                    for (var i = 0; i < data.data.pickElectricalTask.ElectricalTaskUserList.length; i++) {

                        send += "<tr><td style='text-align:center'>" + data.data.pickElectricalTask.ElectricalTaskUserList[i].RealName + "</td><td style='text-align:center'>" + data.data.pickElectricalTask.ElectricalTaskUserList[i].CreateDate + "</td><td style='text-align:center'>" + (data.data.pickElectricalTask.ElectricalTaskUserList[i].IsConfirm ? '是' : '否') + "</td></tr>";
                    }
                    $("#pickElectricalTaskList").append(send);
                }


                
                if (data.data.sendElectricalTask != null) {
                    //$('#sendDispatcherAudit').show();
                    //var sendDispatcherList = "<tr ><th style='text-align:center'>作业类型</th><th style='text-align:center'>审核人</th><th style='text-align:center'>审核状态</th><th style='text-align:center'>审核日期</th><th style='text-align:center'>审核说明</th></tr>";
                    //sendDispatcherList += "<tr><td style='text-align:center'>" + data.data.sendElectricalTask.ElectricalTaskTypeName + "</td><td style='text-align:center'>" + data.data.sendElectricalTask.RealName + "</td><td style='text-align:center'>" + data.data.sendElectricalTask.AuditName + "</td><td style='text-align:center'>" + (data.data.sendElectricalTask.AuditDateString) + "</td><td style='text-align:center'>" + (data.data.sendElectricalTask.AuditMessage == null ? "" : data.data.sendElectricalTask.AuditMessage) + "</td></tr>";
                    //$("#sendDispatcherList").append(sendDispatcherList);

                    $('#sendTaskUser').show();
                    var send = "<tr ><th style='text-align:center'>送电电工</th><th style='text-align:center'>送电时间</th><th style='text-align:center'>是否确认</th></tr>";
                    for (var i = 0; i < data.data.sendElectricalTask.ElectricalTaskUserList.length; i++) {

                        send += "<tr><td style='text-align:center'>" + data.data.sendElectricalTask.ElectricalTaskUserList[i].RealName + "</td><td style='text-align:center'>" + data.data.sendElectricalTask.ElectricalTaskUserList[i].CreateDate + "</td><td style='text-align:center'>" + (data.data.sendElectricalTask.ElectricalTaskUserList[i].IsConfirm ? '是' : '否') + "</td></tr>";
                    }
                    $("#sendElectricalTaskList").append(send);


                    //送电操作单
                    if (data.data.sendElectricalTask.OperationSheet != null) {
                        sendOperationSheetID = data.data.sendElectricalTask.OperationSheet.ID;
                        $("#sendOperationSheet").show();
                        $(".sendOperationSheet").show();
                        $('#sendOperationUserName').html(data.data.sendElectricalTask.OperationSheet.OperationUserName);
                        $('#sendOperationDate').html(data.data.sendElectricalTask.OperationSheet.OperationDateString);
                        $('#sendFinishDate').html(data.data.sendElectricalTask.OperationSheet.FinishDateString);
                        $('#sendGuardianUserName').html(data.data.sendElectricalTask.OperationSheet.GuardianUserName);
                        //$('#sendOperationContent').html(data.data.sendElectricalTask.OperationSheet.Content);
                        var html = "";
                        for (var i = 0; i < data.data.sendElectricalTask.OperationSheet.OperationContentList.length; i++) {
                            if (i == 0)
                                html += " <tr class='anquan'><th  rowspan='" + data.data.sendElectricalTask.OperationSheet.OperationContentList.length + 1 + "'>送电操作内容</th> <td colspan='4'><div class='safe-grey'>" + data.data.sendElectricalTask.OperationSheet.OperationContentList[i].Content + "</div></td></tr>";
                            else
                                html += " <tr class='anquan'> <td colspan='4'><div class='safe-grey'>" + data.data.sendElectricalTask.OperationSheet.OperationContentList[i].Content + "</div></td></tr>";
                        }
                        $('#sendOperationContent').after(html);
                    }

                    //送电联票
                    if (data.data.sendElectricalTask.SendElectricalSheet != null) {
                        $('#sendElectricalSheet').show();
                        $('#sendElectricalRealName').html(data.data.sendElectricalTask.SendElectricalSheet.UserRealName);
                        $('#sendElectricalDate').html(data.data.sendElectricalTask.SendElectricalSheet.SendElectricDateString);
                        $('#WorkFinishContent').html(data.data.sendElectricalTask.SendElectricalSheet.WorkFinishContent);
                        $('#IsEvacuateAllPeople').html(data.data.sendElectricalTask.SendElectricalSheet.IsEvacuateAllPeople?"是":"否");
                        $('#IsRemoveGroundLine').html(data.data.sendElectricalTask.SendElectricalSheet.IsRemoveGroundLine?"是":"否");
                    }
                }

                if (data.data.ApplicationSheet.Audit != '待审核') {
                    $('.applicationSheet').show();
                }


                if (data.data.VoltageType == '高压' && data.data.workSheet != null) {
                    $("#workSheet").show();
                    $(".workSheet").show();
                    var workAudit = "";
                    workAudit += "<tr ><th style='text-align:center'>审核人</th><th style='text-align:center'>审核状态</th><th style='text-align:center'>审核日期</th><th style='text-align:center'>审核说明</th></tr>";
                    workAudit += "<tr><td style='text-align:center'>" + data.data.workSheet.MonitorAuditUserName + "</td><td style='text-align:center'>" + data.data.workSheet.MonitorAuditName + "</td><td style='text-align:center'>" + (data.data.workSheet.MonitorAuditDate == null ? "" : data.data.workSheet.MonitorAuditDateString) + "</td><td style='text-align:center'>" + (data.data.workSheet.MonitorAuditMessage == null ? "" : data.data.workSheet.MonitorAuditMessage) + "</td></tr>";
                    workAudit += "<tr><td style='text-align:center'>" + data.data.workSheet.DeputyAuditUserName + "</td><td style='text-align:center'>" + data.data.workSheet.DeputyAuditName + "</td><td style='text-align:center'>" + (data.data.workSheet.DeputyAuditDate == null ? "" : data.data.workSheet.DeputyAuditDateString) + "</td><td style='text-align:center'>" + (data.data.workSheet.DeputyAuditMessage == null ? "" : data.data.workSheet.DeputyAuditMessage) + "</td></tr>";
                    workAudit += "<tr><td style='text-align:center'>" + data.data.workSheet.ChiefAuditUserName + "</td><td style='text-align:center'>" + data.data.workSheet.ChiefAuditName + "</td><td style='text-align:center'>" + (data.data.workSheet.ChiefAuditDate == null ? "" : data.data.workSheet.ChiefAuditDateString) + "</td><td style='text-align:center'>" + (data.data.workSheet.ChiefAuditMessage == null ? "" : data.data.workSheet.ChiefAuditMessage) + "</td></tr>";
                    $("#workSheetAudit").append(workAudit);
                    $('#Influence').html(data.data.workSheet.Influence);
                   
                }
                GetLayui();
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



function tj() {
    if ($("#AuditDesc").val() == null || $("#AuditDesc").val() == "") {
        layer.msg("请输入审批事由", { icon: 5 });
        return;
    }
    if (applicationSheetID != null && applicationSheetID != "") {
        Audit();
    }
}
function Audit() {

    var path = "/ApplicationSheet/Audit";
    var data = {
        ID: applicationSheetID,
        AuditMessage: $("#AuditDesc").val(),
        Audit: $("#AuditState").val(),

    };
    if (basepost(data, path)) {
        layer.alert('提交成功！', {
            time: 0, //不自动关闭
            icon: 6,
            btn: ['确定'],
            title: "系统提示信息",
            yes: function (index) {
                layer.close(index);
                window.location.href = '../ApplicationSheet/MyApplicationSheetAuditList.html';
            }
        });
    }
}

function cancle() {
    window.history.back();
}

function exportWorkSheet() {
    layer.confirm("确认导出高压工作票？", { title: "系统提示信息" }, function (index) {
        var par = "ID=" + id;
        var path = "/Operation/ExportWorkSheet?" + par;
        var xhr = new XMLHttpRequest();
        xhr.open('GET', path, true); // 也可以使用POST方式，根据接口
        xhr.setRequestHeader("Authorization", localStorage.getItem("Token"));
        xhr.responseType = "blob"; // 返回类型blob
        // 定义请求完成的处理函数，请求前也可以增加加载框/禁用下载按钮逻辑
        xhr.onload = function (e) {
            // 请求完成
            if (this.status === 200) {
                // 返回200
                console.log(this.getResponseHeader('Content-Disposition'));
                var blob = this.response;
                var reader = new FileReader();
                reader.readAsDataURL(blob); // 转换为base64，可以直接放入a表情href
                reader.onload = function (e) {
                    // 转换完成，创建一个a标签用于下载
                    var a = document.createElement('a');
                    var wpoInfo = { "Disposition": xhr.getResponseHeader('Content-Disposition'), };
                    var name = "";
                    var d = wpoInfo.Disposition;
                    if (wpoInfo.Disposition.indexOf("filename=")) {
                        name = wpoInfo.Disposition.split("filename=")[1];
                        name = decodeURIComponent(name);
                        console.log(decodeURIComponent(name));
                    } else
                        name = "高压工作票" + new Date().Format("yyyyMMddHH") + ".docx";
                    a.download = name;
                    a.href = e.target.result;
                    $("body").append(a); // 修复firefox中无法触发click
                    a.click();
                    $(a).remove();
                }
            }
        };
        // 发送ajax请求
        xhr.send();
        layer.closeAll();
    });
}

function exportApplicationSheet() {
    
    layer.confirm("确认导出申请单？", { title: "系统提示信息" }, function (index) {


        var par = "ID=" + applicationSheetID;
        var path = "/ApplicationSheet/Export?" + par;
        var xhr = new XMLHttpRequest();
        xhr.open('GET', path, true); // 也可以使用POST方式，根据接口
        xhr.setRequestHeader("Authorization", localStorage.getItem("Token"));
        xhr.responseType = "blob"; // 返回类型blob
        // 定义请求完成的处理函数，请求前也可以增加加载框/禁用下载按钮逻辑
        xhr.onload = function (e) {
            // 请求完成
            if (this.status === 200) {
                // 返回200
                console.log(this.getResponseHeader('Content-Disposition'));
                var blob = this.response;
                var reader = new FileReader();
                reader.readAsDataURL(blob); // 转换为base64，可以直接放入a表情href
                reader.onload = function (e) {
                    // 转换完成，创建一个a标签用于下载
                    var a = document.createElement('a');
                    var wpoInfo = { "Disposition": xhr.getResponseHeader('Content-Disposition'), };
                    var name = "";
                    var d = wpoInfo.Disposition;
                    if (wpoInfo.Disposition.indexOf("filename=")) {
                        name = wpoInfo.Disposition.split("filename=")[1];
                        name = decodeURIComponent(name);
                        console.log(decodeURIComponent(name));
                    } else
                        name = "基建项目" + new Date().Format("yyyyMMddHH") + ".doc";
                    a.download = name;
                    a.href = e.target.result;
                    $("body").append(a); // 修复firefox中无法触发click
                    a.click();
                    $(a).remove();
                }
            }
        };
        // 发送ajax请求
        xhr.send();
        layer.closeAll();
    });
}

function exportStopOperationSheet() {
    layer.confirm("确认导出停电操作票？", { title: "系统提示信息" }, function (index) {
        var par = "id=" + stopOperationSheetID;
        var path = "/OperationSheet/Export?" + par;
        var xhr = new XMLHttpRequest();
        xhr.open('GET', path, true); // 也可以使用POST方式，根据接口
        xhr.setRequestHeader("Authorization", localStorage.getItem("Token"));
        xhr.responseType = "blob"; // 返回类型blob
        // 定义请求完成的处理函数，请求前也可以增加加载框/禁用下载按钮逻辑
        xhr.onload = function (e) {
            // 请求完成
            if (this.status === 200) {
                // 返回200
                console.log(this.getResponseHeader('Content-Disposition'));
                var blob = this.response;
                var reader = new FileReader();
                reader.readAsDataURL(blob); // 转换为base64，可以直接放入a表情href
                reader.onload = function (e) {
                    // 转换完成，创建一个a标签用于下载
                    var a = document.createElement('a');
                    var wpoInfo = { "Disposition": xhr.getResponseHeader('Content-Disposition'), };
                    var name = "";
                    var d = wpoInfo.Disposition;
                    if (wpoInfo.Disposition.indexOf("filename=")) {
                        name = wpoInfo.Disposition.split("filename=")[1];
                        name = decodeURIComponent(name);
                        console.log(decodeURIComponent(name));
                    } else
                        name = "停送电操作票" + new Date().Format("yyyyMMddHH") + ".docx";
                    a.download = name;
                    a.href = e.target.result;
                    $("body").append(a); // 修复firefox中无法触发click
                    a.click();
                    $(a).remove();
                }
            }
        };
        // 发送ajax请求
        xhr.send();
        layer.closeAll();
    });
}

function exportSendOperationSheet() {
    layer.confirm("确认导出送电操作票？", { title: "系统提示信息" }, function (index) {
        var par = "id=" + sendOperationSheetID;
        var path = "/OperationSheet/Export?" + par;
        var xhr = new XMLHttpRequest();
        xhr.open('GET', path, true); // 也可以使用POST方式，根据接口
        xhr.setRequestHeader("Authorization", localStorage.getItem("Token"));
        xhr.responseType = "blob"; // 返回类型blob
        // 定义请求完成的处理函数，请求前也可以增加加载框/禁用下载按钮逻辑
        xhr.onload = function (e) {
            // 请求完成
            if (this.status === 200) {
                // 返回200
                console.log(this.getResponseHeader('Content-Disposition'));
                var blob = this.response;
                var reader = new FileReader();
                reader.readAsDataURL(blob); // 转换为base64，可以直接放入a表情href
                reader.onload = function (e) {
                    // 转换完成，创建一个a标签用于下载
                    var a = document.createElement('a');
                    var wpoInfo = { "Disposition": xhr.getResponseHeader('Content-Disposition'), };
                    var name = "";
                    var d = wpoInfo.Disposition;
                    if (wpoInfo.Disposition.indexOf("filename=")) {
                        name = wpoInfo.Disposition.split("filename=")[1];
                        name = decodeURIComponent(name);
                        console.log(decodeURIComponent(name));
                    } else
                        name = "停送电操作票" + new Date().Format("yyyyMMddHH") + ".docx";
                    a.download = name;
                    a.href = e.target.result;
                    $("body").append(a); // 修复firefox中无法触发click
                    a.click();
                    $(a).remove();
                }
            }
        };
        // 发送ajax请求
        xhr.send();
        layer.closeAll();
    });
}