import { TextShadow } from "@/dom/TextShadow";

describe("TextShadow class test", () => {
  test("TextShadow_constructor", () => {
    let styleObject;

    styleObject = new TextShadow("1px 2px #fff");
    expect(styleObject.hShadow).toEqual("1px");
    expect(styleObject.vShadow).toEqual("2px");
    expect(styleObject.color).toEqual("#fff");

    styleObject = new TextShadow("1px 2px 3px #fff");
    expect(styleObject.hShadow).toEqual("1px");
    expect(styleObject.vShadow).toEqual("2px");
    expect(styleObject.blur).toEqual("3px");
    expect(styleObject.color).toEqual("#fff");
  });

  test("TextShadow toString", () => {
    let styleObject = new TextShadow("1px 2px #fff");
    styleObject.blur = "3px";
    styleObject.color = "#000";

    let toStringResult = styleObject.toString();
    expect(toStringResult.trim()).toEqual("1px 2px 3px #000");
  });
});
