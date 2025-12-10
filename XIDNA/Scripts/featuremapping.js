
$(function () {
    $("#rFeatures").bind("dblclick", function (e) {
        leftButtonClicked();
        e.preventDefault();
    });
    $("#aFeatures").bind("dblclick", function (e) {
        rightButtonClicked();
        e.preventDefault();
    });;
    $("#left").bind("click", function (e) {
        leftButtonClicked();
        e.preventDefault();
    });
    $("#right").bind("click", function (e) {
        rightButtonClicked();
        e.preventDefault();
    });

    $("#rFeatures").keydown(function (e) {
        switch (e.which) {
            case 37: // left
                leftButtonClicked();
                e.preventDefault();
                break;

                //case 38: // up

                //    $('[id*=rFeatures] option:selected').each(function () {

                //        $(this).insertBefore($(this).prev());

                //    });

                //break;
            case 38: // up
                upButtonClicked();
                e.preventDefault();
                break;
            case 40: // down
                downButtonClicked();
                e.preventDefault();
                break;

            default: return; // exit this handler for other keys
        }

        // prevent the default action (scroll / move caret)
    });
    $("#aFeatures").keydown(function (e) {
        if (e.which == 39) {
            rightButtonClicked();
            e.preventDefault();
        }
        //e.preventDefault(); // prevent the default action (scroll / move caret)
    });
    function upButtonClicked() {
        var options = $("[id*=rFeatures] option:selected");
        if (options.length) {
            options.first().prev().before(options);
            $("#rFeatures").focus().select();
            $('#map').val("Mapped");
        }

    }
    function downButtonClicked() {
        var options = $("[id*=rFeatures] option:selected");
        if (options.length) {
            options.last().next().after(options);
            $("#rFeatures").focus().select();
            $('#map').val("Mapped");
        }
    }
    function leftButtonClicked() {       
        var options = $("[id*=rFeatures] option:selected");
        var alabel = $('[id*=rFeatures] optgroup :selected').parent().attr('label');
        var aid = $('[id*=rFeatures] optgroup :selected').parent().attr('class');
        var aparentid = $('[id*=rFeatures] optgroup option:selected').attr('parent');
        var aValue = $('[id*=rFeatures] optgroup option:selected').attr('value');
        var childlabel = $('[id*=rFeatures] optgroup option:selected').text();
        var optgroup = $('[id*=aFeatures] optgroup[label="' + alabel + '"] ').attr('class');
        if (options.hasClass("disabled")) {
            $('#ResponseText').html('Unable to Remove Parent Attributes');
            $('#Response').removeClass();
            $('#Response').addClass("box-header with-border error");
            $('#Response').show();
        }
        else {
            $('#Response').hide();
            if (options.length != 0) {
                if (aparentid != undefined) {

                    var thisText = $(options).text();

                    $(options).remove();
                    //$('[id*=aFeatures] option').filter(function () { return $(options).text() == thisText; }).prop('disabled', false);
                    var thisGroup = $('[id*=rFeatures] optgroup[label="' + alabel + '"] ');
                    if (thisGroup.find('option:visible').length == 0) {
                        thisGroup.remove();
                    }
                    if (optgroup != undefined) {
                        var opt = "<option parent=" + aparentid + " value=" + aValue + " optGroup=" + alabel + ">" + childlabel + "</option>"
                        $('[id*=aFeatures] optgroup[label="' + alabel + '"] ').append(opt)
                    }
                    else {
                        var opt = "<optgroup class=" + aid + " label=" + alabel + "><option parent=" + aparentid + " value=" + aValue + " optGroup=" + alabel + ">" + childlabel + "</option></optgroup>"
                        $(options).remove();
                        $("[id*=aFeatures]").append(opt);
                        $('#map').val("Mapped");
                    }

                }
                else {
                    var opt = $(options).clone();
                    $(options).remove();
                    $("[id*=aFeatures]").append(opt);
                    $('#map').val("Mapped");
                }
            }
            }
        
    }
    function rightButtonClicked() {       
        var options = $("[id*=aFeatures] option:selected");     
        var alabel = $('[id*=aFeatures] optgroup :selected').parent().attr('label');
        var aid = $('[id*=aFeatures] optgroup :selected').parent().attr('class');
        var aparentid = $('[id*=aFeatures] optgroup option:selected').attr('parent');
        var aValue = $('[id*=aFeatures] optgroup option:selected').attr('value');
        var childlabel = $('[id*=aFeatures] optgroup option:selected').text();
     
        var optgroup = $('[id*=rFeatures] optgroup[label="' + alabel + '"] option').parent().attr('class');
        if (options.length != 0) {
            if (aparentid != undefined) {

                if (aparentid != optgroup) {
                    //var opt = $(options).clone();
                    var opt = "<optgroup class=" + aid + " label=" + alabel + "><option parent=" + aparentid + " value=" + aValue + " optGroup=" + alabel + ">" + childlabel + "</option></optgroup>"
                    $(options).remove();
                    $("[id*=rFeatures]").append(opt);
                    $('#map').val("Mapped");
                    
                    if ($('[id*=aFeatures] optgroup[label="' + alabel + '"] ').find('option').length == 0) {
                        $('[id*=aFeatures] optgroup[label="' + alabel + '"] ').remove();
                    }
                }
                else {
                    $('#ResponseText').html("Please Select One in a Group");
                    $('#Response').removeClass();
                    $('#Response').addClass("box-header with-border warning");
                    $('#Response').show();
                }
            }
            else {
                var opt = $(options).clone();               
                $(options).remove();
                $("[id*=rFeatures]").append(opt);
                $('#map').val("Mapped");
            }
            
        }
    }
    $("#top").click(function (e) {
        upButtonClicked();
        e.preventDefault();
    });
    $("#bottom").click(function (e) {
        downButtonClicked();
        e.preventDefault();
    });
});

