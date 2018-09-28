(function() {
    var commentManager = Kooboo.CommentManager;
    var elementReader = (function() {
        function elementReader() {}
        // use this one to read kooboo object from element. 
        elementReader.prototype.ReadObject = function(el) {
            var self = this;
            var result = new elementResult();
            result.pageId = this.GetPageId(el);
            result.koobooId = el.getAttribute("kooboo-id");
            result.element = el;
            result.koobooObjects = getKoobooObjects(el);
            return result;

            function getKoobooObjects(el) {
                var result = new Array();
                var upresult = self.GetUpResult(el);
                var downresult = self.GetDownResult(el);
                upresult.forEach(function(item) {
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
                    downresult.forEach(function(item) {
                        if (item["end"] && item.boundary == boundary) {
                            testresult = true;
                        }
                    });
                    return testresult;
                }

                function _IsContainByKoobooid(koobooid, el) {
                    if (el.hasAttribute("Kooboo-id")) {
                        var elkoobooid = el.getAttribute("kooboo-id");
                        if (elkoobooid == koobooid) {
                            return true;
                        }
                    }
                    var parent = el.parentElement;
                    if (parent == null) {
                        return false;
                    }
                    if (_IsContainByKoobooid(koobooid, parent)) {
                        return true;
                    }
                    return false;
                }
            }
        };
        elementReader.prototype._convertToKoobooObject = function(input) {
            var result = new KoobooObject();
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
        };
        elementReader.prototype.GetUpResult = function(el) {
            var self = this;
            var result = new Array();
            var doc = el.ownerDocument;
            var treewalker = doc.createTreeWalker(doc, NodeFilter.SHOW_ALL, null, false);
            treewalker.currentNode = el;
            _findobject(treewalker, result);
            // the root comment. 
            for (var index = 0; index < doc.childNodes.length; index++) {
                var element = doc.childNodes[index];
                if (element.nodeType == Node.COMMENT_NODE) {
                    var kooboocomment = commentManager.prototype.ParseComment(element);
                    if (kooboocomment) {
                        var kresult = self._convertToKoobooObject(kooboocomment);
                        self._pushresult(result, kresult);
                    }
                }
            }
            return result;

            function _findobject(walker, result) {
                var previous = walker.currentNode;
                while (previous != null) {
                    if (previous.nodeType == Node.COMMENT_NODE) {
                        var kooboocomment = commentManager.prototype.ParseComment(previous);
                        if (kooboocomment) {
                            var kresult = self._convertToKoobooObject(kooboocomment);
                            self._pushresult(result, kresult);
                        }
                    }
                    var nextnode = walker.previousSibling();
                    if (nextnode == null) {
                        previous = self.getParent(previous);

                        if (previous != null) {
                            walker.currentNode = previous;
                        }
                    } else {
                        previous = nextnode;
                    }
                }
            }
        };
        elementReader.prototype.getParent = function(el) {
            if (el.parentElement) //chrome,firefox
                return el.parentElement;
            if (el.parentNode) //ie
                return el.parentNode;
        };
        elementReader.prototype.GetDownResult = function(el) {
            var self = this;
            var result = new Array();
            var doc = el.ownerDocument;
            var treewalker = doc.createTreeWalker(doc.body, NodeFilter.SHOW_ALL, null, false);

            treewalker.currentNode = el;
            _findDownObject(treewalker, result);
            return result;

            function _findDownObject(walker, result) {
                var currentnode = walker.currentNode;
                while (currentnode != null) {
                    if (currentnode.nodeType == Node.COMMENT_NODE) {
                        var kooboocomment = commentManager.prototype.ParseComment(currentnode);
                        if (kooboocomment) {
                            var kresult = self._convertToKoobooObject(kooboocomment);
                            self._pushresult(result, kresult);
                        }
                    }
                    var nextnode = walker.nextSibling();
                    if (nextnode == null) {
                        currentnode = self.getParent(currentnode);
                        if (currentnode != null) {
                            walker.currentNode = currentnode;
                        }
                    } else {
                        currentnode = nextnode;
                    }
                }
            }
        };
        elementReader.prototype.GetPageId = function(el) {
            var doc = el.ownerDocument;
            var len = doc.childNodes.length;
            for (var index = 0; index < len; index++) {
                var element = doc.childNodes[index];
                if (element.nodeType == Node.COMMENT_NODE) {
                    var comment = element;
                    var result = commentManager.prototype.ParseComment(comment);
                    if (result["objecttype"]) {
                        var type = result["objecttype"];
                        if (type == "page" || type == "Page") {
                            if (result["nameorid"]) {
                                return result["nameorid"];
                            } else if (result["id"]) {
                                return result["id"];
                            }
                        }
                    }
                }
            }
            return null;
        };
        elementReader.prototype._pushresult = function(result, item) {
            result.forEach(function(element) {
                if (element.node && item.node) {
                    if (element.node.isEqualNode(item.node)) {
                        return null;
                    }
                }
            });
            result.push(item);
        };
        return elementReader;
    }());
    Kooboo.ElementReader = elementReader;
    var KoobooObject = (function() {
        function KoobooObject() {}
        return KoobooObject;
    }());
    var elementResult = (function() {
        function elementResult() {}
        return elementResult;
    }());
})();


// public string ObjectType { get; set; }
// public string NameOrId { get; set; }
// public string AttributeName { get; set; }
// public string KoobooId { get; set; }
// public string BindingKey { get; set; }
// //for text content. 
// public string FieldName { get; set; } 
//# sourceMappingURL=elementReader.js.map