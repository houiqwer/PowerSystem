layui.use('element', function() {
    var element = layui.element;
});
//表单启用
layui.use('form', function() {
    var form = layui.form; //只有执行了这一步，部分表单元素才会自动修饰成功
    form.verify({
        //数字或为空
        numberornull: function(value, item) { //value：表单的值、item：表单的DOM对象
            if (value != null && value != "") {
                var errmsg = item.attributes.errmsg.nodeValue;
                if (!new RegExp(/^[0-9]\d*$/).test(value)) {
                    return errmsg;
                }
            }
            //if (/(^\_)|(\__)|(\_+$)/.test(value)) {
            //    return '用户名首尾不能出现下划线\'_\'';
            //}
            //if (/^\d+\d+\d$/.test(value)) {
            //    return '用户名不能全为数字';
            //}
        },
        //小数
        number: function(value, item) { //value：表单的值、item：表单的DOM对象
            var errmsg = item.attributes.errmsg.nodeValue;
            var nullmsg = item.attributes.nullmsg == null ? "不能为空" : item.attributes.nullmsg.nodeValue;
            if (value == null || value == "") {
                return nullmsg;
            }
            ///\^[1-9]\\d{" + min+","+(max-1) + "\}$/************/^[0-9]\d*$/
            eval("var reg =/\^[+-]?([1-9][0-9]*|0)(\.[0-9]+)?%?$/;");
            if (!new RegExp(reg).test(value)) {
                return errmsg;
            }
        },
        //小数
        FloatNum: function(value, item) { //value：表单的值、item：表单的DOM对象
            var errmsg = item.attributes.errmsg.nodeValue;
            var nullmsg = item.attributes.nullmsg == null ? "不能为空" : item.attributes.nullmsg.nodeValue;
            if (value == null || value == "") {
                return nullmsg;
            }
            ///\^[1-9]\\d{" + min+","+(max-1) + "\}$/************/^[0-9]\d*$/
            eval("var reg =/\^([1-9][0-9]*|0)(\.[0-9]+)?%?$/;");
            if (!new RegExp(reg).test(value)) {
                return errmsg;
            }
        },
        //小数或者为空
        FloatNumorNull: function(value, item) { //value：表单的值、item：表单的DOM对象
            if (value != null && value != "") {
                var errmsg = item.attributes.errmsg.nodeValue;
                eval("var reg =/\^([1-9][0-9]*|0)(\.[0-9]+)?%?$/;");
                if (!new RegExp(reg).test(value)) {
                    return errmsg;
                }
            }

        },

        //数字>=0
        numbers: function(value, item) { //value：表单的值、item：表单的DOM对象
            var errmsg = item.attributes.errmsg.nodeValue;
            var nullmsg = item.attributes.nullmsg == null ? "不能为空" : item.attributes.nullmsg.nodeValue;
            if (value == null || value == "") {
                return nullmsg;
            }
            ///\^[1-9]\\d{" + min+","+(max-1) + "\}$/************/^[0-9]\d*$/
            eval("var reg = /\^[0-9]\\d{0,7\}$/;");
            if (!new RegExp(reg).test(value)) {
                return errmsg;
            }
        },
        //金额>0
        money: function(value, item) { //value：表单的值、item：表单的DOM对象
            var errmsg = item.attributes.errmsg.nodeValue;
            var nullmsg = item.attributes.nullmsg == null ? "不能为空" : item.attributes.nullmsg.nodeValue;
            if (value == null || value == "") {
                return nullmsg;
            }
            if (!new RegExp(/^([1-9][\d]{0,7}|0)(\.[\d]{0,8})?$/).test(value)) {
                return errmsg;
            }
        },
        //金额或为空
        moneyornull: function(value, item) { //value：表单的值、item：表单的DOM对象
            if (value != null && value != "") {
                var errmsg = item.attributes.errmsg.nodeValue;
                if (!new RegExp(/^([1-9][\d]{0,7}|0)(\.[\d]{0,8})?$/).test(value)) {
                    return errmsg;
                }
            }

        },

        //不能为空
        empty: function(value, item) { //value：表单的值、item：表单的DOM对象             
            var nullmsg = item.attributes.nullmsg == null ? "不能为空" : item.attributes.nullmsg.nodeValue;
            if (value == null || value == "") {
                return nullmsg;
            }
        },
        //下拉框
        ddl: function(value, item) {
            var errmsg = item.attributes.errmsg.nodeValue;
            if (value == -2 || value == "") {
                return errmsg;
            }
        },
        phone: function(value, item) {
            var errmsg = item.attributes.errmsg.nodeValue;
            var nullmsg = item.attributes.nullmsg == null ? "不能为空" : item.attributes.nullmsg.nodeValue;
            if (value != null && value != "") {
                ///\^[1-9]\\d{" + min+","+(max-1) + "\}$/************/^[0-9]\d*$/
                // eval("var reg = ;");
                var reg = /^1[3456789]\d{9}$/;
                if (!new RegExp(reg).test(value)) {
                    return errmsg;
                }
            } else {
                return nullmsg;
            }

        },
        tel: function(value, item) {
            var errmsg = item.attributes.errmsg.nodeValue;
            //var nullmsg = item.attributes.nullmsg == null ? "不能为空" : item.attributes.nullmsg.nodeValue;
            if (value != null && value != "") {
                ///\^[1-9]\\d{" + min+","+(max-1) + "\}$/************/^[0-9]\d*$/
                // eval("var reg = ;");
                var reg = /^(\(\d{3,4}\)|\d{3,4}-|\s)?\d{7,14}$/;
                if (!new RegExp(reg).test(value)) {
                    return errmsg;
                }
            }

        },
        //        $preg = '/(^(\d{3,4})-(\d{7,8})$)|(^(\d{7,8})$)|(^(\d{3,4})-(\d{7,8})-(\d{1,4})$)|(^(\d{7,8})-(\d{1,4})$)/';
        url: function(value, item) {
            var errmsg = item.attributes.errmsg.nodeValue;
            var nullmsg = item.attributes.nullmsg == null ? "不能为空" : item.attributes.nullmsg.nodeValue;
            var Expression = /http(s)?:\/\/([\w-]+\.)+[\w-]+(\/[\w- .\/?%&=]*)?/;
            if (value != null && value != "") {
                if (!new RegExp(Expression).test(value)) {
                    return errmsg;
                }
            } else {
                return nullmsg;
            }
            //if (value == -2 || value == "") {
            //    return errmsg;
            //}
        },
        mail: function(value, item) {
            var errmsg = item.attributes.errmsg.nodeValue;
            var nullmsg = item.attributes.nullmsg == null ? "不能为空" : item.attributes.nullmsg.nodeValue;
            var Expression = /^[A-Za-z0-9\u4e00-\u9fa5]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$/;
            if (value != null && value != "") {
                if (!new RegExp(Expression).test(value)) {
                    return errmsg;
                }
            } else {
                return nullmsg;
            }
            //if (value == -2 || value == "") {
            //    return errmsg;
            //}
        },
    });
})

