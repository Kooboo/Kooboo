import {
  addDefaultColor,
  addInlineMatchedColors,
  CssColor,
  CssColorGroup,
  getCssColors,
  getCssRules,
  getMatchedColorGroups,
  getMatchedColors,
  getRules,
  getStyles,
  isOneColor,
  matchSelector,
  splitPseudo
} from "@/dom/style";
import { colorProps } from "@/dom/colorProps";

describe("style", () => {
  beforeEach(
    () =>
      (document.body.innerHTML = `<style>
    #test1{
        background-color: white;
    }
    @media screen and (max-width: 300px) {
        #test2{
        background-color: black;
        }
    }
    @media screen and (max-width: 600px) {
        #test2{
        background-color: white;
        }
    }
    #test3：hover{
        background-color: red;
    }
</style>
<style>
@media screen and (max-width: 1200px) {
    #test4{
        background-color: green;
    }
}
</style>
<div id="test1">
    <div id="test2">
        <div id="test4" style="background-color: red">
    </div>
    <div id="test3">
    </div>
</div>`)
  );
  it("getStyles", () => {
    let styles = getStyles();
    let s1 = styles[0] as CSSStyleSheet;
    expect((s1.cssRules[0] as CSSStyleRule).selectorText).equal("#test1");
    expect((s1.cssRules[0] as CSSStyleRule).style.getPropertyValue("background-color")).equal("white");
  });
  it("getRules", () => {
    let styles = getStyles();
    let s1 = styles[0] as CSSStyleSheet;
    let rules = getRules(s1);
    let rule = rules[0].cssRule as CSSStyleRule;
    expect(rule.selectorText).equal("#test1");
  });
  it("getRules_with CSSMedia", () => {
    let styles = getStyles();
    let s1 = styles[1] as CSSStyleSheet;
    let rules = getRules(s1);
    let selectorText = rules[0].cssRule.selectorText;
    let mediaRule = rules[0].mediaRuleList;
    expect(selectorText).equal("#test4");
    expect(mediaRule).equal("screen and (max-width: 1200px)");
  });
  it("getCssRules", () => {
    //待定
  });
  it("getCssColors", () => {
    let el = document.getElementById("test4") as HTMLElement;
    let style = el.style as CSSStyleDeclaration;
    let colors = getCssColors(style);
    expect(colors[0].value).equal("red");
  });
  it("addInlineMatchedColors", () => {
    let el = document.getElementById("test4") as HTMLElement;
    let matchedColors: CssColor[] = [];
    addInlineMatchedColors(el, matchedColors);
    expect(matchedColors[0].prop.prop).equal("background-color");
    expect(matchedColors[0].value).equal("red");
    expect(matchedColors[0].important).equal(false);
  });
  //getCssRules不够完整，没法测
  it("getMatchedColors", () => {
    let el = document.getElementById("test4") as HTMLElement;
    let matchedColors = getMatchedColors(el);
    expect(matchedColors[0].inline).equal(true);
    expect(matchedColors[0].prop.prop).equal("background-color");
    expect(matchedColors[0].value).equal("red");
  });
  //getCssRules不够完整，没法测
  it("getMatchedColorGroups", () => {});
  it("isOneColor_not oneColor", () => {
    let color = "red blue";
    let isOneColorResult = isOneColor(color);
    expect(isOneColorResult).equal(false);
  });
  it("isOneColor_oneColor_color", () => {
    let color = "red";
    let isOneColorResult = isOneColor(color);
    expect(isOneColorResult).equal(true);
  });
  it("isOneColor_oneColor_colorEnum", () => {
    let color = "aquamarine";
    let isOneColorResult = isOneColor(color);
    expect(isOneColorResult).equal(true);
  });
  it("splitPseudo", () => {
    let selector = "#test3:hover";
    let splitPseudoResult = splitPseudo(selector);
    expect(splitPseudoResult!.pseudo).equal(":hover");
    expect(splitPseudoResult!.selector).equal("#test3");
  });
  it("matchSelector", () => {
    let selectors = "#test3:hover,#test4";
    let el = document.getElementById("test3") as HTMLElement;
    let matchSelectorResult = matchSelector(el, selectors);
    expect(matchSelectorResult![0].pseudo).equal(":hover");
    expect(matchSelectorResult![0].selector).equal("#test3:hover");
  });
  it("addDefaultColor", () => {
    let groups: Array<CssColorGroup> = [];
    let prop = "color";
    let colorProp = colorProps.find(f => f.prop == prop)!;
    addDefaultColor(groups, prop, "red");
    expect(groups[0].cssColors[0]).deep.eq({
      inline: true,
      prop: colorProp,
      cssStyleRule: null,
      important: false,
      koobooId: "",
      pseudo: "",
      rawSelector: "",
      styleSequence: 0,
      targetSelector: "",
      url: "",
      value: "red",
      nameorid: undefined,
      objecttype: undefined,
      mediaRuleList: null
    });
  });
});
