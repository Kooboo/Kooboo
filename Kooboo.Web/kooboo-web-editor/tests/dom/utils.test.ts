import { getAllElement } from "../../src/dom/utils";

describe("utils", () => {
  test("getAllElement", () => {
    var el = document.createElement("div");
    el.innerHTML = `
      <div><span></span></div>
      <div><img></img></div>
      <div></div>
      `;
    let elements = getAllElement(el);

    expect([...elements].length).toEqual(5);
  });
});
