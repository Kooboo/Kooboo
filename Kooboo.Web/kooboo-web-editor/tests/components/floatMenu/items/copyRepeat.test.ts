import context from "@/common/context";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";
import { createCopyRepeatItem } from "@/components/floatMenu/items/copyRepeat";
import { KoobooComment } from "@/kooboo/KoobooComment";

describe("copyRepeat", ()=>{
    beforeEach(()=>{
        document.body.innerHTML = "";
    })

    test("createCopyRepeatItem_update", ()=>{
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

        let selectedElement = document.body.children[0].children[0].children[0] as HTMLElement;

        context.lastSelectedDomEventArgs = new SelectedDomEventArgs(selectedElement);

        let elementObject = createCopyRepeatItem();
        expect(elementObject.el.style.display).toEqual("");

        let comments = KoobooComment.getComments(document.body.children[0].children[0].children[0]);
        elementObject.update(comments);
        expect(elementObject.el.style.display).toEqual("block");
    })

    // 必须存在于Repeat类型中
    test("createCopyItem_update_noExistInRepeat", () => {
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

        let elementObject = createCopyRepeatItem();
        expect(elementObject.el.style.display).toEqual("");

        let comments = KoobooComment.getComments(document.body.children[0].children[0].children[0]);
        elementObject.update(comments);
        expect(elementObject.el.style.display).toEqual("none");
    })
})