//基础的登录验证
var store = {
    Api: "",
    userInfo: {
        isLogin: false,
        isKeeping: false,
        token: "",
    },
    expries: ""
};

$(document).ready(function() {

    updateStore();
    Head();
});

//转化日期格式
Date.prototype.Format = function(fmt) {
    var o = {
        "M+": this.getMonth() + 1,
        "d+": this.getDate(),
        "h+": this.getHours(),
        "m+": this.getMinutes(),
        "s+": this.getSeconds(),
        "q+": Math.floor((this.getMonth() + 3) / 3),
        "S": this.getMilliseconds()
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
};


var unity = {
    dealWithDate: function(date) {
        return date.replace("/Date(", "").replace(")/", "");
    },
    stringToDate: function(str) {
        return new Date(Date.parse(str.replace(/-/g, "/")));
    },
    getRequest: function() { //获取url中"?"符后的字串
        var url = location.search;
        var thisRequest = [];
        if (url.indexOf("?") != -1) {
            var str = url.substr(1);
            strs = str.split("&");
            for (var i = 0; i < strs.length; i++) {
                thisRequest[strs[i].split("=")[0]] = decodeURI(strs[i].split("=")[1]);
            }
        }
        return thisRequest;
    },
    getURL: function(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]);
        return null;
    },
    cutFile: function(file, cutSize) {
        var count = file.size / cutSize | 0,
            fileArr = [];
        for (var i = 0; i < count; i++) {
            fileArr.push({
                name: file.name + ".part" + (i + 1),
                file: file.slice(cutSize * i, cutSize * (i + 1))
            });
        };
        fileArr.push({
            name: file.name + ".part" + (count + 1),
            file: file.slice(cutSize * count, file.size)
        });
        return fileArr;
    },
    photoCompress: function(file, w, objDiv) {
        var ready = new FileReader();
        /*开始读取指定的Blob对象或File对象中的内容. 当读取操作完成时,readyState属性的值会成为DONE,如果设置了onloadend事件处理程序,则调用之.同时,result属性中将包含一个data: URL格式的字符串以表示所读取文件的内容.*/
        ready.readAsDataURL(file);
        ready.onload = function() {
            var re = this.result;
            unity.canvasDataURL(re, w, objDiv)
        }
    },
    canvasDataURL: function(path, obj, callback) {
        var img = new Image();
        img.src = path;
        img.onload = function() {
            var that = this;
            // 默认按比例压缩
            var w = that.width,
                h = that.height,
                scale = w / h;
            w = obj.width || w;
            h = obj.height || (w / scale);
            var quality = 0.7; // 默认图片质量为0.7
            //生成canvas
            var canvas = document.createElement('canvas');
            var ctx = canvas.getContext('2d');
            // 创建属性节点
            var anw = document.createAttribute("width");
            anw.nodeValue = w;
            var anh = document.createAttribute("height");
            anh.nodeValue = h;
            canvas.setAttributeNode(anw);
            canvas.setAttributeNode(anh);
            ctx.drawImage(that, 0, 0, w, h);
            // 图像质量
            if (obj.quality && obj.quality <= 1 && obj.quality > 0) {
                quality = obj.quality;
            }
            // quality值越小，所绘制出的图像越模糊
            var base64 = canvas.toDataURL('image/png', quality);
            // 回调函数返回base64的值
            callback(base64);
        }
    },
    fliter: function(func, arr) {
        var r = [];
        for (var i = 0; i < arr.length; i++) {
            if (func(arr[i], i, arr)) {
                r.push(arr[i]);
            }
        }
        return r;
    },
    fliterdata: function(func, arr) {
        for (var i = 0; i < arr.length; i++) {
            if (func(arr[i], i, arr)) {
                return arr[i];
            }
        }
        return null;
    },
    flitercount: function(func, arr) {
        for (var i = 0; i < arr.length; i++) {
            if (func(arr[i], i, arr)) {
                return i;
            }
        }
        return -1;
    },
    html_encode: function(str) {
        var s = "";
        if (str.length == 0) return "";
        s = str.replace(/&/g, "&gt;");
        s = s.replace(/</g, "&lt;");
        s = s.replace(/>/g, "&gt;");
        s = s.replace(/ /g, "&nbsp;");
        s = s.replace(/\'/g, "&#39;");
        s = s.replace(/\"/g, "&quot;");
        s = s.replace(/\n/g, "<br>");
        return s;
    },
    checkNum: function(value) {
        var patrn = /^(-)?\d+(\.\d+)?$/;
        if (patrn.exec(value) == null || value == "") {
            return false
        } else {
            return true
        }
    },
    InitNode: function(nodeName, nodeClass) {
        var node = document.createElement(nodeName);
        if (nodeClass != "" && nodeClass != null)
            node.className = nodeClass;
        return node;
    },
    objectEqual: function(a, b) {
        var propsA = Object.getOwnPropertyNames(a),
            propsB = Object.getOwnPropertyNames(b);
        if (propsA.length != propsB.length) {
            return false;
        }
        for (var i = 0; i < propsA.length; i++) {
            var propName = propsA[i];
            //如果对应属性对应值不相等，则返回false

            if (a[propName] !== b[propName]) {
                //console.log(propName + ":" + a[propName] + "--" + b[propName]);
                return false;
            }
        }
        return true;
    },

    filterHTMLTag: function(str) {
        str = str.replace(/<\/?[^>]*>/g, ''); //去除HTML tag
        str = str.replace(/[ | ]*\n/g, '\n'); //去除行尾空白
        str = str.replace(/ /ig, ''); //去掉 
        return str;
    },
    imgUrlFun: function(str) {
        var data = '';
        str.replace(/<img [^>]*src=['"]([^'"]+)[^>]*>/, function(match, capture) {
            data = capture;
        });
        return data
    }
};

function updateStore() {
    store.userInfo.headerUrl = localStorage.getItem("HeaderUrl");
    store.expries = localStorage.getItem("Expire"); //token 过期时间
    store.userInfo.token = localStorage.getItem("Token");

    if (localStorage.getItem("IS_KEEPING") === "true") {
        store.userInfo.isKeeping = true;
    }
    //判断token是否错误
    if (store.userInfo.token !== "" && store.userInfo.token !== null) {
        //if (getvalidateticket()) {
        //    store.userInfo.isLogin = true;
        //} else {
        //    localStorage.clear();
        //    store.userInfo.isLogin = false;
        //    //window.location.href = "/Index.html";
        //    layer.confirm('请重新登录', { title: "系统提示信息" },
        //        //{
        //        //    btn: ['是']
        //        //},
        //        function () {
        //            window.location.href = "/Login.html";
        //        });
        //}
    } else {
        //localStorage.clear();
        store.userInfo.isLogin = false;
        //window.location.href = "/Index.html";
    }
}
//验证token
function getvalidateticket() {
    var result = false;
    $.ajax({
        url: "/account/getvalidateticket",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify({
            "token": store.userInfo.token,
            "realname": localStorage.getItem("USER_NAME"),
        }),
        dataType: "json",
        async: false,
        success: function(data) {
            if (data.code == 0) {
                localStorage.setItem("Role", data.data.Role);
                result = true;
            } else {
                result = false;
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("当前网络可能有错误", {
                    title: false
                });
            });
        }
    });
    return result;
}

