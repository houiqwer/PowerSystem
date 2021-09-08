$(function () {
    
    InitAH();
    InitEnum("VoltageType", $("#voltageType"), true, "电压类型");
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
        if (obj.event === 'detail') {
            window.location.href = 'OperationDetail.html?id=' + data.ID;
        } else if (obj.event === 'Hang') {
            Hang(data.ID);
        } else if (obj.event === 'Pick') {
            Pick(data.ID);
        }
    });
});


function Page() {
    var ahID = $("#ah").val();
    var voltageType = $("#voltageType").val();
    var date = $("#date").val();
    var beginDate = "1900-01-01";
    var endDate = "9999-12-31";
    if (date != "") {
        beginDate = date.substring(0, 10);
        endDate = date.substring(12, 23);
    }
    //var departmentID = $("#depIDs").val();

    layui.use('table', function () {
        var table = layui.table;
        var ssq = $(".search").val() == undefined ? 0 : $(".search").height();
        var hei = $('.safe-card1').height() - 51 - ssq;
        table.render({
            elem: '#table'
            , url: '/Operation/MyList?ahID=' + ahID + '&voltageType=' + voltageType + "&beginDate=" + beginDate + "&endDate=" + endDate
            , page: true
            , cellMinWidth: 80 //全局定义常规单元格的最小宽度，layui 2.2.1 新增
            , headers: { "Authorization": store.userInfo.token }
            , parseData: function (res) { //res 即为原始返回的数据
                return {
                    "code": res.code, //解析接口状态
                    "msg": res.msg, //解析提示文本
                    "count": res.count, //解析数据长度
                    "data": res.data //解析数据列表
                };
            }
            , cols: [[
                { field: 'AHName', align: 'center', title: '送电柜' }
                , { field: 'VoltageType', align: 'center', title: '电压类型' }
                , { field: 'OperationFlow', align: 'center', title: '作业状态' }
                , { field: 'CreateDate', align: 'center', title: '发起日期' }
                , { field: 'Realname', align: 'center', title: '发起人' }
                , { field: 'title', align: 'center', title: '审核状态', templet: function (d) { return d.ApplicationSheet.Audit; } }
                , { fixed: 'right', align: 'center', toolbar: '#bar', title: '操作', width: 180 }
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

function Hang(id) {
    layer.confirm("确认挂停电牌？", { title: "系统提示信息" }, function (index) {
        var path = "/Operation/Hang";
        var data = {
            "ID": id,
        };
        if (basepost(data, path)) {
            layer.alert('挂停电牌成功！', {
                time: 0,//不自动关闭
                btn: ['确定'],
                title: "系统提示信息",
                yes: function (index) {
                    layer.close(index);
                    window.location.href = "MyOperationList.html";
                }
            });
        }
    });
}

function Pick(id) {
    layer.confirm("确认取停电牌？", { title: "系统提示信息" }, function (index) {
        var path = "/Operation/Pick";
        var data = {
            "ID": id,
        };
        if (basepost(data, path)) {
            layer.alert('取停电牌成功！', {
                time: 0,//不自动关闭
                btn: ['确定'],
                title: "系统提示信息",
                yes: function (index) {
                    layer.close(index);
                    window.location.href = "MyOperationList.html";
                }
            });
        }
    });
}


