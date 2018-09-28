(function(root, factory) {
    if (typeof define === 'function' && define.amd) {
        define(['./lib/knockout'], factory);
    } else if (typeof exports === 'object') {
        module.exports = factory(require('./lib/knockout'));
    } else {
        root.KnockoutFastForeach = factory(root.ko);
    }
})(this, function(ko) {
    ko.bindingHandlers["visible"] = {
        'update': function(element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            var isCurrentlyVisible = !(element.style.display == "none") && getComputedStyle(element).display !== "none";
            if (value && !isCurrentlyVisible) {
                element.setAttribute("kb-show", "");
            } else if ((!value) && isCurrentlyVisible)
                element.removeAttribute("kb-show");
        }
    };
    var MAX_LIST_SIZE = 9007199254740991;

    function isPlainObject(o) {
        return !!o && typeof o === 'object' && o.constructor === Object;
    }
    var commentNodesHaveTextProperty = document && document.createComment("test").text === "<!--test-->";
    var startCommentRegex = commentNodesHaveTextProperty ? /^<!--\s*ko(?:\s+([\s\S]+))?\s*-->$/ : /^\s*ko(?:\s+([\s\S]+))?\s*$/;
    var supportsDocumentFragment = document && typeof document.createDocumentFragment === "function";

    function isVirtualNode(node) {
        return (node.nodeType === 8) && startCommentRegex.test(commentNodesHaveTextProperty ? node.text : node.nodeValue);
    }

    function makeTemplateNode(sourceNode) {
        var container = document.createElement("div");
        var parentNode;
        if (sourceNode.content) {
            parentNode = sourceNode.content;
        } else if (sourceNode.tagName === 'SCRIPT') {
            parentNode = document.createElement("div");
            parentNode.innerHTML = sourceNode.text;
        } else {
            parentNode = sourceNode;
        }
        ko.utils.arrayForEach(ko.virtualElements.childNodes(parentNode), function(child) {
            if (child) {
                container.insertBefore(child.cloneNode(true), null);
            }
        });
        return container;
    }

    function valueToChangeAddItem(value, index) {
        return {
            status: 'added',
            value: value,
            index: index
        };
    }

    function createSymbolOrString(identifier) {
        return typeof Symbol === 'function' ? Symbol(identifier) : identifier;
    }
    var PENDING_DELETE_INDEX_KEY = createSymbolOrString("_ko_ffe_pending_delete_index");

    function FastForEach(spec) {
        this.element = spec.element;
        this.container = isVirtualNode(this.element) ?
            this.element.parentNode : this.element;
        this.$context = spec.$context;
        this.data = spec.data;
        this.as = spec.as;
        this.noContext = spec.noContext;
        this.noIndex = spec.noIndex;
        this.afterAdd = spec.afterAdd;
        this.beforeRemove = spec.beforeRemove;
        this.afterRender = spec.afterRender;
        this.templateNode = makeTemplateNode(
            spec.templateNode || (spec.name ? document.getElementById(spec.name).cloneNode(true) : spec.element)
        );
        this.afterQueueFlush = spec.afterQueueFlush;
        this.beforeQueueFlush = spec.beforeQueueFlush;
        this.changeQueue = [];
        this.firstLastNodesList = [];
        this.indexesToDelete = [];
        this.rendering_queued = false;
        this.pendingDeletes = [];
        ko.virtualElements.emptyNode(this.element);
        var primeData = ko.unwrap(this.data);
        if (primeData.map) {
            this.onArrayChange(primeData.map(valueToChangeAddItem), true);
        }
        if (ko.isObservable(this.data)) {
            if (!this.data.indexOf) {
                this.data = this.data.extend({ trackArrayChanges: true });
            }
            this.changeSubs = this.data.subscribe(this.onArrayChange, this, 'arrayChange');
        }
    }
    FastForEach.PENDING_DELETE_INDEX_KEY = PENDING_DELETE_INDEX_KEY;
    FastForEach.animateFrame = window.requestAnimationFrame || window.webkitRequestAnimationFrame ||
        window.mozRequestAnimationFrame || window.msRequestAnimationFrame ||
        function(cb) { return window.setTimeout(cb, 1000 / 60); };
    FastForEach.prototype.dispose = function() {
        if (this.changeSubs) {
            this.changeSubs.dispose();
        }
        this.flushPendingDeletes();
    };
    FastForEach.prototype.onArrayChange = function(changeSet, isInitial) {
        var self = this;
        var changeMap = {
            added: [],
            deleted: []
        };
        for (var i = 0, len = changeSet.length; i < len; i++) {
            if (changeMap.added.length && changeSet[i].status == 'added') {
                var lastAdd = changeMap.added[changeMap.added.length - 1];
                var lastIndex = lastAdd.isBatch ? lastAdd.index + lastAdd.values.length - 1 : lastAdd.index;
                if (lastIndex + 1 == changeSet[i].index) {
                    if (!lastAdd.isBatch) {
                        lastAdd = {
                            isBatch: true,
                            status: 'added',
                            index: lastAdd.index,
                            values: [lastAdd.value]
                        };
                        changeMap.added.splice(changeMap.added.length - 1, 1, lastAdd);
                    }
                    lastAdd.values.push(changeSet[i].value);
                    continue;
                }
            }
            changeMap[changeSet[i].status].push(changeSet[i]);
        }
        if (changeMap.deleted.length > 0) {
            this.changeQueue.push.apply(this.changeQueue, changeMap.deleted);
            this.changeQueue.push({ status: 'clearDeletedIndexes' });
        }
        this.changeQueue.push.apply(this.changeQueue, changeMap.added);
        if (this.changeQueue.length > 0 && !this.rendering_queued) {
            this.rendering_queued = true;
            if (isInitial) {
                self.processQueue();
            } else {
                FastForEach.animateFrame.call(window, function() { self.processQueue(); });
            }
        }
    };
    FastForEach.prototype.processQueue = function() {
        var self = this;
        var lowestIndexChanged = MAX_LIST_SIZE;
        if (typeof this.beforeQueueFlush === 'function') {
            this.beforeQueueFlush(this.changeQueue);
        }
        ko.utils.arrayForEach(this.changeQueue, function(changeItem) {
            if (typeof changeItem.index === 'number') {
                lowestIndexChanged = Math.min(lowestIndexChanged, changeItem.index);
            }
            self[changeItem.status](changeItem);
        });
        this.flushPendingDeletes();
        this.rendering_queued = false;
        if (typeof this.afterRender == 'function') {
            this.afterRender();
        }
        if (!this.noIndex) {
            this.updateIndexes(lowestIndexChanged);
        }
        if (typeof this.afterQueueFlush === 'function') {
            this.afterQueueFlush(this.changeQueue);
        }
        this.changeQueue = [];
    };

    function extendWithIndex(context) {
        context.$index = ko.observable();
    };
    FastForEach.prototype.added = function(changeItem) {
        var index = changeItem.index;
        var valuesToAdd = changeItem.isBatch ? changeItem.values : [changeItem.value];
        var referenceElement = this.getLastNodeBeforeIndex(index);
        var allChildNodes = [];
        for (var i = 0, len = valuesToAdd.length; i < len; ++i) {
            var childNodes;
            var pendingDelete = this.getPendingDeleteFor(valuesToAdd[i]);
            if (pendingDelete && pendingDelete.nodesets.length) {
                childNodes = pendingDelete.nodesets.pop();
            } else {
                var templateClone = this.templateNode.cloneNode(true);
                var childContext;
                if (this.noContext) {
                    childContext = this.$context.extend({
                        $item: valuesToAdd[i],
                        $index: this.noIndex ? undefined : ko.observable()
                    });
                } else {
                    childContext = this.$context.createChildContext(valuesToAdd[i], this.as || null, this.noIndex ? undefined : extendWithIndex);
                }
                ko.applyBindingsToDescendants(childContext, templateClone);
                childNodes = ko.virtualElements.childNodes(templateClone);
            }
            allChildNodes.push.apply(allChildNodes, Array.prototype.slice.call(childNodes));
            this.firstLastNodesList.splice(index + i, 0, { first: childNodes[0], last: childNodes[childNodes.length - 1] });
        }
        if (typeof this.afterAdd === 'function') {
            this.afterAdd({
                nodeOrArrayInserted: this.insertAllAfter(allChildNodes, referenceElement),
                foreachInstance: this
            });
        } else {
            this.insertAllAfter(allChildNodes, referenceElement);
        }

        if (typeof this.afterRender == 'function') {
            this.afterRender();
        }
    };
    FastForEach.prototype.getNodesForIndex = function(index) {
        var result = [],
            ptr = this.firstLastNodesList[index].first,
            last = this.firstLastNodesList[index].last;
        result.push(ptr);
        while (ptr && ptr !== last) {
            ptr = ptr.nextSibling;
            result.push(ptr);
        }
        return result;
    };
    FastForEach.prototype.getLastNodeBeforeIndex = function(index) {
        if (index < 1 || index - 1 >= this.firstLastNodesList.length)
            return null;
        return this.firstLastNodesList[index - 1].last;
    };
    FastForEach.prototype.insertAllAfter = function(nodeOrNodeArrayToInsert, insertAfterNode) {
        var frag, len, i,
            containerNode = this.element;
        if (nodeOrNodeArrayToInsert.nodeType === undefined && nodeOrNodeArrayToInsert.length === undefined) {
            throw new Error("Expected a single node or a node array");
        }
        if (nodeOrNodeArrayToInsert.nodeType !== undefined) {
            ko.virtualElements.insertAfter(containerNode, nodeOrNodeArrayToInsert, insertAfterNode);
            return [nodeOrNodeArrayToInsert];
        } else if (nodeOrNodeArrayToInsert.length === 1) {
            ko.virtualElements.insertAfter(containerNode, nodeOrNodeArrayToInsert[0], insertAfterNode);
        } else if (supportsDocumentFragment) {
            frag = document.createDocumentFragment();
            for (i = 0, len = nodeOrNodeArrayToInsert.length; i !== len; ++i) {
                frag.appendChild(nodeOrNodeArrayToInsert[i]);
            }
            ko.virtualElements.insertAfter(containerNode, frag, insertAfterNode);
        } else {
            for (i = nodeOrNodeArrayToInsert.length - 1; i >= 0; --i) {
                var child = nodeOrNodeArrayToInsert[i];
                if (!child) { break; }
                ko.virtualElements.insertAfter(containerNode, child, insertAfterNode);
            }
        }
        return nodeOrNodeArrayToInsert;
    };
    FastForEach.prototype.shouldDelayDeletion = function(data) {
        return data && (typeof data === "object" || typeof data === "function");
    };
    FastForEach.prototype.getPendingDeleteFor = function(data) {
        var index = data && data[PENDING_DELETE_INDEX_KEY];
        if (index === undefined) return null;
        return this.pendingDeletes[index];
    };
    FastForEach.prototype.getOrCreatePendingDeleteFor = function(data) {
        var pd = this.getPendingDeleteFor(data);
        if (pd) {
            return pd;
        }
        pd = {
            data: data,
            nodesets: []
        };
        data[PENDING_DELETE_INDEX_KEY] = this.pendingDeletes.length;
        this.pendingDeletes.push(pd);
        return pd;
    };
    FastForEach.prototype.deleted = function(changeItem) {
        if (this.shouldDelayDeletion(changeItem.value)) {
            var pd = this.getOrCreatePendingDeleteFor(changeItem.value);
            pd.nodesets.push(this.getNodesForIndex(changeItem.index));
        } else {
            this.removeNodes(this.getNodesForIndex(changeItem.index));
        }
        this.indexesToDelete.push(changeItem.index);
    };
    FastForEach.prototype.removeNodes = function(nodes) {
        if (!nodes.length) { return; }
        var removeFn = function() {
            var parent = nodes[0].parentNode;
            for (var i = nodes.length - 1; i >= 0; --i) {
                ko.cleanNode(nodes[i]);
                parent.removeChild(nodes[i]);
            }
        };
        if (this.beforeRemove) {
            var beforeRemoveReturn = this.beforeRemove({
                nodesToRemove: nodes,
                foreachInstance: this
            }) || {};
            if (typeof beforeRemoveReturn.then === 'function') {
                beforeRemoveReturn.then(removeFn, ko.onError ? ko.onError : undefined);
            }
        } else {
            removeFn();
        }
    };
    FastForEach.prototype.flushPendingDeletes = function() {
        for (var i = 0, len = this.pendingDeletes.length; i != len; ++i) {
            var pd = this.pendingDeletes[i];
            while (pd.nodesets.length) {
                this.removeNodes(pd.nodesets.pop());
            }
            if (pd.data && pd.data[PENDING_DELETE_INDEX_KEY] !== undefined)
                delete pd.data[PENDING_DELETE_INDEX_KEY];
        }
        this.pendingDeletes = [];
    };
    FastForEach.prototype.clearDeletedIndexes = function() {
        for (var i = this.indexesToDelete.length - 1; i >= 0; --i) {
            this.firstLastNodesList.splice(this.indexesToDelete[i], 1);
        }
        this.indexesToDelete = [];
    };
    FastForEach.prototype.getContextStartingFrom = function(node) {
        var ctx;
        while (node) {
            ctx = ko.contextFor(node);
            if (ctx) { return ctx; }
            node = node.nextSibling;
        }
    };
    FastForEach.prototype.updateIndexes = function(fromIndex) {
        var ctx;
        for (var i = fromIndex, len = this.firstLastNodesList.length; i < len; ++i) {
            ctx = this.getContextStartingFrom(this.firstLastNodesList[i].first);
            if (ctx) { ctx.$index(i); }
        }
    };
    ko.bindingHandlers.fastForEach = {
        init: function init(element, valueAccessor, bindings, vm, context) {
            var ffe, value = valueAccessor();
            if (isPlainObject(value)) {
                value.element = value.element || element;
                value.$context = context;
                ffe = new FastForEach(value);
            } else {
                ffe = new FastForEach({
                    element: element,
                    data: ko.unwrap(context.$rawData) === value ? context.$rawData : value,
                    $context: context,
                    afterRender: value.afterRender
                });
            }
            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
                ffe.dispose();
            });
            return { controlsDescendantBindings: true };
        },
        FastForEach: FastForEach
    };
    ko.virtualElements.allowedBindings.fastForEach = true;
    window.ko = ko;
    return ko;
})