function ResetUserLogStatus(data) {
    store.userInfo.isLogin = true;
    store.userInfo.token = data.data.Token;
    store.expries = unity.dealWithDate(data.data.Expire);

    
    localStorage.setItem("RealName", data.data.Realname);
    localStorage.setItem("UserName", data.data.Username);
    //localStorage.setItem("ProfilePicture", data.data.ProfilePicture);
    localStorage.setItem("Expire", data.data.Expire); //token过期时间
    localStorage.setItem("Token", data.data.Token);
    localStorage.setItem("UserID", data.data.UserID);

    // init.verificationToken();
};

function NewExtendToken() {
    $.ajax({
        url: "/Account/ExtendToken",
        type: "POST",
        data: {
            "Token": localStorage.getItem("Token")
        },
        dataType: "json",
        success: function(data) {
            if (data.code != 0) {
                alert(data.msg);
                localStorage.removeItem("Token");
                window.location.href = "../../Index.html";
            }
        },
        error: function() {
            alert("当前网络可能有错误");
            window.location.href = "../../Index.html";
        }
    });
}


function ExtendToken() {
    layer.alert("正在登陆，请稍后……");
    $.ajax({
        url: "/Account/ExtendToken",
        type: "POST",
        data: {
            "Token": localStorage.getItem("Token")
        },
        dataType: "json",
        success: function(data) {
            if (data.code != 0) {
                alert(data.msg);
                localStorage.removeItem("Token");
                location.reload();
                window.location.href = "../../Login.html";
            } else {

                window.location.href = "../../index.html";
            }
        },
        error: function() {
            alert("当前网络可能有错误");
            location.reload();
            window.location.href = "../../Login.html";
        }
    });
}

