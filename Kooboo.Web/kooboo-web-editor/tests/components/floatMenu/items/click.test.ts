import { createClickItem } from "@/components/floatMenu/items/click";

describe("click", ()=>{
    beforeEach(()=>{ document.body.innerHTML = "" })

    test("createClickItem_update", ()=>{
        let elementObject = createClickItem();
        expect(elementObject.el.style.display).toEqual("");

        elementObject.update();
        expect(elementObject.el.style.display).toEqual("block");
    });
})