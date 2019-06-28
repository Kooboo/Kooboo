import { Operation } from "../../src/models/Operation";
import { KoobooComment } from "../../src/models/KoobooComment";
import { ACTION_TYPE, EDITOR_TYPE } from "../../src/common/constants";

describe("operation", () => {
  let dom = document.createElement("div");
  document.body.appendChild(dom);
  dom.outerHTML = "<div kooboo-id='1-1'>2</div>";
  dom.innerHTML = "2";
  const operation = new Operation(
    "1-1",
    "1",
    dom.innerHTML,
    new KoobooComment(
      "#kooboo--objecttype='view'--nameorid='Home'--boundary='627'"
    ),
    "123",
    ACTION_TYPE.update,
    "",
    EDITOR_TYPE.dom
  );

  test("undo", () => {
    operation.undo(document);
    expect(document.body.children.item(0)!.innerHTML).toEqual("2");
  });

  test("redo", () => {
    operation.undo(document);
    operation.redo(document);
    expect(document.body.children.item(0)!.innerHTML).toEqual("2");
  });
});
