function info(file) {
	
	// !!! נאסרטנוםטÿ פאיכמג - php !!!
	
	dir = "./pages/info/";
	file = dir + file + ".php";
	
	$.post(file, function(e) {
		if($(".infoBox").hasClass("active")) {
			$(".infoBox").removeClass("active");
			
			setTimeout(function() {
				$(".infoBox .insertTo").html(e);
				$(".infoBox").addClass("active");
			}, 200);
		}else{
			$(".infoBox .insertTo").html(e);
			$(".infoBox").addClass("active");
		}
	});
}

$(function() {	
	$(".closeAb").click(function() {
		$(".infoBox").removeClass("active");
	});
});

$(function($){
	$(document).mouseup(function (e){
		var div = $(".infoBox");
		var mark = $(".marker");
		if (!div.is(e.target)
		    && div.has(e.target).length === 0
			&& !mark.is(e.target)
			) {
			div.removeClass("active");
		}
	});
});