import { Background } from "@/dom/Background";

describe("Background class test", () => {
  it("Background constructor ", () => {
    let el = document.createElement("div") as HTMLElement;
    el.style.background = "rgb(255, 255, 255) url(test.png) no-repeat fixed top";
    let background = new Background(el.style.background);
    expect(background.image).equal('url("test.png")');
    expect(background.color).equal("rgb(255, 255, 255)");
    expect(background.repeat).equal("no-repeat");
    expect(background.position).equal("center top");
  });
  it("Background toString", () => {
    let el = document.createElement("div") as HTMLElement;
    el.style.background = "rgb(255, 255, 255) url(test.png) no-repeat fixed top";
    let background = new Background(el.style.background);
    let toStringResult = background.toString();
    expect(toStringResult.trim()).equal("rgb(255, 255, 255) url('test.png') center top no-repeat fixed");
  });
});
