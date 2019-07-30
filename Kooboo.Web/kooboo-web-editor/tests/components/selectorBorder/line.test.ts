import { createLine } from "@/components/selectorBorder/line";
import { HOVER_BORDER_WIDTH, HOVER_BORDER_COLOR } from "@/common/constants";

describe("line", () => {
  test("create border line", () => {
    let line = createLine(HOVER_BORDER_WIDTH, HOVER_BORDER_COLOR);
    line.update({ top: 1, left: 2, width: 3, height: 4 });
    expect(line.el.style.top).toEqual("1px");
    expect(line.el.style.left).toEqual("2px");
    expect(line.el.style.width).toEqual("3px");
    expect(line.el.style.height).toEqual("4px");
  });
});