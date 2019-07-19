import { KoobooComment } from "@/kooboo/KoobooComment";

describe("KoobooComment", () => {
  test("repeat comment", () => {
    let commentStr =
      "<!--#kooboo--objecttype='contentrepeater'--nameorid='70179bab-c26e-a3b0-2304-35964b303881'--folderid='2cc7e3ce-8262-e236-c726-32e88b9a1d03'--bindingvalue='ListSolution_Item'--boundary='7654'-->";
    let comment = new KoobooComment(commentStr);
    expect(comment.attributename).toBeFalsy();
    expect(comment.bindingvalue).toEqual("ListSolution_Item");
    expect(comment.boundary).toEqual("7654");
    expect(comment.end).toBeFalsy();
    expect(comment.fieldname).toBeFalsy();
    expect(comment.folderid).toEqual("2cc7e3ce-8262-e236-c726-32e88b9a1d03");
    expect(comment.koobooid).toBeFalsy();
    expect(comment.nameorid).toEqual("70179bab-c26e-a3b0-2304-35964b303881");
    expect(comment.objecttype).toEqual("contentrepeater");
  });

  test("content comment", () => {
    let commentStr =
      "<!--#kooboo--objecttype='content'--nameorid='70179bab-c26e-a3b0-2304-35964b303881'--bindingvalue='ListSolution_Item.SolutionName'--fieldname='SolutionName'--koobooid='1-0-5-1-1-3-1-1-1'-->";
    let comment = new KoobooComment(commentStr);
    expect(comment.attributename).toBeFalsy();
    expect(comment.bindingvalue).toEqual("ListSolution_Item.SolutionName");
    expect(comment.boundary).toBeFalsy();
    expect(comment.end).toBeFalsy();
    expect(comment.fieldname).toEqual("SolutionName");
    expect(comment.folderid).toBeFalsy();
    expect(comment.koobooid).toEqual("1-0-5-1-1-3-1-1-1");
    expect(comment.nameorid).toEqual("70179bab-c26e-a3b0-2304-35964b303881");
    expect(comment.objecttype).toEqual("content");
  });

  test("view comment", () => {
    let commentStr = "<!--#kooboo--objecttype='view'--nameorid='Testimony'--boundary='7644'-->";
    let comment = new KoobooComment(commentStr);
    expect(comment.attributename).toBeFalsy();
    expect(comment.bindingvalue).toBeFalsy();
    expect(comment.boundary).toEqual("7644");
    expect(comment.end).toBeFalsy();
    expect(comment.fieldname).toBeFalsy();
    expect(comment.folderid).toBeFalsy();
    expect(comment.koobooid).toBeFalsy();
    expect(comment.nameorid).toEqual("Testimony");
    expect(comment.objecttype).toEqual("view");
  });

  test("Label comment", () => {
    let commentStr = "<!--#kooboo--objecttype='Label'--attributename='k-label'--bindingvalue='ExploreText'--koobooid='1-0-1-1-1-3-1-1'-->";
    let comment = new KoobooComment(commentStr);
    expect(comment.attributename).toEqual("k-label");
    expect(comment.bindingvalue).toEqual("ExploreText");
    expect(comment.boundary).toBeFalsy();
    expect(comment.end).toBeFalsy();
    expect(comment.fieldname).toBeFalsy();
    expect(comment.folderid).toBeFalsy();
    expect(comment.koobooid).toEqual("1-0-1-1-1-3-1-1");
    expect(comment.nameorid).toBeFalsy();
    expect(comment.objecttype).toEqual("Label");
  });

  test("htmlblock comment", () => {
    let commentStr = "<!--#kooboo--objecttype='htmlblock'--nameorid='GetStart'--boundary='7645'-->";
    let comment = new KoobooComment(commentStr);
    expect(comment.attributename).toBeFalsy();
    expect(comment.bindingvalue).toBeFalsy();
    expect(comment.boundary).toEqual("7645");
    expect(comment.end).toBeFalsy();
    expect(comment.fieldname).toBeFalsy();
    expect(comment.folderid).toBeFalsy();
    expect(comment.koobooid).toBeFalsy();
    expect(comment.nameorid).toEqual("GetStart");
    expect(comment.objecttype).toEqual("htmlblock");
  });

  test("layout comment", () => {
    let commentStr = "<!--#kooboo--objecttype='layout'--nameorid='index'--boundary='7705'-->";
    let comment = new KoobooComment(commentStr);
    expect(comment.attributename).toBeFalsy();
    expect(comment.bindingvalue).toBeFalsy();
    expect(comment.boundary).toEqual("7705");
    expect(comment.end).toBeFalsy();
    expect(comment.fieldname).toBeFalsy();
    expect(comment.folderid).toBeFalsy();
    expect(comment.koobooid).toBeFalsy();
    expect(comment.nameorid).toEqual("index");
    expect(comment.objecttype).toEqual("layout");
  });

  test("page comment", () => {
    let commentStr = "<!--#kooboo--objecttype='page'--nameorid='151bd44e-02ed-4104-b898-e57113e0a45a'-->";
    let comment = new KoobooComment(commentStr);
    expect(comment.attributename).toBeFalsy();
    expect(comment.bindingvalue).toBeFalsy();
    expect(comment.boundary).toBeFalsy();
    expect(comment.end).toBeFalsy();
    expect(comment.fieldname).toBeFalsy();
    expect(comment.folderid).toBeFalsy();
    expect(comment.koobooid).toBeFalsy();
    expect(comment.nameorid).toEqual("151bd44e-02ed-4104-b898-e57113e0a45a");
    expect(comment.objecttype).toEqual("page");
  });

  test("end comment", () => {
    let commentStr = "<!--#kooboo--end='true'--objecttype='view'--boundary='7691'-->";
    let comment = new KoobooComment(commentStr);
    expect(comment.end).toBeTruthy();
  });

  test("getComments", () => {
    let temp = document.createElement("div");
    temp.innerHTML = `
    <!--#kooboo--objecttype='view'--nameorid='Testimony'--boundary='330'-->
    <section id="fh5co-testimony" data-section="testimony" kooboo-id="1-0">
      <!--#kooboo--objecttype='attribute'--nameorid='2'--attributename='src'--bindingvalue='{List_Item.Icon}'--koobooid='1-0-1-1-1-1-1-1-1'-->
      <div class="container" kooboo-id="1-0-1">
        <div class="owl-carousel-fullwidth" kooboo-id="1-0-1-1-1-1-1">
          <!--#kooboo--objecttype='contentrepeater'--nameorid='26571c99-dad5-cb9c-80a5-5858eb3aff04'--folderid='5e6ffc4d-a01b-d0c2-b416-f09f810d0094'--bindingvalue='ListTestimony_Item'--boundary='341'-->
          <p>ccc</p>
          <!--#kooboo--end=true--objecttype='contentrepeater'--boundary='341'-->
          <!--#kooboo--objecttype='contentrepeater'--nameorid='26571c99-dad5-cb9c-80a5-5858eb3aff04'--folderid='5e6ffc4d-a01b-d0c2-b416-f09f810d0094'--bindingvalue='ListTestimony_Item'--boundary='341'-->
          <div class="item" kooboo-id="1-0-1-1-1-1-1-1">
            <!--#kooboo--objecttype='attribute'--nameorid='fbdc6f3b-19ea-565e-84f6-a174b9cfb8f0'--attributename='src'--bindingvalue='{List_Item.Icon}'--koobooid='1-0-1-1-1-1-1-1-1'-->
            <p>aaa</p>
            <!--#kooboo--objecttype='attribute'--nameorid='2'--attributename='src'--bindingvalue='{List_Item.Icon}'--koobooid='1-0-1-1-1-1-1-1-1'-->
            <!--#kooboo--objecttype='attribute'--nameorid='1'--attributename='alt'--bindingvalue='{List_Item.Name}'--koobooid='1-0-1-1-1-1-1-1-1'-->
            <div
              class="testimony-slide active text-center"
              kooboo-id="1-0-1-1-1-1-1-1-1"
            ></div>
          </div>
          <!--#kooboo--end=true--objecttype='contentrepeater'--boundary='341'-->
        </div>
      </div>
    </section>
    <!--#kooboo--end='true'--objecttype='view'--boundary='330'-->
    `;

    let el = temp.querySelector("[kooboo-id='1-0-1-1-1-1-1-1-1']") as HTMLElement;
    let comments = KoobooComment.getComments(el);
    expect(comments.length).toBe(4);
    expect(comments[0].nameorid).toBe("1");
    expect(comments[1].nameorid).toBe("2");
    expect(comments[2].nameorid).toBe("26571c99-dad5-cb9c-80a5-5858eb3aff04");
    expect(comments[3].nameorid).toBe("Testimony");
  });
});
