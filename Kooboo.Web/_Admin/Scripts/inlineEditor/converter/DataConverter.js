function DataConverter(beforeSave){
    var param={
        _jstreeObj:null,
        _dataType:null,
        treeTextField:"content"
    }
    
    function doGetEditData(jstreeObject) {
        var finalData = {};
        if (!jstreeObject || !jstreeObject._model || !jstreeObject._model.data) return finalData;
        var collectedId = [],
            treeData = jstreeObject._model.data;

        var keys = _.sortBy(Object.keys(treeData), [function(o) {
            return o.indexOf("_") > -1 && parseInt(o.split("_")[1]);
        }]);
        //filter jstree root key
        keys = _.filter(keys, function(key) {
            return key != "#"
        });
        $.each(keys, function(index, key) {
            if (collectedId.indexOf(key) == -1) {
                if (treeData[key].children && treeData[key].children.length) {
                    var children = getChildren(treeData[key].children);
                    if (param._dataType == "Menu") {
                        finalData["children"] = children;
                    } else {
                        $.each(children, function(idx, child) {
                            finalData[idx] = child;
                        });
                    }
                } else {
                    if ($.type(treeData[key].original.customData) == "string") {
                        finalData[treeData[key].text] = treeData[key].original.customData;
                    }
                }
                collectedId.push(key);
            }
        });

        return finalData;

        function getChildren(children) {
            var list = [];
            $.each(children, function(index, key) {
                var childObj = treeData[key],
                    data = childObj.original.customData;

                if (childObj.children && childObj.children.length) {
                    data["children"] = getChildren(childObj.children);
                }
                list.push(data);
                collectedId.push(key);
            });

            return list;
        }
    }
    function addTreeItem() {
        var jstreeObj = param._jstreeObj,
            selected = jstreeObj.get_selected();
        if (!selected.length) return false;

        if (param._dataType == "Menu") {
            addMenuTreeItem();
        } else if (param._dataType == "ContentList") {
            addListTreeItem();
        }
    }
    function addMenuTreeItem() {
        var jstreeObj = param._jstreeObj,
            selected = jstreeObj.get_selected(),
            newItem = {
                url: "",
                name: "",
                template: "",
            };
        var oriData = _.cloneDeep(jstreeObj._model.data[selected].original.customData);
        if (oriData && !oriData["SubItemContainer"]) {
            oriData["SubItemContainer"] = "";
        }

        jstreeObj._model.data[selected].original.customData = oriData;

        selected = jstreeObj.create_node(selected, { icon: "fa fa-folder icon-state-warning", type: "default", customData: newItem });
        if (selected) {
            jstreeObj.edit(selected);
        }
    }
    function addListTreeItem() {
        var jstreeObj = param._jstreeObj,
            selected = jstreeObj.get_selected(),
            newItem = {};
        var temp = _.cloneDeep(param.data);

        //create ListItem Data
        if (temp instanceof Object) {
            $.each(temp, function(key, tempDetail) {
                $.each(tempDetail, function(detailKey, value) {
                    newItem[detailKey] = "";
                })
                return false;
            })
        }

        selected = jstreeObj.create_node(selected, { icon: "fa fa-file icon-state-warning", type: "file", customData: newItem });
        if (selected) {
            jstreeObj.edit(selected);
        }
    }
    function deleteTreeItem() {
        var jstreeObj = param._jstreeObj,
            selected = jstreeObj.get_selected();

        if (selected && selected.length) {

            var parentId = jstreeObj._model.data[selected[0]].parent;
            if (!jstreeObj._model.data[parentId].children.length) {
                delete jstreeObj._model.data[parentId].original.customData["SubItemContainer"];
            }
            jstreeObj.delete_node(selected);
            $(".data-converter").find(".btnAdd").hide();
            $(".data-converter").find(".btnDelete").hide();
            $(".data-converter").find(".pEditContainer").empty();
        } else {
            return false;
        }
    }
    function generateJsTree(jstreeData) {
        $(".data-converter").find(".pMenuContainer").jstree({
                "core": {
                    "check_callback": true,
                    "data": function(obj, cb) {
                        cb.call(this, jstreeData);
                    },
                    "themes": {
                        "responsive": true
                    }
                },
                "types": {
                    "default": {
                        "icon": "fa fa-folder icon-state-warning"
                    },
                    "file": {
                        "icon": "fa fa-file icon-state-warning"
                    }
                },
                "plugins": ["types"]
            })
            .on("loaded.jstree", function(e, tree) {
                param._jstreeObj = tree.instance;
            }).on("changed.jstree", function(e, data) {
                if (data.selected && data.selected.length) {
                    var jstreeData = param._jstreeObj._model.data;
                    var selected = data.selected[0];
                    if (jstreeData && jstreeData[selected] && jstreeData[selected].original) {
                        var originalData=jstreeData[selected].original;
                        originalData.text=jstreeData[selected].text;
                        if(param._dataType=="Menu"){
                            if(originalData.customData && originalData && !originalData.name &&originalData.text && !originalData.existNode){
                                originalData.customData.name=originalData.text;
                            }
                        }else if(param._dataType=="ContentList"){

                            if( originalData.customData && !originalData.existNode){
                                originalData.customData[param.treeTextField]=originalData.text
                            }
                                
                        }
                        generateArea(originalData);
                    }
                }
            }).on("select_node.jstree", function(e, selected) {
                controllBtnVisible(selected);

            });
    }
    function controllBtnVisible(selected) {
        var selectedText = selected.node.text;
        var $btnAdd = $(".data-converter").find(".btnAdd");
        var $btnDelete = $(".data-converter").find(".btnDelete");
        switch (selectedText) {
            case "List": //contentList
            case "children": //menu
                $btnAdd.css("display", "inline-block");
                $btnDelete.hide();
                break;
            case "SubItemContainer": //menu
                $btnAdd.hide();
                $btnDelete.hide();
                break;
            default:
                $btnDelete.css("display", "inline-block");
                $btnAdd.hide();
                // selectedNodeType ?
                //     $btnAdd.css("display", "inline-block") :
                //     $btnAdd.hide();
                break;
        }
    }
    function generateArea(treeOriginal) {
        var originalData = treeOriginal.customData,
            $pEditContainer = $(".data-converter").find(".pEditContainer");
        $pEditContainer.empty();
        switch ($.type(originalData)) {
            case "object":
                $.each(originalData, function(key, data) {
                    if (key !== "children") {

                        var formGroup = $("<div>");
                        $(formGroup).addClass("form-group");

                        //add lable
                        var label = $("<lable>");
                        var showLabelKey = getShowKeyName(key);
                        $(label).text(showLabelKey);

                        //add input
                        var input = $("<textarea>");
                        $(input).addClass("form-control autosize").attr("data-key", key).css({
                            "min-height": 0
                        }).val(originalData[key]);

                        $(input).on("blur", function() {
                            var saveKey = $(this).attr("data-key");
                            originalData[saveKey] = $(this).val();
                            //select Text
                            if((param._dataType=="Menu" && saveKey == "name")||
                                (param._dataType=="ContentList" && saveKey == param.treeTextField)){
                                    resetSelectNodeText($(this).val());
                            }
                        });

                        $(formGroup).append(label).append(input);

                        $pEditContainer.append(formGroup);
                    }
                })
                break;
            case "string": //SubItemContainer
                var input = $("<textarea>");
                $(input).addClass("form-control autosize").css({
                    "min-height": 120
                }).val(originalData);

                $(input).on("blur", function() {
                    treeOriginal.customData = $(this).val();
                });

                $pEditContainer.append(input);
                break;
        }
        setTimeout(function() {
            $(".autosize").textareaAutoSize().trigger("keyup");
        }, 20);
    }
    function getShowKeyName(key) {
        if (key == "url")
            key = "href";
        else if (key == "name") {
            key = "text";
        }
        return key;
    }
    function resetSelectNodeText(newText) {
        var jstree = $(".data-converter").find(".pMenuContainer").jstree(true);
        var selectednode = jstree.get_selected(true);
        newText=getTreeTextValue(newText);
        jstree.rename_node(selectednode, newText);
    }
    function getTreeData(data, type) {
        var keys = Object.keys(data);
        param._dataType = type;

        var jstreeData = [];

        if (param._dataType == "Menu") {
            jstreeData = getMenuData(data);
        } else if (param._dataType == "ContentList") {
            jstreeData = getContentListData(data);
        }
        return jstreeData;
    }
    function getMenuData(data) {
        var keys = Object.keys(data),
            menuData = [];
        $.each(data, function(key, dataDetail) {
            var menuItem = {};
            var text=getTreeTextValue(key);
            if (dataDetail instanceof Array) {
                var children = getTreeChildrenData(dataDetail);
                menuItem = { text: text,existNode:true, children: children, type: "default" };
            } else {
                menuItem = { text: text,existNode:true, icon: "fa fa-file icon-state-warning", type: "file", customData: dataDetail };
            }
            menuData.push(menuItem);
        });
        return menuData;
    }
    function getTreeTextField(data){
        var field="content";
        var contentFields=[];
        $.each(data, function(key, dataDetail) {
            var keys=Object.keys(dataDetail);
            contentFields=_.filter(keys,function(detailKey){return detailKey.indexOf("content")==0});
            return false;
        });
        contentFields=_.sortBy(contentFields);

        $.each(contentFields,function(key,contentField){
            var hasValueContents=_.filter(data,function(detail){return detail[contentField]});
            if(hasValueContents.length>0){
                field=contentField;
                return false;;
            }
        });

        return field;
    }
    //some filed has html elements
    function getTreeTextValue(value){
        if(!value) return "";
        //html with <br> will cause $(value) error,so use $.parseHTML
        var parseValue=$($.parseHTML(value));
        if(parseValue.length==0) return value;
        return parseValue.text();
    }
    function getContentListData(data) {
        var listData = [];
        var listTitleItem = {
            text: "List",
            type: "default",
            icon: "fa fa-folder icon-state-warning"
        };

        var childListItem = [];
        param.treeTextField=getTreeTextField(data);
        $.each(data, function(key, dataDetail) {
            var originalText=dataDetail[param.treeTextField];
            var value=getTreeTextValue(originalText);

            var item = {
                text: value,
                existNode:true,
                icon: "fa fa-file icon-state-warning",
                type: "file",
                customData: dataDetail
            }
            childListItem.push(item);
        });

        listTitleItem["children"] = childListItem;
        listData.push(listTitleItem);
        return listData;
    }
    function getTreeChildrenData(data) {
        var children = [];

        $.each(data, function(key, dataDetial) {
            var text=getTreeTextValue(dataDetial.name);
            var child = {
                text: text,
                existNode:true,
                icon: "fa fa-file icon-state-warning",
                type: "file",
                customData: dataDetial
            };
            if (dataDetial["children"]) {
                var subchildren = getTreeChildrenData(dataDetial["children"]);
                $.extend(child, {
                    children: subchildren,
                    icon: "fa fa-folder icon-state-warning",
                    type: "default"
                });
            }
            children.push(child);
        });

        return children;
    }
    return {
        dialogSetting:{
            title:Kooboo.text.inlineEditor.editData,
            width:"800px",
            zIndex:Kooboo.InlineEditor.zIndex.middleZIndex,
            beforeSave:beforeSave
        },
        getHtml:function(){
            k.setHtml("dataHtml","DataConverter.html");
            var data={
                deletetag : Kooboo.text.common.delete,
                add : Kooboo.text.common.add,
                list : Kooboo.text.common.listName,
                editArea : Kooboo.text.inlineEditor.editArea
            };
            return _.template(dataHtml)(data);
        },
        init:function(){
            $(".data-converter").find(".btnAdd").bind("click", function() {
                addTreeItem();
            });
            $(".data-converter").find(".btnDelete").bind("click", function() {
                deleteTreeItem();
            });
        },
        setData:function(data, type) {
            param.data = _.cloneDeep(data);
            //clear edit content
            $(".data-converter").find(".pEditContainer").empty();
            $(".data-converter").find(".btnDelete").hide();
            $(".data-converter").find(".btnAdd").hide();
    
            param.convertType = type;
            param._jstreeData = getTreeData(param.data, type);
            generateJsTree(param._jstreeData);
        },
        getEditData:function() {
            var jstreeObject = param._jstreeObj;
            return doGetEditData(jstreeObject);
        } 
    }
}