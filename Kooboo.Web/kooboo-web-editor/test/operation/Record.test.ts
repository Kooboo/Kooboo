import { operationRecord } from "@/operation/Record";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { Log } from "@/operation/Log";
import { KOOBOO_GUID } from "@/common/constants";

describe("operationRecord", () => {
  it("undo", () => {
    let div = document.createElement("div");
    div.setAttribute(KOOBOO_GUID, "guid1");
    div.innerHTML = "<p>abc</p>";
    document.body.innerHTML = "";
    document.body.append(div);

    let units = [];
    units.push(new AttributeUnit("col-md-6", "class"));
    units.push(new AttributeUnit("data", "data-app"));

    let record = new operationRecord(units, new Array<Log>(), "guid1");
    record.undo();

    expect(div.getAttribute("class")).equal("col-md-6");
    expect(div.getAttribute("data-app")).equal("data");
  });

  it("redo", () => {
    let div = document.createElement("div");
    div.setAttribute(KOOBOO_GUID, "guid1");
    div.innerHTML = "<p>abc</p>";
    document.body.innerHTML = "";
    document.body.append(div);

    let units = [];
    let unit1 = new AttributeUnit("", "class");
    unit1.newValue = "col-md-6";
    let unit2 = new AttributeUnit("", "data-app");
    unit2.newValue = "data";
    units.push(unit1);
    units.push(unit2);

    let record = new operationRecord(units, new Array<Log>(), "guid1");
    record.redo();

    expect(div.getAttribute("class")).equal("col-md-6");
    expect(div.getAttribute("data-app")).equal("data");
  });
});
