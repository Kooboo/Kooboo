import context from "@/common/context";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";
import { createDeleteHtmlBlockItem } from "@/components/floatMenu/items/deleteHtmlBlock";

describe("deleteHtmlBlock", ()=>{
    beforeEach(()=>{
        document.body.innerHTML = "";
    })

    test("createDeleteHtmlBlockItem_update", ()=>{
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

        let elementObject = createDeleteHtmlBlockItem();
        expect(elementObject.el.style.display).toEqual("");

        // 不能是body元素
        context.lastSelectedDomEventArgs = new SelectedDomEventArgs(document.body as HTMLElement);
        elementObject.update();
        expect(elementObject.el.style.display).toEqual("none");

        context.lastSelectedDomEventArgs = new SelectedDomEventArgs(document.body.children[0].children[0].children[0] as HTMLElement);
        elementObject.update();
        expect(elementObject.el.style.display).toEqual("block");
    })

    test("createDeleteHtmlBlockItem_update_noExistInHtmlblock", ()=>{
        document.body.innerHTML = `
        <!--#kooboo--objecttype='view'--nameorid='member'--boundary='154'-->
        <div class="widget widget-intro" kooboo-id="1-0">
            <article class="content" kooboo-id="1-0-1-1-1-1">
                <h2 class="title" kooboo-id="1-0-1-1-1-1-1">平等，责任</h2>
                <p kooboo-id="1-0-1-1-1-1-5">大数据与人工智能是未来主要研发方向。</p>
            </article>
        </div>
        <!--#kooboo--end='true'--objecttype='view'--boundary='154'-->
        `;

        let selectedElement = document.body.children[0].children[0].children[0] as HTMLElement;

        context.lastSelectedDomEventArgs = new SelectedDomEventArgs(selectedElement);

        let elementObject = createDeleteHtmlBlockItem();
        expect(elementObject.el.style.display).toEqual("");

        elementObject.update();
        expect(elementObject.el.style.display).toEqual("none");
    })
})