import context from "@/common/context";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";
import DeleteHtmlBlockItem from "@/components/floatMenu/items/deleteHtmlBlock";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { Menu } from "@/components/floatMenu/menu";

describe("deleteHtmlBlock", ()=>{
    beforeEach(()=>{
        document.body.innerHTML = "";
    })

    test("DeleteHtmlBlockItem_update", ()=>{
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

        let elementObject = new DeleteHtmlBlockItem(new Menu());
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
    })

    test("DeleteHtmlBlockItem_update_noExistInHtmlblock", ()=>{
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

        let elementObject = new DeleteHtmlBlockItem(new Menu());
        expect(elementObject.el.style.display).toEqual("");

        let comments = KoobooComment.getComments(document.body.children[0].children[0].children[0]);
        elementObject.update(comments);
        expect(elementObject.el.style.display).toEqual("none");
    })
})