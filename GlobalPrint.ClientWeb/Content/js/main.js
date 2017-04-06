window.onload = function() {
	$(".load").fadeOut(500);
}

$(function() {
	$(".slider").owlCarousel({
		items : 1,
		nav : false,
		navText : ["", ""],
		autoplay: true,
		autoplayTimeout: 10000
	});
	
	$( window ).scroll(function() {
		
		if($( window ).width() >= 1000) {
			if( $( this ).scrollTop() > 100 ) {
				$(".menu").addClass("minimized");
			}else{
				$(".menu").removeClass("minimized");
			}
		}
		
		
	})
});