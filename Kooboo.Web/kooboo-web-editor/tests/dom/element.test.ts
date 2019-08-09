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

  test("createButton", () => {
    let element = createButton("testbtn");

    expect(element.tagName.toLowerCase()).toEqual("div");
    expect(element.innerText).toEqual("testbtn");
  });

  test("createDiv", () => {
    let element = createDiv();

    expect(element.tagName.toLowerCase()).toEqual("div");
  });

  test("createInput", () => {
    let element = createInput();

    expect(element.tagName.toLowerCase()).toEqual("input");
  });

  test("createIframe", () => {
    let element = createIframe();

    expect(element.tagName.toLowerCase()).toEqual("iframe");
  });

  test("createImg", () => {
    let element = createImg();

    expect(element.tagName.toLowerCase()).toEqual("img");
  });

  test("createP", () => {
    let element = createP();

    expect(element.tagName.toLowerCase()).toEqual("p");
  });

  test("createPrimaryButton", () => {
    let element = createPrimaryButton("testbtn");

    expect(element.tagName.toLowerCase()).toEqual("div");
    expect(element.innerText).toEqual("testbtn");
  });

  test("createLabelInput", () => {
    let elementObject = createLabelInput("testInput", 100);
    elementObject.setContent("test");

    let input = getChildElementsForTagName(elementObject.input, "input")[0] as HTMLInputElement;
    expect(input.value).toEqual("test");

    let content = elementObject.getContent();
    expect(content).toEqual("test");
  });

  test("createRadioInput", () => {
    let elementObject = createRadioInput("testInput");
    elementObject.setChecked(true);

    let input = getChildElementsForTagName(elementObject.radio, "input")[0] as HTMLInputElement;
    expect(input.checked).toEqual(true);
  });
});
