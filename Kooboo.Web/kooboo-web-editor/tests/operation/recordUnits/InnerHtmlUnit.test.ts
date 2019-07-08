import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";

describe("InnerHtmlUnit", () => {
  test("undo", () => {
    let node = document.createElement("div");
    node.innerHTML = "<div></div>";

    let unit = new InnerHtmlUnit("<p>abc</p>");
    unit.undo(node);

    expect(node.innerHTML).toEqual("<p>abc</p>");
    expect(unit.newValue).toEqual("<div></div>");
  });

  test("undo_existComment", () => {
    let node = document.createElement("div");
    node.innerHTML = "<div></div>";

    let unit = new InnerHtmlUnit("<!--abc--><p>abc</p>");
    unit.undo(node);

    expect(node.innerHTML).toEqual("<!--abc--><p>abc</p>");
    expect(unit.newValue).toEqual("<div></div>");
  });

  test("redo", () => {
    let node = document.createElement("div");
    node.innerHTML = "<p>abc</p>";

    let unit = new InnerHtmlUnit("<p>abc</p>");
    unit.newValue = "<div></div>";

    unit.redo(node);

    expect(node.innerHTML).toEqual("<div></div>");
  });

  test("redo_existComment", () => {
    let node = document.createElement("div");
    node.innerHTML = "<p>abc</p>";

    let unit = new InnerHtmlUnit("<p>abc</p>");
    unit.newValue = "<!--abc--><div></div>";

    unit.redo(node);

    expect(node.innerHTML).toEqual("<!--abc--><div></div>");
  });
});
