import { ContentLog } from "@/operation/recordLogs/ContentLog";
import { ActionType } from "@/operation/ActionType";
import { EDITOR_TYPE } from "@/common/constants";

describe("ContentLog", () => {
  it("createCopy", () => {
    let log = ContentLog.createCopy("1", "2");
    expect(log.nameOrId).equal("1");
    expect(log.orgNameOrId).equal("2");
    expect(log.action).equal(ActionType.copy);
    expect(log.editorType).equal(EDITOR_TYPE.content);
    expect(log.fieldName).not.ok;
    expect(log.value).not.ok;
  });

  it("createDelete", () => {
    let log = ContentLog.createDelete("1");
    expect(log.nameOrId).equal("1");
    expect(log.orgNameOrId).not.ok;
    expect(log.action).equal(ActionType.delete);
    expect(log.editorType).equal(EDITOR_TYPE.content);
    expect(log.fieldName).not.ok;
    expect(log.value).not.ok;
  });

  it("createUpdate", () => {
    let log = ContentLog.createUpdate("1", "a", "b");
    expect(log.nameOrId).equal("1");
    expect(log.orgNameOrId).not.ok;
    expect(log.action).equal(ActionType.update);
    expect(log.editorType).equal(EDITOR_TYPE.content);
    expect(log.fieldName).equal("a");
    expect(log.value).equal("b");
  });
});
