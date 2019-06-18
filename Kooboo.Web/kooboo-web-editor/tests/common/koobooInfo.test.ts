import { getKoobooInfo, cleanKoobooInfo } from "../../src/common/koobooInfo";
import { KOOBOO_ID } from "../../src/constants";

describe("domAnalyze", () => {
  test("getKoobooInfo", () => {
    let el = document.createElement("div");
    el.innerHTML = `
      <section id="fh5co-team" data-section="team" kooboo-id="1-0" class="animated">
    <div class="fh5co-team" kooboo-id="1-0-1">
      <div class="container" kooboo-id="1-0-1-1">
        <div class="row" kooboo-id="1-0-1-1-1">
          <div
            class="col-md-12 section-heading text-center"
            kooboo-id="1-0-1-1-1-1"
          >
            <!--#kooboo--objecttype='Label'--attributename='k-label'--bindingvalue='TeamTitle'--koobooid='1-0-1-1-1-1-1'-->
            <h2
              class="to-animate fadeIn animated"
              k-label="TeamTitle"
              kooboo-id="1-0-1-1-1-1-1"
            >
              Meet The Team
            </h2>
            <div class="row" kooboo-id="1-0-1-1-1-1-3">
              <div
                class="col-md-8 col-md-offset-2 subtext"
                kooboo-id="1-0-1-1-1-1-3-1"
              >
                <!--#kooboo--objecttype='Label'--attributename='k-label'--bindingvalue='TeamText'--koobooid='1-0-1-1-1-1-3-1-1'-->
                <h3
                  class="to-animate fadeIn animated test"
                  k-label="TeamText"
                  kooboo-id="1-0-1-1-1-1-3-1-1"
                >
                  Far far away, behind the word mountains, far from the countries
                  Vokalia and Consonantia, there live the blind texts. Separated
                  they live in Bookmarksgrove.
                </h3>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
      `;
    let testEl = el.getElementsByClassName("test").item(0);
    let kbInfo = getKoobooInfo(testEl as HTMLElement);
    expect(kbInfo.comments.length).toEqual(1);
    expect(kbInfo.comments[0].bindingvalue).toEqual("TeamText");
    expect(kbInfo.koobooId).toEqual("1-0-1-1-1-1-3-1-1");
  });

  test("cleanKoobooInfo", () => {
    let dom = `
              <div
                class="col-md-8 col-md-offset-2 subtext"
                kooboo-id="1-0-1-1-1-1-3-1"
              >
                <!--#kooboo--objecttype='Label'--attributename='k-label'--bindingvalue='TeamText'--koobooid='1-0-1-1-1-1-3-1-1'-->
                <h3
                  class="to-animate fadeIn animated test"
                  k-label="TeamText"
                  kooboo-id="1-0-1-1-1-1-3-1-1"
                >
                  Far far away, behind the word mountains, far from the countries
                  Vokalia and Consonantia, there live the blind texts. Separated
                  they live in Bookmarksgrove.
                </h3>
              </div>
      `;
    let result = cleanKoobooInfo(dom);

    expect(result.indexOf(KOOBOO_ID)).toEqual(-1);
    expect(result.indexOf("#kooboo")).toEqual(-1);
    expect(result.indexOf("<!--")).toEqual(-1);
  });
});
