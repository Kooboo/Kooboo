import { createNotice } from "@/components/actionBar/Notice";

describe("notice", () => {
  beforeEach(() => (document.body.innerHTML = ""));

  it("createNotice", () => {
    let notice = createNotice();

    expect(notice.style.visibility).equal("");

    notice.setCount(1);
    expect(notice.style.visibility).equal("visible");

    notice.setCount(-1);
    expect(notice.style.visibility).equal("hidden");
  });
});
