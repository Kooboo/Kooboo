import { createMenu } from "@/components/floatMenu/menu";
import context from "@/common/context";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";
import { HoverDomEventArgs } from "@/events/HoverDomEvent";

describe("menu", () => {
  beforeEach(() => {
    document.body.innerHTML = "";
  });

  test("createMenu", () => {
    let element = createMenu();

    expect(element.el.style.display).toEqual("none");
  });

  // 显示浮动菜单，需要知道最后选择的元素
  test("createMenu_update", () => {
    document.body.innerHTML = `
        <!--#kooboo--objecttype='content'--nameorid='e695ad0d-4db9-92e2-0bd6-739bd6c94531'--bindingvalue='List_Item.name'--fieldname='name'--koobooid='1-0-1-1-1-1-3'-->
        <h2 kooboo-id="1-0-1-1-1-1-3" spellcheck="false" kooboo-guid="2882b009-1c32-4ac7-b21e-8c15f4e94421">Kooboo CMS1<!--empty--></h2>
        <!--#kooboo--objecttype='content'--nameorid='e695ad0d-4db9-92e2-0bd6-739bd6c94531'--bindingvalue='List_Item.contentIndex'--fieldname='contentIndex'--koobooid='1-0-1-1-1-1-5'-->
        `;
    let selectedElement = document.body.children[0] as HTMLElement;
    context.lastHoverDomEventArgs = new HoverDomEventArgs(selectedElement, selectedElement);
    context.lastSelectedDomEventArgs = new SelectedDomEventArgs(selectedElement);

    let { el, hidden, update } = createMenu();
    document.body.appendChild(el);

    update(100, 200);
    expect(el.style.display).toEqual("block");
    expect(el.style.top).toEqual("200px");
    expect(el.style.left).toEqual("100px");
  });

  test("createMenu_hidden", () => {
    let element = createMenu();

    element.hidden();
    expect(element.el.style.display).toEqual("none");
  });
});
