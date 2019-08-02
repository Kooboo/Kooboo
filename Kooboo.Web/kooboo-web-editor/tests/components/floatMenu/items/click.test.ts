import ClickItem from "@/components/floatMenu/items/click";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { Menu } from "@/components/floatMenu/menu";

describe("click", () => {
  beforeEach(() => {
    document.body.innerHTML = "";
  });

  test("ClickItem_update", () => {
    let elementObject = new ClickItem(new Menu());
    expect(elementObject.el.style.display).toEqual("");

    elementObject.update([]);
    expect(elementObject.el.style.display).toEqual("block");
  });
});
