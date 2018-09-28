function DomOperationHtmlBlock(item){
    function replaceHtmlBlock(needRemoveValues, replaceHtml) {
        var htmlplaceHolder = removeElements(needRemoveValues);
        var replaceValues = $(replaceHtml).insertAfter(htmlplaceHolder);

        $(htmlplaceHolder).remove();
        return replaceValues;
    }
    function removeElements(values) {
        var index = 0;
        var htmlplaceholderId;
        $.each(values, function(i, value) {
            if (index == 0)
                htmlplaceholderId = $("<p id='htmlplaceholderId'>aa</p>").insertBefore($(value));
            $(value).remove();
            index++;
        });
        return htmlplaceholderId;
    }
    function getValueByHtmlblockNodes(siblings) {
        var joinHtmls = [];
        $.each(siblings, function(i, sibling) {
            joinHtmls.push(sibling.outerHTML);
        });
        return joinHtmls.join(" ");
    }
    function getHtmlblockNodes(context) {
        var htmlblockNodes = [];

        //var objectType = self.context.type;
        var koobooObject = Kooboo.KoobooObjectManager.getRightKoobooObjectByContext(null, context);
        if (koobooObject) {
            var commentNode = koobooObject.node;
            var nextSibling = commentNode;
            var hasNextSibling = true;
            while (hasNextSibling) {
                nextSibling = nextSibling.nextSibling;

                if (!nextSibling) {
                    hasNextSibling = false;
                    return;
                } else if (nextSibling.nodeType == Kooboo.NodeType.Comment) {
                    var commentData = Kooboo.commentManager.ParseCommentLine(nextSibling.nodeValue);
                    var objectType = commentData.objecttype;
                    //exist koobooObject
                    if (objectType && Kooboo.SiteEditorTypes.ObjectType[objectType.toLowerCase()]) {
                        hasNextSibling = false;
                    } else {
                        htmlblockNodes.push(nextSibling);
                    }
                } else {
                    htmlblockNodes.push(nextSibling);
                }
            }
        }
        return htmlblockNodes;
    }
    return {
        // undo: function() {
        //     //item.oldSiblings = self.replaceHtmlBlock(item.newSiblings, item.oldValue);
        //     item.oldHtmlblockNodes = self.replaceHtmlBlock(item.newHtmlblockNodes, item.oldValue);
        // },
        // redo: function() {
        //     //item.newSiblings = self.replaceHtmlBlock(item.oldHtmlblockNodes, item.newValue);
        //     item.newHtmlblockNodes = self.replaceHtmlBlock(item.oldHtmlblockNodes, item.newValue);
        // },
        update: function(value) {
            var htmlblockNodes = getHtmlblockNodes(item.context);
            replaceHtmlBlock(htmlblockNodes, value);
        },
        
    }
}