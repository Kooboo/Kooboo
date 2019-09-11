import * as Chai from "chai";

declare global {
  interface Window {
    expect: Chai.ExpectStatic;
  }
  var expect: Chai.ExpectStatic;
}

declare module "tinymce-declaration";
