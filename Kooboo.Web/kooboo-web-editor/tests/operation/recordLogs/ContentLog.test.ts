import { ContentLog } from "@/operation/recordLogs/ContentLog";
import { ActionType } from "@/operation/ActionType";
import { EDITOR_TYPE } from "@/common/constants";

describe("ContentLog", () => {
  test("createCopy", () => {
    let log = ContentLog.createCopy("1", "2");
    expect(log.nameOrId).toEqual("1");
    expect(log.orgNameOrId).toEqual("2");
    expect(log.action).toEqual(ActionType.copy);
    expect(log.editorType).toEqual(EDITOR_TYPE.content);
    expect(log.fieldName).toBeFalsy();
    expect(log.value).toBeFalsy();
  });

  test("createDelete", () => {
    let log = ContentLog.createDelete("1");
    expect(log.nameOrId).toEqual("1");
    expect(log.orgNameOrId).toBeFalsy();
    expect(log.action).toEqual(ActionType.delete);
    expect(log.editorType).toEqual(EDITOR_TYPE.content);
    expect(log.fieldName).toBeFalsy();
    expect(log.value).toBeFalsy();
  });

  test("createUpdate", () => {
    let log = ContentLog.createUpdate("1", "a", "b");
    expect(log.nameOrId).toEqual("1");
    expect(log.orgNameOrId).toBeFalsy();
    expect(log.action).toEqual(ActionType.update);
    expect(log.editorType).toEqual(EDITOR_TYPE.content);
    expect(log.fieldName).toEqual("a");
    expect(log.value).toEqual("b");
  });
});
