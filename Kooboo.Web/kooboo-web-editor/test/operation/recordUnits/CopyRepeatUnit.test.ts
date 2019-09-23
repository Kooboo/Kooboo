import { CopyRepeatUnit } from "@/operation/recordUnits/CopyRepeatUnit";

describe("CopyRepeatUnit", () => {
  afterEach(() => (document.body.innerHTML = ""));

  it("undo", () => {
    document.body.innerHTML = `
    <!--#kooboo--objecttype='contentrepeater'--nameorid='515e4944-bdf4-c085-d317-792bb2f4fbd5'--folderid='e58436af-b505-24b9-c7ff-c7f7f5376b99'--bindingvalue='List_Item'--boundary='4879'-->
<li kooboo-id="1-0-3-1-1-3-1-5-1"><i class="icon-check2" kooboo-id="1-0-3-1-1-3-1-5-1-0"></i>
<!--#kooboo--objecttype='content'--nameorid='515e4944-bdf4-c085-d317-792bb2f4fbd5'--bindingvalue='List_Item.ProjectName'--fieldname='ProjectName'--koobooid='1-0-3-1-1-3-1-5-1-1'-->
<span kooboo-id="1-0-3-1-1-3-1-5-1-1" id="dom">Semantics a large language ocean</span></li>
<!--#kooboo--end=true--objecttype='contentrepeater'--boundary='4879'-->
    `;
    let unit = new CopyRepeatUnit("<!--guid-->");
    unit.undo(document.getElementById("dom")!);
    expect(document.body.innerHTML.trim()).equal("<!--guid-->");
  });

  it("redo", () => {
    let node = document.createElement("img");
    document.body.appendChild(node);
    let unit = new CopyRepeatUnit("<!--guid-->");
    unit.newValue = "<div></div>";
    unit.redo(node);
    expect(unit.oldValue).equal("<!--guid-->");
    expect(document.body.innerHTML).equal("<div></div>");
  });
});