function BuildMenu() {
    $.ajax({
        url: "/Menu/FullList",
        type: "GET",
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        success: function(data) {
            if (data.code == 0) {
                var html = "";
                var url = window.location.pathname;

                for (var i = 0; i < data.data.length; i++) {
                    var menuHtml = "";
                    var isSelected = false;

                    if (data.data[i].ChidernMenuList != null) {
                        menuHtml = menuHtml + "<dl class=\"layui-nav-child\">";
                        for (var j = 0; j < data.data[i].ChidernMenuList.length; j++) {
                            if (data.data[i].ChidernMenuList[j].Path == url) {
                                isSelected = true;
                                menuHtml = menuHtml + "<dd><a href=\"" + data.data[i].ChidernMenuList[j].Path + "\" class=\"layui-this\">" + data.data[i].ChidernMenuList[j].Name + "</a></dd>";

                                if ($("#topMenuName") != null && $("#menuName") != null) {
                                    $("#topMenuName").html(data.data[i].Name);
                                    $("#menuName").html(data.data[i].ChidernMenuList[j].Name);
                                }
                            } else {
                                menuHtml = menuHtml + "<dd><a href=\"" + data.data[i].ChidernMenuList[j].Path + "\">" + data.data[i].ChidernMenuList[j].Name + "</a></dd>";
                            }

                        }
                        menuHtml = menuHtml + "</dl></li>";
                    }

                    var selectedHtml = "";

                    if (url == data.data[i].Path) {
                        selectedHtml = "layui-this";
                    }

                    if (isSelected) {

                        menuHtml = "<li class=\"layui-nav-item layui-nav-itemed " + selectedHtml + "\">" +
                            "<a href=\"javascript:\">" + data.data[i].Name + "</a>" +
                            menuHtml;

                    } else {
                        menuHtml = "<li class=\"layui-nav-item " + selectedHtml + "\">" +
                            "<a href=\"javascript:\">" + data.data[i].Name + "</a>" +
                            menuHtml;
                    }

                    html = html + menuHtml;
                }

                $("#leftMenu").html(html);

                layui.use('element', function() {
                    var element = layui.element;
                    element.init();
                });

            } else {
                alert(data.data);
            }
        },
        error: function() {
            alert("数据提交存在问题，请检查当前网络");
        }
    });
}


//公用post方法
function basepost(data, path) {
    var result = false;
    $.ajax({
        url: path,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(data),
        async: false,
        dataType: "json",
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        success: function(data) {
            if (data.code == 0) {
                //layer.alert('提交成功', function () {
                //    result = true;
                //});
                result = true;
            } else {
                Failure(data);
                //layer.ready(function () {
                //    title: false
                //    layer.alert(data.Data, {
                //        title: false
                //    });
                //});
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    icon: 5,
                    title: false,

                });
            });
        }
    });

    return result;
}


//文件上传用
function basefilepost(data, path) {
    var result = false;
    $.ajax({
        url: path,
        type: "POST",
        data: data,
        async: false,
        dataType: "json",
        cache: false,
        processData: false,
        contentType: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", localStorage.getItem("Token"));
            layer.load();
            //alter("正在执行，请稍候");
        },
        success: function(data) {
            layer.closeAll('loading');
            if (data.code == 0) {
                result = true;
            } else {
                Failure(data);

                //layer.ready(function () {
                //    title: false
                //    layer.alert(data.Data, {
                //        title: false
                //    });
                //});
            }
        },
        error: function() {
            layer.closeAll('loading');
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });

        }
    });

    return result;
}



function Failure(data) {
    if (data.data != null && data.data.length > 0 && data.data.indexOf("请重新登录") > 0) {
        layer.closeAll();
        parent.layer.closeAll();
        layer.alert(data.msg, {
            time: 0, //不自动关闭
            btn: ['确定'],
            title: "系统提示信息",
            yes: function(index) {
                layer.close(index);
                location.reload();
                window.location.href = "../../Login.html";
            }
        });
    } else {
        layer.ready(function() {
            title: false
            layer.alert(data.msg, {
                title: false
            });
        });


    }

}

function GetLayui() {
    layui.use('element', function() {
        var element = layui.element;
        element.init();
    });
    layui.use('form', function() {
        var form = layui.form; //只有执行了这一步，部分表单元素才会自动修饰成功
        form.render();
    })
}

function GetChecked(type, name) {
    var result = new Array();
    $("input[id^='" + type + "']").each(function() {

        if ($(this).get(0).checked) {
            result.push(parseInt($(this).attr("id").replace(type + "-", "")));
        }
    });
    if (result.length === 0) {
        alert("请选择" + name);
        return "";
    }

    return result;
}


//function InitEnum(type, firstresult, value) {
//    var returndata = null;
//    var data = new Array();

//    data = {
//        "type": type,
//        "firstresult": firstresult,
//        "value": value,
//    };

//    $.ajax({
//        url: "/Base/getEnum",
//        type: "GET",
//        data: data,
//        async: false,
//        dataType: "json",
//        beforeSend: function (XHR) {
//            XHR.setRequestHeader("Authorization", store.userInfo.token);
//        },
//        success: function (data) {
//            if (data.code == 0) {
//                returndata = data.data;
//            } else {
//                Failure(data);
//                //layer.ready(function () {
//                //    title: false
//                //    layer.alert(data.Data, {
//                //        title: false
//                //    });
//                //});
//            }
//        },
//        error: function () {
//            layer.ready(function () {
//                title: false
//                layer.alert("数据提交存在问题，请检查当前网络", {
//                    title: false
//                });
//            });
//        }
//    });

