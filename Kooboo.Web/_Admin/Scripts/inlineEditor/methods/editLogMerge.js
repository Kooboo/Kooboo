function EditLogMerge(){
    function getDoms(editLogs) {
        var doms = {};
        $.each(editLogs, function(i, editLog) {
            if (
                (editLog.action == Kooboo.SiteEditorTypes.ActionType.update||
                editLog.action == Kooboo.SiteEditorTypes.ActionType.delete) &&
                editLog.editorType.toLocaleLowerCase() == Kooboo.SiteEditorTypes.EditorType.dom &&
                !editLog.attributeName) {
                var groupkey = getDomGroup(editLog);
                var groupLogs = [];
                if (doms[groupkey]) {
                    groupLogs = doms[groupkey];
                } else {
                    doms[groupkey] = groupLogs;
                }
                var sameDomLog = findSameDom(groupLogs, editLog);
                if (sameDomLog) {
                    sameDomLog.value = editLog.value;
                } else {
                    groupLogs.push(editLog);
                }

            }
        });
        return doms;
    }
    function findSameDom(groupLogs, editLog) {
        var findLog = _.find(groupLogs, function(log) {
            return log.koobooId == editLog.koobooId;
        });
        return findLog;
    }
    function getDomKey(editlog) {
        var nameOrId = editlog.nameOrId || "";
        var objectType = editlog.objectType || "";
        var koobooId = editlog.koobooId;
        return koobooId + "-" + objectType + "-" + nameOrId;
    }
    function getDomGroup(editlog) {
        var nameOrId = editlog.nameOrId || "";
        var objectType = editlog.objectType || "";
        return objectType + "-" + nameOrId;
    }
    function getAttributes(editLogs) {
        var attributes = {};
        $.each(editLogs, function(i, editLog) {
            if (editLog.action == Kooboo.SiteEditorTypes.ActionType.update &&
                editLog.editorType.toLocaleLowerCase() == Kooboo.SiteEditorTypes.EditorType.dom && editLog.attributeName) {
                var groupKey = getDomGroup(editLog);

                var groupAttrs = attributes[groupKey];
                if (!groupAttrs) {
                    groupAttrs = [];
                }
                $.each(groupAttrs, function(index, attribute) {
                    if (attribute && attribute.koobooId == editLog.koobooId &&
                        attribute.attributeName == editLog.attributeName) {
                        groupAttrs.splice(index, 1);
                    }
                });
                groupAttrs.push(editLog);
                attributes[groupKey] = groupAttrs;
            }
        });
        return attributes;
    }
    function getLabels(editLogs) {
        var labels = {};
        $.each(editLogs, function(i, editLog) {
            if(editLog.editorType.toLocaleLowerCase() == Kooboo.SiteEditorTypes.EditorType.label){
                if (editLog.action == Kooboo.SiteEditorTypes.ActionType.update){
                    labels[editLog["nameOrId"]] = editLog;
                }
            }
        });
        return labels;
    }
    function getContents(editLogs) {
        var contents = {};

        $.each(editLogs, function(i, editLog) {
            if (editLog.editorType.toLocaleLowerCase() == Kooboo.SiteEditorTypes.EditorType.content) {
                var key = editLog["nameOrId"];
                if (editLog.action == Kooboo.SiteEditorTypes.ActionType.update) {

                    if (editLog["fieldName"]) {
                        key += editLog["fieldName"];
                    }
                    contents[key] = editLog;
                } else if (editLog.action == Kooboo.SiteEditorTypes.ActionType.copy) {
                    addContent(contents, editLog);
                } else if (editLog.action == Kooboo.SiteEditorTypes.ActionType.delete) {
                    if (contents[key] &&
                        contents[key].action == Kooboo.SiteEditorTypes.ActionType.copy) {
                        delete contents[key];
                    } else {
                        contents[key] = editLog;
                    }

                }
            }
        });
        return contents;
    }
    function addContent(contents, editLog) {
        contents[editLog["nameOrId"]] = contents[editLog["nameOrId"]] || {};
        contents[editLog["nameOrId"]] = editLog;
        //contents[editLog["nameOrId"]][editLog["fieldName"]] = editLog;
    }
    function getHtmlBlocks(editLogs) {
        var htmlblocks = {};
        $.each(editLogs, function(i, editLog) {
            if (editLog.action == Kooboo.SiteEditorTypes.ActionType.update &&
                editLog.editorType.toLocaleLowerCase() ==Kooboo.SiteEditorTypes.EditorType.htmlblock)
                htmlblocks[editLog["nameOrId"]] = editLog;
        });
        return htmlblocks;
    }
    function getStyles(editLogs, mergeDoms) {
        var stylesObj = {};
        $.each(editLogs, function(i, editLog) {
            if (editLog.action == Kooboo.SiteEditorTypes.ActionType.update &&
                editLog.editorType.toLocaleLowerCase() == Kooboo.SiteEditorTypes.EditorType.style) {
                var key = Kooboo.EditLogHelper.getStyleLogKey(editLog);
                if (key) {
                    stylesObj[key] = editLog;
                }
            }

        });
        var styles = _.values(stylesObj);
        return styles;
    }
    //merge inline style
    function getMergeStyles(styles, mergeDoms) {
        var mergeStyles = [];
        $.each(styles, function(i, style) {
            if (style.koobooId && !style.styleTagKoobooId && !style.styleSheetUrl) {
                var exist = false;
                $.each(mergeDoms, function(j, mergeDom) {
                    if (isChildrenKoobooId(mergeDom.koobooId, style.koobooId) &&
                        mergeDom.nameOrId == style.nameOrId && mergeDom.objectType == style.objectType) {
                        exist = true;
                        mergeDom.value = getNewDomValueById(mergeDom.koobooId, mergeDom);
                        return false;
                    }
                });
                if (!exist) {
                    mergeStyles.push(style);
                }

            } else {
                mergeStyles.push(style);
            }
        });

        return mergeStyles;
    }
    function getConverters(editLogs) {
        var converters = [];
        $.each(editLogs, function(i, editLog) {
            if (editLog.editorType == Kooboo.SiteEditorTypes.EditorType.converter)
                converters.push(editLog);
        });
        return converters;
    }
    function getNewDomValueById(id, log) {
        var fdoc = Kooboo.InlineEditor.getIFrameDoc();
        var html,
            nameOrId = log.nameOrId,
            objectType = log.objectType;
        var el = Kooboo.KoobooElementManager.getElement(id, nameOrId, objectType);
        html = Kooboo.InlineEditor.cleanUnnecessaryHtml(el);

        if (!html) {
            html = "";
        }
        html = Kooboo.EditLogHelper.removeKoobooId(html);
        return html;
    }
    function getKoobooObject(element) {
        var context = Kooboo.elementReader.readObject(element);
        var koobooObjects = context.koobooObjects;
        return koobooObjects;
    }
    function deleteInnerDoms(doms) {
        for (var groupKey in doms) {
            //sort by koobooId key
            var groupLogs = sortByKoobooId(doms[groupKey]);
            var deleteKeys = getDeleteKeys(groupLogs, groupLogs);
            groupLogs = _.remove(doms[groupKey], function(log) {
                return $.inArray(log.koobooId, deleteKeys);
            });
            doms[groupKey] = groupLogs;
        }
        return doms;
    }
    function deleteInnerAttrs(doms, attrs) {
        $.each(doms, function(groupKey, groupLogs) {
            groupLogs = sortByKoobooId(groupLogs);

            var attrGroupLogs = attrs[groupKey];
            if (attrGroupLogs) {
                attrGroupLogs = sortByKoobooId(attrGroupLogs);
                var deleteKeys = getDeleteKeys(groupLogs, attrGroupLogs);
                attrGroupLogs = _.remove(attrGroupLogs, function(log) {
                    return $.inArray(log.koobooId, deleteKeys);
                });
                attrs[groupKey] = attrGroupLogs;
            }
        });
        return attrs;
    }
    function getDeleteKeys(firstGroupLogs, secondGroupLogs) {
        var deleteKeys = [];
        $.each(firstGroupLogs, function(i, log) {
            var parentKey = log.koobooId;
            $.each(secondGroupLogs, function(j, attrLog) {
                var key = attrLog.koobooId;
                if (isChildrenKoobooId(parentKey, key)) {
                    deleteKeys.push(key);
                    log.isChildDelete = 1;
                }
            });
        });
        return deleteKeys;
    }

    //id(1-1-1) is parentId(1-1) children
    function isChildrenKoobooId(parentId, id) {
        return eval("/^" + parentId + "[0-9-]+$/i").test(id);
    }
    function sortByKoobooId(logs) {
        logs = _.sortBy(logs, function(group) { return group.koobooId ? group.koobooId.length:0; });
        return logs;
    }

    function getMergeDom(editLogs) {
        var doms = getDoms(editLogs);
        doms = deleteInnerDoms(doms);
        var attrs = getAttributes(editLogs);
        attrs = deleteInnerAttrs(doms, attrs);
        var mergeDoms = [];
        $.each(attrs, function(groupkey, groupAttrs) {
            $.each(groupAttrs, function(i, attr) {
                mergeDoms.push(attr);
            });

        });

        $.each(doms, function(groupkey, groupDoms) {
            $.each(groupDoms, function(i, log) {
                if (log.isChildDelete) {
                    var koobooId = log["koobooId"];
                    log["value"] = getNewDomValueById(koobooId, log);
                }
                mergeDoms.push(log);

            });
        });
        return mergeDoms;
    }
    return {
        deleteInnerDoms:deleteInnerDoms,
        deleteInnerAttrs:deleteInnerAttrs,

        mergeEditLog: function(editLogs) {
            var result = [];
            var mergeDoms = getMergeDom(editLogs);
            result = result.concat(mergeDoms);

            var styles = getStyles(editLogs);
            var mergeStyles = getMergeStyles(styles, mergeDoms);
            result = result.concat(mergeStyles);

            var htmlblocks = getHtmlBlocks(editLogs);
            result = result.concat(_.values(htmlblocks));

            var labels = getLabels(editLogs);
            result = result.concat(_.values(labels));

            var contents = getContents(editLogs);
            result = result.concat(_.values(contents));

            var converters = getConverters(editLogs);
            result = result.concat(_.values(converters));
            return result;
        },
        
    }
}