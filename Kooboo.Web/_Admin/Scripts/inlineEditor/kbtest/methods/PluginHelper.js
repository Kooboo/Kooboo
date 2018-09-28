var SiteEditorTypes = Kooboo.SiteEditorTypes;
var PluginHelper = Kooboo.PluginHelper;

function isCanOperateType() {
    var context = {
        koobooObjects: [{
            type: SiteEditorTypes.ObjectType.htmlblock,
            nameOrId: "htmlblock"
        }, {
            type: SiteEditorTypes.ObjectType.page,
            nameOrId: "page"
        }]
    };
    var isCanOperateType = PluginHelper.isCanOperateType(context);
    expect(isCanOperateType).to.eql(false);

    context = {
        koobooObjects: [{
            type: SiteEditorTypes.ObjectType.page,
            nameOrId: "page"
        }]
    };
    isCanOperateType = PluginHelper.isCanOperateType(context);
    expect(isCanOperateType).to.eql(true);

    context = {
        koobooObjects: [{
            attributeName: "href",
            bindingValue: "/index.aspx",
            koobooId: "2-9-1-1-0",
            type: "Url"
        }, {
            nameOrId: "430b7a6e-2772-450b-96e8-9bd5b697068e",
            type: "page"
        }]
    };
    isCanOperateType = PluginHelper.isCanOperateType(context);
    expect(isCanOperateType).to.eql(true);
}

function isContainDynamicData() {
    var html = "<!--#kooboo--objecttype='content'--nameorid='6ced37d6-a713-5d22-4b1f-0f3390c27f26'--folderid='3e62ecfc-ab67-5160-6841-da1e839c0c42'--bindingvalue='List_Item'--boundary='5fb9652d456b402e8be1953d4d74f4a4'-->";
    var isContain = PluginHelper.isContainDynamicData(html);
    expect(isContain).to.eql(true);

    html = "<!--#kooboo--objecttype='htmlblock'--nameorid='6ced37d6-a713-5d22-4b1f-0f3390c27f26'--folderid='3e62ecfc-ab67-5160-6841-da1e839c0c42'--bindingvalue='List_Item'--boundary='5fb9652d456b402e8be1953d4d74f4a4'-->";
    var isContain = PluginHelper.isContainDynamicData(html);
    expect(isContain).to.eql(true);

    html = "<!--#kooboo--objecttype='attribute'--nameorid='6ced37d6-a713-5d22-4b1f-0f3390c27f26'--folderid='3e62ecfc-ab67-5160-6841-da1e839c0c42'--bindingvalue='List_Item'--boundary='5fb9652d456b402e8be1953d4d74f4a4'-->";
    var isContain = PluginHelper.isContainDynamicData(html);
    expect(isContain).to.eql(true);

    html = "<!--#kooboo--objecttype='Label'--nameorid='6ced37d6-a713-5d22-4b1f-0f3390c27f26'--folderid='3e62ecfc-ab67-5160-6841-da1e839c0c42'--bindingvalue='List_Item'--boundary='5fb9652d456b402e8be1953d4d74f4a4'-->";
    var isContain = PluginHelper.isContainDynamicData(html);
    expect(isContain).to.eql(true);

    html = "<div>test</div>";
    var isContain = PluginHelper.isContainDynamicData(html);
    expect(isContain).to.eql(false);
    html = undefined;
    var isContain = PluginHelper.isContainDynamicData(html);
    expect(isContain).to.eql(false);
}

function isBodyTag() {
    var doc = document.implementation.createHTMLDocument("");
    var html = "<html><body><div id='testdiv'>test</div></body></html>";

    doc.write(html);
    var el = $("body", doc)[0];
    var isBody = PluginHelper.isBodyTag(el);
    expect(isBody).to.eql(true);

    el = doc.getElementById("testdiv");
    isBody = PluginHelper.isBodyTag(el);
    expect(isBody).to.eql(false);
}

function isEmptyKoobooId() {
    var doc = document.implementation.createHTMLDocument("");
    var html = "<html><body><div id='testdiv'>test</div></body></html>";

    doc.write(html);
    var el = $("#testdiv", doc)[0];
    var isEmpty = PluginHelper.isEmptyKoobooId(el);
    expect(isEmpty).to.eql(true);
}