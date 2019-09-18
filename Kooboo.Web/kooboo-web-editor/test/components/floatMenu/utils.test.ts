import {
  getEditComment,
  getViewComment,
  getAttributeComment,
  hasOperation,
  getMenuComment,
  getFormComment,
  getHtmlBlockComment,
  getRepeatComment,
  getUrlComment,
  changeNameOrId,
  getFirstComment,
  isViewComment,
  isEditComment,
  isRepeatComment
} from "@/components/floatMenu/utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { operationManager } from "@/operation/Manager";
import { operationRecord } from "@/operation/Record";

describe("utils", () => {
  beforeEach(() => {
    document.body.innerHTML = "";
  });

  it("getEditComment", () => {
    let editComment = new KoobooComment(
      "#kooboo--objecttype='content'--nameorid='e695ad0d-4db9-92e2-0bd6-739bd6c94531'--bindingvalue='List_Item.name'--fieldname='name'--koobooid='1-0-1-1-1-1-3'"
    );
    let noEditComment = new KoobooComment(
      "#kooboo--objecttype='contentrepeater'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    let comments = [editComment, noEditComment];

    let result = getEditComment(comments) as KoobooComment;
    expect(result.nameorid).equal("e695ad0d-4db9-92e2-0bd6-739bd6c94531");
  });

  it("getViewComment", () => {
    let viewComment = new KoobooComment("#kooboo--objecttype='view'--nameorid='member'--boundary='106'");
    let noViewComment = new KoobooComment(
      "#kooboo--objecttype='contentrepeater'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    let comments = [viewComment, noViewComment];

    let result = getViewComment(comments) as KoobooComment;
    expect(result.nameorid).equal("member");
  });

  it("getAttributeComment", () => {
    let srcAttributeComment = new KoobooComment(
      "#kooboo--objecttype='attribute'--nameorid='27bf63e5-d18d-3899-f00b-536800d2c6e2'--attributename='src'--bindingvalue='{ListItem.img}'--koobooid='1-0-1-1-3-1'"
    );
    let classAttributeComment = new KoobooComment(
      "#kooboo--objecttype='attribute'--nameorid='27bf63e5-d18d-3899-f00b-536800d2c6e3'--attributename='class'--bindingvalue='{ListItem.img}'--koobooid='1-0-1-1-3-1'"
    );
    let noAttributeComment = new KoobooComment(
      "#kooboo--objecttype='contentrepeater'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    let comments = [srcAttributeComment, classAttributeComment, noAttributeComment];

    let result = getAttributeComment(comments) as KoobooComment;
    expect(result.nameorid).equal("27bf63e5-d18d-3899-f00b-536800d2c6e2");

    result = getAttributeComment(comments, "class") as KoobooComment;
    expect(result.nameorid).equal("27bf63e5-d18d-3899-f00b-536800d2c6e3");
  });

  it("hasOperation", () => {
    let manager = new operationManager();
    let result = hasOperation(manager);
    expect(result).equal(false);

    manager.add(new operationRecord([], [], ""));
    result = hasOperation(manager);
    expect(result).equal(true);
  });

  it("getMenuComment", () => {
    let comment = new KoobooComment("#kooboo--objecttype='menu'--nameorid='headerMenu'--boundary='101'");
    let suportComment = new KoobooComment(
      "#kooboo--objecttype='contentrepeater'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    let comments = [comment, suportComment];

    let result = getMenuComment(comments) as KoobooComment;
    expect(result.nameorid).equal("headerMenu");
  });

  it("getFormComment", () => {
    let comment = new KoobooComment("#kooboo--objecttype='form'--nameorid='form'--boundary='101'");
    let suportComment = new KoobooComment(
      "#kooboo--objecttype='contentrepeater'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    let comments = [comment, suportComment];

    let result = getFormComment(comments) as KoobooComment;
    expect(result.nameorid).equal("form");
  });

  it("getHtmlBlockComment", () => {
    let comment = new KoobooComment("#kooboo--objecttype='htmlblock'--nameorid='test'--boundary='101'");
    let suportComment = new KoobooComment(
      "#kooboo--objecttype='contentrepeater'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    let comments = [comment, suportComment];

    let result = getHtmlBlockComment(comments) as KoobooComment;
    expect(result.nameorid).equal("test");
  });

  it("getRepeatComment", () => {
    let suportComment = new KoobooComment("#kooboo--objecttype='htmlblock'--nameorid='test'--boundary='101'");
    let comment = new KoobooComment(
      "#kooboo--objecttype='contentrepeater'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    let comments = [comment, suportComment];

    let result = getRepeatComment(comments) as KoobooComment;
    expect(result.nameorid).equal("d8eee011-69d3-0a6c-632f-822df385ab3c");
  });

  it("getUrlComment", () => {
    let comment = new KoobooComment("#kooboo--objecttype='Url'--nameorid='test'--boundary='101'");
    let suportComment = new KoobooComment(
      "#kooboo--objecttype='contentrepeater'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    let comments = [comment, suportComment];

    let result = getUrlComment(comments) as KoobooComment;
    expect(result.nameorid).equal("test");
  });

  it("changeNameOrId", () => {
    let comment = document.createComment(
      "#kooboo--objecttype='content'--nameorid='e695ad0d-4db9-92e2-0bd6-739bd6c94531'--bindingvalue='List_Item.name'--fieldname='name'--koobooid='1-0-1-1-1-1-3'"
    );

    changeNameOrId(comment, "test", "e695ad0d-4db9-92e2-0bd6-739bd6c94531");

    let koobooComment = new KoobooComment(comment.nodeValue);
    expect(koobooComment.nameorid).equal("test");
  });

  it("changeNameOrId_element", () => {
    document.body.innerHTML = `
        <div>
            <!--#kooboo--objecttype='content'--nameorid='e695ad0d-4db9-92e2-0bd6-739bd6c94531'--bindingvalue='List_Item.name'--fieldname='name'--koobooid='1-0-1-1-1-1-3'-->
            <!--#kooboo--objecttype='content'--nameorid='e695ad0d-4db9-92e2-0bd6-739bd6c94531'--bindingvalue='List_Item.name'--fieldname='name'--koobooid='1-0-1-1-1-1-3'-->
        </div>`;

    let element = document.body.children[0];

    changeNameOrId(element, "test", "e695ad0d-4db9-92e2-0bd6-739bd6c94531");

    let koobooComment = new KoobooComment(element.childNodes[1]);
    expect(koobooComment.nameorid).equal("test");

    let koobooComment1 = new KoobooComment(element.childNodes[3]);
    expect(koobooComment1.nameorid).equal("test");
  });

  // 无法对updateDomImage进行测试
  it("updateDomImage", () => {});

  // 无法对updateAttributeImage进行测试
  it("updateAttributeImage", () => {});

  // 调用了异步等待方法，无法进行测试
  it("updateDomLink", () => {});

  it("getFirstComment", () => {
    let comment = new KoobooComment("#kooboo--objecttype='content'--nameorid='test'--boundary='101'");
    let suportComment = new KoobooComment(
      "#kooboo--objecttype='Url'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    let comments = [suportComment, comment];

    let result = getFirstComment(comments) as KoobooComment;
    expect(result.nameorid).equal("test");
  });

  it("isViewComment", () => {
    let comment = new KoobooComment("#kooboo--objecttype='view'--nameorid='test'--boundary='101'");
    let suportComment = new KoobooComment(
      "#kooboo--objecttype='Url'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    expect(isViewComment(comment)).equal(true);
    expect(isViewComment(suportComment)).equal(false);
  });

  it("isEditComment", () => {
    let comment = new KoobooComment("#kooboo--objecttype='view'--nameorid='test'--boundary='101'");
    let suportComment = new KoobooComment(
      "#kooboo--objecttype='Url'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    expect(isEditComment(comment)).equal(true);
    expect(isEditComment(suportComment)).equal(false);
  });

  it("isRepeatComment", () => {
    let comment = new KoobooComment("#kooboo--objecttype='contentrepeater'--nameorid='test'--boundary='101'");
    let suportComment = new KoobooComment(
      "#kooboo--objecttype='Url'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    expect(isRepeatComment(comment)).equal(true);
    expect(isRepeatComment(suportComment)).equal(false);
  });
});
