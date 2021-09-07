layui.use('element', function () {
    var element = layui.element;

});


var id = unity.getURL("id");
var menuData;
$(function () {
    NewExtendToken();
    InitMenu(id);
    layui.use('form', function () {
        var form = layui.form; //只有执行了这一步，部分表单元素才会自动修饰成功
        form.on('checkbox', function (data) {
            if (data.elem.id.indexOf("Module-P") > -1) {
                if (data.elem.checked) {
                    var d = $("#" + data.elem.id).parent().next();
                    d.find("input[type='checkbox']").each(function () {
                        $(this).next('div').addClass('layui-form-checked');
                        $(this).prop("checked", true)
                    });
                    //一级菜单选中

                }
                else {
                    var d = $("#" + data.elem.id).parent().next();
                    d.find("input[type='checkbox']").each(function () {
                        $(this).next('div').removeClass('layui-form-checked');
                        $(this).prop("checked", false)
                    });
                }
            }
            else if (data.elem.id.indexOf("Module-C") > -1) {
                if (data.elem.checked) {
                    //var d = $("#" + data.elem.id).parent().prev();
                    //d.find("input[type='checkbox']").each(function () {
                    //    $(this).next('div').addClass('layui-form-checked');
                    //    $(this).prop("checked", true)
                    //});
                    var d = $("#" + data.elem.id).next().next();
                    if (d.find("input[type='checkbox']").length > 0) {
                        d.find("input[type='checkbox']").each(function () {
                            $(this).next('div').addClass('layui-form-checked');
                            $(this).prop("checked", true)
                        });
                    }
                    else {
                        $("#" + data.elem.id).parent().prev('div').addClass('layui-form-checked').prop("checked", true);
                        //$("#" + data.elem.id).parent().parent().prev().find("input[type='checkbox']").each(function () {
                        //    $(this).next('div').addClass('layui-form-checked');
                        //    $(this).prop("checked", true);
                        //})
                    }
                }
                else {
                    var d = $("#" + data.elem.id).next().next();
                    if (d.find("input[type='checkbox']").length > 0) {
                        d.find("input[type='checkbox']").each(function () {
                            $(this).next('div').removeClass('layui-form-checked');
                            $(this).prop("checked", false)
                        });
                    }
                    else {
                        if ($("#" + data.elem.id).parent().find("div[class='layui-unselect layui-form-checkbox layui-form-checked']").length == 0) {
                            $("#" + data.elem.id).parent().prev('div').removeClass('layui-form-checked');
                            $("#" + data.elem.id).parent().prev().prev("input[type='checkbox']").prop("checked", false);
                        }
                    }

                }
                CheckPModule(data.elem);
            }
            //console.log(data);
        });
    })
    //判断一级菜单是否选中
    function CheckPModule($this) {
        var thCheck = $this.parentElement.previousSibling.children[0];
        var tdCheck = $this.parentElement;
        if ($this.parentElement.previousSibling.innerHTML.indexOf('input') == -1) {
            thCheck = $this.parentElement.parentElement.previousSibling.children[0];
            tdCheck = $this.parentElement.parentElement;
        }
        var isSelect = false;
        if ($(tdCheck).find("input[type='checkbox']").length > 0) {
            $(tdCheck).find("input[type='checkbox']").each(function () {
                //$(this).next('div').removeClass('layui-form-checked');
                if ($(this).prop("checked")) {
                    isSelect = true;
                    return true;
                }
            });
        }
        if (isSelect) {
            $(thCheck).next('div').addClass('layui-form-checked');
            $(thCheck).prop("checked", true);
        }
        else {
            $(thCheck).next('div').removeClass('layui-form-checked');
            $(thCheck).prop("checked", false);
        }
    }
    $('input[id^="Module-C"]').on('click', function () {
        $(this).next('div').removeClass('layui-form-checked');
        //var sid = $(this).attr("id").replace("Module-P-", "");
        // var tds = jQuery('td[id^="Module-C"]');
        var s = $(this).parent();
        var d = s.prev();
        //var f = d.find("input[type='checkbox']").each(function () {
        //    $(this).parent('i').removeClass('checkbox_bg_check');
        //});
        if ($(this).prop('checked')) {
            d.find("input[type='checkbox']").each(function () {
                $(this).next('div').addClass('layui-form-checked');
            });
            $(this).next('div').addClass('layui-form-checked');
        }
    });

})


