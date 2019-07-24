import { createNotice } from "@/components/actionBar/Notice";

describe("notice", ()=>{
    beforeEach(()=>document.body.innerHTML = "")

    test("createNotice", ()=>{
        let notice = createNotice();
        expect(notice.style.visibility).toEqual("hidden");

        notice.setCount(1);
        expect(notice.style.visibility).toEqual("visible");

        notice.setCount(-1);
        expect(notice.style.visibility).toEqual("hidden");
    })
})