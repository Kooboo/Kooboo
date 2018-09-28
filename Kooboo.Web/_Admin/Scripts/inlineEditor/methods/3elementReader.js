
function elementReader() {


    function GetPageId(el) {
        var doc = el.ownerDocument;
        var len = doc.childNodes.length;
        for (var index = 0; index < len; index++) {

            var element = doc.childNodes[index];
            if (element.nodeType == Node.COMMENT_NODE) {
                var comment = element;

                var result = Kooboo.commentManager.Parse(comment);

                if (result["objecttype"]) {

                    var type = result["objecttype"];
                    if (type == "page" || type == "Page") {

                        if (result["nameorid"]) 
                        {
                            return result["nameorid"];
                        } else if (result["id"]) 
                        {
                            return result["id"];
                        }
                    }
                }
            }
        }

        return null;
    }



    function readObject(el) {
        var self = this;
        var result = {};
        result.pageId = GetPageId(el);
        result.koobooId = el.getAttribute("kooboo-id");
        result.element = el;
        result.el = el;
        result.koobooObjects = getKoobooObjects(el);
        return result;

    }
 
    function getKoobooObjects(el) {
        var result = []; 
        var upresult =  GetUpResult(el);
        var downresult =  GetDownResult(el);
        upresult.forEach(function (item) {
            if (!item["end"]) {
                if (item.koobooId) {
                    // has koobooid. 
                    if (_IsContainByKoobooid(item.koobooId, el)) {
                        result.push(item);
                    }
                } else if (item.boundary) {
                    if (_IsWithBoundary(item.boundary, downresult)) {
                        result.push(item);
                    }
                } else {
                    if (item.node) {
                        if (item.node.parentElement == null || item.node.isEqualNode(item.node.ownerDocument)) {
                            result.push(item);
                        }
                    }
                }
            }
        });


        return result;

        function _IsWithBoundary(boundary, downresult) {
            if (!downresult || downresult.length == 0) {
                return false;
            }
            var testresult = false;
            downresult.forEach(function (item) {
                if (item["end"] && item.boundary == boundary) {
                    testresult = true;
                }
            });
            return testresult;
        }

        function _IsContainByKoobooid(koobooid, el) {
            if (el.hasAttribute && el.hasAttribute("Kooboo-id")) {
                var elkoobooid = el.getAttribute("kooboo-id");
                if (elkoobooid == koobooid) {
                    return true;
                }
            }
            var parent = getParent(el);
            if (parent == null) {
                return false;
            }
            if (_IsContainByKoobooid(koobooid, parent)) {
                return true;
            }
            return false;
        }
    }


    var GetUpResult = function (el) {

        var result = [];

        var doc = el.ownerDocument;
        var treewalker = doc.createTreeWalker(doc, NodeFilter.SHOW_ALL, null, false);
        treewalker.currentNode = el;
        _findobject(treewalker, result);

        // the root comment. 
        for (var index = 0; index < doc.childNodes.length; index++) {
            var element = doc.childNodes[index];
            if (element.nodeType == Node.COMMENT_NODE) {
                var kooboocomment = Kooboo.commentManager.Parse(element);
                if (kooboocomment) {
                    var kresult = convertToKoobooObject(kooboocomment);
                    pushresult(result, kresult);
                }
            }
        }
        return result;

        function _findobject(walker, result) {

            var previous = walker.currentNode;
            while (previous != null) {
                if (previous.nodeType == Node.COMMENT_NODE) {
                    var kooboocomment = Kooboo.commentManager.Parse(previous);
                    if (kooboocomment) {
                        var kresult = convertToKoobooObject(kooboocomment);
                        pushresult(result, kresult);
                    }
                }
                var nextnode = walker.previousSibling();
                if (nextnode == null) {
                    previous = getParent(previous);

                    if (previous != null) {
                        walker.currentNode = previous;
                    }
                } else {
                    previous = nextnode;
                }
            }
        }
    }


    var getParent = function (el) {
        if (el.parentElement) //chrome,firefox
            return el.parentElement;
        if (el.parentNode) //ie
            return el.parentNode;
    }



    var GetDownResult = function (el) {

        var result = [];
        var doc = el.ownerDocument;
        var treewalker = doc.createTreeWalker(doc.body, NodeFilter.SHOW_ALL, null, false);

        treewalker.currentNode = el;
        _findDownObject(treewalker, result);
        return result;

        function _findDownObject(walker, result) {
            var currentnode = walker.currentNode;
            while (currentnode != null) {
                if (currentnode.nodeType == Node.COMMENT_NODE) {
                    var kooboocomment = Kooboo.commentManager.Parse(currentnode);
                    if (kooboocomment) {
                        var kresult = convertToKoobooObject(kooboocomment);
                        pushresult(result, kresult);
                    }
                }
                var nextnode = walker.nextSibling();
                if (nextnode == null) {
                    currentnode = getParent(currentnode);
                    if (currentnode != null) {
                        walker.currentNode = currentnode;
                    }
                } else {
                    currentnode = nextnode;
                }
            }
        }
    }

    var pushresult = function (result, item) {

        result.forEach(function (element) {
            if (element.node && item.node) {
                if (element.node.isEqualNode(item.node)) {
                    return null;
                }
            }
        });

        result.push(item);
    }



    var convertToKoobooObject = function (input) {
        var result = {};
        if (input["objecttype"]) {
            result.type = input["objecttype"];
        }
        if (input["nameorid"]) {
            result.nameOrId = input["nameorid"];
        }
        if (input["koobooid"]) {
            result.koobooId = input["koobooid"];
        }
        if (input["boundary"]) {
            result.boundary = input["boundary"];
        }
        if (input["attributename"]) {
            result.attributeName = input["attributename"];
        }
        if (input["bindingvalue"]) {
            result.bindingValue = input["bindingvalue"];
        }
        if (input["fieldname"]) {
            result.fieldName = input["fieldname"];
        }
        if (input["folderid"]) {
            result.folderId = input["folderid"];
        }
        if (input["end"]) {
            result["end"] = input["end"];
        }
        if (input["node"]) {
            result["node"] = input["node"];
        }
        return result;
    }


    var returnresult = {};

    returnresult.readObject = readObject;
    returnresult.getUpResult = GetUpResult;

    returnresult.getDownResult = GetDownResult;
    returnresult.getPageId = GetPageId;

    return returnresult;

}