function InitMenu(RoleID) {

    $.ajax({
        url: "/Menu/list",
        type: "GET",
        data: { Role: RoleID },
        dataType: "json",
        async: false,
        beforeSend: function (XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function (data) {
            if (data.code == 0) {
                //填充数据
                if (data.data != null && data.data.length > 0) {
                    var html = "";
                    var table = $("#Module");
                    for (var i = 0; i < data.data.length; i++) {
                        html = html + " <tr>";
                        if (data.data[i].PWhether) {
                            html = html + "<th><input type=\"checkbox\"  id=\"Module-P-" + data.data[i].PModule.ID + "\" value='" + data.data[i].PModule.ID + "' lay-skin=\"primary\" title=\"" + data.data[i].PModule.Name + "\" checked=\"\"></th>";
                        }
                        else {
                            html = html + "<th><input type=\"checkbox\"  id=\"Module-P-" + data.data[i].PModule.ID + "\" value='" + data.data[i].PModule.ID + "' lay-skin=\"primary\" title=\"" + data.data[i].PModule.Name + "\" ></th>";
                        }

                        if (data.data[i].Module != null && data.data[i].Module.length > 0) {
                            html = html + "<td>";
                            for (var j = 0; j < data.data[i].Module.length; j++) {
                                if (data.data[i].Module[j].CWhether) {
                                    html = html + "<input type=\"checkbox\"  id=\"Module-C-" + data.data[i].Module[j].ID + "\" value='" + data.data[i].Module[j].ID + "' lay-skin=\"primary\" title=\"" + data.data[i].Module[j].Name + "\" checked=\"\">";
                                }
                                else {
                                    html = html + "<input type=\"checkbox\"  id=\"Module-C-" + data.data[i].Module[j].ID + "\" value='" + data.data[i].Module[j].ID + "' lay-skin=\"primary\" title=\"" + data.data[i].Module[j].Name + "\" >";
                                }
                                //html += " <div class=\"table_xr_2 layui-icon \"> <input type=\"checkbox\" id=\"Module-C-4\" value=\"4\" lay-skin=\"primary\" title=\"zzz\"></div>";
                                for (var k = 0; k < data.data[i].Module[j].childlist.length; k++) {
                                    if (k == 0) {
                                        html += "<div class=\"table_xr_2 layui-icon \">";
                                    }
                                    if (data.data[i].Module[j].childlist[k].CWhether) {
                                        html = html + "<input type=\"checkbox\"  id=\"Module-C-" + data.data[i].Module[j].childlist[k].ID + "\" value='" + data.data[i].Module[j].childlist[k].ID + "' lay-skin=\"primary\" title=\"" + data.data[i].Module[j].childlist[k].Name + "\" checked=\"\">";
                                    }
                                    else {
                                        html = html + "<input type=\"checkbox\"  id=\"Module-C-" + data.data[i].Module[j].childlist[k].ID + "\" value='" + data.data[i].Module[j].childlist[k].ID + "' lay-skin=\"primary\" title=\"" + data.data[i].Module[j].childlist[k].Name + "\" >";
                                    }
                                    if (k == data.data[i].Module[j].childlist.length - 1) {
                                        html += "</div>";
                                    }
                                }
                            }
                            html = html + "</td>";
                        }
                        //html = html + "</td>";
                        html = html + "</tr>";

                    }

                    table.html(html);
                    layui.use('form', function () {
                        
                        var form = layui.form; //只有执行了这一步，部分表单元素才会自动修饰成功
                    })
                    GetLayui();
                    //$.inputStyle();
                }
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

function tj() {
    var moduleList = GetChecked("Module-P", "菜单");

    // var CmoduleList = GetChecked("Module-C", "菜单");
    if (moduleList == "") {
        //alert("请选择考核年级");
        return false;
    }
    var CmoduleList = GetChecked("Module-C", "子菜单");
    if (CmoduleList == "") {
        //alert("请选择考核年级");
        return false;
    }
    var list = moduleList.concat(CmoduleList)
    add(list);

}
function add(moduleList) {
    var data = {
        "Role": parseInt(id),
        "IDList": moduleList,
    };
    var path = "/Menu/RoleMenuAdd";
    if (basepost(data, path)) {
        layer.alert('提交成功！', {
            time: 0 //不自动关闭
            , btn: ['确定']
            , title: "系统提示信息"
            , yes: function (index) {
                layer.close(index);
                window.location.href = "RoleList.html";
            }
        });
    }
}