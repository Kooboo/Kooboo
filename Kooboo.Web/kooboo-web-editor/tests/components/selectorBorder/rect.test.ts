import { createRect } from "@/components/selectorBorder/rect";

describe("create border rect", () => {
  test("hidden", () => {
    let rect = createRect();
    expect(rect.el.style.display).toEqual("");
    rect.hidden();
    expect(rect.el.style.display).toEqual("none");
  });

  test("updatePosition", () => {
    let rect = createRect();
    let el = document.createElement("div");
    rect.updatePosition(el);
    expect(rect.el.style.display).toEqual("block");
  });
});
