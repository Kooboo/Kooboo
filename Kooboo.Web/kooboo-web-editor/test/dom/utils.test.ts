import { getAllElement, getAllNode, isBody, isImg, isLink, canJump, previousNodes, nextNodes, getParentElements, isInTable } from "@/dom/utils";
import { previousComment } from "@/kooboo/utils";

describe("utils", () => {
  beforeEach(
    () =>
      (document.body.innerHTML = `
`)
  );

  it("getAllElement", () => {
    var el = document.createElement("div");
    el.innerHTML = `
      <div><span></span></div>
      <div><img></img></div>
      <div></div>
      `;
    let elements = getAllElement(el);

    expect([...elements].length).equal(5);
  });

  it("getAllElement_existTextNode", () => {
    var el = document.createElement("div");
    el.innerHTML = `
      <div>Text<span></span></div>
      <div>Text<img></img></div>
      <div></div>
      `;
    let elements = getAllElement(el);

    expect([...elements].length).equal(5);
  });

  it("getAllElement_containSelf", () => {
    var el = document.createElement("div");
    el.innerHTML = `
      <div><span></span></div>
      <div><img></img></div>
      <div></div>
      `;
    let elements = getAllElement(el, true);

    expect([...elements].length).equal(6);
  });

  it("getAllNode", () => {
    var el = document.createElement("div");
    el.innerHTML = `Text<!--Comment--><div>ElementText</div>`;
    let elements = getAllNode(el);
    let es = [...elements];

    expect(es.length).equal(4);
    expect(es[0].textContent).equal("Text");
    expect(es[1].textContent).equal("Comment");
    expect(es[2].textContent).equal("ElementText");
  });

  it("isBody", () => {
    var el = document.createElement("div");
    el.innerHTML = `Text<!--Comment--><div>ElementText</div>`;
    expect(isBody(document.body)).equal(true);
    expect(isBody(el)).equal(false);
  });

  it("isImg", () => {
    var el = document.createElement("img");
    var divel = document.createElement("div");
    expect(isImg(el)).equal(true);
    expect(isImg(divel)).equal(false);
  });

  it("isLink", () => {
    var el = document.createElement("a");
    var divel = document.createElement("div");
    expect(isLink(el)).equal(true);
    expect(isLink(divel)).equal(false);
  });

  it("canJump", () => {
    var el = document.createElement("a");
    el.setAttribute("href", "https://abc");

    expect(canJump(el)).equal(false);
  });

  it("canJump_otherElement", () => {
    var el = document.createElement("div");
    el.setAttribute("href", "https://abc");

    expect(canJump(el)).equal(false);
  });

  it("canJump_href#", () => {
    var el = document.createElement("a");
    el.setAttribute("href", "#abc");

    expect(canJump(el)).equal(false);
  });
  it("canJump_href_mail", () => {
    var el = document.createElement("a");
    el.setAttribute("href", "mailto:test@test.com");

    expect(canJump(el)).equal(false);
  });

  it("previousNodes", () => {
    let el = document.createElement("div");
    el.innerHTML = `<div id="elChild1"></div><div id="elChild2"><div2>`;
    let iter = previousNodes(el.children[1]);
    let previousNode = iter.next().value;

    expect((previousNode as Element).id).equal("elChild1");
  });

  it("previousNodes_containSelf", () => {
    let el = document.createElement("div");
    el.innerHTML = `<div id="elChild1"></div><div id="elChild2"><div2>`;
    let iter = previousNodes(el.children[1], true);
    let previousNode = iter.next().value;

    expect((previousNode as Element).id).equal("elChild2");
  });

  it("previousNodes_parent", () => {
    let el = document.createElement("div");
    el.id = "el";
    el.innerHTML = `<div id="elChild1"></div><div id="elChild2"><div2>`;
    let iter = previousNodes(el.children[0], false, true);
    let previousNode = iter.next().value;

    expect((previousNode as Element).id).equal("el");

    previousNode = iter.next().value;
    expect(previousNode).equal(undefined);
  });

  it("previousNodes_textNode", () => {
    let el = document.createElement("div");
    el.innerHTML = `<div id="elChild1"></div>Text<div id="elChild2"><div2>`;
    let iter = previousNodes(el.children[1]);
    let previousNode = iter.next().value as HTMLElement;

    expect(previousNode.textContent).equal("Text");
  });

  it("nextNodes", () => {
    let el = document.createElement("div");
    el.innerHTML = `<div id="elChild1"></div><div id="elChild2"><div2>`;
    let iter = nextNodes(el.children[0]);
    let nextNode = iter.next().value;

    expect((nextNode as Element).id).equal("elChild2");
  });

  it("nextNodes_containSelf", () => {
    let el = document.createElement("div");
    el.innerHTML = `<div id="elChild1"></div><div id="elChild2"><div2>`;
    let iter = nextNodes(el.children[0], true);
    let nextNode = iter.next().value;

    expect((nextNode as Element).id).equal("elChild1");
  });

  it("nextNodes_parent", () => {
    let el = document.createElement("div");
    el.id = "el";
    el.innerHTML = `<div id="elChild1"></div><div id="elChild2"><div2>`;
    let iter = nextNodes(el.children[1], false, true);
    let nextNode = iter.next().value;

    expect((nextNode as Element).id).equal("el");

    nextNode = iter.next().value;
    expect(nextNode).equal(undefined);
  });

  it("nextNodes_textNode", () => {
    let el = document.createElement("div");
    el.innerHTML = `<div id="elChild1"></div>Text<div id="elChild2"><div2>`;
    let iter = nextNodes(el.children[0]);
    let nextNode = iter.next().value as HTMLElement;

    expect(nextNode!.textContent).equal("Text");
  });

  it("previousComment", () => {
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

    expect(comment instanceof Comment).is.ok;
    expect((comment as Comment).nodeValue).equal(
      "#kooboo--objecttype='attribute'--nameorid='fbdc6f3b-19ea-565e-84f6-a174b9cfb8f0'--attributename='alt'--bindingvalue='{List_Item.Name}'--koobooid='1-0-1-1-1-1-1-1-1'"
    );
  });

  it("getParentElements", () => {
    document.body.innerHTML = `
      <div>
        <p>
          <i></i>
        </p>
      </div>
    `;

    let el = document.querySelector("i")!;
    let elements = getParentElements(el);
    let elements2 = getParentElements(el, false);
    expect(elements.length).eq(5);
    expect(elements2.length).eq(4);
  });

  it("isInTable", () => {
    // language=HTML
    document.body.innerHTML = `
    `;
    //是表格元素
    let el1 = document.createElement("tbody");
    expect(isInTable(el1)).equal(true);
    //不是表格元素
    let el2 = document.createElement("div");
    expect(isInTable(el2)).equal(false);
  });
});
