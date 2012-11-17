
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

var make_button_active = function()
{
  //Get item siblings
  var siblings = $("#header ul li a");

  //Remove active class on all buttons
  siblings.each(function (index)
    {
      $(this).removeClass('current');
    }
  )

  //Add the clicked button class
  $(this).addClass('current');
}

//Attach events to menu
$(document).ready(
  function()
  {
    $("#header ul li a").click(make_button_active);

    $('#faq h4').each(function() {
	    var tis = $(this), state = false, answer = tis.next('div').slideUp();
	    tis.click(function() {
	      state = !state;
	      answer.slideToggle(state);
	      tis.toggleClass('active',state);
	    });
	  });

  }  
)
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