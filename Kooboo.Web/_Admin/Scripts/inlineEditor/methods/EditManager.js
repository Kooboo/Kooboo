function EditManager(){
    var param={
        templateLogs:[],
        updateCallback:null
    }
    return {
        addLog : function(tempLog) {
            var domOperation =Kooboo.dom.DomOperationHelper.getDomOperationByType(tempLog.domOperationType, tempLog);
            domOperation.update(tempLog.newValue);

            switch (tempLog.logType) {
                case Kooboo.SiteEditorTypes.LogType.log:
                    this.change(tempLog);
                    break;
                case Kooboo.SiteEditorTypes.LogType.tempLog:
                    param.templateLogs.push(tempLog);
                    break;
            }
        },


        /**
         * @param log is a object. The following keys are necessary.
         *  isContentImage: true,
            attr: "src",
            koobooId: koobooId,
            el: el,
            oldValue: self.defaultImage.url,
            newValue: url,
         */
        editImages : function(log) {
            log.action = Kooboo.SiteEditorTypes.ActionType.update;
            log.editorType = Kooboo.SiteEditorTypes.EditorType.dom;
            log.domOperationType = Kooboo.SiteEditorTypes.DomOperationType.images;
            log.logType = Kooboo.SiteEditorTypes.LogType.tempLog;
            this.addLog(log);
        },
        /**
         *@param log is a object. The following keys are necessary.
         *  domOperationType: SiteEditorTypes.DomOperationType.attribute/SiteEditorTypes.DomOperationType.links,
            el: self.curEl,
            name: self.getAtrr(self.curEl),
            oldValue: self.oldValue,
            newValue: url
         */
        editLink :function(log) {
            log.action = Kooboo.SiteEditorTypes.ActionType.update;
            log.editorType = Kooboo.SiteEditorTypes.EditorType.dom;
            log.logType = Kooboo.SiteEditorTypes.LogType.tempLog;
            this.addLog(log);
        },

        /**
         * @param log is a object. The following keys are necessary.
         *  domOperationDetailType(SiteEditorTypes.DomOperationDetailType.htmlImageToText)
         *  logType(SiteEditorTypes.LogType.tempLog)
         *  context 
         *  newValue:newvalue is object. ({value:"text",height:"100px",width:"100px"})
         */
        replaceWithText :function(log) {
            this.updateDom(log);
        },
        /**
         * @param log is a object. The following keys are necessary.
         * domOperationDetailType: SiteEditorTypes.DomOperationDetailType.htmlTextToImage,
            context: self.context,
            newValue: newValue
         */
        replaceWithImage : function(log) {
            log.logType = Kooboo.SiteEditorTypes.LogType.tempLog;
            this.updateDom(log);
        },
        /**
         * @param log is a object. The following keys are necessary.
         * el: self.context.el,
            context: self.context,
            oldValue: self.oldValue,
            newValue: newValue
         */
        editImage : function(log) {
            log.action = Kooboo.SiteEditorTypes.ActionType.update;
            log.editorType = Kooboo.SiteEditorTypes.EditorType.dom;
            log.domOperationType = Kooboo.SiteEditorTypes.DomOperationType.image;
            log.logType = Kooboo.SiteEditorTypes.LogType.tempLog;
            this.addLog(log);
        },
        /**
         * @param log is a object. The following keys are necessary.
         * domOperationDetailType: SiteEditorTypes.DomOperationDetailType.editTreeData,
            logType: SiteEditorTypes.LogType.tempLog,
            context: self.context,
            el: self.treeparent,
            oldValue: self.oldValue,
            replaceHtml: newHtml.join(" "),
         */
        editTreeDom : function(log) {
            this.updateDom(log);
        },
        /**
         * @param log is a object. The following keys are necessary.
         *  logType: SiteEditorTypes.LogType.tempLog,
            context: self.context,
            el: self.$table[0],
            oldValue: self.oldValue,
            newValue: newValue
         */
        editTableDom : function(log) {
            this.updateDom(log);
        },

        /**
         * @param log is a object. The following keys are necessary.
         *  
            domOperationDetailType: SiteEditorTypes.DomOperationDetailType.delete,
            logType: SiteEditorTypes.LogType.log,
            context: self.context,
         */
        updateDom : function(log) {
            log.action = Kooboo.SiteEditorTypes.ActionType.update;
            log.editorType = Kooboo.SiteEditorTypes.EditorType.dom;
            log.domOperationType = Kooboo.SiteEditorTypes.DomOperationType.html;
            this.addLog(log);
        },
        deleteDom:function(log){
            log.action = Kooboo.SiteEditorTypes.ActionType.delete;
            log.editorType = Kooboo.SiteEditorTypes.EditorType.dom;
            log.domOperationType = Kooboo.SiteEditorTypes.DomOperationType.html;
            this.addLog(log);
        },
        /**
         * @param log is a object. The following keys are necessary.
         * 
            domOperationType: SiteEditorTypes.DomOperationType.styles,
            el: self.context.el,
            cssRule: cssRule,
            jsonRule: jsonRule,
            property: property,
            important: jsonRule.important,
            oldValue: cssRule[property],
            newValue: "",
            shorthandProperty: shorthandProperty
         */
        updateStyle : function(log) {
            log.action = Kooboo.SiteEditorTypes.ActionType.update;
            log.editorType = Kooboo.SiteEditorTypes.EditorType.style;
            log.logType = Kooboo.SiteEditorTypes.LogType.tempLog;
            this.addLog(log);
        },
        /**
         * @param log is a object. The following keys are necessary.
         * action: SiteEditorTypes.ActionType.delete,
            editorType: SiteEditorTypes.EditorType.content,
            domOperationType: SiteEditorTypes.DomOperationType.contentRepeater,
            logType: SiteEditorTypes.LogType.log,
            el: parent[0],
            context: self.context,
         */
        removeContent : function(log) {
            this.addLog(log);
        },
        /**
         * @param log is a object. The following keys are necessary.
         *  action: SiteEditorTypes.ActionType.delete,
            editorType: SiteEditorTypes.EditorType.content,
            domOperationType: SiteEditorTypes.DomOperationType.contentRepeater,
            logType: SiteEditorTypes.LogType.log,
            el: parent[0],
            context: self.context,
         */
        editContent : function(log) {
            this.addLog(log)
        },
        /**
         * @param log is a object. The following keys are necessary.
         * action: SiteEditorTypes.ActionType.copy,
            editorType: SiteEditorTypes.EditorType.content,
            domOperationType: SiteEditorTypes.DomOperationType.contentRepeater,
            logType: SiteEditorTypes.LogType.log,
            context: self.context
         */
        copyContent : function(log) {
            this.addLog(log);
        },
        /**
         * @param log is a object. The following keys are necessary.
         *  action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.menu,
            domOperationType: SiteEditorTypes.DomOperationType.menu,
            logType: SiteEditorTypes.LogType.noLog,
            context: self.context,
            newValue: newValue
         */
        editMenu : function(log) {
            this.addLog(log);
        },
        /**
         * @param log is a object. The following keys are necessary.
         *  action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.htmlblock,
            domOperationType: SiteEditorTypes.DomOperationType.htmlblock,
            logType: SiteEditorTypes.LogType.noLog,
            context: self.context,
            newValue: newValue
         */
        editHtmlBlock : function(log) {
            this.addLog(log);
        },
        /**
         * @param log is a object. The following keys are necessary.
         * action: SiteEditorTypes.ActionType.add,
            editorType: SiteEditorTypes.EditorType.converter,
            domOperationType: SiteEditorTypes.DomOperationType.converter,
            logType: SiteEditorTypes.LogType.log,
            currentResult: self.currentResult,
            oldValue: value,
            newValue: value,
            el: el,
            undo: self.undo,
            redo: self.redo
         */
        updateConverter : function(log) {
            this.addLog(log);
        },

        removeLogs : function() {
            if (param.templateLogs.length == 0) return;
            for (var i = param.templateLogs.length - 1; i >= 0; i--) {
                var templateLog = param.templateLogs[i];
                var domOperation = Kooboo.dom.DomOperationHelper.getDomOperationByType(templateLog.domOperationType, templateLog);
                domOperation.update(templateLog.oldValue);
            }
            param.templateLogs = [];
        },
        saveLogs : function() {
            if (param.templateLogs.length == 0) return;
            var mergeTempLog = Kooboo.EditTempLogMerge.merge(param.templateLogs);
            //将日志保存到editLog
            this.change(mergeTempLog);
            param.templateLogs = [];
        },
        // apply a new change and save it in the log
        change : function(log) {
            Kooboo.EditLog.add(log);

            if (param.updateCallback) {
                param.updateCallback();
            }
        },

        // does the edit log have a next item
        hasNext : function() {
            return Kooboo.EditLog.hasNext();
        },

        // does the edit log have a previous item
        hasPrev : function() {
            return Kooboo.EditLog.hasPrevious();
        },

        remove : function(isExistDataFn) {
            var removeItem = Kooboo.EditLog.removeEditLog(isExistDataFn);
            if (removeItem) {
                var domOperation = Kooboo.dom.DomOperationHelper.getDomOperationByType(removeItem.domOperationType, removeItem);
                domOperation.update(removeItem.oldValue);
            }

            if (param.updateCallback) {
                param.updateCallback();
            }
            return removeItem;
        },

        // undo the last change
        undo : function() {
            if (this.hasPrev()) {
                var log = Kooboo.EditLog.getPrevious();
                var changes = [log];
                if (log.changes && log.changes.length > 0) {
                    changes = log.changes;
                }
                for (var i = changes.length - 1; i >= 0; i--) {
                    var change = changes[i];
                    var domOperation = Kooboo.dom.DomOperationHelper.getDomOperationByType(change.domOperationType, change);

                    if (domOperation != null) {
                        domOperation.update(change.oldValue);
                        //converter
                        if (change.undo) {
                            change.undo(change);
                        }
                    }
                }

            }
        },

        // redo the last change
        //mulit change only with one time redo
        redo : function() {
            if (this.hasNext()) {
                var log = Kooboo.EditLog.getNext();
                var changes = [log];
                if (log.changes && log.changes.length > 0) {
                    changes = log.changes;
                }
                for (var i = 0; i <= changes.length - 1; i++) {
                    var change = changes[i];
                    var domOperation = Kooboo.dom.DomOperationHelper.getDomOperationByType(change.domOperationType, change);
                    if (domOperation != null) {
                        domOperation.update(change.newValue);
                        //converter
                        if (change.redo) {
                            change.redo(change);
                        }
                    }
                }

            }
        },

        // convert log to format for saving
        // TODO: improve merging
        convert : function() {
            return Kooboo.EditLog.decorateAndMergeLogs();
        },
        setUpdateCallback : function(fn) {
            param.updateCallback = fn;
        }
    }
}