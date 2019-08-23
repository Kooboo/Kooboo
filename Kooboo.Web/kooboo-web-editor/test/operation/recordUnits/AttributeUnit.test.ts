import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";

describe("AttributeUnit", () => {
  it("undo", () => {
    let node = document.createElement("img");
    let unit = new AttributeUnit("http://localhost/1.jpg", "src");
    unit.undo(node);
    expect(node.src).equal("http://localhost/1.jpg");
    expect(unit.newValue).not.ok;
  });

  it("redo", () => {
    let node = document.createElement("img");
    let unit = new AttributeUnit("http://localhost/1.jpg", "src");
    unit.newValue = "http://localhost/2.jpg";
    unit.redo(node);
    expect(unit.oldValue).not.ok;
    expect(node.src).equal("http://localhost/2.jpg");
  });
});
