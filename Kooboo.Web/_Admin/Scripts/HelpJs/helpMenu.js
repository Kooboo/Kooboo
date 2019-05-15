$(function() {
    $('.menu_btn, #col_side').click(function() {
        if ($('#menu_container').is(':visible')) {
            $('#col_side').toggle(200);
        }
    });
    $(window).resize(function() {
        if (!$("#menu_container").is(":visible")) {
            if (!$("#col_side").is(":visible")) {
                $("#col_side").attr("style", "display:table-cell");
            }

        } else {
            $("#col_side").attr("style", "display:none");
        }
    });

    
        // var iframe= document.getElementById("iframe");
        // iframe.onload=function(){
        //     document.getElementById("iframe").height=0;
        //     var height=document.getElementById("iframe").contentWindow.document.body.scrollHeight+10;
        //     document.getElementById("iframe").height=height;
        // }
});