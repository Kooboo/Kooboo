function DomOperationRepeater(item){
    var param={
        item:item
    }
    var NodeType=Kooboo.NodeType;
    
    function getContentCommontNodes() {
        var nameOrId = param.item.nameOrId;
        var contentRepeaterObject = getContentRepeaterObject(nameOrId),
            contentOrAttributeObj = getContentOrAttributeObj(nameOrId),
            domNodes;

        if (contentRepeaterObject) {
            domNodes = getDomNodeByKoobooObject(contentRepeaterObject);
        } else if (contentOrAttributeObj) {
            domNodes = getDomNodeByKoobooObject(contentOrAttributeObj, true);
        }
        var contentCommentNodes = domNodes ? findContentCommentNodes(domNodes) : null;
        return contentCommentNodes;
    }
    function getContentOrAttributeObj(nameOrId) {
        var contentObject = getContentObject(nameOrId),
            attributeObject = getAttributeObject(nameOrId);
        if (contentObject) return contentObject;
        if (attributeObject) return attributeObject;
        return null;
    }
    //是否为单个content,未包在contentRepeater
    function isSameNameOrId(contentRepeaterObject, contentRelativeObj) {
        return contentRepeaterObject && contentRelativeObj && isBelongToRepeater(contentRepeaterObject, contentRelativeObj);
    }
    function isBelongToRepeater(contentRepeaterObject, contentRelativeObj) {
        return contentRepeaterObject.nameOrId == contentRelativeObj.nameOrId;
    }
    function getFieldName(commentData) {
        if (commentData) {
            if (commentData.fieldname)
                return commentData.fieldname;

            var bindingvalue = commentData.bindingvalue;

            if (bindingvalue) {
                var fieldReg = /\{\.|\w+\}/;
                //bindingvalue:/chs/{userkey}
                //bindingvalue:{GetById.img}
                var matches = bindingvalue.match(fieldReg);
                if (matches && matches.length > 0) {
                    var fieldname = matches[0].replace("{", "").replace("}", "");
                    return fieldname;
                }
                //bindingvalue:List_Item.ct
                if (bindingvalue.indexOf(".") > -1) {
                    bindingvalue = bindingvalue.substring(bindingvalue.indexOf(".") + 1);
                    return bindingvalue;
                }
            }
        }

        return "";
    }
    function isAttributeComment(commentData) {
        return commentData.attributename ? true : false;
    }
    function isNeedRepeaterComment(commentData) {
        if (commentData) {
            if (commentData.objecttype && 
                commentData.objecttype.toLowerCase() == Kooboo.SiteEditorTypes.ObjectType.content)
                return true;
            if (isAttributeComment(commentData))
                return true;
        }

        return false;
    }
    function getFieldValue(data, fieldName, commentData) {
        var fieldValue = data[fieldName];
        if (fieldValue) {
            var bindingvalue = commentData.bindingvalue;
            if (bindingvalue) {
                //repeat:first?
                if (bindingvalue.indexOf("{") > -1 &&
                    bindingvalue.indexOf("}") > -1) {
                    var reg = /\{[^\}]*\}/;
                    //chs/{userkey}
                    //background-image: url({special1_Item.img})
                    return bindingvalue.replace(reg, fieldValue);
                } else {
                    //List_Item.content
                    var fieldWithDot = "." + fieldName;
                    if (bindingvalue.indexOf(fieldWithDot) > -1)
                        return fieldValue;
                }


            }
        }
        return "";
    }
    function doModifyContentRepeater(contentCommentNodes, data) {
        var nameOrId = param.item.nameOrId;

        $.each(contentCommentNodes, function(i, commentNode) {
            var commentData = getCommentDataByCommentNode(commentNode);
            if (!commentData) return true;

            var rightData = getRightDataByComment(commentData.nameorid, data, nameOrId);
            //delete  data
            if (!rightData) {
                // var element = getNextElement(commentNode);
                // if (element)
                //     $(element).remove();
                return true; //continue;
            }
            var fieldName = getFieldName(commentData);
            if (fieldName) {
                var fieldValue = getFieldValue(rightData, fieldName, commentData);
                var element = getNextElement(commentNode);
                if (isAttributeComment(commentData)) {
                    //objecttype='attribute'--attributename='href'--bindingvalue='List_Item.href'
                    $(element).attr(commentData.attributename, fieldValue);
                } else {
                    //objecttype='content'--nameorid='cd6b8f09-2146-73d3-cade-4e832627b4f6'--bindingvalue='List_Item.content'--fieldname='content'
                    $(element).html(fieldValue);
                }

            }
        });
    }
    function getRightDataByComment(commentNameOrId, data, nameOrId) {
        //same nameOrId 
        if (commentNameOrId == nameOrId) {
            return data;
        }
        var categories = data.__categories;
        var embeddeds = data.__embeddeds;
        var matchData = null;

        var datas = [categories, embeddeds];
        for (var i = 0; i < datas.length; i++) {
            $.each(datas[i], function(i, dataDetails) {
                $.each(dataDetails, function(j, detail) {
                    if (detail.id == commentNameOrId && detail.values) {
                        matchData = detail.values;
                        return false;
                    }
                    if (matchData) {
                        return false;
                    }
                });

            });
            if (matchData) {
                break;
            }
        }
        return matchData;
    }
    function findContentCommentNodes(contentRepeaterElements) {
        var contentCommentNodes = [];
        $.each(contentRepeaterElements, function(i, element) {
            doFindChildrenCommentNodes(contentCommentNodes, element);
        });

        return contentCommentNodes;
    }
    function getCommentDataByCommentNode(commentNode) {
        var nodeValue = commentNode.nodeValue;
        var commentData = Kooboo.commentManager.ParseCommentLine(nodeValue);

        if (commentData && commentData.objecttype && 
            Kooboo.SiteEditorTypes.ObjectType[commentData.objecttype.toLowerCase()]) {
            return commentData;
        }
        return null;
    }
    function doFindChildrenCommentNodes(contentCommentNodes, element) {
        //comment
        if (element.nodeType == Kooboo.NodeType.Comment) {
            var commentData = getCommentDataByCommentNode(element);
            if (isNeedRepeaterComment(commentData)) {
                contentCommentNodes.push(element);
            }
        }
        var childNodes = element.childNodes;
        if (childNodes && childNodes.length > 0) {
            $.each(childNodes, function(i, childNode) {
                if (childNode.nodeType == Kooboo.NodeType.Comment) {
                    var commentData = getCommentDataByCommentNode(childNode);
                    if (isNeedRepeaterComment(commentData)) {
                        contentCommentNodes.push(childNode);
                    }
                } else {
                    doFindChildrenCommentNodes(contentCommentNodes, childNode);
                }
            });
        }
    }
    function replaceCommontGuid(content, guid) {
        var reg = /nameorid\='\w{8}-(\w{4}-){3}\w{12}/g;
        return content.replace(reg, "--nameorid='" + guid + "'");
    }
    function getContentRepeaterParentElement() {
        var nameOrId = param.item.nameOrId;
        var contentRepeaterObject = getContentRepeaterObject(nameOrId);
        var nextElement = getNotCommentElement(contentRepeaterObject.node);
        return $(nextElement).parent();
    }
    function getNextElement(node) {
        if (node && node.nodeType != Kooboo.NodeType.Element)
            return node.nextElementSibling;
       
        return node;
    }
    function getNotCommentElement(element) {
        if (element.nodeType == Kooboo.NodeType.Comment)
            return getNotCommentElement(element.nextSibling);
        return element;
    }
    function getWrapComment(data) {
        return "<!--" + data + "-->";
    }
    function getContentRepeaterObject(nameOrId) {
        return getKoobooObjectByType(Kooboo.SiteEditorTypes.ObjectType.contentrepeater, nameOrId);
    }
    function getContentObject(nameOrId) {
        return getKoobooObjectByType(Kooboo.SiteEditorTypes.ObjectType.content, nameOrId);
    }
    function getAttributeObject(nameOrId) {
        return getKoobooObjectByType(Kooboo.SiteEditorTypes.ObjectType.attribute, nameOrId);
    }
    function getKoobooObjectByType(objectType, nameOrId) {
        var object = null;
        var koobooObjects = param.item.context.koobooObjects;
        $.each(koobooObjects, function(i, koobooObject) {
            if (!koobooObject.type) return true;
            if (koobooObject.type.toLowerCase() == objectType && 
            koobooObject.nameOrId == nameOrId) {
                object = koobooObject;
                return false;
            }
        });
        return object;
    }
    //获取contentRepeater或者content的Dom
    function getDomNodeByKoobooObject(koobooObject, isSingleContent) {
        var node = koobooObject.node,
            nextSiblingArr = [];

        nextSiblingArr.push(node);
        var nextSibling = node.nextSibling;

        var hasNextSibling = true;

        var maxCircleCount = 10,
            count = 1;

        while (hasNextSibling) {
            if (nextSibling) {
                var nodeType = nextSibling.nodeType;
                //comment
                if (nodeType == Kooboo.NodeType.Comment) {
                    var nodeValue = nextSibling.nodeValue;

                    var commentData = Kooboo.commentManager.ParseCommentLine(nodeValue);
                    if (commentData.objecttype.toLowerCase() == Kooboo.SiteEditorTypes.ObjectType.contentrepeater &&
                        commentData.boundary == koobooObject.boundary) {
                        nextSiblingArr.push(nextSibling);
                        hasNextSibling = false;
                        break;
                    }
                    nextSiblingArr.push(nextSibling);
                } else {
                    nextSiblingArr.push(nextSibling);
                }

                //content不包含在repeater，只需要获取最接近的元素
                if (isSingleContent &&
                    nextSibling.nodeType == Kooboo.NodeType.Element) {
                    break;
                }
                nextSibling = nextSibling.nextSibling;
                if (count > maxCircleCount)
                    break;
                count++;

            } else {
                hasNextSibling = false;
            }
        }
        return nextSiblingArr;

    }
    function modifyContentRepeater(data) {
        // if (data.Online != "True") {
        //     self.removeContentRepeater();
        // }
        var contentCommentNodes = getContentCommontNodes();
        doModifyContentRepeater(contentCommentNodes, data);

    }
    function removeContentRepeater() {
        var nameOrId = param.item.nameOrId;
        var koobooObject = getContentRepeaterObject(nameOrId);
        var contentRepeaterElements = getDomNodeByKoobooObject(koobooObject);
        $.each(contentRepeaterElements, function(i, element) {
            $(element).remove();
        });
    }
    function copyContentRepeater() {
        var nameOrId = param.item.nameOrId;
        var koobooObject = getContentRepeaterObject(nameOrId);
        var contentRepeaterElements = getDomNodeByKoobooObject(koobooObject);
        if (!contentRepeaterElements || contentRepeaterElements.length == 0) return;
        var endContentRepaterElements = contentRepeaterElements[contentRepeaterElements.length - 1];

        var lastInsertElement = endContentRepaterElements;

        var copyHtml = [];
        var newGuid = Kooboo.Guid.NewGuid();
        $.each(contentRepeaterElements, function(i, element) {

            if (element.nodeType == Kooboo.NodeType.Element) {
                var html = replaceCommontGuid(element.outerHTML, newGuid);
                copyHtml.push(html);

            } else if (element.nodeType == Kooboo.NodeType.Comment) { //comment
                var wrapComment = getWrapComment(element.nodeValue);
                wrapComment = replaceCommontGuid(wrapComment, newGuid);
                copyHtml.push(wrapComment);
            }
        });
        $(copyHtml.join(' ')).insertAfter(lastInsertElement);

        var contentId = koobooObject["nameOrId"];
        Kooboo.dom.DomOperationHelper.addRptCopyFrom(newGuid, contentId);
        var copyFromcontentId =Kooboo.dom.DomOperationHelper.getRptCopyFromId(contentId);
        return {
            contentId: copyFromcontentId,
            newContentId: newGuid
        }
        //copyElements = self._findCopyElementNodes(copyNodes);
        //return newGuid;
    }
     return {
        getFieldValue:getFieldValue,
        getFieldName:getFieldName,

        update: function(value) {
            //判断值的类型,不同类型，一种是第一次dom操作后，才能获取到值。
            if (param.item.action == Kooboo.SiteEditorTypes.ActionType.update) {
                modifyContentRepeater(value);
            } else if (!value) {
                switch (param.item.action) {
                    case Kooboo.SiteEditorTypes.ActionType.delete:
                        var parent = getContentRepeaterParentElement();
                        var oldValue = $(parent).html();
                        removeContentRepeater();
                        var newValue = $(parent).html();
                        item.el = parent[0];
                        item.oldValue = oldValue;
                        item.newValue = newValue;
                        break;
                    case Kooboo.SiteEditorTypes.ActionType.copy:
                        var parent = getContentRepeaterParentElement();
                        var oldValue = $(parent).html();
                        var data = copyContentRepeater();
                        var newValue = $(parent).html();
                        item.el = parent[0];
                        item.oldValue = oldValue;
                        item.OrgNameOrId = data.contentId;
                        item.newNameOrId = data.newContentId;
                        item.newValue = newValue;
                        break;
                }
            } else {
                Kooboo.dom.DomOperationHelper.updateHtml(param.item.el, value);
            }
        },
        
    }
}