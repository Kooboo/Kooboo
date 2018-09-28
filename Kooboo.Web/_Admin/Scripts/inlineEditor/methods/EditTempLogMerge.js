function EditTempLogMerge(){
    function isEqualValue(tempLog) {
        return tempLog.oldValue == tempLog.newValue;
    }
    function mergeStyles (tempLogs) {
        var logs = [];
        var logObjs = {};
        //merge same templog
        $.each(tempLogs, function(i, tempLog) {
            var jsonRule = tempLog.jsonRule;
            var log = {
                styleSheetUrl: jsonRule.styleSheetUrl,
                selector: jsonRule.selector,
                property: tempLog.property,
                styleTagKoobooId: jsonRule.styleTagKoobooId,
                koobooId: jsonRule.koobooId
            };
            var key = Kooboo.EditLogHelper.getStyleLogKey(log);
            if (logObjs[key]) {
                tempLog.oldValue = logObjs[key].oldValue
            }
            logObjs[key] = tempLog;
        });
        //remove same value
        $.each(logObjs, function(i, log) {
            if (!isEqualValue(log)) {
                logs.push(log);
            }
        });

        return logs;
    }
    return {
        merge:function(tempLogs) {
            var mergeTempLog = {};
            if (!tempLogs || tempLogs.length == 0) {
                throw ("tempLogs length must be greater than zero");
            }
    
            var domOperationType = tempLogs[0].domOperationType;
            if (domOperationType == Kooboo.SiteEditorTypes.DomOperationType.styles) {
                var changes = mergeStyles(tempLogs);
                mergeTempLog = {
                    action: tempLogs[0].action,
                    editorType: tempLogs[0].editorType,
                    domOperationType: domOperationType
                };
                mergeTempLog.changes = changes;
            } else if (domOperationType == Kooboo.SiteEditorTypes.DomOperationType.links ||
                domOperationType == Kooboo.SiteEditorTypes.DomOperationType.images) {
                mergeTempLog = {
                    action: tempLogs[0].action,
                    editorType: tempLogs[0].editorType,
                    domOperationType: domOperationType
                };
                var changes = [];
                $.each(tempLogs, function(i, tempLog) {
                    if (!isEqualValue(tempLog))
                        changes.push(tempLog);
                });
                mergeTempLog.changes = changes;
            } else {
                if (tempLogs.length == 1) {
                    mergeTempLog = tempLogs[0];
                } else {
                    var lastTempLog = tempLogs[tempLogs.length - 1];
                    lastTempLog.oldValue = tempLogs[0].oldValue;
                    mergeTempLog = lastTempLog;
                }
            }
    
            return mergeTempLog;
        },

    }
}