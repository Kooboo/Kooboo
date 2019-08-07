import { Outline } from "@/dom/Outline";

describe("Outline class test", () => {
  test("Outline_constructor", () => {
    let styleObject;

    styleObject = new Outline("1px solid #fff");
    expect(styleObject.width).toEqual("1px");
    expect(styleObject.style).toEqual("solid");
    expect(styleObject.color).toEqual("#fff");
  });

  test("Outline toString", () => {
    let styleObject = new Outline("1px solid #fff");
    styleObject.width = "2px";
    styleObject.color = "#fff0";

    let toStringResult = styleObject.toString();
    expect(toStringResult.trim()).toEqual("2px solid #fff0");
  });
});