//    return returndata;
//}

function InitBaseDate(typeName, document) {
    var data = {
        typeName: typeName
    }
    $.ajax({
        url: "/SysData/GetListByType",
        //headers: { Authorization: store.userInfo.token },
        type: "get",
        data: data,
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function(data) {
            if (data.code == 0) {
                var html = '<option value="-2">请选择' + typeName + '</option>';
                for (var i = 0; i < data.data.length; i++) {
                    html += "<option value=\"" + data.data[i].ID + "\">" + data.data[i].DataName + "</option>";
                }
                $("#" + document).html(html);
            } else {
                Failure(data);
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    })
}

function GetPName(name) {
    var arr = localStorage.getItem("MName").split('>');
    if (arr.length == 3) {
        $("#head").html(localStorage.getItem("MName") + ">" + name);
        localStorage.setItem("MName", $("#head").text());
    }
    if (arr.length == 4) {
        $("#head").html(arr[0] + ">" + arr[1] + ">" + arr[2] + ">" + name);
        localStorage.setItem("MName", $("#head").text());
    }
}

function Head() {
    // var title = "";
    var MName = localStorage.getItem("MName");

    var id = unity.getURL("id");
    //alert(window.location.pathname);
    var path = window.location.pathname;
    if (path.indexOf("StartTransactionAdd") > 0) {
        MName += ">发起事务";
    } else if (path.indexOf("CustomizeFlowAuditEdit") > 0) {
        MName += ">流程设置";
    } else if (path.indexOf("RoleMenuAdd") > 0) {
        MName += ">权限配置";
    } else if (path.indexOf("Document_DownAdd") > 0) {
        MName += ">下发";
    } else if (path.indexOf("AddressBookList") > 0) {
        MName += "";
    } else if (path.indexOf("Edit") > 0) {
        if (id != null && id != "") {
            MName += ">编辑";
        } else {
            MName += ">添加";
        }

    } else if (path.indexOf("Detail") > 0) {
        MName += ">详情";
    } else if (path.indexOf("Select") > 0) {
        MName += ">查阅";
    } else if (path.indexOf("Sign") > 0) {
        MName += ">签到";
    } else if (path.includes("RoleMenu")) {
        MName += ">菜单配置";
    } else if (path.indexOf("Add") > 0) {
        if (id != null && id != "") {
            MName += ">编辑";
        } else {
            MName += ">添加";
        }
    } else if (path.indexOf("Import") > 0) {
        MName += ">导入";
    } else if (path.indexOf("TeacherUpload") > 0) {
        MName += ">档案上传";
    } else if (path.indexOf("Scan") > 0) {
        MName += ">扫描";
    }




    var head = "";
    if (MName != null) {
        var arr = MName.split(',');
        for (var i = 0; i < arr.length; i++) {
            if (i == arr.length - 1) {
                head += '<a class="active">' + arr[i] + '</a>';
            } else {
                head += '<a>' + arr[i] + '</a>';
            }
        }
    }

    $("#head").html(head);

}

function DepTreeList(ele, radio, clickClose, tips) {
    clickClose = clickClose || true;
    if (tips == null || tips == "") {
        tips = '请选择部门';
    } else {
        tips = '请选择' + tips;
    }
    $.ajax({
        url: "/Department/List",
        type: "GET",
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        success: function(data) {
            if (data.code == 0) {
                DepTree = xmSelect.render({
                    el: '#' + ele,
                    tips: tips,
                    autoRow: true,
                    filterable: true,
                    tree: {
                        show: true,
                        showFolderIcon: true,
                        showLine: true,
                        indent: 20,
                        strict: false,

                    },
                    clickClose: clickClose,
                    radio: radio,
                    filterable: true,
                    height: 'auto',
                    data: data.data.ChildList,
                    theme: {
                        color: '#6688FD',
                    }

                })
            } else {
                Failure(data);
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    });
}

function AreaTreeList(ele, radio, clickClose, tips) {
    clickClose = clickClose || true;
    if (tips == null || tips == "") {
        tips = '请选择区域';
    } else {
        tips = '请选择' + tips;
    }
    $.ajax({
        url: "/Area/List",
        type: "GET",
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        success: function(data) {
            if (data.code == 0) {
                AreaTree = xmSelect.render({
                    el: '#' + ele,
                    tips: tips,
                    autoRow: true,
                    filterable: true,
                    tree: {
                        show: true,
                        showFolderIcon: true,
                        showLine: true,
                        indent: 20,
                        strict: false,

                    },
                    clickClose: clickClose,
                    radio: radio,
                    filterable: true,
                    height: 'auto',
                    data: data.data.ChildList,
                    theme: {
                        color: '#6688FD',
                    }

                })
            } else {
                Failure(data);
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    });
}

function GetCompanyName(element) {

    $.ajax({
        url: "/Company/GetCompanyList",
        type: "get",
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function(data) {
            if (data.code == 0) {
                var html = '<option value=\"\">请选择所属单位</option>';
                for (var i = 0; i < data.data.length; i++) {
                    html += "<option value=\"" + data.data[i].ID + "\">" + data.data[i].Name + "</option>";
                }
                $("#" + element).html(html);
                if (localStorage.getItem("RoleName") === "企业管理员") {
                    $("#" + element).val(localStorage.getItem("CompanyID"));
                    $("#" + element).attr('disabled', true);

                }

            } else {
                Failure(data);
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    })
}

function CityTreeList(ele) {

    $.ajax({
        url: '../CityCode/CityCode.json',
        type: "GET",
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        success: function(data) {
            console.log(data);

            CityTree = xmSelect.render({
                el: '#' + ele,
                tips: "请选择所属区域",
                autoRow: true,

                cascader: {
                    show: true,
                    indent: 200,
                    strict: true,
                },
                //clickClose: true,
                //radio: radio,
                //filterable: true,
                height: 'auto',

                data: data,
                theme: {
                    color: '#6688FD',
                }


            })

        }
    });

}

function BuildMultTree(data) {
    var html = "";
    for (var i = 0; i < data.length; i++) {
        if (data[i].children == null || data[i].children.length == 0) {
            html += "<li><a href='javascript:;' name='child' id='" + data[i].ID + "'>" + data[i].title + "</a></li>";

        } else {

            html += "<li class='folder-root open '><a href='javascript:;' id='" + data[i].ID + "'>" + data[i].title + "</a><ul>" + BuildMultTree(data[i].children) + "</ul></li>";
        }
    }
    return html;
}

function GetCityName(codeArray, func) {
    var url = "/CityCode/CityCode.json"

    var request = new XMLHttpRequest();

    request.open("get", url);

    request.send(null);

    request.onload = function() {

        if (request.status == 200) {
            var provinceArray = JSON.parse(request.responseText);
            var nameArray = [];
            for (var j = 0; j < codeArray.length; j++) {
                for (var i = 0; i < provinceArray.length; i++) {
                    var city = unity.fliterdata(function(f) { return codeArray[j] == f.value }, provinceArray[i].children);
                    if (city != null) {
                        nameArray.push(city.name);
                        break;
                    }
                }
            }

            func(nameArray);

            //var cityArray = unity.fliter(function (e) { return unity.fliterdata(function (f) { return codeArray.indexOf(f.value) > -1 }, e.children) }, json);
            //if (cityArray != null) {
            //    var nameArray = [];
            //    for (var i = 0; i < codeArray.length; i++) {
            //        var province = unity.fliterdata(function (e) { return unity.fliterdata(function (f) { return codeArray[i] == f.value }, e.children) }, cityArray);
            //        nameArray.push(unity.fliterdata(function (f) { return f.value == codeArray[i] }, province.children).name);
            //    }
            //    func(nameArray);
            //} else {
            //    func(null);
            //}
        }

    }
}

function baseExel(base64) {
    return b64toBlob(getData(base64), getContentType(base64));
}


function getContentType(base64) {
    return /data:([^;]*);/i.exec(base64)[1];
}

function getData(base64) {
    return base64.substr(base64.indexOf("base64,") + 7, base64.length);
}

function b64toBlob(b64Data, contentType, sliceSize) {
    contentType = contentType || '';
    sliceSize = sliceSize || 512;
    var byteCharacters = atob(b64Data);
    var byteArrays = [];
    for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
        var slice = byteCharacters.slice(offset, offset + sliceSize);
        var byteNumbers = new Array(slice.length);
        for (var i = 0; i < slice.length; i++) {
            byteNumbers[i] = slice.charCodeAt(i);
        }
        var byteArray = new Uint8Array(byteNumbers);
        byteArrays.push(byteArray);
    }
    var blob = new Blob(byteArrays, { type: contentType });
    return blob;
}

function checkUserId(value, index, ar) {
    if (value == localStorage.getItem("UserID")) {
        return true;
    } else {
        return false;
    }
}

function InitSchool(control, isDefault = false) {

    $.ajax({
        url: "/School/List",
        type: "get",
        data: {
            page: 1,
            limit: 99
        },
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function(data) {
            if (data.code == 0) {
                var html = "";
                if (isDefault) {
                    html = html + "<option value=\"-2\">-- 请选择学校 --</option>";
                }
                for (var i = 0; i < data.data.length; i++) {
                    html = html + "<option value='" + data.data[i].value + "'>" + data.data[i].name + "</option>";
                }
                control.html(html);
            } else {
                Failure(data);
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    })
}

function InitOtherSchool(control) {

    $.ajax({
        url: "/School/List",
        type: "get",
        data: {
            page: 1,
            limit: 99
        },
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function(data) {
            if (data.code == 0) {
                var html = "";
                for (var i = 0; i < data.data.length; i++) {
                    html = html + "<option value='" + data.data[i].GID + "'>" + data.data[i].name + "</option>";
                }
                control.html(html);
            } else {
                Failure(data);
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    })
}

function InitBoardroom(control, isDefault = false) {

    $.ajax({
        url: "/Boardroom/List",
        type: "get",
        data: {
            name: "",
            page: 1,
            limit: 999
        },
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function(data) {
            if (data.code == 0) {
                var html = "";
                if (isDefault) {
                    html = html + "<option value=\"-2\">-- 请选择会议室 --</option>";
                }
                for (var i = 0; i < data.data.length; i++) {
                    html = html + "<option value='" + data.data[i].ID + "'>" + data.data[i].Name + "</option>";
                }
                control.html(html);
            } else {
                Failure(data);
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    })
}

function InitDaySet(control, isDefault = false) {

    $.ajax({
        url: "/DaySet/List",
        type: "get",
        data: {
            name: "",
            page: 1,
            limit: 999
        },
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function(data) {
            if (data.code == 0) {
                var html = "";
                if (isDefault) {
                    html = html + "<option value=\"-2\">-- 请选择流程天数配置--</option>";
                }
                for (var i = 0; i < data.data.length; i++) {
                    html = html + "<option value='" + data.data[i].ID + "'>" + data.data[i].Name + "</option>";
                }
                control.html(html);
            } else {
                Failure(data);
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    })
}

function InitEnum(type, control, isDefault = false,value) {
    $.ajax({
        url: "/Base/GetEnum",
        type: "get",
        data: {
            type: type
        },
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function(data) {
            if (data.code == 0) {
                var html = "";
                if (isDefault) {
                    html = html + "<option value=\"\">-- "+ value + " --</option>";
                }
                for (var i = 0; i < data.data.List.length; i++) {
                    html = html + "<option value='" + data.data.List[i].EnumValue + "'>" + data.data.List[i].EnumName + "</option>";
                }
                control.html(html);
            } else {
                Failure(data);
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    })
}

function InitNotes(control, isDefault = false) {
    $.ajax({
        url: "/DocumentNote/List",
        type: "get",
        data: {
            content: "",
            page: 1,
            limit: 999
        },
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function(data) {
            if (data.code == 0) {
                var html = "";
                if (isDefault) {
                    html = html + "<option value=\"\">-- 请选择批注 --</option>";
                }
                for (var i = 0; i < data.data.length; i++) {
                    html = html + "<option value='" + data.data[i].Content + "'>" + data.data[i].Content + "</option>";
                }
                control.html(html);
            } else {
                Failure(data);
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    })
}

function InitUser_Type(typeName, control, isDefault = false) {
    $.ajax({
        url: "/SysUser_Type/GetListByType",
        type: "get",
        data: {
            typeName: typeName
        },
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function(data) {
            if (data.code == 0) {
                var html = "";
                if (isDefault) {
                    html = html + "<option value=\"-2\">-- 请选择 --</option>";
                }
                for (var i = 0; i < data.data.length; i++) {
                    html = html + "<option value='" + data.data[i].ID + "'>" + data.data[i].realName + "</option>";
                }
                control.html(html);
            } else {
                Failure(data);
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    })
}

function InitSysDate(typeName, control, isDefault = false) {
    $.ajax({
        url: "/SysData/GetListByType",
        type: "get",
        data: {
            typeName: typeName
        },
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function(data) {
            if (data.code == 0) {
                var html = "";
                if (isDefault) {
                    html = html + "<option value=\"-2\">-- 请选择 --</option>";
                }
                for (var i = 0; i < data.data.length; i++) {
                    html = html + "<option value='" + data.data[i].ID + "'>" + data.data[i].DataName + "</option>";
                }
                control.html(html);
            } else {
                Failure(data);
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    })
}

function getClass(tagName, parentClassName, className) {

    var tags = document.getElementsByTagName(tagName); //获取标签   
    var tagArr = []; //用于返回类名为className的元素
    for (var i = 0; i < tags.length; i++) {
        if (tags[i].className.indexOf(className) >= 0 && tags[i].parentElement.className == parentClassName) {

            tagArr[tagArr.length] = tags[i]; //保存满足条件的元素
        }
    }
    return tagArr;

}

function InitNode(nodeName, nodeClass) {
    var node = document.createElement(nodeName);
    if (nodeClass != "" && nodeClass != null)
        node.className = nodeClass;
    return node;
}


function GetMyUserType(typeName) {
    var result = false;
    $.ajax({
        url: "/SysUser_Type/GetMyUserType",
        type: "get",
        data: {
            typeName: typeName
        },
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function(data) {
            if (data.code == 0) {
                result = data.data;
            } else {
                Failure(data);
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    })

    return result;
}

function InitAdministrationNumber(control, data) {

    $.ajax({
        url: "/Document_Down/GetMaxAdministrationNumber",
        type: "get",
        data: {
            administrationTypeID: data
        },
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function(data) {
            if (data.code == 0) {

                control.val(data.data);
                GetLayui();
            } else {
                Failure(data);
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    })
}

function InitDocumentNumber(control, data) {

    $.ajax({
        url: "/Document_Down/GetMaxDocumentNumber",
        type: "get",
        data: {
            documentTypeID: data
        },
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function(data) {
            if (data.code == 0) {

                control.val(data.data);
                GetLayui();
            } else {
                Failure(data);
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    })
}

function InitAdministrationType(control, isDefault = false) {

    $.ajax({
        url: "/AdministrationType/List",
        type: "get",
        data: {
            page: 1,
            limit: 99
        },
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function(data) {
            if (data.code == 0) {
                var html = "";
                if (isDefault) {
                    html = html + "<option value=\"-2\">-- 请选择政务类型 --</option>";
                }
                for (var i = 0; i < data.data.length; i++) {
                    html = html + "<option value='" + data.data[i].ID + "'>" + data.data[i].TypeName + "</option>";
                }
                control.html(html);
                GetLayui();
            } else {
                Failure(data);
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    })
}

function InitDocumentType(control, isDefault = false) {

    $.ajax({
        url: "/DocumentType/List",
        type: "get",
        data: {
            page: 1,
            limit: 99
        },
        dataType: "json",
        async: false,
        beforeSend: function(XHR) {
            XHR.setRequestHeader("Authorization", store.userInfo.token);
        },
        success: function(data) {
            if (data.code == 0) {
                var html = "";
                if (isDefault) {
                    html = html + "<option value=\"-2\">-- 请选择文号类型 --</option>";
                }
                for (var i = 0; i < data.data.length; i++) {
                    html = html + "<option value='" + data.data[i].ID + "'>" + data.data[i].TypeName + "</option>";
                }
                control.html(html);
                GetLayui();
            } else {
                Failure(data);
            }
        },
        error: function() {
            layer.ready(function() {
                title: false
                layer.alert("数据提交存在问题，请检查当前网络", {
                    title: false
                });
            });
        }
    })
}
var check = function(e) {

    e = e || window.event; //alert(e.which||e.keyCode);
    if ((e.which || e.keyCode) == 116) {
        if (e.preventDefault) {
            e.preventDefault();
            window.location.reload();
        } else {
            event.keyCode = 0;
            e.returnValue = false;
            window.location.reload();
        }
    }
}

if (document.addEventListener) {
    document.addEventListener("keydown", check, false);
} else {
    document.attachEvent("onkeydown", check);
}



function ReduceAlarm() {

    $("a", window.parent.document).each(function(e) {
        if ($(this).attr("onclick") != null && $(this).attr("onclick").indexOf(window.location.pathname) > 0) {
            var alarmCountDiv = $(this).children(".layui-badge")[0];
            var alarmCount = $(alarmCountDiv).html();
            $(alarmCountDiv).html(parseInt(alarmCount - 1));
        }
    });

}

function Preview(url) {
    var w = ($(window).width() * 0.7);
    var h = ($(window).height());
    var a = url.substring(url.lastIndexOf('.'));
    url = url.replace(/\%/g, "%25");
    url = url.replace(/\#/g, "%23");
    url = url.replace(/\&/g, "%26");
    console.log(a);
    if (a == "") {
        layer.alert("暂不支持在线预览");
        return false;
    } else {
        //alert(decodeURI(id));
        layer.open({
            type: 2,
            title: "在线预览",
            area: [w + 'px', h + 'px'],
            shade: 0.4,
            maxmin: true,
            content: '/ReadOnLine.html?id=' + url,
        })
    }
}
function Download(id) {

    var par = "?ID=" + id;
    var path = "/Accessory/Download" + par;

    var xhr = new XMLHttpRequest();

    xhr.open('GET', path, true);        // 也可以使用POST方式，根据接口
    xhr.setRequestHeader("Authorization", localStorage.getItem("Token"));
    xhr.responseType = "blob";    // 返回类型blob
    layer.load();
    // 定义请求完成的处理函数，请求前也可以增加加载框/禁用下载按钮逻辑
    xhr.onload = function (e) {
        // 请求完成
        if (this.status === 200) {
            // 返回200
            console.log(this.getResponseHeader('Content-Disposition'));
            var blob = this.response;
            var reader = new FileReader();
            reader.readAsDataURL(blob);    // 转换为base64，可以直接放入a表情href
            reader.onload = function (e) {
                // 转换完成，创建一个a标签用于下载
                var a = document.createElement('a');
                var wpoInfo = { "Disposition": xhr.getResponseHeader('Content-Disposition'), };
                var name = "";
                var text = this
                var d = wpoInfo.Disposition;
                layer.closeAll();
                if (d != null) {
                    if (wpoInfo.Disposition.indexOf("filename=")) {
                        name = wpoInfo.Disposition.split("filename=")[1];
                        name = decodeURIComponent(name);
                    }
                    a.download = name;
                    if (e.target.result == "data:") {
                        a.href = "data:application/octet-stream;base64,"
                    } else {
                        var blobExel = baseExel(e.target.result); //把base64位文件转化为exel文件
                        a.setAttribute('href', URL.createObjectURL(blobExel));
                    }
                    $("body").append(a);    // 修复firefox中无法触发click
                    a.click();
                    $(a).remove();
                    layer.closeAll();


                }
                else {
                    layer.alert('下载失败', { icon: 5, title: "系统提示信息" });
                }
            }
        }
    };
    // 发送ajax请求
    xhr.send();
    //layer.closeAll();
}

function imagesSrcFile(type) {
    switch (type) {
        case "wjj":
            return "/images/file_icon/ic_folder.png"
        case "xls":
            return "/images/file_icon/ic_excel.png"
        case "xlsx":
            return "/images/file_icon/ic_excel.png"
        case "pdf":
            return "/images/file_icon/ic_pdf.png"
        case "rar":
            return "/images/file_icon/ic_rar.png"
        case "zip":
            return "/images/file_icon/ic_rar.png"
        case "doc":
            return "/images/file_icon/ic_word.png"
        case "docx":
            return "/images/file_icon/ic_word.png"
        case "png":
            return "/images/file_icon/ic_photo.png"
        case "gif":
            return "/images/file_icon/ic_photo.png"
        case "jpg":
            return "/images/file_icon/ic_photo.png"
        case "jpeg":
            return "/images/file_icon/ic_photo.png"
        case "ppt":
            return "/images/file_icon/ic_ppt.png"
        case "txt":
            return "/images/file_icon/ic_txt.png"

        // 未知文件名
        default:
            return "/images/file_icon/ic_unknown.png"
    }
}