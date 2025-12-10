NumberOfLiveEasys = 0;

EasyAlert = {};

EasyAlert.Easy = function (data) {
    debugger;
    if ($("body .Easy.go")){ // if an Easy already exists on page
        $("body .Easy.go").each(function(){
            var currentBottomPixels = $(this).css("bottom"); // get bottom margin
            var currentBottom = currentBottomPixels.split('p')[0]; // remove the px
            var EasyHeight = $(this).height() + Number(currentBottom) + 30; // the height of the Easy + current bottom margin
            $(this).css("bottom", EasyHeight); // Apply new margin amount
        });
    }
    NumberOfLiveEasys = NumberOfLiveEasys + 1;
    var CurrentEasy = NumberOfLiveEasys + 1;
    
    // Options
    var text = data.text;
    var removeTime = data.time;
    var bkgrndColour = data.bkgrndColour;
    var textColour = data.textColour;

    // Create EasyAlert
    $("body").append("<div class='Easy Easy-"+CurrentEasy+"' style='background-color: "+bkgrndColour+"'><p style='color:"+textColour+"'>"+text+"</p></div>");
    setTimeout(function() {
        $(".Easy-"+CurrentEasy+"").addClass("go"); // animation to display
    }, 250);
    setTimeout(function() {
        $(".Easy-"+CurrentEasy+"").addClass("stop"); // animation to remove
        setTimeout(function(){
            $(".Easy-"+CurrentEasy+"").remove(); // remove EasyAlert from DOM
        }, 2000);
    }, removeTime);
}