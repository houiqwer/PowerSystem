
		var setting = {
		    check: {
		        enable: true,
		        chkboxType: {"Y":"", "N":""}
		    },
		    view: {
		        showIcon:false,
		        dblClickExpand: false
		    },
		    data: {
		        simpleData: {
		            enable: true
		        }
		    },
		    callback: {
		        //beforeClick: beforeClick,
		        onCheck: onCheck
		    }
		};

//var zNodes =[
//    {id:1, pId:0, name:"北京"},
//    {id:2, pId:0, name:"天津"},
//    {id:3, pId:0, name:"上海"},
//    {id:6, pId:0, name:"重庆"},
//    {id:4, pId:0, name:"河北省", open:true, nocheck:true},
//    {id:41, pId:4, name:"石家庄"},
//    {id:42, pId:4, name:"保定"},
//    {id:43, pId:4, name:"邯郸"},
//    {id:44, pId:4, name:"承德"},
//    {id:5, pId:0, name:"广东省", open:true, nocheck:true},
//    {id:51, pId:5, name:"广州"},
//    {id:52, pId:5, name:"深圳"},
//    {id:53, pId:5, name:"东莞"},
//    {id:54, pId:5, name:"佛山"},
//    {id:6, pId:0, name:"福建省", open:true, nocheck:true},
//    {id:61, pId:6, name:"福州"},
//    {id:62, pId:6, name:"厦门"},
//    {id:63, pId:6, name:"泉州"},
//    {id:64, pId:6, name:"三明"}
//];

//function beforeClick(treeId, treeNode) {
//    var zTree = $.fn.zTree.getZTreeObj("treeDemo");
//    zTree.checkNode(treeNode, !treeNode.checked, null, true);
//    return false;
//}
		
function onCheck(e, treeId, treeNode) {
    var zTree = $.fn.zTree.getZTreeObj("treeDemo"),
    nodes = zTree.getCheckedNodes(true),
    n= "";
    v = "";
    for (var i=0, l=nodes.length; i<l; i++) {
        n += nodes[i].name + ",";
        v += nodes[i].id + ",";
    }
    if (n.length > 0) n = n.substring(0, n.length - 1);
    if (v.length > 0 ) v = v.substring(0, v.length-1);
    var cityObj = $("#citySel");
    $("#depIDs").val(v);
    cityObj.attr("value", n);
    hideMenu();
}

function showMenu() {
    var cityObj = $("#citySel");
    var cityOffset = $("#citySel").offset();
    $("#menuContent").css({left:cityOffset.left + "px", top:cityOffset.top + cityObj.outerHeight() + "px"}).slideDown("fast");

    $("body").bind("mousedown", onBodyDown);
}
function hideMenu() {
    $("#menuContent").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}
function onBodyDown(event) {
    if (!(event.target.id == "menuBtn" || event.target.id == "citySel" || event.target.id == "menuContent" || $(event.target).parents("#menuContent").length>0)) {
        hideMenu();
    }
}

//$(document).ready(function(){
//    $.fn.zTree.init($("#treeDemo"), setting, zNodes);
//});
