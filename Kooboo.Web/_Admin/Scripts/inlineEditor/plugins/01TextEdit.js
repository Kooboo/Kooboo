function TextEdit(){
    var reExcept=/^img|button|input|textarea|br$/i,
    opt={
        context:null,
        oldValue:"",
        origParentHtml:"",
        parent:null
    };

    function isElementExist(){
        $(opt.parent).find(opt.context.el).length > 0;
    }
    function getLabelLog(context,oldValue,origParentHtml) {
        var log = {
            action: Kooboo.SiteEditorTypes.ActionType.update,
            editorType: Kooboo.SiteEditorTypes.EditorType.label,
            domOperationType: Kooboo.SiteEditorTypes.DomOperationType.label,
            oldValue: isElementExist() ? oldValue : origParentHtml,
            context: context,
            el: context.el,
            newValue: $(context.el).html()

        };
        return log;
    }
    function getContentLog(context,oldValue,origParentHtml) {
        var el = context.el;
        var log = {
            action: Kooboo.SiteEditorTypes.ActionType.update,
            editorType: Kooboo.SiteEditorTypes.EditorType.content,
            domOperationType: Kooboo.SiteEditorTypes.DomOperationType.content,
            oldValue: isElementExist() ? oldValue : origParentHtml,
            newValue: $(el).html(),
            el: el,
            context: context
        };
        var contentObject = context.koobooObjects[0];
        log.fieldName = contentObject.fieldName;
        return log;
    }
    function getDomLog(context,parent,oldValue,origParentHtml) {
        var el = isElementExist() ? context.el : $(parent)[0];
        var childrens = el.children;
        //for bold text can edit,and so on 
        Kooboo.KoobooElementManager.resetNewElementsKoobooId(childrens);
        var log = {
            action: Kooboo.SiteEditorTypes.ActionType.update,
            editorType: Kooboo.SiteEditorTypes.EditorType.dom,
            domOperationType: Kooboo.SiteEditorTypes.DomOperationType.html,
            oldValue: isElementExist() ? oldValue : origParentHtml,
            el: el,
            newValue: Kooboo.InlineEditor.cleanUnnecessaryHtml(el)
        };
        return log;
    }
    function getLog(context,parent,oldValue,origParentHtml) {
        var objectType = Kooboo.KoobooObjectManager.getObjectType(context);
        var log = {};
        if (objectType.toLocaleLowerCase() == Kooboo.SiteEditorTypes.ObjectType.content &&
            !Kooboo.KoobooObjectManager.isContentAttribute(context.el)) {
            log = getContentLog(context,oldValue,origParentHtml);
        } else if (objectType.toLocaleLowerCase() == Kooboo.SiteEditorTypes.ObjectType.label) {
            log = getLabelLog(context,oldValue,origParentHtml);
        } else {
            log = getDomLog(context,parent,oldValue,origParentHtml);
        }
        return log;
    }
    
    function afterEditText() {
        var log = getLog(opt.context,opt.parent,opt.oldValue,opt.origParentHtml);
        if (log.newValue != log.oldValue) {
            log.logType = Kooboo.SiteEditorTypes.LogType.log;
            opt.context.editManager.addLog(log);
        }
    }
    function bindAfterEditText() {
        window.__gl.siteEditor.afterEditText = afterEditText;
    }
    function beforeEditText() {
        opt.parent = $(opt.context.el).parent();
        if (opt.parent&& opt.parent.length>0)
            opt.origParentHtml = Kooboo.InlineEditor.cleanUnnecessaryHtml(opt.parent);

        var oldValue = Kooboo.InlineEditor.cleanUnnecessaryHtml(opt.context.el);
        opt.oldValue = oldValue;
    }
    function getLabelObject(el){
        var koobooObject=Kooboo.KoobooObjectManager.getFirstWrapKoobooObject(el);

        if(koobooObject&&koobooObject.objectType && koobooObject.objectType.toLowerCase()==Kooboo.SiteEditorTypes.EditorType.label){
            return koobooObject;
        }
        return null;
    }
    return {
        menuName: Kooboo.text.common.edit,
        isShow: function(context) {
            opt.context = context;
            var el = context.el;

            if(Kooboo.PluginHelper.isBindVariable()) return false;

            if (Kooboo.PluginHelper.isEmptyKoobooId(el)){
                var labelObject=getLabelObject(opt.context.el);
                if(labelObject){
                    return true;
                }
                return false;
            }
            
            var html = $(context.el).html();
            if (Kooboo.KoobooObjectManager.isNoNameIdContent(context)) return false;

            var koobooObjects = context.koobooObjects;
            if (Kooboo.KoobooObjectManager.isMenu(koobooObjects)) return false;

            return el && !reExcept.test(el.tagName) &&
                !Kooboo.PluginHelper.isContainDynamicData(html);
        },
        getHtml:function(){
            return "";
        },
        init:function(){

        },
        click: function() {
            var self = this;
            //temp only support label/view
            //htmlblock，content--》no koobooId

            bindAfterEditText();
            beforeEditText();
            window.__gl.iframe.createTinyMceEditor(opt.context);
        },
       
    }
}