import { DeleteRepeatUnit } from "@/operation/recordUnits/DeleteRepeatUnit";
import { getAllNode } from "@/dom/utils";

describe("DeleteRepeatUnit", () => {
  it("undo", () => {
    let nodeParent = document.createElement("div");
    nodeParent.innerHTML = "<!--abcde-->";
    let unit = new DeleteRepeatUnit("<div><p>abc</p><p>cdf</p></div>");
    unit.undo(nodeParent.childNodes[0]);

    expect(nodeParent.children[0].outerHTML).equal("<div><p>abc</p><p>cdf</p></div>");
    expect(unit.newValue).equal("<!--abcde-->");
  });

  it("redo", () => {
    let nodeParent = document.createElement("div");
    let html = `<!--#kooboo--objecttype='contentrepeater'--nameorid='abc'-->
      <li kooboo-id="1-0-3-1-1-3-1-5-1"></li>
    <!--#kooboo--end=true--objecttype='contentrepeater'--boundary='311'-->`;
    nodeParent.innerHTML = html;

    let unit = new DeleteRepeatUnit(html);
    unit.newValue = "<!--abc-->";
    let comment!: Node;
    for (const iterator of getAllNode(nodeParent)) {
      if (iterator.nodeType == Node.COMMENT_NODE) {
        comment = iterator;
        break;
      }
    }
    unit.redo(comment);

    expect(nodeParent.innerHTML.trim()).equal("<!--abc-->");
    expect(unit.oldValue).equal(html);
  });
});
