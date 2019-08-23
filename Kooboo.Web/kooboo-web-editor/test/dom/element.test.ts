import {
  createButton,
  createDiv,
  createInput,
  createIframe,
  createImg,
  createP,
  createPrimaryButton,
  createLabelInput,
  createRadioInput
} from "@/dom/element";

function getChildElementsForTagName(element: Element, tagName: string): Array<Element> {
  let elements = [];
  if (element.tagName.toLowerCase() == tagName) {
    elements.push(element);
  }

  for (let n = 0; n < element.children.length; n++) {
    let childElements = getChildElementsForTagName(element.children[n], tagName);
    elements = elements.concat(childElements);
  }

  return elements;
}

describe("element", () => {
  beforeEach(() => (document.body.innerHTML = ""));

  it("createButton", () => {
    let element = createButton("testbtn");

    expect(element.tagName.toLowerCase()).equal("div");
    expect(element.innerText).equal("testbtn");
  });

  it("createDiv", () => {
    let element = createDiv();

    expect(element.tagName.toLowerCase()).equal("div");
  });

  it("createInput", () => {
    let element = createInput();

    expect(element.tagName.toLowerCase()).equal("input");
  });

  it("createIframe", () => {
    let element = createIframe();

    expect(element.tagName.toLowerCase()).equal("iframe");
  });

  it("createImg", () => {
    let element = createImg();

    expect(element.tagName.toLowerCase()).equal("img");
  });

  it("createP", () => {
    let element = createP();

    expect(element.tagName.toLowerCase()).equal("p");
  });

  it("createPrimaryButton", () => {
    let element = createPrimaryButton("testbtn");

    expect(element.tagName.toLowerCase()).equal("div");
    expect(element.innerText).equal("testbtn");
  });

  it("createLabelInput", () => {
    let elementObject = createLabelInput("testInput", 100);
    elementObject.setContent("test");

    let input = getChildElementsForTagName(elementObject.input, "input")[0] as HTMLInputElement;
    expect(input.value).equal("test");

    let content = elementObject.getContent();
    expect(content).equal("test");
  });

  it("createRadioInput", () => {
    let elementObject = createRadioInput("testInput");
    elementObject.setChecked(true);

    let input = getChildElementsForTagName(elementObject.radio, "input")[0] as HTMLInputElement;
    expect(input.checked).equal(true);
  });
});
