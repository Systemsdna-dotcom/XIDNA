$(function () {
	$(window).scroll(function () {
		var scroll = $(window).scrollTop();
		var height = $('.top-header').height() + 28;
		//var height = 28;
		if (scroll >= height) {
			$(".top-header").addClass("fixed-top");
			//$(".filter-section").css("margin-top","175px");
		} else {
			$(".top-header").removeClass("fixed-top");
			//$(".filter-section").css("margin-top","0px");
		};
	});
});

///////////////Scroll to Top///////////////
$(window).scroll(function () {
	if ($(this).scrollTop() >= 50) { // If page is scrolled more than 50px
		$('#headerSection').addClass('headerFix');
		$('#return-to-top').fadeIn(200); // Fade in the arrow
	} else {
		$('#headerSection').removeClass('headerFix');
		$('#return-to-top').fadeOut(200); // Else fade out the arrow
	}
});
returnToTop = function () { // When arrow is clicked
	$('body,html').animate({scrollTop: 0}, 500); // Scroll to top of body
	return false;
};
function contactUs(){
    $('html, body').animate({'scrollTop':   $('#contactUs').offset().top - $("#headerSection").height()},500);
}

//////////////////////////////////////

$(document).ready(function () {
	$(window).on('resize load scroll', example).trigger('resize');//
	function example(){
		setTimeout(function(){
			//$('.body-breadcrumb-sticky').css('position', 'fixed');
			$('.body-breadcrumb-sticky').css({'width':$('.body-section').width(),'position':'fixed'})
		},100);
		var top = $(window).scrollTop();	
		if ($(window).width() <= 960) {
			if ($(this).scrollTop() >= 100) {
				$('.body-breadcrumb-sticky').css({'top': $('.top-header').height(), 'position': 'fixed'});
				$('.body-breadcrumb > .breadcrumb').hide();
			} else {
				$('.body-breadcrumb-sticky').css({'top': 'inherit', 'position': 'absolute'});
				$('.body-breadcrumb > .breadcrumb').show();
			}
		}
	};
});
//////////////////////////////////////
//MegaMenu//
$(document).ready(function() {
    "use strict";

    $('.menu > ul > li:has( > ul)').addClass('menu-dropdown-icon');
    $('.menu > ul > li > ul:not(:has(ul))').addClass('normal-sub');
    $(".menu > ul").before("<a href=\"\" class=\"menu-mobile\"><span class=\"one\"></span><span class=\"two\"></span><span class=\"three\"></span></a>");

    $(".menu > ul > li").hover(function(e) {
        if ($(window).width() > 943) {
            $(this).children("ul").stop(true, false).fadeToggle(150);
            e.preventDefault();
        }
    });

    $(".menu > ul > li").click(function() {
        if ($(window).width() <= 943) {
            $(this).children("ul").fadeToggle(150);
            $(this).toggleClass("expand").siblings().removeClass('expand');
            $(this).siblings().children("ul").hide();
        }
    });

    $(".menu-mobile").click(function(e) {
        $(".menu > ul").toggleClass('show-on-mobile');
        e.preventDefault();
    });

    $(".menu-mobile").click(function () {
        $(".menu-mobile").toggleClass('crossed');
    });

});
/*****************************************/
$("#sectionInsurer").owlCarousel({
    loop: true,
    //margin:10,
    nav: false,
    dots: true,
    autoplay: true,
    autoplayTimeout: 10000,
    responsive: {
        0: {
            items: 1,
            dots: false,
        },
        600: {
            items: 3,
            dots: false,
        },
        1000: {
            items: 5,
			nav: true,
			navText: ["<i class='fa ion-ios-arrow-back'></i>","<i class='fa ion-ios-arrow-forward'></i>"]
        }
    }
})