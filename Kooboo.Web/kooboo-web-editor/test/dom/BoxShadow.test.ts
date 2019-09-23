import { BoxShadow } from "@/dom/BoxShadow";

describe("BoxShadow class test", () => {
  it("BoxShadow_constructor", () => {
    let styleObject;

    styleObject = new BoxShadow("1px 2px #fff");
    expect(styleObject.hShadow).equal("1px");
    expect(styleObject.vShadow).equal("2px");
    expect(styleObject.color).equal("#fff");

    styleObject = new BoxShadow("1px 2px 3px #fff");
    expect(styleObject.hShadow).equal("1px");
    expect(styleObject.vShadow).equal("2px");
    expect(styleObject.blur).equal("3px");
    expect(styleObject.color).equal("#fff");

    styleObject = new BoxShadow("1px 2px 3px 4px #fff");
    expect(styleObject.hShadow).equal("1px");
    expect(styleObject.vShadow).equal("2px");
    expect(styleObject.blur).equal("3px");
    expect(styleObject.spread).equal("4px");
    expect(styleObject.color).equal("#fff");

    styleObject = new BoxShadow("1px 2px 3px 4px #fff inset");
    expect(styleObject.hShadow).equal("1px");
    expect(styleObject.vShadow).equal("2px");
    expect(styleObject.blur).equal("3px");
    expect(styleObject.spread).equal("4px");
    expect(styleObject.color).equal("#fff");
    expect(styleObject.inset).equal("inset");

    styleObject = new BoxShadow("#fff 1px 2px 3px");
    expect(styleObject.hShadow).equal("1px");
    expect(styleObject.vShadow).equal("2px");
    expect(styleObject.blur).equal("3px");
    expect(styleObject.color).equal("#fff");

    styleObject = new BoxShadow("gray 1px 2px 3px");
    expect(styleObject.hShadow).equal("1px");
    expect(styleObject.vShadow).equal("2px");
    expect(styleObject.blur).equal("3px");
    expect(styleObject.color).equal("gray");

    styleObject = new BoxShadow("rgba(250, 250, 0, 1.0) 1px 2px 3px");
    expect(styleObject.hShadow).equal("1px");
    expect(styleObject.vShadow).equal("2px");
    expect(styleObject.blur).equal("3px");
    expect(styleObject.color).equal("rgba(250,250,0,1.0)");
  });

  it("BoxShadow toString", () => {
    let styleObject = new BoxShadow("1px 2px #fff");
    styleObject.blur = "3px";
    styleObject.spread = "4px";
    styleObject.inset = "inset";

    let toStringResult = styleObject.toString();
    expect(toStringResult.trim()).equal("1px 2px 3px 4px #fff inset");
  });
});
