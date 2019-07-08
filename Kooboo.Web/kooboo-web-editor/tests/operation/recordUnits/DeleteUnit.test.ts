import { DeleteUnit } from "@/operation/recordUnits/DeleteUnit";

describe("DeleteUnit", () => {
  test("undo", () => {
    let nodeParent = document.createElement("div");
    nodeParent.innerHTML = "<!--abc-->";

    let unit = new DeleteUnit("<div></div>");
    unit.undo(nodeParent.childNodes[0]);

    expect((nodeParent.childNodes[0] as Element).outerHTML).toEqual("<div></div>");
    expect(unit.newValue).toEqual("<!--abc-->");
  });

  test("redo", () => {
    let nodeParent = document.createElement("div");
    nodeParent.innerHTML = "<div></div>";

    let unit = new DeleteUnit("<div></div>");
    unit.newValue = "<!--abc-->";

    unit.redo(nodeParent.childNodes[0]);

    expect(nodeParent.childNodes[0].nodeValue).toEqual("abc");
  });
});
