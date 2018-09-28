$(function() {
    function getType(content){
        try{
            var elements=$(content);
            return "markup";
        }catch(err){
            return "javascript";
        }
    }
    
    //source code
    var code = $(".code").val();
    var type=getType(code);

    //$("code") hightlight code container
    $("code").attr("class"," language-"+type);

    var html = Prism.highlight(code, Prism.languages[type], type);
    $("code")[0].innerHTML = html;
});