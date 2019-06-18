import { KoobooId } from "../../src/models/KoobooId";

describe("koobooId", () => {
  test("id to number", () => {
    let koobooid = new KoobooId("1-0-3-1-1-1");
    expect(koobooid.value).toBe(1);
  });

  test("id add 1", () => {
    let koobooid = new KoobooId("1-0-3-1-1-9");
    expect(koobooid.next).toBe("1-0-3-1-1-10");
  });
});
