import context from "@/common/context";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";
import { createEditRepeatLinkItem } from "@/components/floatMenu/items/editRepeatLink";
import { KoobooComment } from "@/kooboo/KoobooComment";

describe("editRepeatLink", () => {
  beforeEach(() => {
    document.body.innerHTML = "";
  });

  test("createEditRepeatLinkItem_update", () => {
    document.body.innerHTML = `
        <!--#kooboo--objecttype='contentrepeater'--nameorid='6a883ab8-435c-cd7c-9ac1-5473fd6f1788'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='171'-->
        <div class="widget widget-intro" kooboo-id="1-0">
            <article class="content" kooboo-id="1-0-1-1-1-1">
                <!--#kooboo--objecttype='attribute'--nameorid='6a883ab8-435c-cd7c-9ac1-5473fd6f1788'--attributename='href'--bindingvalue='{ListPricing_Item.planUrl}'--koobooid='1-0-1-1-1-1-1'-->
                <a kooboo-id="1-0-1-1-1-1-1" href="#" kooboo-guid="6a883ab8-435c-cd7c-9ac1-5473fd6f1788">Select Plan</a>
                <p kooboo-id="1-0-1-1-1-1-5">大数据与人工智能是未来主要研发方向。</p>
            </article>
        </div>
        <!--#kooboo--end=true--objecttype='contentrepeater'--boundary='171'-->
        `;

    let elementObject = createEditRepeatLinkItem();
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

  test("createEditRepeatLinkItem_update_noExistAttrComment", () => {
    document.body.innerHTML = `
        <!--#kooboo--objecttype='contentrepeater'--nameorid='6a883ab8-435c-cd7c-9ac1-5473fd6f1788'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='171'-->
        <div class="widget widget-intro" kooboo-id="1-0">
            <article class="content" kooboo-id="1-0-1-1-1-1">
                <a kooboo-id="1-0-1-1-1-1-1" href="#" kooboo-guid="6a883ab8-435c-cd7c-9ac1-5473fd6f1788">Select Plan</a>
                <p kooboo-id="1-0-1-1-1-1-5">大数据与人工智能是未来主要研发方向。</p>
            </article>
        </div>
        <!--#kooboo--end=true--objecttype='contentrepeater'--boundary='171'-->
        `;

    let selectedElement = document.body.children[0].children[0].children[0] as HTMLElement;

    context.lastSelectedDomEventArgs = new SelectedDomEventArgs(selectedElement);

    let elementObject = createEditRepeatLinkItem();
    expect(elementObject.el.style.display).toEqual("");

    let comments = KoobooComment.getComments(document.body.children[0].children[0].children[0]);
    elementObject.update(comments);
    expect(elementObject.el.style.display).toEqual("none");
  });
});
