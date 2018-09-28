function DomOperationMenu(item){
    function modify(newValue) {
        var menuObject = getMenuObject();
        var menuComment = menuObject.node;
        var nodes = getNodesInMenuComment(menuComment);

        var menuplaceholderId = removeNodes(nodes);
        $(newValue).insertAfter(menuplaceholderId);

        $(menuplaceholderId).remove();
    }
    function removeNodes(nodes) {
        var index = 0;
        var menuplaceholderId;
        $.each(nodes, function(i, node) {
            if (index == 0)
                menuplaceholderId = $("<p id='menuplaceholderId'></p>").insertBefore($(node));
            $(node).remove();
            index++;
        });
        return menuplaceholderId;

    }
    function getNodesInMenuComment(commentNode) {
        var nextSibling = commentNode;
        var hasNextSibling = true;
        var nodes = [];
        while (hasNextSibling) {

            nextSibling = nextSibling.nextSibling;

            if (nextSibling.nodeType == Kooboo.NodeType.Comment) {
                var nodeValue = nextSibling.nodeValue;

                var commentData = Kooboo.commentManager.ParseCommentLine(nodeValue);
                //menu end 结束
                if (commentData.objecttype && commentData.objecttype.toLowerCase() == Kooboo.SiteEditorTypes.ObjectType.menu) {
                    hasNextSibling = false;
                }
            } else {
                nodes.push(nextSibling);
                hasNextSibling = true;
            }

        }
        return nodes;
    }
    function getMenuObject() {
        return getKoobooObjectByType(Kooboo.SiteEditorTypes.ObjectType.menu);
    }
    function getKoobooObjectByType(objectType) {
        var object = null;
        var koobooObjects = item.context.koobooObjects;
        $.each(koobooObjects, function(i, koobooObject) {
            if (koobooObject.type.toLowerCase() == objectType) {
                object = koobooObject;
                return false;
            }
        });
        return object;
    }
    return {
        update: function(newValue) {
            var menuObject = getMenuObject();
            var menuComment = menuObject.node;
            var nodes = getNodesInMenuComment(menuComment);

            var menuplaceholderId = removeNodes(nodes);
            $(newValue).insertAfter(menuplaceholderId);

            $(menuplaceholderId).remove();
        },
        
    }
}