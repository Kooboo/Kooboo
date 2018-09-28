function EditLog(){
    var list = [];
    var itemindex = -1;
    return {
        clear:function(){
            list=[],
            itemindex=-1;
        },

        add : function(item) {
            if (this.hasNext()) {
                list.splice(itemindex + 1);
            }
            list.push(item);
            itemindex++;
        },
        hasPrevious : function() {
            if (itemindex > -1) {
                return true;
            }
            return false;
        },

        hasNext : function() {
            if (list.length > 0 && itemindex < list.length - 1) {
                return true;
            }
            return false;
        },
        //get the previous item, when click undo.
        getPrevious : function() {
            if (this.hasPrevious()) {
                var log = list[itemindex];
                itemindex--;
                return log;
            }
        },
        // get the next item when click redo.
        getNext : function() {
            if (this.hasNext()) {
                itemindex++;
                var log = list[itemindex];
                return log;
            }
            return null;
        },
        count : function() {
            return list.length;
        },
        removeEditLog : function(isExistDataFn) {
            var index = _.findLastIndex(list, function(item) {
                return isExistDataFn(item);
            });
            var removeItem = null;
            if (index > -1) {
                removeItem = list.splice(index, 1)[0];
                if (index <= itemindex) {
                    itemindex = itemindex - 1;
                }
            }
            return removeItem;
        },
        decorateAndMergeLogs : function() {
            var decorateLogs = [];
            for (var i = 0; i <= itemindex; i++) {
                var splitItems = Kooboo.EditLogSplit.getSplitItems(list[i]);
                splitItems.forEach(function(splitItem) {
                    var decorateLog = Kooboo.EditLogHelper.convert(splitItem);
                    if (decorateLog instanceof Array) {
                        decorateLogs = decorateLogs.concat(decorateLog);
                    } else {
                        decorateLogs.push(decorateLog);
                    }
                });
            }
            return Kooboo.EditLogMerge.mergeEditLog(decorateLogs);
        }
    }
}