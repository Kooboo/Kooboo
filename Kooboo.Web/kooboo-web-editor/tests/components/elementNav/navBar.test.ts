import { createNavBar } from "@/components/elementNav/navBar";

describe("navBar", () => {
  test("update", () => {
    let { el, update } = createNavBar();
    update([document.createElement("div"), document.createElement("p"), document.createElement("img")]);

    expect(el.style.display).toEqual("inline-block");
    expect(el.children.length).toEqual(3);
  });
});
