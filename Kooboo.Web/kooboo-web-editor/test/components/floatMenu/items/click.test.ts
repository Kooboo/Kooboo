import ClickItem from "@/components/floatMenu/items/click";
import { Menu } from "@/components/floatMenu/menu";

describe("click", () => {
  beforeEach(() => {
    document.body.innerHTML = "";
  });

  it("ClickItem_update", () => {
    let elementObject = new ClickItem(new Menu());
    expect(elementObject.el.style.display).equal("");

    elementObject.update();
    expect(elementObject.el.style.display).equal("block");
  });
});
