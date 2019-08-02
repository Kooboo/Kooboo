import { Background } from "@/dom/Background";

describe("Background class test", () => {
  test("Background constructor ", () => {
    let el = document.createElement("div") as HTMLElement;
    el.style.background = "rgb(255, 255, 255) url(test.png) no-repeat fixed top";
    let background = new Background(el.style.background);
    expect(background.image).toEqual("url(test.png)");
    expect(background.color).toEqual("rgb(255, 255, 255)");
    expect(background.repeat).toEqual("no-repeat");
    expect(background.position).toEqual("top");
  });
  test("Background toString", () => {
    let el = document.createElement("div") as HTMLElement;
    el.style.background = "rgb(255, 255, 255) url(test.png) no-repeat fixed top";
    let background = new Background(el.style.background);
    let toStringResult = background.toString();
    expect(toStringResult.trim()).toEqual("rgb(255, 255, 255) url(test.png) top no-repeat fixed");
  });
});
