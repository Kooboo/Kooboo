import context from "@/common/context";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";
import { createEditRepeatItem } from "@/components/floatMenu/items/editRepeat";
import { KoobooComment } from "@/kooboo/KoobooComment";

describe("edit", () => {
  beforeEach(() => {
    document.body.innerHTML = "";
  });

  test("createEditItem_update", () => {
    document.body.innerHTML = `
        <!--#kooboo--objecttype='contentrepeater'--nameorid='6a883ab8-435c-cd7c-9ac1-5473fd6f1788'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='171'-->
        <div class="widget widget-intro" kooboo-id="1-0">
            <article class="content" kooboo-id="1-0-1-1-1-1">
                <!--#kooboo--objecttype='content'--nameorid='6a883ab8-435c-cd7c-9ac1-5473fd6f1788'--bindingvalue='List_Item.name'--fieldname='name'--koobooid='1-0-1-1-1-1-3'-->
                <h2 kooboo-id="1-0-1-1-1-1-3">Mailaaaa</h2>
                <p kooboo-id="1-0-1-1-1-1-5">大数据与人工智能是未来主要研发方向。</p>
            </article>
        </div>
        <!--#kooboo--end=true--objecttype='contentrepeater'--boundary='171'-->
        `;

    let elementObject = createEditRepeatItem();
    expect(elementObject.el.style.display).toEqual("");

    // 不能是body元素
    context.lastSelectedDomEventArgs = new SelectedDomEventArgs(document.body as HTMLElement);
    let comments = KoobooComment.getComments(document.body);
    elementObject.update(comments);
    expect(elementObject.el.style.display).toEqual("none");

    context.lastSelectedDomEventArgs = new SelectedDomEventArgs(document.body.children[0].children[0].children[0] as HTMLElement);
    comments = KoobooComment.getComments(document.body.children[0].children[0].children[0]);
    elementObject.update(comments);
    expect(elementObject.el.style.display).toEqual("block");
  });

  test("createEditItem_update_existRelatedRepeatComment", () => {
    document.body.innerHTML = `
        <!--#kooboo--objecttype='contentrepeater'--nameorid='6a883ab8-435c-cd7c-9ac1-5473fd6f1788'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='171'-->
        <!--#kooboo--end=true--objecttype='contentrepeater'--boundary='171'-->
        <div class="widget widget-intro" kooboo-id="1-0">
            <article class="content" kooboo-id="1-0-1-1-1-1">
                <!--#kooboo--objecttype='content'--nameorid='6a883ab8-435c-cd7c-9ac1-5473fd6f1788'--bindingvalue='List_Item.name'--fieldname='name'--koobooid='1-0-1-1-1-1-3'-->
                <h2 kooboo-id="1-0-1-1-1-1-3">Mailaaaa</h2>
                <p kooboo-id="1-0-1-1-1-1-5">大数据与人工智能是未来主要研发方向。</p>
            </article>
        </div>
        `;

    let elementObject = createEditRepeatItem();
    expect(elementObject.el.style.display).toEqual("");

    // 不能是body元素
    context.lastSelectedDomEventArgs = new SelectedDomEventArgs(document.body as HTMLElement);
    let comments = KoobooComment.getComments(document.body.children[0].children[0].children[0]);
    elementObject.update(comments);
    expect(elementObject.el.style.display).toEqual("none");

    context.lastSelectedDomEventArgs = new SelectedDomEventArgs(document.body.children[0].children[0].children[0] as HTMLElement);
    comments = KoobooComment.getComments(document.body.children[0].children[0].children[0]);
    elementObject.update(comments);
    expect(elementObject.el.style.display).toEqual("block");
  });

  test("createEditItem_update_noExistRepeatComment", () => {
    document.body.innerHTML = `
        <!--#kooboo--objecttype='view'--nameorid='member'--boundary='154'-->
        <div class="widget widget-intro" kooboo-id="1-0">
            <article class="content" kooboo-id="1-0-1-1-1-1">
                <!--#kooboo--objecttype='htmlblock'--nameorid='test1'--boundary='155'-->
                <h2 class="title" kooboo-id="1-0-1-1-1-1-1">平等，责任</h2>
                <!--#kooboo--end='true'--objecttype='htmlblock'--boundary='155'-->
                <p kooboo-id="1-0-1-1-1-1-5">大数据与人工智能是未来主要研发方向。</p>
            </article>
        </div>
        <!--#kooboo--end='true'--objecttype='view'--boundary='154'-->
        `;

    let selectedElement = document.body.children[0].children[0].children[0] as HTMLElement;

    context.lastSelectedDomEventArgs = new SelectedDomEventArgs(selectedElement);

    let elementObject = createEditRepeatItem();
    expect(elementObject.el.style.display).toEqual("");

    let comments = KoobooComment.getComments(document.body.children[0].children[0].children[0]);
    elementObject.update(comments);
    expect(elementObject.el.style.display).toEqual("none");
  });
});
