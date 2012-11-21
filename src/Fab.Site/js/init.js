
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

		$('#faq h4').each(function() {
			var tis = $(this), state = false, answer = tis.next('div').slideUp();
			tis.click(function() {
				state = !state;
				answer.slideToggle(state);
				tis.toggleClass('active', state);
			});
		});

	});
});

$(function (){
	$('#header ul li a').each(function(){
		var path = window.location.href;
		var current = path.substring(path.lastIndexOf('#'));
		var url = $(this).attr('href');
		
		if(url == current){
			$(this).addClass('current');
		};
	});
});

/* FAQ */

$('.faqlink').click(function(){
	$('.content').hide();
	$(this).next('.content').show();
});