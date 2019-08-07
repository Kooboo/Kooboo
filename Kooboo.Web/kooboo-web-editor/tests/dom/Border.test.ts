import { Border } from "@/dom/Border";

describe("Border class test", () => {
  test("Border_constructor", () => {
    let styleObject;

    styleObject = new Border("1px solid #fff");
    expect(styleObject.width).toEqual("1px");
    expect(styleObject.style).toEqual("solid");
    expect(styleObject.color).toEqual("#fff");
  });

  test("Border toString", () => {
    let styleObject = new Border("1px solid #fff");
    styleObject.width = "2px";
    styleObject.color = "#fff0";

    let toStringResult = styleObject.toString();
    expect(toStringResult.trim()).toEqual("2px solid #fff0");
  });
});
