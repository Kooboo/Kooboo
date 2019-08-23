import { ActionType } from "@/operation/ActionType";
import { EDITOR_TYPE, OBJECT_TYPE } from "@/common/constants";
import { StyleLog } from "@/operation/recordLogs/StyleLog";

describe("StyleLog", () => {
  it("createUpdate", () => {
    let log = StyleLog.createUpdate("1", OBJECT_TYPE.dom, "a", "color", "koobooId");
    expect(log.nameOrId).equal("1");
    expect(log.action).equal(ActionType.update);
    expect(log.editorType).equal(EDITOR_TYPE.style);
    expect(log.value).equal("a");
    expect(log.objectType).equal(OBJECT_TYPE.dom);
    expect(log.KoobooId).equal("koobooId");
    expect(log.styleSheetUrl).not.ok;
    expect(log.styleTagKoobooId).not.ok;
    expect(log.important).not.ok;
    expect(log.property).equal("color");
    expect(log.ruleId).not.ok;
    expect(log.selector).not.ok;
    expect(log.styleId).not.ok;
  });
});
