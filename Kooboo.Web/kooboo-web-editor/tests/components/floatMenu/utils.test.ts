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
  changeNameOrId
} from "@/components/floatMenu/utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { operationManager } from "@/operation/Manager";
import { operationRecord } from "@/operation/Record";

describe("utils", () => {
  beforeEach(() => {
    document.body.innerHTML = "";
  });

  test("getEditComment", () => {
    let editComment = new KoobooComment(
      "#kooboo--objecttype='content'--nameorid='e695ad0d-4db9-92e2-0bd6-739bd6c94531'--bindingvalue='List_Item.name'--fieldname='name'--koobooid='1-0-1-1-1-1-3'"
    );
    let noEditComment = new KoobooComment(
      "#kooboo--objecttype='contentrepeater'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    let comments = [editComment, noEditComment];

    let result = getEditComment(comments) as KoobooComment;
    expect(result.nameorid).toEqual("e695ad0d-4db9-92e2-0bd6-739bd6c94531");
  });

  test("getViewComment", () => {
    let viewComment = new KoobooComment("#kooboo--objecttype='view'--nameorid='member'--boundary='106'");
    let noViewComment = new KoobooComment(
      "#kooboo--objecttype='contentrepeater'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    let comments = [viewComment, noViewComment];

    let result = getViewComment(comments) as KoobooComment;
    expect(result.nameorid).toEqual("member");
  });

  test("getAttributeComment", () => {
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
    expect(result.nameorid).toEqual("27bf63e5-d18d-3899-f00b-536800d2c6e2");

    result = getAttributeComment(comments, "class") as KoobooComment;
    expect(result.nameorid).toEqual("27bf63e5-d18d-3899-f00b-536800d2c6e3");
  });

  test("hasOperation", () => {
    let manager = new operationManager();
    let result = hasOperation(manager);
    expect(result).toEqual(false);

    manager.add(new operationRecord([], [], ""));
    result = hasOperation(manager);
    expect(result).toEqual(true);
  });

  test("getMenuComment", () => {
    let comment = new KoobooComment("#kooboo--objecttype='menu'--nameorid='headerMenu'--boundary='101'");
    let suportComment = new KoobooComment(
      "#kooboo--objecttype='contentrepeater'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    let comments = [comment, suportComment];

    let result = getMenuComment(comments) as KoobooComment;
    expect(result.nameorid).toEqual("headerMenu");
  });

  test("getFormComment", () => {
    let comment = new KoobooComment("#kooboo--objecttype='form'--nameorid='form'--boundary='101'");
    let suportComment = new KoobooComment(
      "#kooboo--objecttype='contentrepeater'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    let comments = [comment, suportComment];

    let result = getFormComment(comments) as KoobooComment;
    expect(result.nameorid).toEqual("form");
  });

  test("getHtmlBlockComment", () => {
    let comment = new KoobooComment("#kooboo--objecttype='htmlblock'--nameorid='test'--boundary='101'");
    let suportComment = new KoobooComment(
      "#kooboo--objecttype='contentrepeater'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    let comments = [comment, suportComment];

    let result = getHtmlBlockComment(comments) as KoobooComment;
    expect(result.nameorid).toEqual("test");
  });

  test("getRepeatComment", () => {
    let suportComment = new KoobooComment("#kooboo--objecttype='htmlblock'--nameorid='test'--boundary='101'");
    let comment = new KoobooComment(
      "#kooboo--objecttype='contentrepeater'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    let comments = [comment, suportComment];

    let result = getRepeatComment(comments) as KoobooComment;
    expect(result.nameorid).toEqual("d8eee011-69d3-0a6c-632f-822df385ab3c");
  });

  test("getUrlComment", () => {
    let comment = new KoobooComment("#kooboo--objecttype='Url'--nameorid='test'--boundary='101'");
    let suportComment = new KoobooComment(
      "#kooboo--objecttype='contentrepeater'--nameorid='d8eee011-69d3-0a6c-632f-822df385ab3c'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='111'"
    );

    let comments = [comment, suportComment];

    let result = getUrlComment(comments) as KoobooComment;
    expect(result.nameorid).toEqual("test");
  });

  test("changeNameOrId", () => {
    let comment = document.createComment(
      "#kooboo--objecttype='content'--nameorid='e695ad0d-4db9-92e2-0bd6-739bd6c94531'--bindingvalue='List_Item.name'--fieldname='name'--koobooid='1-0-1-1-1-1-3'"
    );

    changeNameOrId(comment, "test", "e695ad0d-4db9-92e2-0bd6-739bd6c94531");

    let koobooComment = new KoobooComment(comment.nodeValue);
    expect(koobooComment.nameorid).toEqual("test");
  });

  test("changeNameOrId_element", () => {
    document.body.innerHTML = `
        <div>
            <!--#kooboo--objecttype='content'--nameorid='e695ad0d-4db9-92e2-0bd6-739bd6c94531'--bindingvalue='List_Item.name'--fieldname='name'--koobooid='1-0-1-1-1-1-3'-->
            <!--#kooboo--objecttype='content'--nameorid='e695ad0d-4db9-92e2-0bd6-739bd6c94531'--bindingvalue='List_Item.name'--fieldname='name'--koobooid='1-0-1-1-1-1-3'-->
        </div>`;

    let element = document.body.children[0];

    changeNameOrId(element, "test", "e695ad0d-4db9-92e2-0bd6-739bd6c94531");

    let koobooComment = new KoobooComment(element.childNodes[1]);
    expect(koobooComment.nameorid).toEqual("test");

    let koobooComment1 = new KoobooComment(element.childNodes[3]);
    expect(koobooComment1.nameorid).toEqual("test");
  });
});
