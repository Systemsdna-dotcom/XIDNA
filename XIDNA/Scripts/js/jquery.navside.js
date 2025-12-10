/*
 * Nav _Side - carlosmarch.es/navside
 * 
 * Open source under the DO WHAT THE FUCK YOU WANT License. 
 * 
 * © 2013 Carlos March
 *

*/
(function($){

$.fn.navside = function(options){
        $('#calling').prepend( '<span id="navside_icon" class="hidde" href="#"></span>');
       
        var $content = $('#calling'),
            $sided_menu = $('#navside'),
            $menu_icon = $('#navside_icon'),
            $menu_open = false;

        // Establish our default settings
        var settings = $.extend({
            position         : 'right',
            scroll           : 'hidde'
        }, options);

        // Hide the menu. We will stablish the menu position below.
        $sided_menu.addClass('hidde');
        
        if((typeof options==='undefined') || (typeof options.position==='undefined') || (!options) || (!options.position)){
            // We could't grab options
            // Set options to settings & Run defaults
            options = new Object();
            options.position=settings.position;
        }


        // First Position the icon
        if (options.position == 'right'){
            $menu_icon.addClass('right').removeClass('hidde');
        }
        if (options.position == 'left'){
            $menu_icon.addClass('left').removeClass('hidde');
        }
        

        
        $menu_icon.click(function(event) {
            if (options.position == 'right'){
                navsideRight();
            }
            if (options.position == 'left'){
                navsideLeft();
            }
        });//click
                

        //Left SIDE - stablish the menu position
        function openLeft(){                          
            $sided_menu.addClass('right').removeClass('hidde').animate({
                right:"-=300",      
            },300,'easeOutExpo');

            $content.animate({
                zIndex:"999",
                right:"-=150"   
            },300,'easeOutExpo').css({
                position: 'absolute'
            });
            $menu_icon.animate({
                left:"300",
                backgroundPosition: '-60px',      
            },600,'easeOutExpo');
        };
        function closeLeft(){
            $sided_menu.animate({
                right:"+=300",      
            },300,'easeOutExpo');

            $content.animate({
                right:"+=150",   
            },300,'easeOutExpo');
            $menu_icon.animate({
                left:"10",
                backgroundPosition: '0px',      
            },700,'easeOutExpo');
        };
        function navsideLeft(){
            if( $menu_open == false){
                openLeft();
                $menu_open = true;
            } else{
                closeLeft(); 
                $menu_open = false;
            }
        };//navsideLeft


        //Right SIDE - stablish the menu position
        function openRight(){
            $sided_menu.addClass('left').removeClass('hidde').animate({
                left:"-=300",      
            },300,'easeOutExpo');

            $content.animate({
                zIndex:"999",
                left:"-=150"   
            },300,'easeOutExpo').css({
                position: 'absolute'
            });
            $menu_icon.animate({
                right:"300",
                backgroundPosition: '-60px',      
            },600,'easeOutExpo');
        };
        function closeRight(){
            $sided_menu.animate({
                left:"+=300",      
            },300,'easeOutExpo');

            $content.animate({
                left:"+=150",   
            },300,'easeOutExpo');
            $menu_icon.animate({
                right:"10",
                backgroundPosition: '0px',      
            },700,'easeOutExpo');
        };

        function navsideRight(){
            if( $menu_open == false){
                openRight();
                $menu_open = true;
            }else{
                closeRight();
                $menu_open = false;
            }
        };//navsideRight

        // Position the menu onResize
        function positionNav(){
            var $containerWidth = $(window).width();
            
            if ($menu_open == false) {
                if (options.position == 'right'){
                    $sided_menu.css({
                        left: $containerWidth,
                    });
                }
                if (options.position == 'left'){
                    $sided_menu.css({
                        right: $containerWidth,
                    });
                }
            }
            else {
                if (options.position == 'right'){
                    $sided_menu.css({
                        left: ($containerWidth) - ('300'),
                    });
                }
                if (options.position == 'left'){
                    $sided_menu.css({
                        right: ($containerWidth) - ('300'),
                    });
                }
            }
        }//positionNav

        $(window).resize(function () {
            positionNav();
        });//resize


    // Scrolling
    $(window).bind('scroll',function(e){
        if(($(window).scrollTop() >=500) && ($menu_open == true)){
            if(!options.scroll){
                // We could't grab options
                // Set options to settings & Run defaults
                options.scroll=settings.scroll;
            }

            if (options.scroll == 'fixed'){
                //doNoting
            }
            if (options.scroll == 'hidde'){

                if (options.position == 'right'){
                     closeRight();
                }
                if (options.position == 'left'){
                    closeLeft();
                }
                
                $menu_open = false; 
            }
        }
    });

};//fn

})(jQuery);

