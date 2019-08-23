import { Border } from "@/dom/Border";

describe("Border class test", () => {
  it("Border_constructor", () => {
    let styleObject;

    styleObject = new Border("1px solid #fff");
    expect(styleObject.width).equal("1px");
    expect(styleObject.style).equal("solid");
    expect(styleObject.color).equal("rgb(255, 255, 255)");
  });

  it("Border toString", () => {
    let styleObject = new Border("1px solid #fff");
    styleObject.width = "2px";
    styleObject.color = "#fff0";

    let toStringResult = styleObject.toString();
    expect(toStringResult.trim()).equal("2px solid #fff0");
  });
});
