$(document).ready(function() {
	$('.chooseall').click(function(){
		$(".layui-table input").prop("checked", true);
	})
	$('.antichoose').click(function(){
		$(".layui-table input").each(function (){
			if ($(this).prop("checked") == true) {
				$(this).prop("checked", false);
			} else {
				$(this).prop("checked", true);
			}
		});	
	})
	$('.nochoose').click(function(){
		$(".layui-table input").prop("checked", false);
	})
	
	/*侧边栏收回*/
	$('.whid').click(function(){
		if ($(".layui-side").hasClass('whidact')) { //伸
			$('.whid i').removeClass('layui-icon-spread-left');
			$(".layui-side").removeClass('whidact');
			$(".layui-body").removeClass('whidact2');
		}
		else{//缩
			$('.whid i').addClass('layui-icon-spread-left');
			$(".layui-nav-item").removeClass('layui-nav-itemed');
			$(".layui-side").addClass('whidact');
		    $(".layui-body").addClass('whidact2');//leftFrame			
		}
	})
	$('.layui-layout').on('click', '.whidact .layui-nav-item', function(){//伸
			$('.whid i').removeClass('layui-icon-spread-left');
			$(".layui-side").removeClass('whidact');
			$(".layui-body").removeClass('whidact2');


	})
	
	
	
  var headhei = $('.layui-card-header').height() == undefined ? 0 : $('.layui-card-header').height();
   var hei1 = $('.safe-card1').height()-76;
	var hei2 = $('.safe-card1').height()-headhei;
  $(".safe-item").height(hei1) ;
	$(".safe-item1").height(hei2) ;
   	
			
	
		

	
})