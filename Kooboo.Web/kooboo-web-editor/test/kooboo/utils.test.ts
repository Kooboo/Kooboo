import { clearKoobooInfo, getCloseElement, markDirty, setGuid, getGuidComment, isDynamicContent } from "@/kooboo/utils";
import { getAllElement } from "@/dom/utils";
import { KOOBOO_DIRTY, KOOBOO_GUID } from "@/common/constants";

describe("utils", () => {
  beforeEach(() => (document.body.innerHTML = ""));

  it("clearKoobooInfo", () => {
    let domString = `<div class="col-md-8 col-md-offset-2" kooboo-id="1-0-3-1-1-1-1" spellcheck="false" kooboo-dirty="" kooboo-guid="93ba5a2e-a8cb-41f4-9891-5493700b2199"><!--kooboo-guid aaf3d051-6693-45b9-92fc-b63ecd12db98-->
    <div class="call-to-action" kooboo-id="1-0-3-1-1-1-1-3" kooboo-dirty=""><a href="#" class="demo to-animate fadeInUp animated" kooboo-id="1-0-3-1-1-1-1-3-1" kooboo-dirty="">aaDemo<!--empty--></a><a href="#" class="demo to-animate fadeInUp animated" kooboo-id="1-0-3-1-1-1-1-3-2" kooboo-dirty="">Demo<!--empty--></a> <a href="#" class="download to-animate fadeInUp animated" kooboo-id="1-0-3-1-1-1-1-3-4" kooboo-dirty="">Download<!--empty--></a></div></div>`;

    let cleanDomString = clearKoobooInfo(domString);

    expect(cleanDomString.indexOf("kooboo-guid")).eq(-1);
    expect(cleanDomString.indexOf("kooboo-dirty")).eq(-1);
    expect(cleanDomString.indexOf("<!--kooboo-guid aaf3d051-6693-45b9-92fc-b63ecd12db98-->")).eq(-1);
  });

  it("getCloseElement have koobooId", () => {
    let temp = document.createElement("div");
    temp.innerHTML = `
    <div class="col-md-8 col-md-offset-2" kooboo-id="1-0-3-1-1-1-1">
			<div class="call-to-action" kooboo-id="1-0-3-1-1-1-1-3">
				<a href="#" class="demo to-animate fadeInUp animated" kooboo-id="1-0-3-1-1-1-1-3-1">Demo</a><a href="#" class="demo to-animate fadeInUp animated" kooboo-id="1-0-3-1-1-1-1-3-2">Demo</a>
				<a href="#" class="download to-animate fadeInUp animated" kooboo-id="1-0-3-1-1-1-1-3-4">Download</a>
			</div>
		</div>
    `;

    let link = temp.querySelector("a[kooboo-id='1-0-3-1-1-1-1-3-4']") as HTMLElement;
    let closeElement = getCloseElement(link);

    expect(closeElement).is.ok;
    expect(closeElement!.getAttribute("kooboo-id")).eq("1-0-3-1-1-1-1-3-4");
  });

  it("getCloseElement not koobooId", () => {
    let temp = document.createElement("div");
    temp.innerHTML = `
    <div class="col-md-8 col-md-offset-2" kooboo-id="1-0-3-1-1-1-1">
			<div class="call-to-action" kooboo-id="1-0-3-1-1-1-1-3">
				<a href="#" class="demo to-animate fadeInUp animated" kooboo-id="1-0-3-1-1-1-1-3-1">Demo</a><a href="#" class="demo to-animate fadeInUp animated" kooboo-id="1-0-3-1-1-1-1-3-2">Demo</a>
				<a href="#" class="download to-animate fadeInUp animated" >Download</a>
			</div>
		</div>
    `;

    let link = temp.querySelector("a[class~='download']") as HTMLElement;
    let closeElement = getCloseElement(link);

    expect(closeElement).is.ok;
    expect(closeElement!.getAttribute("kooboo-id")).eq("1-0-3-1-1-1-1-3");
  });

  it("markDirty", () => {
    let temp = document.createElement("div");
    temp.innerHTML = `
    <div class="col-md-8 col-md-offset-2" kooboo-id="1-0-3-1-1-1-1">
			<div class="call-to-action" kooboo-id="1-0-3-1-1-1-1-3">
				<a href="#" class="demo to-animate fadeInUp animated" kooboo-id="1-0-3-1-1-1-1-3-1">Demo</a><a href="#" class="demo to-animate fadeInUp animated" kooboo-id="1-0-3-1-1-1-1-3-2">Demo</a>
				<a href="#" class="download to-animate fadeInUp animated" kooboo-id="1-0-3-1-1-1-1-3-4">Download</a>
			</div>
		</div>
    `;

    let el = temp.querySelector("div[kooboo-id='1-0-3-1-1-1-1']") as HTMLElement;
    markDirty(el);
    for (const i of getAllElement(el)) {
      expect(i.hasAttribute(KOOBOO_DIRTY)).is.ok;
    }
  });

  it("setGuid replase", () => {
    let el = document.createElement("div");
    setGuid(el, "aa");
    expect(el.getAttribute(KOOBOO_GUID)).eq("aa");
  });

  it("getGuidComment", () => {
    let result = getGuidComment("aa");
    expect(result).eq(`<!--${KOOBOO_GUID} aa-->`);
  });

  it("isDynamicContent", () => {
    let temp = document.createElement("div");
    temp.innerHTML = `
    <div class="col-md-12 section-heading text-center" kooboo-id="1-0-1-1-1">
					
<!--#kooboo--objecttype='Label'--attributename='k-label'--bindingvalue='ExploreTitle'--koobooid='1-0-1-1-1-1'-->
<h2 class="to-animate fadeInUp animated" k-label="ExploreTitle" kooboo-id="1-0-1-1-1-1">Explore Our Products</h2>
					<div class="row" kooboo-id="1-0-1-1-1-3">
						<div class="col-md-8 col-md-offset-2 subtext to-animate fadeInUp animated" kooboo-id="1-0-1-1-1-3-1">
							
<!--#kooboo--objecttype='Label'--attributename='k-label'--bindingvalue='ExploreText'--koobooid='1-0-1-1-1-3-1-1'-->
<h3 k-label="ExploreText" kooboo-id="1-0-1-1-1-3-1-1">Far far away, behind the word mountains, far from the countries Vokalia and Consonantia, there live the blind texts.</h3>
						</div>
					</div>
				</div>
    `;
    expect(isDynamicContent(temp)).is.ok;
  });
});
