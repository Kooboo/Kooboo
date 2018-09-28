
var koobooParentWindow = window.parent;
var koobooParentWindow$ = koobooParentWindow.$;
koobooParentWindow$(function() {
    
    function KoobooIframe(){
        var $ = koobooParentWindow.$,
            parentKooboo = koobooParentWindow.Kooboo,
            gl = koobooParentWindow.__gl;        

        function closeLoading() {
            gl.siteEditor.loading(false);
        }
        function getWindowOffset(){
            return {
                top: $(window).scrollTop(),
                left: $(window).scrollLeft(),
            };
        }
        function createTinyMceEditor(context) {
            gl.domSelector.setEdit(true);
            var editor = KoobooTinymceEditor(window,{
                el: context.el,
                initFn: function(el) {
                    var offset = getWindowOffset();
                    gl.shadow.mask({ el: el }, null, offset);
                },
                saveFn: function() {
                    if (gl.siteEditor.afterEditText) {
                        gl.siteEditor.afterEditText(context);
                        setTimeout(function() {
                            gl.domSelector.setEdit(false);
                        }, 50);
    
                        gl.shadow.unmask();
                    }
                },
                pickImage: gl.siteEditor.showMediagrid,
                pickPage: gl.siteEditor.showPickPage,
                inframe:true
            });
            editor.create();
            return editor;
        }
        return {
            start:function(){
                closeLoading();
                gl.iframe={
                    createTinyMceEditor:createTinyMceEditor
                };
                $(window).on("scroll",function(){
                    gl.domSelector.resetPosition();
                    if(gl.resetConvertersPosition)
                        gl.resetConvertersPosition();
                });
            }
        }
        
    }
    var iframe=KoobooIframe();
    iframe.start();
});