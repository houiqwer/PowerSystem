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
    dopage();
})


function dopage() {
    layui.use('table', function () {
        var table = layui.table;
        var seahei = $('.search').height() == undefined ? 0 : $('.layui-card-header').height();;
        var headhei = $('.layui-card-header').height() == undefined ? 0 : $('.layui-card-header').height();
        var hei = $('.safe-card1').height() - seahei - headhei;
        table.render({
            elem: '#demo',
            url: '/Base/RoleList',
            page: true,
            cellMinWidth: 80, //全局定义常规单元格的最小宽度，layui 2.2.1 新增
            headers: { "Authorization": localStorage.getItem("Token") },

            cols: [[
                //{ type: 'radio', fixed: 'left' },
                { field: 'EnumName', align: 'center', title: '角色名称' },

                { fixed: 'right', align: 'center', toolbar: '#barDemo', title: '操作' }
            ]]
        });
        $(".layui-table-view").height(hei);
    });

}
layui.use('table', function () {
    var table = layui.table;
    //监听工具条
    table.on('tool(demo)', function (obj) {
        var data = obj.data;
        if (obj.event === 'deploy') {
            window.location.href = 'RoleMenu.html?id=' + data.EnumValue;
        }
    });

    $('.demoTable .layui-btn').on('click', function () {
        var type = $(this).data('type');
        active[type] ? active[type].call(this) : '';
    });
});
