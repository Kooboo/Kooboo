import context from "@/common/context";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";
import DeleteItem from "@/components/floatMenu/items/delete";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { Menu } from "@/components/floatMenu/menu";

describe("delete", () => {
  beforeEach(() => {
    document.body.innerHTML = "";
  });

  it("DeleteItem_update", () => {
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

    let elementObject = new DeleteItem(new Menu());
    expect(elementObject.el.style.display).equal("");

    // 不能是body元素
    context.lastSelectedDomEventArgs = new SelectedDomEventArgs(document.body as HTMLElement);
    let comments = KoobooComment.getComments(document.body);
    elementObject.update();
    expect(elementObject.el.style.display).equal("none");

    context.lastSelectedDomEventArgs = new SelectedDomEventArgs(document.body.children[0].children[0].children[0] as HTMLElement);
    comments = KoobooComment.getComments(document.body.children[0].children[0].children[0]);
    elementObject.update();
    expect(elementObject.el.style.display).equal("block");
  });

  //   // 第一个注释必须是可编辑的类型(逻辑变更)
  //   it("DeleteItem_update_noEditType", () => {
  //     document.body.innerHTML = `
  //         <!--#kooboo--objecttype='view'--nameorid='member'--boundary='154'-->
  //         <div class="widget widget-intro" kooboo-id="1-0">
  //             <article class="content" kooboo-id="1-0-1-1-1-1">
  //                 <!--#kooboo--objecttype='attribute'--nameorid='6a883ab8-435c-cd7c-9ac1-5473fd6f1788'--attributename='src'--bindingvalue='{Item.img-logo}'--koobooid='1-0-1-1-1-1-1-1'-->
  //                 <h2 kooboo-id="1-0-1-1-1-1-3">Mailaaaa</h2>
  //                 <p kooboo-id="1-0-1-1-1-1-5">大数据与人工智能是未来主要研发方向。</p>
  //             </article>
  //         </div>
  //         <!--#kooboo--end='true'--objecttype='view'--boundary='154'-->
  //         `;

  //     let selectedElement = document.body.children[0].children[0].children[0] as HTMLElement;

  //     context.lastSelectedDomEventArgs = new SelectedDomEventArgs(selectedElement);

  //     let elementObject = DeleteItem();
  //     expect(elementObject.el.style.display).equal("");

  //     let comments = KoobooComment.getComments(document.body.children[0].children[0].children[0]);
  //     elementObject.update(comments);
  //     expect(elementObject.el.style.display).equal("none");
  //   });

  // 不能存在于repeat中
  it("DeleteItem_update_existInRepeat", () => {
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

    let elementObject = new DeleteItem(new Menu());
    expect(elementObject.el.style.display).equal("");

    let comments = KoobooComment.getComments(document.body.children[0].children[0].children[0]);
    elementObject.update();
    expect(elementObject.el.style.display).equal("none");
  });

  // 选择的元素的父元素不能存在其他类型类型中（逻辑变更）
  //   it("createCopyItem_update_existOtherType", () => {
  //     document.body.innerHTML = `
  //         <!--#kooboo--objecttype='view'--nameorid='6a883ab8-435c-cd7c-9ac1-5473fd6f1788'--folderid='ffa232c4-ca49-9c07-8b43-fd30d5ec5e8b'--bindingvalue='List_Item'--boundary='171'-->
  //         <div class="widget widget-intro" kooboo-id="1-0">
  //             <article class="content" kooboo-id="1-0-1-1-1-1">
  //                 <h2 class="title" kooboo-id="1-0-1-1-1-1-1">平等，责任</h2>
  //                 <p kooboo-id="1-0-1-1-1-1-5">大数据与人工智能是未来主要研发方向。</p>
  //                 <!--#kooboo--objecttype='contentrepeater'--nameorid='cf41fed8-d56c-4b97-9c09-594d3e01c865'--folderid='4b507243-a096-1a79-3247-345641080783'--bindingvalue='ListItem'--boundary='169'-->
  //                 <div kooboo-id="1-0-1-1-3">
  //                 </div>
  //                 <!--#kooboo--end=true--objecttype='contentrepeater'--boundary='169'-->
  //             </article>
  //         </div>
  //         <!--#kooboo--end=true--objecttype='view'--boundary='171'-->
  //         `;

  //     let selectedElement = document.body.children[0].children[0].children[0] as HTMLElement;

  //     context.lastSelectedDomEventArgs = new SelectedDomEventArgs(selectedElement);

  //     let elementObject = DeleteItem();
  //     expect(elementObject.el.style.display).equal("");

  //     let comments = KoobooComment.getComments(document.body.children[0].children[0].children[0]);
  //     elementObject.update(comments);
  //     expect(elementObject.el.style.display).equal("none");
  //   });
});
