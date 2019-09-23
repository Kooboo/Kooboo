import { TextShadow } from "@/dom/TextShadow";

describe("TextShadow class test", () => {
  it("TextShadow_constructor", () => {
    let styleObject;

    styleObject = new TextShadow("1px 2px #fff");
    expect(styleObject.hShadow).equal("1px");
    expect(styleObject.vShadow).equal("2px");
    expect(styleObject.color).equal("#fff");

    styleObject = new TextShadow("1px 2px 3px #fff");
    expect(styleObject.hShadow).equal("1px");
    expect(styleObject.vShadow).equal("2px");
    expect(styleObject.blur).equal("3px");
    expect(styleObject.color).equal("#fff");

    styleObject = new TextShadow("#fff 1px 2px 3px");
    expect(styleObject.hShadow).equal("1px");
    expect(styleObject.vShadow).equal("2px");
    expect(styleObject.blur).equal("3px");
    expect(styleObject.color).equal("#fff");

    styleObject = new TextShadow("gray 1px 2px 3px");
    expect(styleObject.hShadow).equal("1px");
    expect(styleObject.vShadow).equal("2px");
    expect(styleObject.blur).equal("3px");
    expect(styleObject.color).equal("gray");

    styleObject = new TextShadow("rgba(250, 250, 0, 1.0) 1px 2px 3px");
    expect(styleObject.hShadow).equal("1px");
    expect(styleObject.vShadow).equal("2px");
    expect(styleObject.blur).equal("3px");
    expect(styleObject.color).equal("rgba(250,250,0,1.0)");
  });

  it("TextShadow toString", () => {
    let styleObject = new TextShadow("1px 2px #fff");
    styleObject.blur = "3px";
    styleObject.color = "#000";

    let toStringResult = styleObject.toString();
    expect(toStringResult.trim()).equal("1px 2px 3px #000");
  });
});
