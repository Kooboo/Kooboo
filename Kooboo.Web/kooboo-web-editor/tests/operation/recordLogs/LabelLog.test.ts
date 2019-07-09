import { ActionType } from "@/operation/ActionType";
import { EDITOR_TYPE, OBJECT_TYPE } from "@/common/constants";
import { LabelLog } from "@/operation/recordLogs/LabelLog";

describe("LabelLog", () => {
  test("createDelete", () => {
    let log = LabelLog.createDelete("1");
    expect(log.nameOrId).toEqual("1");
    expect(log.action).toEqual(ActionType.delete);
    expect(log.editorType).toEqual(EDITOR_TYPE.label);
    expect(log.value).toBeFalsy();
    expect(log.objectType).toEqual(OBJECT_TYPE.Label);
    expect(log.value).toBeFalsy();
  });

  test("createUpdate", () => {
    let log = LabelLog.createUpdate("1", "a");
    expect(log.nameOrId).toEqual("1");
    expect(log.action).toEqual(ActionType.update);
    expect(log.editorType).toEqual(EDITOR_TYPE.label);
    expect(log.value).toEqual("a");
    expect(log.objectType).toEqual(OBJECT_TYPE.Label);
  });
});
