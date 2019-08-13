import { BoxShadow } from "@/dom/BoxShadow";

describe("BoxShadow class test", () => {
  test("BoxShadow_constructor", () => {
    let styleObject;

    styleObject = new BoxShadow("1px 2px #fff");
    expect(styleObject.hShadow).toEqual("1px");
    expect(styleObject.vShadow).toEqual("2px");
    expect(styleObject.color).toEqual("#fff");

    styleObject = new BoxShadow("1px 2px 3px #fff");
    expect(styleObject.hShadow).toEqual("1px");
    expect(styleObject.vShadow).toEqual("2px");
    expect(styleObject.blur).toEqual("3px");
    expect(styleObject.color).toEqual("#fff");

    styleObject = new BoxShadow("1px 2px 3px 4px #fff");
    expect(styleObject.hShadow).toEqual("1px");
    expect(styleObject.vShadow).toEqual("2px");
    expect(styleObject.blur).toEqual("3px");
    expect(styleObject.spread).toEqual("4px");
    expect(styleObject.color).toEqual("#fff");

    styleObject = new BoxShadow("1px 2px 3px 4px #fff inset");
    expect(styleObject.hShadow).toEqual("1px");
    expect(styleObject.vShadow).toEqual("2px");
    expect(styleObject.blur).toEqual("3px");
    expect(styleObject.spread).toEqual("4px");
    expect(styleObject.color).toEqual("#fff");
    expect(styleObject.inset).toEqual("inset");

    styleObject = new BoxShadow("#fff 1px 2px 3px");
    expect(styleObject.hShadow).toEqual("1px");
    expect(styleObject.vShadow).toEqual("2px");
    expect(styleObject.blur).toEqual("3px");
    expect(styleObject.color).toEqual("#fff");

    styleObject = new BoxShadow("gray 1px 2px 3px");
    expect(styleObject.hShadow).toEqual("1px");
    expect(styleObject.vShadow).toEqual("2px");
    expect(styleObject.blur).toEqual("3px");
    expect(styleObject.color).toEqual("gray");

    styleObject = new BoxShadow("rgba(250, 250, 0, 1.0) 1px 2px 3px");
    expect(styleObject.hShadow).toEqual("1px");
    expect(styleObject.vShadow).toEqual("2px");
    expect(styleObject.blur).toEqual("3px");
    expect(styleObject.color).toEqual("rgba(250,250,0,1.0)");
  });

  test("BoxShadow toString", () => {
    let styleObject = new BoxShadow("1px 2px #fff");
    styleObject.blur = "3px";
    styleObject.spread = "4px";
    styleObject.inset = "inset";

    let toStringResult = styleObject.toString();
    expect(toStringResult.trim()).toEqual("1px 2px 3px 4px #fff inset");
  });
});
