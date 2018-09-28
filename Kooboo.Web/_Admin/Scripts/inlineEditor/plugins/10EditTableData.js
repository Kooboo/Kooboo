function EditTableData(){
    var rowClass = "kooboo-hover",
        colClass = "kooboo-hover",
        opRowSelector = "tr.kooboo-op-row",
        opColumnSelector = "td.kooboo-op-column",
        opColumnRowSelector = "tr:has(td.kooboo-op-column)",
        contentTrSelector = "> tbody > tr,> thead > tr,> tr",
        contentNoOpTrSelector = "> tbody > tr:not(:has( > .kooboo-op-column)),> thead > tr:not(:has( > .kooboo-op-column)),> tr:not(:has( > .kooboo-op-column))",
        contentNoOpColumnSelector = "td:not(.kooboo-op-column),th";
    var param={
        context:null,
        $table:null,
        btnRowTemplate : '<tr class="kooboo-op-row"><td class="table-action"><a class="btn btn-xs btn-default btnRemove" href="javascript:;" title="Remove"><i class="fa fa-minus"></i></a></td><td class="table-action"><a class="btn btn-xs btn-default btnCopy" href="javascript:;" title="Copy"><i class="fa fa-plus"></i></a></td></tr>',
        btnColumnTemplate : '<td class="kooboo-op-column"><a href="javascript:;" class="btn btn-xs btn-default btnRemoveHead" title="Remove"><i class="fa fa-minus"></i></a><a href="javascript:;" class="btn btn-xs btn-default btnCopyHead" title="Copy"><i class="fa fa-plus"></i></a></td>',
        mainTableClass : ".tpMainTable",
        tableContainerClass : ".tpTableContainer",
        oldValue:""
    }

    function getTable(context) {
        var el = context.el;
        if (!Kooboo.PluginHelper.isBodyTag(el)) {
            return $(el).closest("table");
        }
        return null;
    }
    function isTable(context) {
        var table = getTable(context);
        var isTable = table && table.length>0;
        return isTable;
    }
    function setButtons() {
        var $trs = param.innerTable.find(contentNoOpTrSelector),
            colCount = $trs.eq(0).find("> td, > th").length,
            tmp;
        // add row button
        $trs.each(function() {
            Kooboo.PluginHelper.getElementSelector(param.mainTableClass).append(param.btnRowTemplate);
        });
        //add column button
        for (var i = 0; i < colCount; i++) {
            tmp += param.btnColumnTemplate;
        }
        tmp = "<tr>" + tmp + "</tr>";
        $trs.eq(0).before(tmp);
    }

    function setRowSpan() {
        var rowSpan=Kooboo.PluginHelper.getElementSelector(param.mainTableClass).find(opRowSelector).length + 1;
        Kooboo.PluginHelper.getElementSelector(param.tableContainerClass).attr("rowspan",rowSpan);
    }
    function adjustHeight() {
        param.innerTable.find(contentTrSelector).each(function(i) {
            Kooboo.PluginHelper.getElementSelector(param.mainTableClass).find(contentTrSelector).eq(i).height($(this).height());
        });
    }
    function headMouseOver(e){
        getColumn(e, true).addClass(colClass);
    }
    function headMouseout(e){
        getColumn(e, true).removeClass(colClass);
    }
    function getColumn(e, withoutOp) {
        var index = $(e.target).closest("tr").find("> td").index($(e.target).closest("td")) + 1,
            suffix = ":nth-child(" + index + ")" + (withoutOp ? ":not(.kooboo-op-column)" : ""),
            selector = ["> tbody > tr > td" + suffix,
                "> thead > tr > th" + suffix,
                "> thead > tr > td" + suffix,
                "> tr > td" + suffix,
                "> tr > th" + suffix
            ].join(",");
        return param.innerTable.find(selector);
    }
    function removeColumn(e){
        getColumn(e).remove();
        clearTable();
        setData();
    }
    function copyColumn(e){
        getColumn(e).each(function() {
            var cloneEl = $(this).clone()[0];
            $(cloneEl).removeClass(colClass).insertAfter(this);
            Kooboo.KoobooElementManager.resetElementKoobooId(cloneEl);
        });
        setData();
    }
    function edit(e){
        e.preventDefault();
        e.stopPropagation();
        var el = e.currentTarget;
        if (param.editor &&
            param.editor.ed &&
            param.editor.ed.targetElm &&
            param.editor.ed.targetElm.contains(el)) {
            return;
        }
        if (param.editor && param.editor.ed && param.editor.ed.getDoc()) {
            param.editor.ed.save();
            param.editor.ed.remove();
        }

        //var _adjustHeight = _.throttle(_.bind(this._adjustHeight, this), 200);
        var editor = new Kooboo.KoobooTinymceEditor(window,{
            el: el,
            initFn: function() {},
            saveFn: function() {
                setData(el);
            },
            pickImage: window.__gl.siteEditor.showMediagrid,
            pickPage: window.__gl.siteEditor.showPickPage
        });
       
        editor.create();
        param.editor=editor;
    }
    function setData() {
        tinymceSave();

        var table = Kooboo.PluginHelper.getElementSelector(param.tableContainerClass).find("table");
        var cloneTable = table[0].cloneNode(true);
        $(cloneTable).find("tr:first").remove();
        var newValue = $(cloneTable).html();

        var el = param.$table[0];
        param.context.editManager.editTreeDom({
            logType: Kooboo.SiteEditorTypes.LogType.tempLog,
            // domOperationDetailType: SiteEditorTypes.DomOperationDetailType.editTreeData,
            context: param.context,
            el: param.$table[0],
            oldValue: param.oldValue,
            newValue: newValue
        });
    }
    function clearTable() {
        if (!param.innerTable.find(contentNoOpColumnSelector).length || 
            !Kooboo.PluginHelper.getElementSelector(param.mainTableClass).find(opRowSelector).length) {
                param.innerTable.find(contentTrSelector).remove();
            Kooboo.PluginHelper.getElementSelector(param.mainTableClass).find(opRowSelector).remove();
        }
    }

    function trMouseOver(e){
        getRow(e).addClass(rowClass);
    }

    function trMouseout(e){
        getRow(e).removeClass(rowClass);
    }
    function getRow(e) {
        var index = Kooboo.PluginHelper.getElementSelector(param.mainTableClass).find(opRowSelector).index($(e.target).closest("tr"));
        return param.innerTable.find(contentNoOpTrSelector).eq(index);
    }
    function removeRow(e){
        getRow(e).remove();
        $(e.target).closest("tr").remove();
        setRowSpan();
        clearTable();
        adjustHeight();
        setData();
    }
    function copyRow(e){
        var $row = getRow(e),
            newTr = $row.clone()[0],
            opRow = $(e.target).closest("tr");
        $(newTr).find("*").andSelf().removeClass(rowClass);
        $(newTr).insertAfter($row);

        Kooboo.KoobooElementManager.resetElementKoobooId(newTr);
        opRow.clone().insertAfter(opRow);
        setRowSpan();
        clearTable();
        adjustHeight();
        setData();
    }
    function adjustHeight() {
        param.innerTable.find(contentTrSelector).each(function(i) {
            Kooboo.PluginHelper.getElementSelector(param.mainTableClass).find(contentTrSelector).eq(i).height($(this).height());
        });
    }
    function tinymceSave() {
        if (param.editor && param.editor.ed && param.editor.ed.getDoc()) {
            param.editor.ed.save();
            param.editor.ed.remove();
        }
    }
    return {
        dialogSetting: {
            title: Kooboo.text.inlineEditor.editData,
            width: "1000px",
        },
        menuName: Kooboo.text.inlineEditor.editData,
        isShow: function(context) {
            param.context = context;
            if (Kooboo.PluginHelper.isBindVariable()) return false;
            var tree = Kooboo.PluginHelper.getTree(context);
            if (tree && ["td", "tr"].indexOf(tree[0].tagName.toLocaleLowerCase()) == -1) {
                return false;
            }
            table = getTable(context);
            var istable = isTable(context);
            var $parent;
            if (istable && table.parent()) {
                $parent = table.parent();
            }
            if ($parent && $parent.length > 0) {
                var html = $parent.html();
                if (Kooboo.PluginHelper.isContainDynamicData(html)) return false;
            }
            return !Kooboo.PluginHelper.isBodyTag(context.el) && 
                    istable && 
                    Kooboo.PluginHelper.isCanOperateType(context) && 
                    !Kooboo.PluginHelper.isEmptyKoobooId(context.el);
        },
        getHtml:function(){
            k.setHtml("tableHtml","EditTableData.html");
            param.$table=getTable(param.context);
            var data={
                baseClass:"kb-table-editor",
                $table:param.$table
            }
            var html=_.template(tableHtml)(data);
            return html;
        },
        init:function(){
            param.oldValue = Kooboo.InlineEditor.cleanUnnecessaryHtml(param.$table);
            param.innerTable = Kooboo.PluginHelper.getElementSelector(param.tableContainerClass).find("> table");

            setButtons();
            param.innerTable.addClass("table");
            setRowSpan();
            adjustHeight();

            var $tableContainer = Kooboo.PluginHelper.getElementSelector(param.tableContainerClass),
                $mainTable = Kooboo.PluginHelper.getElementSelector(param.mainTableClass);
         
            $tableContainer.on("mouseover",opColumnSelector,headMouseOver);
            $tableContainer.on("mouseout",opColumnSelector,headMouseout);

            $tableContainer.on("click",".btnRemoveHead",removeColumn);
            $tableContainer.on("click",".btnCopyHead",copyColumn);
            $tableContainer.on("click",contentNoOpColumnSelector,edit);

            $mainTable.on("mouseover",opRowSelector,trMouseOver);
            $mainTable.on("mouseout",opRowSelector,trMouseout);
            
            $mainTable.on("click",".btnRemove",removeRow);
            $mainTable.on("click",".btnCopy",copyRow);

        
        }
    }
}