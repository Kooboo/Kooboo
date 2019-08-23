import { listenHover, emitHoverEvent, emitSelectedEvent } from "@/dom/events";
import context from "@/common/context";
import { HoverDomEventArgs } from "@/events/HoverDomEvent";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";

function listenHoverAndSetEventHandle(handle: (e: HoverDomEventArgs) => void) {
  listenHover();
  context.hoverDomEvent.addEventListener(handle);
}

function listenClickAndSetEventHandle(handle: (e: SelectedDomEventArgs) => void) {
  listenHover();
  context.domChangeEvent.addEventListener(handle);
}

describe("events", () => {
  beforeEach(() => {
    document.body.innerHTML = "";
    context.hoverDomEvent.handlers;
    context.lastHoverDomEventArgs = undefined!;
  });

  it("listenHover", () => {
    let hoverArg!: HoverDomEventArgs;
    listenHoverAndSetEventHandle((arg: HoverDomEventArgs) => {
      hoverArg = arg;
    });

    document.body.innerHTML = '<h2 id="test" kooboo-id="1-0-1-1-1-1-3">Kooboo CMS</h2>';
    let element = document.body.children[0];

    let event = document.createEvent("MouseEvent");
    event.initEvent("mouseover", true);

    element.dispatchEvent(event);

    expect(hoverArg.element.id).equal("test");
    expect(hoverArg.closeElement.id).equal("test");
  });

  it("listenHover_hoverChild", () => {
    let hoverArg!: HoverDomEventArgs;
    listenHoverAndSetEventHandle((arg: HoverDomEventArgs) => {
      hoverArg = arg;
    });

    document.body.innerHTML = `
        <ul id="test" class="menu" kooboo-id="1-0-1-1-1-1-3">
            <li id="testli" class="className"></li>
        </ul>
        `;
    let element = document.body.children[0].children[0];

    let event = document.createEvent("MouseEvent");
    event.initEvent("mouseover", true);

    element.dispatchEvent(event);

    expect(hoverArg.element.id).equal("testli");
    expect(hoverArg.closeElement.id).equal("test");
  });

  it("listenHover_ExistKoobooComment", () => {
    let hoverArg!: HoverDomEventArgs;
    listenHoverAndSetEventHandle((arg: HoverDomEventArgs) => {
      hoverArg = arg;
    });

    document.body.innerHTML = `
        <!--#kooboo--objecttype='menu'--nameorid='headerMenu'--boundary='101'-->
        <ul id="test" class="menu">
            <li class="className"></li>
        </ul>
        <!--#kooboo--end='true'--objecttype='menu'--boundary='101'-->
        `;
    let element = document.body.children[0];

    let event = document.createEvent("MouseEvent");
    event.initEvent("mouseover", true);

    element.dispatchEvent(event);

    expect(hoverArg.element.id).equal("test");
    expect(hoverArg.closeElement.id).equal("test");
  });

  it("listenHover_ExistOtherComment", () => {
    let hoverArg!: HoverDomEventArgs;
    listenHoverAndSetEventHandle((arg: HoverDomEventArgs) => {
      hoverArg = arg;
    });

    document.body.innerHTML = `
        <!--#kooboo--objecttype='menu'--nameorid='headerMenu'--boundary='101'-->
        <!-- test comment -->
        <ul id="test" class="menu">
            <li class="className"></li>
        </ul>
        <!--#kooboo--end='true'--objecttype='menu'--boundary='101'-->
        `;
    let element = document.body.children[0];

    let event = document.createEvent("MouseEvent");
    event.initEvent("mouseover", true);

    element.dispatchEvent(event);

    expect(hoverArg.element.id).equal("test");
    expect(hoverArg.closeElement.id).equal("test");
  });

  it("emitHoverEvent", () => {
    let hoverArg!: HoverDomEventArgs;
    listenHoverAndSetEventHandle((arg: HoverDomEventArgs) => {
      hoverArg = arg;
    });

    document.body.innerHTML = '<h2 id="test" kooboo-id="1-0-1-1-1-1-3">Kooboo CMS</h2>';
    let element = document.body.children[0] as HTMLElement;

    emitHoverEvent(element);

    expect(hoverArg.element.id).equal("test");
    expect(hoverArg.closeElement.id).equal("test");
  });

  it("emitHoverEvent_emitChild", () => {
    let hoverArg!: HoverDomEventArgs;
    listenHoverAndSetEventHandle((arg: HoverDomEventArgs) => {
      hoverArg = arg;
    });

    document.body.innerHTML = `
        <ul id="test" class="menu" kooboo-id="1-0-1-1-1-1-3">
            <li id="testli" class="className"></li>
        </ul>
        `;
    let element = document.body.children[0].children[0] as HTMLElement;

    emitHoverEvent(element);

    expect(hoverArg.element.id).equal("testli");
    expect(hoverArg.closeElement.id).equal("test");
  });

  // 发射选择事件会使最后的Hover事件参数的Element
  it("emitSelectedEvent", () => {
    let hoverArg!: SelectedDomEventArgs;
    listenClickAndSetEventHandle((arg: SelectedDomEventArgs) => {
      hoverArg = arg;
    });

    document.body.innerHTML = '<h2 id="test" kooboo-id="1-0-1-1-1-1-3">Kooboo CMS</h2>';
    let element = document.body.children[0] as HTMLElement;

    context.lastHoverDomEventArgs = new HoverDomEventArgs(element, element);

    emitSelectedEvent();

    expect(hoverArg.element.id).equal("test");
  });
});
