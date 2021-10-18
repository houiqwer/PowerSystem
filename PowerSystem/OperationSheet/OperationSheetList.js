$(function () {

    InitAH();
    InitEnum("ElectricalTaskType", $("#electricalTaskType"), true, "任务类型");
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
        if (obj.event === 'export') {
            Export(data.ID);
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
            , url: '/OperationSheet/List?ahID=' + ahID + '&electricalTaskType=' + electricalTaskType + "&beginDate=" + beginDate + "&endDate=" + endDate
            , page: true
            , cellMinWidth: 80 //全局定义常规单元格的最小宽度，layui 2.2.1 新增
            , headers: { "Authorization": store.userInfo.token }

            , cols: [[
                { field: 'Name', align: 'center', title: '送电柜' }
                , { field: 'VoltageType', align: 'center', title: '电压类型' }
                , { field: 'ElectricalTaskType', align: 'center', title: '任务类型' }
                , { field: 'CreateDate', align: 'center', title: '发起日期' }
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








function Export(id) {
    layer.confirm("确认导出操作票？", { title: "系统提示信息" }, function (index) {
        var par = "id=" + id;
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


