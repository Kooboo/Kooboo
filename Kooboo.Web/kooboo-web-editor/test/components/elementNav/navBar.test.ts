import { createNavBar } from "@/components/elementNav/navBar";

describe("navBar", () => {
  it("update", () => {
    let { el, update } = createNavBar();
    update([document.createElement("div"), document.createElement("p"), document.createElement("img")]);

    expect(el.style.display).equal("inline-block");
    expect(el.children.length).equal(3);
  });
});
