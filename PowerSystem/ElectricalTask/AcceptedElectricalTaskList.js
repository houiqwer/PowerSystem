$(function () {
    InitAH();
    InitElectricalTaskType();
    Page();
    GetLayui();
})

//JavaScript代码区域
layui.use('element', function () {
    var element = layui.element;
});

//表单启用
layui.use('form', function () {
    var form = layui.form; //只有执行了这一步，部分表单元素才会自动修饰成功
})

//日期
layui.use('laydate', function () {
    var laydate = layui.laydate;
    //日期范围
    laydate.render({
        elem: '#date'
        , range: true
    });
})
//表格
var table = layui.table;

//转换静态表格
layui.use('table', function () {
    var table = layui.table;
    //监听单元格事件
    table.on('tool(table)', function (obj) {
        var data = obj.data;
        if (obj.event === 'confirm') {
            Confirm(data.ID, data.VoltageType, data.ElectricalTaskType);
        } else if (obj.event === 'back') {
            Back(data.ID);
        }
    });
});


function Page() {
    var ahID = $("#ah").val();
    var electricalTaskType = $("#electricalTaskType").val();
    var date = $("#date").val();
    var beginDate = "1900-01-01";
    var endDate = "9999-12-31";
    if (date != "") {
        beginDate = date.substring(0, 10);
        endDate = date.substring(12, 23);
    }

    layui.use('table', function () {
        var table = layui.table;
        var ssq = $(".search").val() == undefined ? 0 : $(".search").height();
        var hei = $('.safe-card1').height() - 51 - ssq;
        table.render({
            elem: '#table'
            , url: '/ElectricalTask/AcceptedList?ahID=' + ahID + '&electricalTaskType=' + electricalTaskType + "&beginDate=" + beginDate + "&endDate=" + endDate
            , page: true
            , cellMinWidth: 80 //全局定义常规单元格的最小宽度，layui 2.2.1 新增
            , headers: { "Authorization": store.userInfo.token }
            , cols: [[
                { field: 'AHName', align: 'center', title: '送电柜' }
                , { field: 'VoltageTypeName', align: 'center', title: '电压类型' }
                , { field: 'ElectricalTaskTypeName', align: 'center', title: '作业类型' }
                , { field: 'CreateDate', align: 'center', title: '发起日期' }
                , { field: 'ReciveCount', align: 'center', title: '当前接收人数' }
                , { fixed: 'right', align: 'center', toolbar: '#bar', title: '操作', width: 160 }
            ]]
            , done: function (res, cur, count) {
                if (cur > 1 && res.data.length === 0) {
                    window.localStorage.setItem(window.location.pathname + "PageIndex", cur - 1);
                    Page();
                }
                else {
                    window.localStorage.setItem(window.location.pathname + "PageIndex", cur);
                }
            }
        });
        $(".layui-table-view").height(hei);
    });
}

$("#search").click(function () {
    localStorage.setItem(window.location.pathname + "PageIndex", 1);
    Page();
})


function InitAH() {
    var data = {
        page: 1,
        limit: 999
    }
    $.ajax({
        url: "/AH/List",
        type: "get",
        data: data,
        dataType: "json",
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function (data) {
            if (data.code == 0) {
                var html = "<option>所有送电柜</option>";
                for (var i = 0; i < data.data.length; i++) {
                    html += "<option value=\"" + data.data[i].ID + "\">" + data.data[i].Name + "</option>";
                }
                $("#ah").html(html);
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


function InitElectricalTaskType() {
    var data = {
        type: "ElectricalTaskType"
    }
    $.ajax({
        url: "/Base/GetEnum",
        type: "get",
        data: data,
        dataType: "json",
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function (data) {
            if (data.code == 0) {
                var html = "<option>所有作业</option>";
                for (var i = 0; i < data.data.List.length; i++) {
                    html += "<option value=\"" + data.data.List[i].EnumValue + "\">" + data.data.List[i].EnumName + "</option>";
                }
                $("#electricalTaskType").html(html);
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




function Back(id) {
    var path = "/ElectricalTask/Back";
    var data = {
        'ID': id,
    };
    if (basepost(data, path)) {
        layer.alert("已退回", {
            time: 0, //不自动关闭
            btn: ['确定'],
            title: "系统提示信息",
            yes: function (index) {
                layer.close(index);
                Page();
            }
        });
    }
}


var w = ($(window).width()*0.7);
var h = ($(window).height()*0.6);

function Confirm(id, VoltageType, ElectricalTaskType) {
    if (VoltageType == 1 || ElectricalTaskType == 3) {//低压或摘牌任务
        var path = "/ElectricalTask/Confirm";
        var data = {
            'ID': id,
        };
        if (basepost(data, path)) {
            layer.alert("已确认", {
                time: 0, //不自动关闭
                btn: ['确定'],
                title: "系统提示信息",
                yes: function (index) {
                    layer.close(index);
                    Page();
                }
            });
        }
    } else {//高压
        layer.open({
            type: 2,
            title: "操作票",
            area: [w + 'px', h + 'px'],
            shade: 0.4,
            maxmin: true,
            content: 'OperationSheetAdd.html?id=' + id + '&ElectricalTaskType='+ElectricalTaskType,
        })
        
    }
}