$(function () {
    list();
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
        } else if (obj.event === 'export') {
            Export(data.ApplicationSheet.ID);
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
    var departmentID = $("#depIDs").val();

    layui.use('table', function () {
        var table = layui.table;
        var ssq = $(".search").val() == undefined ? 0 : $(".search").height();
        var hei = $('.safe-card1').height() - 51 - ssq;
        table.render({
            elem: '#table'
            , url: '/Operation/List?ahID=' + ahID + '&voltageType=' + voltageType + "&beginDate=" + beginDate + "&endDate=" + endDate + "&departmentID=" + departmentID
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
                , { field: 'VoltageType', align: 'center', title: '电压类型', width: 100}
                , { field: 'OperationFlow', align: 'center', title: '作业状态' }
                , { field: 'CreateDate', align: 'center', title: '发起日期' }
                , { field: 'Realname', align: 'center', title: '发起人', width: 120}
               // , { field: 'title', align: 'center', title: '审核状态', templet: function (d) { return d.ApplicationSheet.Audit; } }
                , { field: 'OperationAuditName', align: 'center', title: '审核状态' }
                , { fixed: 'right', align: 'center', toolbar: '#bar', title: '操作' }
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


var zNodes = new Array();
function list() {

    $.ajax({
        url: "/Department/List",
        type: "GET",
        dataType: "json",
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        success: function (data) {
            if (data.code == 0) {
                //console.log(data.data.ChildList);
                //填充table的数据
                showall(data.data.ChildList);

                //setting.check.enable = false;
                //setting.view.dblClickExpand = false;
                var zTree = $.fn.zTree.init($("#treeDemo"), setting, zNodes);



            } else {
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
    });
}

function showall(menu_list) {
    //获取缓存值
    for (var menu in menu_list) {
        //如果有子节点，则遍历该子节点
        if (menu_list[menu].children.length > 0) {
            zNodes.push({ id: menu_list[menu].value, pId: (menu_list[menu].pid == null ? 0 : menu_list[menu].pid), name: menu_list[menu].name });
            showall(menu_list[menu].children);
        }
        //如果该节点没有子节点，则直接将该节点li以及文本创建好直接添加到父亲节点中
        else {
            zNodes.push({ id: menu_list[menu].value, pId: (menu_list[menu].pid == null ? 0 : menu_list[menu].pid), name: menu_list[menu].name });
        }
    }
}
var setting = {
    view: {
        dblClickExpand: true
    },
    data: {
        simpleData: {
            enable: true
        }
    },
    callback: {
        //beforeClick: beforeClick,
        onClick: onClick
    }
};

function onClick(e, treeId, treeNode) {
    var zTree = $.fn.zTree.getZTreeObj("treeDemo"),
        nodes = zTree.getSelectedNodes(),
        v = "";
    n = "";
    nodes.sort(function compare(a, b) { return a.id - b.id; });
    for (var i = 0, l = nodes.length; i < l; i++) {
        n += nodes[i].name + ",";
        v += nodes[i].id + ",";
    }

    if (n.length > 0) n = n.substring(0, n.length - 1);
    if (v.length > 0) v = v.substring(0, v.length - 1);
    var cityObj = $("#citySel");
    console.log(v);
    $("#depIDs").val(v);
    window.localStorage.setItem("seldep", v);
    cityObj.attr("value", n);
    hideMenu();
}

function showMenu() {
    var cityObj = $("#citySel");
    var cityOffset = $("#citySel").offset();
    $("#menuContent").css({ left: cityOffset.left + "px", top: cityOffset.top + cityObj.outerHeight() + "px" }).slideDown("fast");

    $("body").bind("mousedown", onBodyDown);
}
function hideMenu() {
    $("#menuContent").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}
function onBodyDown(event) {
    if (!(event.target.id == "menuBtn" || event.target.id == "menuContent" || $(event.target).parents("#menuContent").length > 0)) {
        hideMenu();
    }
}

$(".inputdel").click(function () {
    $(this).siblings("input").attr("value", "");;
    //$("#citySel").val("");
    $("#depIDs").val('');
})


function Export(id) {
    layer.confirm("确认导出全流程表单？", { title: "系统提示信息" }, function (index) {


        var par = "ID=" + id;
        var path = "/ApplicationSheet/ExportAllSheet?" + par;
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
                        name = "停送电全流程表单" + new Date().Format("yyyyMMddHH") + ".doc";
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

function ExportList() {
    var ahID = $("#ah").val();
    var voltageType = $("#voltageType").val();
    var date = $("#date").val();
    var beginDate = "1900-01-01";
    var endDate = "9999-12-31";
    if (date != "") {
        beginDate = date.substring(0, 10);
        endDate = date.substring(12, 23);
    }
    var departmentID = $("#depIDs").val();

    layer.confirm("确认导出列表数据？", { title: "系统提示信息" }, function (index) {

        let load = layer.msg('数据导出中', { icon: 16, shade: 0.3, time: 0 });
        var par = 'ahID=' + ahID + '&voltageType=' + voltageType + "&beginDate=" + beginDate + "&endDate=" + endDate + "&departmentID=" + departmentID;
        var path = "/Operation/ExportByList?" + par;
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
                        name = "停送电全列表数据导出" + new Date().Format("yyyyMMddHH") + ".doc";
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


