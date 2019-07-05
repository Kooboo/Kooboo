import { ActionType } from "@/operation/ActionType";
import { EDITOR_TYPE, OBJECT_TYPE } from "@/common/constants";
import { DomLog } from "@/operation/recordLogs/DomLog";

describe("DomLog", () => {
  test("createDelete", () => {
    let log = DomLog.createDelete("1", "2", OBJECT_TYPE.dom);
    expect(log.nameOrId).toEqual("1");
    expect(log.action).toEqual(ActionType.delete);
    expect(log.editorType).toEqual(EDITOR_TYPE.dom);
    expect(log.value).toBeFalsy();
  });

  test("createUpdate", () => {
    let log = DomLog.createUpdate("1", "a", "koobooId", OBJECT_TYPE.dom);
    expect(log.nameOrId).toEqual("1");
    expect(log.action).toEqual(ActionType.update);
    expect(log.editorType).toEqual(EDITOR_TYPE.dom);
    expect(log.value).toEqual("a");
    expect(log.koobooId).toEqual("koobooId");
    expect(log.attributeName).toBeFalsy();
  });
});
