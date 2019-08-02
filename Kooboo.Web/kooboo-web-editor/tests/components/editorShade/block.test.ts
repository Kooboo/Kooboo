import { EDITOR_SHADE_COLOR, STANDARD_Z_INDEX } from "@/common/constants";
import { createBlock } from "@/components/editorShade/block";

describe("block", () => {
  test("create shade block", () => {
    let block = createBlock();
    block.update({ top: 1, left: 2, width: 3, height: 4 });
    expect(block.el.style.top).toEqual("1px");
    expect(block.el.style.left).toEqual("2px");
    expect(block.el.style.width).toEqual("3px");
    expect(block.el.style.height).toEqual("4px");
    expect(block.el.style.position).toEqual("fixed");
    expect(block.el.style.display).toEqual("block");
    let backgroundColor = block.el.style.backgroundColor;
    // @ts-ignore
    expect(backgroundColor.replace(/\s/g, "")).toEqual(EDITOR_SHADE_COLOR.replace(/\s/g, ""));
    expect(block.el.style.zIndex).toEqual(STANDARD_Z_INDEX + "");
    expect(block.el.style.cursor).toEqual("not-allowed");
  });
});
