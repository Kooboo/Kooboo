import { createRect } from "@/components/selectorBorder/rect";
import { HOVER_BORDER_WIDTH, HOVER_BORDER_COLOR } from "@/common/constants";

describe("create border rect", () => {
  it("hidden", () => {
    let rect = createRect(HOVER_BORDER_WIDTH, HOVER_BORDER_COLOR);
    expect(rect.el.style.display).equal("");
    rect.hidden();
    expect(rect.el.style.display).equal("none");
  });

  it("updatePosition", () => {
    let rect = createRect(HOVER_BORDER_WIDTH, HOVER_BORDER_COLOR);
    let el = document.createElement("div");
    rect.updatePosition(el);
    expect(rect.el.style.display).equal("block");
  });
});
