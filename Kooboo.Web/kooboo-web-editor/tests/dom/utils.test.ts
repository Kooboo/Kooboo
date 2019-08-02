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
  getParentElements,
  isInEditorContainer,
  getBackgroundImage,
  clearBackgroundImage,
  getBackgroundColor,
  isInTable
} from "@/dom/utils";
import { HOVER_BORDER_SKIP } from "@/common/constants";
import { previousComment } from "@/kooboo/utils";

describe("utils", () => {
  beforeEach(
    () =>
      (document.body.innerHTML = `
`)
  );
  test("isInEditorContainer", () => {
    let div = document.createElement("div");
    div.id = HOVER_BORDER_SKIP;
    div.setAttribute("editorContainer", "true");
    document.body.append(div);
    let editorContainer = getEditorContainer();
    editorContainer.addEventListener("click", e => {
      expect(isInEditorContainer(e)).toEqual(true);
      //id 不等于 HOVER_BORDER_SKIP时返回false
      editorContainer.id = "test";
      expect(isInEditorContainer(e)).toEqual(false);
    });
    editorContainer.click();
  });
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

    expect(canJump(el)).toEqual(false);
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

  test("getParentElements", () => {
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
    expect(elements.length).toBe(5);
    expect(elements2.length).toBe(4);
  });
  test("getBackgroundImage", () => {
    // language=HTML
    document.body.innerHTML = `
      <div>
    <img id = 'test' style="background: rgb(255, 255, 255) url(test.png) no-repeat fixed top"/>
      </div>
    `;
    let el = document.getElementById("test");
    let getBackgroundImageResult = getBackgroundImage(el!);
    let image = getBackgroundImageResult.image;
    let imageInBackground = getBackgroundImageResult.imageInBackground;
    expect(image).toEqual("url(test.png)");
    expect(imageInBackground).toEqual(true);
  });
  test("getBackgroundImage_imageInBackground undefined", () => {
    // language=HTML
    document.body.innerHTML = `
      <div>
    <img id = 'test'/>
      </div>
    `;
    let el = document.getElementById("test");
    let getBackgroundImageResult = getBackgroundImage(el!);
    let image = getBackgroundImageResult.image;
    let imageInBackground = getBackgroundImageResult.imageInBackground;
    expect(image).toEqual("");
    expect(imageInBackground).toEqual(false);
  });
  test("getBackgroundImage_getBackgroundColor_ inline style defined but Embedded style undefined", () => {
    // language=HTML
    document.body.innerHTML = `
      <style type="text/css">
        #test{
          height: 500px;
          width: 500px;
          background: background: rgb(255, 255, 255) url(http://iph.href.lu/200x300?text=test) no-repeat fixed top"
        }

      </style>
      <div>
    <img id = 'test'/>
      </div>
    `;
    let el = document.getElementById("test");
    let getBackgroundImageResult = getBackgroundImage(el!);
    let image = getBackgroundImageResult.image;
    let imageInBackground = getBackgroundImageResult.imageInBackground;
    expect(image).toEqual("url(http://iph.href.lu/200x300?text=test)");
    expect(imageInBackground).toEqual(false);
  });
  test("clearBackgroundImage", () => {
    // language=HTML
    document.body.innerHTML = `
        
      <div>
        <img id = 'test' style="background: rgb(255, 255, 255) url(http://iph.href.lu/200x300?text=test) no-repeat fixed top"/>
      </div>
    `;
    let el = document.getElementById("test") as HTMLImageElement;
    clearBackgroundImage(el, true);
    expect(el!.style!.backgroundImage!.trim()).toEqual("url(none)");
  });
  test("getBackgroundColor", () => {
    // language=HTML
    document.body.innerHTML = `
      <div>
        <img id = 'test' style="background: rgb(255, 255, 255) url(http://iph.href.lu/200x300?text=test) no-repeat fixed top"/>
      </div>
    `;
    let el = document.getElementById("test") as HTMLImageElement;
    let getBackgroundColorResult = getBackgroundColor(el);
    let color = getBackgroundColorResult.color;
    let colorInBackground = getBackgroundColorResult.colorInBackground;
    expect(color.trim()).toEqual("rgb(255, 255, 255)");
    expect(colorInBackground).toEqual(true);
  });
  test("getBackgroundColor_ inline style defined but Embedded style undefined", () => {
    // language=HTML
    document.body.innerHTML = `
      <style type="text/css">
        #test{
          height: 500px;
          width: 500px;
          background: background: rgb(255, 255, 255) url(http://iph.href.lu/200x300?text=test) no-repeat fixed top"
        }

      </style>
      <div>
        <img id = 'test' style="background:  url(none) no-repeat fixed top"/>
      </div>
    `;
    let el = document.getElementById("test") as HTMLImageElement;
    el!.parentElement!.style.backgroundColor = "red";
    let getBackgroundColorResult = getBackgroundColor(el);
    let color = getBackgroundColorResult.color;
    let colorInBackground = getBackgroundColorResult.colorInBackground;
    expect(color.trim()).toEqual("rgb(255, 255, 255)");
    expect(colorInBackground).toEqual(false);
  });
  test("isInTable", () => {
    // language=HTML
    document.body.innerHTML = `
    `;
    //是表格元素
    let el1 = document.createElement("tbody");
    expect(isInTable(el1)).toEqual(true);
    //不是表格元素
    let el2 = document.createElement("div");
    expect(isInTable(el2)).toEqual(false);
  });
});
