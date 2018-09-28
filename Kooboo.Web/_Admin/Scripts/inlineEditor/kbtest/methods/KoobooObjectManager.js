// require(['siteEditor/KoobooObjectManager',
//     'siteEditor/SiteEditorTypes'
// ], function(KoobooObjectManager, SiteEditorTypes) {


// });
var SiteEditorTypes = Kooboo.SiteEditorTypes;
var koobooObjectManager =  Kooboo.KoobooObjectManager;


function isNeedGetParentKoobooObject() {
    // spyOn(koobooObjectManager, "_isExistObjectType").and.callFake(function(objectType) {
    //     return true;
    // });
    var domOperationType = "html";
    var objectType = "page";
    var needGetParent = koobooObjectManager.isNeedGetParentKoobooObject(objectType, domOperationType);
    expect(needGetParent).to.eql(false);

    objectType = "attribute";
    needGetParent = koobooObjectManager.isNeedGetParentKoobooObject(objectType, domOperationType);
    expect(needGetParent).to.eql(true);

    objectType = "content";
    needGetParent = koobooObjectManager.isNeedGetParentKoobooObject(objectType, domOperationType);
    expect(needGetParent).to.eql(true);

    domOperationType = "content";
    needGetParent = koobooObjectManager.isNeedGetParentKoobooObject(objectType, domOperationType);
    expect(needGetParent).to.eql(false);

    objectType = "contentrepeater";
    needGetParent = koobooObjectManager.isNeedGetParentKoobooObject(objectType, domOperationType);
    expect(needGetParent).to.eql(true);

    domOperationType = "contentRepeater"
    needGetParent = koobooObjectManager.isNeedGetParentKoobooObject(objectType, domOperationType);
    expect(needGetParent).to.eql(false);

    domOperationType = null;
    needGetParent = koobooObjectManager.isNeedGetParentKoobooObject(objectType, domOperationType);
    expect(needGetParent).to.eql(false);
}

function isNeedGetNewNameOrId() {
    //is category
    var koobooObject = {
        bindingValue: "ByFolder_Item_category_Item.type",
        fieldName: "type",
        koobooId: "1-0-3-1",
        nameOrId: "2ae45631-46b2-b804-de92-a93ed761532d",
        type: "content"
    };
    //don't need get parent Repeater,so remove this logic
    var isNeed = koobooObjectManager.isNeedGetNewNameOrId(koobooObject);
    expect(isNeed).to.eql(false);

    //not exist type
    koobooObject = {
        attributeName: "href",
        bindingValue: "/setup-step-1.shtml",
        koobooId: "1-0-3-1",
        type: "Url"
    };
    isNeed = koobooObjectManager.isNeedGetNewNameOrId(koobooObject);
    expect(isNeed).to.eql(true);

    //category repeater
    koobooObject = {
        bindingValue: "ByFolder_Item_category_Item",
        boundary: "128",
        folderId: "7f245369-8a41-d98b-f76d-53cb4d7290f4",
        nameOrId: "2ae45631-46b2-b804-de92-a93ed761532d",
        type: "contentrepeater"
    };
    isNeed = koobooObjectManager.isNeedGetNewNameOrId(koobooObject);
    expect(isNeed).to.eql(false);

    koobooObject = {
        bindingValue: "ByFolder_Item",
        boundary: "127",
        folderId: "4011954e-1d43-3717-126e-4f221d3e7ac9",
        nameOrId: "de68543f-fb0b-11e1-586f-7649a3c8d115",
        type: "contentrepeater"
    };
    isNeed = koobooObjectManager.isNeedGetNewNameOrId(koobooObject);
    expect(isNeed).to.eql(false);

    koobooObject = {
        bindingValue: "ByFolder_Item.category.type",
        boundary: "127",
        folderId: "4011954e-1d43-3717-126e-4f221d3e7ac9",
        nameOrId: "de68543f-fb0b-11e1-586f-7649a3c8d115",
        type: "contentrepeater"
    };
    isNeed = koobooObjectManager.isNeedGetNewNameOrId(koobooObject);
    expect(isNeed).to.eql(true);

    koobooObject = {
        boundary: "127",
        folderId: "4011954e-1d43-3717-126e-4f221d3e7ac9",
        nameOrId: "byId_singleCategory",
        type: "view"
    };
    isNeed = koobooObjectManager.isNeedGetNewNameOrId(koobooObject);
    expect(isNeed).to.eql(true);
}
function getNameOrIdFromContext() {
    var koobooObjects=[{
            type: SiteEditorTypes.ObjectType.htmlblock,
            nameOrId: "htmlblock"
        }, {
            type: SiteEditorTypes.ObjectType.page,
            nameOrId: "page"
        }];
    var nameOrId = koobooObjectManager.getNameOrIdFromContext(koobooObjects,SiteEditorTypes.ObjectType.htmlblock);
    expect(nameOrId).to.eql("htmlblock");
    nameOrId = koobooObjectManager.getNameOrIdFromContext(koobooObjects,SiteEditorTypes.ObjectType.page);
    expect(nameOrId).to.eql("page");

    nameOrId = koobooObjectManager.getNameOrIdFromContext(koobooObjects,SiteEditorTypes.ObjectType.content);
    expect(nameOrId).to.eql(null);
}

function getObjectType() {
    var context = {
        koobooObjects: [{
            type: SiteEditorTypes.ObjectType.htmlblock,
            nameOrId: "htmlblock"
        }, {
            type: SiteEditorTypes.ObjectType.page,
            nameOrId: "page"
        }]
    };
    var objectType = koobooObjectManager.getObjectType(context);
    expect(objectType).to.eql(SiteEditorTypes.ObjectType.htmlblock);

    context = {
        koobooObjects: []
    };
    var objectType = koobooObjectManager.getObjectType(context);
    expect(objectType).to.eql("");
}