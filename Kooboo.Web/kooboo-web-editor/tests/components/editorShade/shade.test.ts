import { createShade } from "@/components/editorShade/shade";

describe("shade", () => {
  test("hidden", () => {
    let shade = createShade();
    expect(shade.el.style.display).toEqual("block");
    shade.hidden();
    expect(shade.el.style.display).toEqual("none");
  });

  test("updatePosition", () => {
    let shade = createShade();
    let el = document.createElement("div");
    shade.updatePosition(el);
    expect(shade.el.style.display).toEqual("block");
  });
});
