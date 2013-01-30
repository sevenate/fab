
/* Orbit Slider */
$(window).load(function() {

	$("#featured").orbit({
			animation: 'fade',
			animationSpeed: 800,
			timer: false,
			bullets: false,
			pauseOnHover: true,
			pauseTime: 1000,
			fluid: true
		});

	var makeButtonActive = function() {
		$("#header ul li a").each(function(index) {
			$(this).removeClass('current');
		});

		$(this).addClass('current');
	};

	//Attach events to menu
	$(document).ready(function() {
		$("#header ul li a").click(makeButtonActive);
	});
});