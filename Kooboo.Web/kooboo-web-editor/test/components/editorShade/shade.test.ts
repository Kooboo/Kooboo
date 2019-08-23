import { createShade } from "@/components/editorShade/shade";

describe("shade", () => {
  it("hidden", () => {
    let shade = createShade();
    expect(shade.el.style.display).equal("");
    shade.hidden();
    expect(shade.el.style.display).equal("none");
  });

  it("updatePosition", () => {
    let shade = createShade();
    let el = document.createElement("div");
    shade.updatePosition(el);
    expect(shade.el.style.display).equal("block");
  });
});
