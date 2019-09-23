import { ActionType } from "@/operation/ActionType";
import { EDITOR_TYPE, OBJECT_TYPE } from "@/common/constants";
import { LabelLog } from "@/operation/recordLogs/LabelLog";

describe("LabelLog", () => {
  it("createDelete", () => {
    let log = LabelLog.createDelete("1");
    expect(log.nameOrId).equal("1");
    expect(log.action).equal(ActionType.delete);
    expect(log.editorType).equal(EDITOR_TYPE.label);
    expect(log.value).not.ok;
    expect(log.objectType).equal(OBJECT_TYPE.Label);
    expect(log.value).not.ok;
  });

  it("createUpdate", () => {
    let log = LabelLog.createUpdate("1", "a");
    expect(log.nameOrId).equal("1");
    expect(log.action).equal(ActionType.update);
    expect(log.editorType).equal(EDITOR_TYPE.label);
    expect(log.value).equal("a");
    expect(log.objectType).equal(OBJECT_TYPE.Label);
  });
});
