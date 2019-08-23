import { EDITOR_SHADE_COLOR, STANDARD_Z_INDEX } from "@/common/constants";
import { createBlock } from "@/components/editorShade/block";

describe("block", () => {
  it("create shade block", () => {
    let block = createBlock();
    block.update({ top: 1, left: 2, width: 3, height: 4 });
    expect(block.el.style.top).equal("1px");
    expect(block.el.style.left).equal("2px");
    expect(block.el.style.width).equal("3px");
    expect(block.el.style.height).equal("4px");
    expect(block.el.style.position).equal("fixed");
    expect(block.el.style.display).equal("block");
    let backgroundColor = block.el.style.backgroundColor;
    // @ts-ignore
    expect(backgroundColor.replace(/\s/g, "")).equal(EDITOR_SHADE_COLOR.replace(/\s/g, ""));
    expect(block.el.style.zIndex).equal(STANDARD_Z_INDEX + "");
    expect(block.el.style.cursor).equal("not-allowed");
  });
});
