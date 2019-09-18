import { ActionType } from "@/operation/ActionType";
import { EDITOR_TYPE, OBJECT_TYPE } from "@/common/constants";
import { DomLog } from "@/operation/recordLogs/DomLog";

describe("DomLog", () => {
  it("createDelete", () => {
    let log = DomLog.createDelete("1", "2", OBJECT_TYPE.dom);
    expect(log.nameOrId).equal("1");
    expect(log.action).equal(ActionType.delete);
    expect(log.editorType).equal(EDITOR_TYPE.dom);
    expect(log.value).not.ok;
  });

  it("createUpdate", () => {
    let log = DomLog.createUpdate("1", "a", "koobooId", OBJECT_TYPE.dom);
    expect(log.nameOrId).equal("1");
    expect(log.action).equal(ActionType.update);
    expect(log.editorType).equal(EDITOR_TYPE.dom);
    expect(log.value).equal("a");
    expect(log.koobooId).equal("koobooId");
    expect(log.attributeName).not.ok;
  });
});
