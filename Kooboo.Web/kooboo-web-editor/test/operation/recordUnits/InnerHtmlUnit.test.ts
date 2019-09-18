import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";

describe("InnerHtmlUnit", () => {
  it("undo", () => {
    let node = document.createElement("div");
    node.innerHTML = "<div></div>";

    let unit = new InnerHtmlUnit("<p>abc</p>");
    unit.undo(node);

    expect(node.innerHTML).equal("<p>abc</p>");
    expect(unit.newValue).equal("<div></div>");
  });

  it("undo_existComment", () => {
    let node = document.createElement("div");
    node.innerHTML = "<div></div>";

    let unit = new InnerHtmlUnit("<!--abc--><p>abc</p>");
    unit.undo(node);

    expect(node.innerHTML).equal("<!--abc--><p>abc</p>");
    expect(unit.newValue).equal("<div></div>");
  });

  it("redo", () => {
    let node = document.createElement("div");
    node.innerHTML = "<p>abc</p>";

    let unit = new InnerHtmlUnit("<p>abc</p>");
    unit.newValue = "<div></div>";

    unit.redo(node);

    expect(node.innerHTML).equal("<div></div>");
  });

  it("redo_existComment", () => {
    let node = document.createElement("div");
    node.innerHTML = "<p>abc</p>";

    let unit = new InnerHtmlUnit("<p>abc</p>");
    unit.newValue = "<!--abc--><div></div>";

    unit.redo(node);

    expect(node.innerHTML).equal("<!--abc--><div></div>");
  });
});
