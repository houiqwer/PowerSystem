layui.use('element', function () {
    var element = layui.element;

});

layui.use('form', function () {
    var form = layui.form; //只有执行了这一步，部分表单元素才会自动修饰成功
})
//表单启用

//日期
layui.use('laydate', function () {
    var laydate = layui.laydate;
    //日期范围
    laydate.render({
        elem: '#JoinDate'
        , range: true
    });
})

$(".sea-text").keydown(function (e) {
    var curKey = e.which;
    if (curKey == 13) {
        nowpage = 1;
        dopage();
    }
});


$(function () {
    NewExtendToken();
    InitEnum("LogCode", $("#LogCode"), true, "日志类型");

    if (localStorage.getItem(window.location.pathname + "realName") != null) {
        $("#realName").val(localStorage.getItem(window.location.pathname + "realName"));
    }
    if (localStorage.getItem(window.location.pathname + "LogCode") != null) {
        $("#LogCode").val(localStorage.getItem(window.location.pathname + "LogCode"));
    }

    var date = $("#JoinDate").val();
    if (date != "") {
        var begindate = date.substring(0, 10);
        var enddate = date.substring(12, 23);
        page($("#realName").val(), $("#LogCode").val(), begindate, enddate);
    }
    else {
        page($("#realName").val(), $("#LogCode").val(), begindate, enddate);
    }
    GetLayui();
})

$("#search").click(function () {

    localStorage.setItem(window.location.pathname + "PageIndex", 1);
    localStorage.setItem(window.location.pathname + "date", $("#JoinDate").val());
    localStorage.setItem(window.location.pathname + "LogCode", $("#LogCode").val());
    localStorage.setItem(window.location.pathname + "realName", $("#realName").val());

    var date = $("#JoinDate").val();
    if (date != "") {
        var begindate = date.substring(0, 10);
        var enddate = date.substring(12, 23);
        page($("#realName").val(), $("#LogCode").val(), begindate, enddate);
    }
    else {
        page($("#realName").val(), $("#LogCode").val(), begindate, enddate);
    }
})

function page(realName, LogCode, begindate, enddate) {

    layui.use('table', function () {
        var table = layui.table;
        var seahei = $('.search').height();
        var headhei = $('.layui-card-header').height() == undefined ? 0 : $('.layui-card-header').height();
        var hei = $('.safe-card1').height() - seahei - headhei;
        //展示已知数据
        table.render({/*宽度设置要加上padding值：150→180，因为表格没设置borderbox*/
            elem: '#demo'
            , url: '/Log/List'
            , where: { realName: realName, LogCode: LogCode, beginDate: begindate, endDate: enddate }
            , cellMinWidth: 80 //全局定义常规单元格的最小宽度，layui 2.2.1 新增
            , skin: 'line' //表格风格
            , even: true
            , page: {
                curr: localStorage.getItem(window.location.pathname + "PageIndex") == null ? 1 : localStorage.getItem(window.location.pathname + "PageIndex")
            }
            , headers: { "Authorization": localStorage.getItem("Token") }
            , cols: [[
                //{ type: 'checkbox' },
                { field: 'LogCode', align: 'center', title: '日志类型' }
                , { field: 'Content', align: 'center', title: '日志内容', }
                , { field: 'CreateDate', align: 'center', title: '操作时间' }
                , { field: 'CreateUser', align: 'center', title: '操作人' }
            ]],

            done: function (res, cur, count) {
                if (cur > 1 && res.data.length === 0) {
                    window.localStorage.setItem(window.location.pathname + "PageIndex", cur - 1);
                    page();
                }
                else {
                    window.localStorage.setItem(window.location.pathname + "PageIndex", cur);
                }


            }
        });
        $(".layui-table-view").height(hei);
    });
}