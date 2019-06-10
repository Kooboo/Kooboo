import { KoobooComment } from "../../src/models/KoobooComment";

test("init booboo info", () => {
  let comment = "#kooboo--objecttype='view'--nameorid='Home'--boundary='627'";
  let koobooComment = new KoobooComment(comment);
  expect(koobooComment.objecttype).toEqual("view");
  expect(koobooComment.nameorid).toEqual("Home");
  expect(koobooComment.boundary).toEqual("627");
});
