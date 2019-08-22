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
@media screen and (max-width: 300px) {
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
  test("getStyles", () => {
    let styles = getStyles();
    let s1 = styles.next().value as CSSStyleSheet;
    expect((s1.cssRules[0] as CSSStyleRule).selectorText).toEqual("#test1");
    expect((s1.cssRules[0] as CSSStyleRule).style.getPropertyValue("background-color")).toEqual("white");
  });
  test("getRules", () => {
    let styles = getStyles();
    let s1 = styles.next().value as CSSStyleSheet;
    let rules = getRules(s1);
    let rule = rules.next().value.cssRule as CSSStyleRule;
    expect(rule.selectorText).toEqual("#test1");
  });
  test("getRules_with CSSMedia", () => {
    //jest不支持matchMedia，返回假数据
    window.matchMedia = jest.fn().mockImplementation(query => {
      return {
        matches: true,
        media: query,
        onchange: null,
        addListener: jest.fn(),
        removeListener: jest.fn(),
        addEventListener: jest.fn(),
        removeEventListener: jest.fn(),
        dispatchEvent: jest.fn()
      };
    });
    let styles = getStyles();
    styles.next();
    let s1 = styles.next().value as CSSStyleSheet;
    let rules = getRules(s1);
    let ruleResult = rules.next();
    let mediaRule = ruleResult.value.mediaRuleList;
    let rule = ruleResult.value.cssRule as CSSStyleRule;
    expect(mediaRule).toEqual("screen and (max-width: 300px)");
    expect(rule.selectorText).toEqual("#test4");
  });
  test("getCssRules", () => {
    //待定
  });
  test("getCssColors", () => {
    let el = document.getElementById("test4") as HTMLElement;
    let style = el.style as CSSStyleDeclaration;
    let colors = getCssColors(style);
    expect(colors[0].value).toEqual("red");
  });
  test("addInlineMatchedColors", () => {
    let el = document.getElementById("test4") as HTMLElement;
    let matchedColors: CssColor[] = [];
    addInlineMatchedColors(el, matchedColors);
    expect(matchedColors[0].prop.prop).toEqual("background-color");
    expect(matchedColors[0].value).toEqual("red");
    expect(matchedColors[0].important).toEqual(false);
  });
  //getCssRules不够完整，没法测
  test("getMatchedColors", () => {
    let el = document.getElementById("test4") as HTMLElement;
    let matchedColors = getMatchedColors(el);
    expect(matchedColors[0].inline).toEqual(true);
    expect(matchedColors[0].prop.prop).toEqual("background-color");
    expect(matchedColors[0].value).toEqual("red");
  });
  //getCssRules不够完整，没法测
  test("getMatchedColorGroups", () => {});
  //理解不了（有的时候color是 #fff #fff 多个颜色组成）
  test("isOneColor", () => {
    let el = document.getElementById("test4") as HTMLElement;
    let groups = getMatchedColorGroups(el);
  });
  test("splitPseudo", () => {
    let selector = "#test3:hover";
    let splitPseudoResult = splitPseudo(selector);
    expect(splitPseudoResult!.pseudo).toEqual(":hover");
    expect(splitPseudoResult!.selector).toEqual("#test3");
  });
  test("matchSelector", () => {
    let selectors = "#test3:hover,#test4";
    let el = document.getElementById("test3") as HTMLElement;
    let matchSelectorResult = matchSelector(el, selectors);
    expect(matchSelectorResult!.pseudo).toEqual(":hover");
    expect(matchSelectorResult!.selector).toEqual("#test3:hover");
  });
  //没写完
  test("addDefaultColor", () => {
    let groups: Array<CssColorGroup> = [];
    let prop = "color";
    let colorProp = colorProps.find(f => f.prop == prop)!;
    addDefaultColor(groups, prop, "red");
    expect(groups[0].cssColors[0]).toEqual({
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