import { CopyUnit } from "@/operation/recordUnits/CopyUnit";

describe("CopyUnit", () => {
  test("undo", () => {
    let nodeParent = document.createElement("div");
    nodeParent.innerHTML = "<img>";

    let unit = new CopyUnit("<div></div>");
    unit.undo(nodeParent.children[0]);

    expect(nodeParent.children[0].outerHTML).toEqual("<div></div>");
    expect(unit.newValue).toEqual("<img>");
  });

  test("redo", () => {
    let nodeParent = document.createElement("div");
    nodeParent.innerHTML = "<img>";
    let unit = new CopyUnit("<img>");
    unit.newValue = "<div></div>";
    unit.redo(nodeParent.children[0]);
    expect(unit.oldValue).toEqual("<img>");
    expect(nodeParent.children[0].outerHTML).toEqual("<div></div>");
  });
});
