import { Operation } from "../../src/models/Operation";
import { KoobooComment } from "../../src/models/KoobooComment";
import { ACTION_TYPE } from "../../src/constants";

describe("operation", () => {
  let dom = document.createElement("div");
  dom.innerHTML = "2";
  const operation = new Operation(
    dom,
    "1",
    dom.innerHTML,
    new KoobooComment(
      "#kooboo--objecttype='view'--nameorid='Home'--boundary='627'"
    ),
    "123",
    ACTION_TYPE.update
  );

  test("undo", () => {
    operation.undo();
    expect(operation.dom.innerHTML).toEqual("1");
  });

  test("redo", () => {
    operation.undo();
    operation.redo();
    expect(operation.dom.innerHTML).toEqual("2");
  });
});