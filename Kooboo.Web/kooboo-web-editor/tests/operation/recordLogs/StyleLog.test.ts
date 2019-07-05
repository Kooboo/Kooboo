import { ActionType } from "@/operation/ActionType";
import { EDITOR_TYPE, OBJECT_TYPE } from "@/common/constants";
import { StyleLog } from "@/operation/recordLogs/StyleLog";

describe("StyleLog", () => {
  test("createUpdate", () => {
    let log = StyleLog.createUpdate("1", OBJECT_TYPE.dom, "a", "color", "koobooId");
    expect(log.nameOrId).toEqual("1");
    expect(log.action).toEqual(ActionType.update);
    expect(log.editorType).toEqual(EDITOR_TYPE.style);
    expect(log.value).toEqual("a");
    expect(log.objectType).toEqual(OBJECT_TYPE.dom);
    expect(log.KoobooId).toEqual("koobooId");
    expect(log.StyleSheetUrl).toBeFalsy();
    expect(log.StyleTagKoobooId).toBeFalsy();
    expect(log.important).toBeFalsy();
    expect(log.property).toEqual("color");
    expect(log.ruleId).toBeFalsy();
    expect(log.selector).toBeFalsy();
    expect(log.styleId).toBeFalsy();
  });
});
