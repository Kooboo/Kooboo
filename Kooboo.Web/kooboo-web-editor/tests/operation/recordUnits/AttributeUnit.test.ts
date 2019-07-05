import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";

describe("AttributeUnit", () => {
  test("undo", () => {
    let node = document.createElement("img");
    let unit = new AttributeUnit("http://localhost/1.jpg", "src");
    unit.undo(node);
    expect(node.src).toEqual("http://localhost/1.jpg");
    expect(unit.newValue).toBeFalsy();
  });

  test("redo", () => {
    let node = document.createElement("img");
    let unit = new AttributeUnit("http://localhost/1.jpg", "src");
    unit.newValue = "http://localhost/2.jpg";
    unit.redo(node);
    expect(unit.oldValue).toBeFalsy();
    expect(node.src).toEqual("http://localhost/2.jpg");
  });
});
