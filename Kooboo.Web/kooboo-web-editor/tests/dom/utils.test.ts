import {
  getEditorContainer,
  getAllElement,
  getAllNode,
  isBody,
  isImg,
  isLink,
  canJump,
  previousNodes,
  nextNodes,
  previousComment
} from "@/dom/utils";
import { HOVER_BORDER_SKIP } from "@/common/constants";

describe("utils", () => {
  test("getEditorContainer", () => {
    let div = document.createElement("div");
    div.id = HOVER_BORDER_SKIP;
    div.setAttribute("editorContainer", "true");
    document.body.append(div);

    let editorContainer = getEditorContainer();
    expect(editorContainer.getAttribute("editorContainer")).toEqual("true");
  });

  test("getAllElement", () => {
    var el = document.createElement("div");
    el.innerHTML = `
      <div><span></span></div>
      <div><img></img></div>
      <div></div>
      `;
    let elements = getAllElement(el);

    expect([...elements].length).toEqual(5);
  });

  test("getAllElement_existTextNode", () => {
    var el = document.createElement("div");
    el.innerHTML = `
      <div>Text<span></span></div>
      <div>Text<img></img></div>
      <div></div>
      `;
    let elements = getAllElement(el);

    expect([...elements].length).toEqual(5);
  });

  test("getAllElement_containSelf", () => {
    var el = document.createElement("div");
    el.innerHTML = `
      <div><span></span></div>
      <div><img></img></div>
      <div></div>
      `;
    let elements = getAllElement(el, true);

    expect([...elements].length).toEqual(6);
  });

  test("getAllNode", () => {
    var el = document.createElement("div");
    el.innerHTML = `Text<!--Comment--><div>ElementText</div>`;
    let elements = getAllNode(el);
    let es = [...elements];

    expect(es.length).toEqual(4);
    expect(es[0].textContent).toEqual("Text");
    expect(es[1].textContent).toEqual("Comment");
    expect(es[2].textContent).toEqual("ElementText");
  });

  test("isBody", () => {
    var el = document.createElement("div");
    el.innerHTML = `Text<!--Comment--><div>ElementText</div>`;
    expect(isBody(document.body)).toEqual(true);
    expect(isBody(el)).toEqual(false);
  });

  test("isImg", () => {
    var el = document.createElement("img");
    var divel = document.createElement("div");
    expect(isImg(el)).toEqual(true);
    expect(isImg(divel)).toEqual(false);
  });

  test("isLink", () => {
    var el = document.createElement("a");
    var divel = document.createElement("div");
    expect(isLink(el)).toEqual(true);
    expect(isLink(divel)).toEqual(false);
  });

  test("canJump", () => {
    var el = document.createElement("a");
    el.setAttribute("href", "https://abc");

    expect(canJump(el)).toEqual(true);
  });

  test("canJump_otherElement", () => {
    var el = document.createElement("div");
    el.setAttribute("href", "https://abc");

    expect(canJump(el)).toEqual(false);
  });

  test("canJump_href#", () => {
    var el = document.createElement("a");
    el.setAttribute("href", "#abc");

    expect(canJump(el)).toEqual(false);
  });

  test("previousNodes", () => {
    let el = document.createElement("div");
    el.innerHTML = `<div id="elChild1"></div><div id="elChild2"><div2>`;
    let iter = previousNodes(el.children[1]);
    let previousNode = iter.next().value;

    expect((previousNode as Element).id).toEqual("elChild1");
  });

  test("previousNodes_containSelf", () => {
    let el = document.createElement("div");
    el.innerHTML = `<div id="elChild1"></div><div id="elChild2"><div2>`;
    let iter = previousNodes(el.children[1], true);
    let previousNode = iter.next().value;

    expect((previousNode as Element).id).toEqual("elChild2");
  });

  test("previousNodes_parent", () => {
    let el = document.createElement("div");
    el.id = "el";
    el.innerHTML = `<div id="elChild1"></div><div id="elChild2"><div2>`;
    let iter = previousNodes(el.children[0], false, true);
    let previousNode = iter.next().value;

    expect((previousNode as Element).id).toEqual("el");

    previousNode = iter.next().value;
    expect(previousNode).toEqual(undefined);
  });

  test("previousNodes_textNode", () => {
    let el = document.createElement("div");
    el.innerHTML = `<div id="elChild1"></div>Text<div id="elChild2"><div2>`;
    let iter = previousNodes(el.children[1]);
    let previousNode = iter.next().value;

    expect(previousNode.textContent).toEqual("Text");
  });

  test("nextNodes", () => {
    let el = document.createElement("div");
    el.innerHTML = `<div id="elChild1"></div><div id="elChild2"><div2>`;
    let iter = nextNodes(el.children[0]);
    let nextNode = iter.next().value;

    expect((nextNode as Element).id).toEqual("elChild2");
  });

  test("nextNodes_containSelf", () => {
    let el = document.createElement("div");
    el.innerHTML = `<div id="elChild1"></div><div id="elChild2"><div2>`;
    let iter = nextNodes(el.children[0], true);
    let nextNode = iter.next().value;

    expect((nextNode as Element).id).toEqual("elChild1");
  });

  test("nextNodes_parent", () => {
    let el = document.createElement("div");
    el.id = "el";
    el.innerHTML = `<div id="elChild1"></div><div id="elChild2"><div2>`;
    let iter = nextNodes(el.children[1], false, true);
    let nextNode = iter.next().value;

    expect((nextNode as Element).id).toEqual("el");

    nextNode = iter.next().value;
    expect(nextNode).toEqual(undefined);
  });

  test("nextNodes_textNode", () => {
    let el = document.createElement("div");
    el.innerHTML = `<div id="elChild1"></div>Text<div id="elChild2"><div2>`;
    let iter = nextNodes(el.children[0]);
    let nextNode = iter.next().value;

    expect(nextNode.textContent).toEqual("Text");
  });

  test("previousComment", () => {
    let temp = document.createElement("div");
    temp.innerHTML = `
    <div class="testimonial-thumb" kooboo-id="1-0-1-1-1-1-1-1">
                                    
      <!--#kooboo--objecttype='attribute'--nameorid='fbdc6f3b-19ea-565e-84f6-a174b9cfb8f0'--attributename='src'--bindingvalue='{List_Item.Icon}'--koobooid='1-0-1-1-1-1-1-1-1'-->

      <!--#kooboo--objecttype='attribute'--nameorid='fbdc6f3b-19ea-565e-84f6-a174b9cfb8f0'--attributename='alt'--bindingvalue='{List_Item.Name}'--koobooid='1-0-1-1-1-1-1-1-1'-->

      <img k-attributes="src {List_Item.Icon};alt {List_Item.Name};" kooboo-id="1-0-1-1-1-1-1-1-1" src="/moban41-template/assets/img/testimonial_img3.png" alt="Name 3">
      
    </div>
    `;

    let img = temp.querySelector("[kooboo-id='1-0-1-1-1-1-1-1-1']")!;
    let comment = previousComment(img);

    expect(comment instanceof Comment).toBeTruthy();
    expect((comment as Comment).nodeValue).toEqual(
      "#kooboo--objecttype='attribute'--nameorid='fbdc6f3b-19ea-565e-84f6-a174b9cfb8f0'--attributename='alt'--bindingvalue='{List_Item.Name}'--koobooid='1-0-1-1-1-1-1-1-1'"
    );
  });
});
