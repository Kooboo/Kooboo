import { KoobooId } from "@/kooboo/KoobooId";

describe("KoobooId", () => {
  test("get value", () => {
    let koobooId = new KoobooId("1-0-5-1-1-3-1-1-1");
    expect(koobooId.value).toEqual(1);
  });
  test("id value", () => {
    let koobooId = new KoobooId("1-0-5-1-1-3-1-1-1");
    expect(koobooId.id).toEqual("1-0-5-1-1-3-1-1-1");
  });
  test("next value", () => {
    let koobooId = new KoobooId("1-0-5-1-1-3-1-1-1");
    expect(koobooId.next).toEqual("1-0-5-1-1-3-1-1-2");
  });
});
