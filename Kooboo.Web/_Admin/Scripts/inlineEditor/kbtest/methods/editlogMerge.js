var SiteEditorTypes = Kooboo.SiteEditorTypes;
function mergeEditLog_label(){
    var logs = [{
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.label,
            nameOrId: "123",
            value: "testaaa"
        },
        {
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.label,
            nameOrId: "123",
            value: "test"
        },
    ];

    var labels = Kooboo.EditLogMerge.mergeEditLog(logs);
    expect(Object.keys(labels).length).to.eql(1);
    expect(labels[0].nameOrId).to.eql("123");
    expect(labels[0].value).to.eql("test");
}

function deleteInnerDoms() {
    var doms = {
        "view-test": [{ koobooId: "1-3-1" }, { koobooId: "1-1" }, { koobooId: "1-1-1" }, ]
    };
    var doms = Kooboo.EditLogMerge.deleteInnerDoms(doms);
    var groupDom = doms["view-test"];
    expect(groupDom.length).to.eql(2);

    var existdeleteItem;
    $.each(groupDom, function(i, dom) {
        if (dom.koobooId == "1-1-1") {
            existdeleteItem = dom;
        }
        if (dom.isChildDelete) {
            //children delete
            expect(dom.koobooId).to.eql("1-1");
        } else {
            expect(dom.koobooId).to.eql("1-3-1");
        }
    });
    expect(existdeleteItem).to.eql(undefined);
}

function deleteInnerAttrs() {
    var doms = {
        "view-test": [{ "koobooId": "1-1" }, { "koobooId": "1-2" }, { "koobooId": "1-3-4" }]
    };
    var attrs = {
        "view-test": [{ "koobooId": "1-2-3" }],
        "page-test": [{ "koobooId": "1-2-3" }],
    }
    attrs = Kooboo.EditLogMerge.deleteInnerAttrs(doms, attrs);
    expect(Object.keys(attrs).length).to.eql(2);
    var viewTestGroup = attrs["view-test"];
    expect(viewTestGroup.length).to.eql(0);

    var pageGroup = attrs["page-test"];
    expect(pageGroup.length).to.eql(1);

    var viewDom = doms["view-test"];
    expect(viewDom[0].isChildDelete).to.eql(undefined);
    expect(viewDom[1].koobooId).to.eql("1-2");
    expect(viewDom[1].isChildDelete).to.eql(1);
}

function mergeEditLog_mergeDom(){
    var editLogs = [{
        action: SiteEditorTypes.ActionType.update,
        editorType: SiteEditorTypes.EditorType.dom,
        nameOrId: "test",
        objectType: "view",
        koobooId: "2-1",
        value: "1"
    }, {
        action: SiteEditorTypes.ActionType.update,
        editorType: SiteEditorTypes.EditorType.dom,
        nameOrId: "test",
        objectType: "view",
        koobooId: "2-1",
        value: "2"
    }];
    var doms = Kooboo.EditLogMerge.mergeEditLog(editLogs);
    var groupKey = "view-test";
    expect(doms.length).to.eql(1);
    expect(doms[0].value).to.eql('2');
}

