import { OperationManager } from "../../src/models/operationManager";
import { Operation } from "../../src/models/Operation";
import { KoobooComment } from "../../src/models/KoobooComment";
import { ACTION_TYPE } from "../../src/constants";

describe("OperationManager", () => {
  const operationManager = new OperationManager();
  beforeAll(() => {
    operationManager.add(
      new Operation(
        document.createElement("div"),
        "1",
        "2",
        new KoobooComment(
          "#kooboo--objecttype='view'--nameorid='Home'--boundary='627'"
        ),
        "123",
        ACTION_TYPE.update
      )
    );
  });

  test("add operation", () => {
    expect(operationManager.operations.length).toBe(1);
    expect(operationManager.operations[0].newInnerHTML).toBe("2");
  });

  test("previous operation", () => {
    operationManager.previous();

    expect(operationManager.operations.length).toBe(0);
    expect(operationManager.backupOperations.length).toBe(1);
  });

  test("next operation", () => {
    operationManager.previous();
    operationManager.next();

    expect(operationManager.operations.length).toBe(1);
    expect(operationManager.backupOperations.length).toBe(0);
  });
});