function mergeEditLog_getMergeContent(){
    var logs = [{
            action: SiteEditorTypes.ActionType.copy,
            editorType: SiteEditorTypes.EditorType.content,
            nameOrId: "123",
            value: "test"
        },
        {
            action: SiteEditorTypes.ActionType.delete,
            editorType: SiteEditorTypes.EditorType.content,
            nameOrId: "1231",
            value: "testchild"
        },


    ];

    var contents = Kooboo.EditLogMerge.mergeEditLog(logs);
    expect(Object.keys(contents).length).to.eql(2);
    expect(contents[0].action).to.eql("copy");
    expect(contents[0].value).to.eql("test");

    expect(contents[1].action).to.eql("delete");
    expect(contents[1].value).to.eql("testchild");

    logs = [{
            action: SiteEditorTypes.ActionType.copy,
            editorType: SiteEditorTypes.EditorType.content,
            nameOrId: "123",
            value: "test"
        },
        {
            action: SiteEditorTypes.ActionType.delete,
            editorType: SiteEditorTypes.EditorType.content,
            nameOrId: "123",
            value: "testchild"
        }
    ];
    contents = Kooboo.EditLogMerge.mergeEditLog(logs);
    expect(Object.keys(contents).length).to.eql(0);

    logs = [{
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.content,
            nameOrId: "123",
            value: "test",

        },
        {
            action: SiteEditorTypes.ActionType.delete,
            editorType: SiteEditorTypes.EditorType.content,
            nameOrId: "123",
            value: "testchild"
        }
    ];
    contents = Kooboo.EditLogMerge.mergeEditLog(logs);
    expect(Object.keys(contents).length).to.eql(1);
    expect(contents[0].action).to.eql("delete");
    expect(contents[0].value).to.eql("testchild");

    logs = [{
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.content,
            fieldName: "color",
            nameOrId: "123",
            value: "red"
        },
        {
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.content,
            fieldName: "color",
            nameOrId: "123",
            value: "green"
        },
        {
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.content,
            fieldName: "color",
            nameOrId: "1231",
            value: "blue"
        },
    ];
    contents = Kooboo.EditLogMerge.mergeEditLog(logs);
    expect(Object.keys(contents).length).to.eql(2);
    expect(contents[0].action).to.eql("update");
    expect(contents[0].value).to.eql("green");

    expect(contents[1].action).to.eql("update");
    expect(contents[1].value).to.eql("blue");

    logs = [{
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.content,
            fieldName: "color",
            nameOrId: "123",
            value: "red"
        },
        {
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.content,
            fieldName: "color2",
            nameOrId: "123",
            value: "green"
        },
    ];
    contents = Kooboo.EditLogMerge.mergeEditLog(logs);
    expect(Object.keys(contents).length).to.eql(2);

    expect(contents[0].value).to.eql("red");

    expect(contents[1].value).to.eql("green");


    logs = [{
            action: SiteEditorTypes.ActionType.copy,
            editorType: SiteEditorTypes.EditorType.content,
            fieldName: "color",
            nameOrId: "123",
            value: "red"
        },
        {
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.content,
            fieldName: "color",
            nameOrId: "123",
            value: "green"
        },
    ];
    contents = Kooboo.EditLogMerge.mergeEditLog(logs);
    expect(Object.keys(contents).length).to.eql(2);
    //from copy
    expect(contents[0].value).to.eql("red");
    //from update
    expect(contents[1].value).to.eql("green");
}
function mergeEditLog_style_sameType(){
    //same styleSheet Url
    var logs = [{
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.style,
            styleSheetUrl: "http://xxx/aa.css",
            selector: "div div",
            property: "color",
            value: "green"
        },
        {
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.style,
            styleSheetUrl: "http://xxx/aa.css",
            selector: "div div",
            property: "color",
            value: "red"
        },
    ];
    var styles = Kooboo.EditLogMerge.mergeEditLog(logs);
    expect(styles.length).to.eql(1);
    expect(styles[0]["value"]).to.eql("red");

    //same styleTagKoobooId
    logs = [{
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.style,
            styleTagKoobooId: "1-2",
            selector: "div div",
            property: "color",
            value: "green",
            nameOrId: "123",
            objectType: "page"
        },
        {
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.style,
            styleTagKoobooId: "1-2",
            selector: "div div",
            property: "color",
            value: "red",
            nameOrId: "123",
            objectType: "page"
        },
    ];
    var styles = Kooboo.EditLogMerge.mergeEditLog(logs);
    expect(styles.length).to.eql(1);
    expect(styles[0]["value"]).to.eql("red");

    //same koobooId
    logs = [{
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.style,
            selector: "div div",
            property: "color",
            value: "green",
            nameOrId: "123",
            koobooId: "3-1",
            objectType: "page"
        },
        {
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.style,
            selector: "div div",
            property: "color",
            value: "red",
            nameOrId: "123",
            koobooId: "3-1",
            objectType: "page"
        },
    ];
    var styles = Kooboo.EditLogMerge.mergeEditLog(logs);
    expect(styles.length).to.eql(1);
    expect(styles[0]["value"]).to.eql("red");

    //same selector koobooId
    logs = [{
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.style,
            selector: "div div",
            property: "color",
            value: "green",
            nameOrId: "123",
            objectType: "page"
        },
        {
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.style,
            selector: "div div",
            property: "color",
            value: "red",
            nameOrId: "123",
            objectType: "page"
        },
    ];
    var styles = Kooboo.EditLogMerge.mergeEditLog(logs);
    expect(styles.length).to.eql(1);
    expect(styles[0]["value"]).to.eql("red");
}

function mergeEditLog_styleDiffLog(){
    //diff styleSheet Url
    var logs = [{
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.style,
            styleSheetUrl: "http://xxx/aa.css",
            selector: "div div",
            property: "color",
            value: "green"
        },
        {
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.style,
            styleSheetUrl: "http://xxx/aa.css",
            selector: "div div",
            property: "background",
            value: "red"
        },
    ];
    var styles = Kooboo.EditLogMerge.mergeEditLog(logs);
    expect(styles.length).to.eql(2);

    //diff styleTagKoobooId
    logs = [{
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.style,
            styleTagKoobooId: "1-3",
            selector: "div div",
            property: "color",
            value: "green",
            nameOrId: "123",
            objectType: "page"
        },
        {
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.style,
            styleTagKoobooId: "1-2",
            selector: "div div",
            property: "color",
            value: "red",
            nameOrId: "123",
            objectType: "page"
        },
    ];
    var styles = Kooboo.EditLogMerge.mergeEditLog(logs);
    expect(styles.length).to.eql(2);
    //diff koobooId
    logs = [{
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.style,
            property: "color",
            value: "green",
            nameOrId: "123",
            koobooId: "3-1-1",
            objectType: "page"
        },
        {
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.style,
            property: "color",
            value: "red",
            nameOrId: "123",
            koobooId: "3-1",
            objectType: "page"
        },
    ];
    var styles = Kooboo.EditLogMerge.mergeEditLog(logs);
    expect(styles.length).to.eql(2);

    //diff selector koobooId
    logs = [{
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.style,
            selector: "div .divaa",
            property: "color",
            value: "green",
            nameOrId: "123",
            objectType: "page"
        },
        {
            action: SiteEditorTypes.ActionType.update,
            editorType: SiteEditorTypes.EditorType.style,
            selector: "div div",
            property: "color",
            value: "red",
            nameOrId: "123",
            objectType: "page"
        },
    ];
    var styles = Kooboo.EditLogMerge.mergeEditLog(logs);
    expect(styles.length).to.eql(2);
}
function mergeEditLog_getConverters(){
    var logs = [{
            action: SiteEditorTypes.ActionType.add,
            editorType: SiteEditorTypes.EditorType.converter,
            value: "test"
        },
        {
            action: SiteEditorTypes.ActionType.add,
            editorType: SiteEditorTypes.EditorType.converter,
            value: "testabc"
        },
    ];
    var converters = Kooboo.EditLogMerge.mergeEditLog(logs);
    expect(converters.length).to.eql(2);
    expect(converters[0]["value"]).to.eql("test");
    expect(converters[1]["value"]).to.eql("testabc");
}