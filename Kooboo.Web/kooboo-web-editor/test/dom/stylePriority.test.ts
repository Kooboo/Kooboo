import { getStylePriority, compareStylePriority, sortStylePriority } from "@/dom/stylePriority";
import { CssColor } from "@/dom/style";

describe("stylePriority", () => {
  it("getPriority ", () => {
    let styleSelector = { targetSelector: "button.button > i.icon:before", important: true, inline: false, styleSequence: 88 } as CssColor;
    let priority = getStylePriority(styleSelector);
    expect(priority).deep.eq([1, 0, 0, 2, 3, 0, 88]);
  });
  it("compareStylePriority ", () => {
    let styleSelector1 = { targetSelector: "button.button > i.icon:before", important: true, inline: false, styleSequence: 88 } as CssColor;
    let priority1 = getStylePriority(styleSelector1);
    let styleSelector2 = { targetSelector: "li:first-line", important: false, inline: true, styleSequence: 88 } as CssColor;
    let priority2 = getStylePriority(styleSelector2);
    let compareResult = compareStylePriority(priority1, priority2);
    expect(compareResult).eq(1);
  });
  it("stylePriority ", () => {
    let styleSelectorList = [
      { targetSelector: "ul ol li.red" },
      { targetSelector: "li" },
      { targetSelector: "li:first-line" },
      { targetSelector: "ul li" },
      { targetSelector: "ul ol + li" },
      { targetSelector: "h1 + *[rel=up]" },
      { targetSelector: "*" },
      { targetSelector: "li.red.level" }
    ] as CssColor[];

    let sortStylePriorityResult = sortStylePriority(styleSelectorList);
    expect(sortStylePriorityResult).deep.eq([
      { targetSelector: "*" },
      { targetSelector: "li" },
      { targetSelector: "li:first-line" },
      { targetSelector: "ul li" },
      { targetSelector: "ul ol + li" },
      { targetSelector: "h1 + *[rel=up]" },
      { targetSelector: "ul ol li.red" },
      { targetSelector: "li.red.level" }
    ]);
  });
  it("sortStylePriority_changeIndex ", () => {
    var styleSelectorList = [
      { targetSelector: "h1 + *[rel=up]" },
      { targetSelector: "ul ol li.red" },
      { targetSelector: "li" },
      { targetSelector: "li:first-line" },
      { targetSelector: "ul li" },
      { targetSelector: "ul ol + li" },
      { targetSelector: "*" },
      { targetSelector: "li.red.level" }
    ] as CssColor[];

    let sortStylePriorityResult = sortStylePriority(styleSelectorList);
    expect(sortStylePriorityResult).deep.eq([
      { targetSelector: "*" },
      { targetSelector: "li" },
      { targetSelector: "li:first-line" },
      { targetSelector: "ul li" },
      { targetSelector: "ul ol + li" },
      { targetSelector: "h1 + *[rel=up]" },
      { targetSelector: "ul ol li.red" },
      { targetSelector: "li.red.level" }
    ]);
  });
  it("sortStylePriority_inline ", () => {
    let styleSelectorList = [
      { targetSelector: "h1 + *[rel=up]" },
      { targetSelector: "ul ol li.red" },
      { targetSelector: "li" },
      { targetSelector: "li:first-line" },
      { targetSelector: "ul li" },
      { targetSelector: "ul ol + li" },
      { targetSelector: "*", inline: true },
      { targetSelector: "li.red.level" }
    ] as CssColor[];

    let sortStylePriorityResult = sortStylePriority(styleSelectorList);
    expect(sortStylePriorityResult).deep.eq([
      { targetSelector: "li" },
      { targetSelector: "li:first-line" },
      { targetSelector: "ul li" },
      { targetSelector: "ul ol + li" },
      { targetSelector: "h1 + *[rel=up]" },
      { targetSelector: "ul ol li.red" },
      { targetSelector: "li.red.level" },
      { targetSelector: "*", inline: true }
    ]);
  });
  it("sortStylePriority_inline_impartant ", () => {
    let styleSelectorList = [
      { targetSelector: "h1 + *[rel=up]" },
      { targetSelector: "ul ol li.red" },
      { targetSelector: "li" },
      { targetSelector: "li:first-line" },
      { targetSelector: "ul li", inline: true },
      { targetSelector: "ul ol + li" },
      { targetSelector: "*", important: true, inline: true },
      { targetSelector: "li.red.level" }
    ] as CssColor[];

    let sortStylePriorityResult = sortStylePriority(styleSelectorList);
    expect(sortStylePriorityResult).deep.eq([
      { targetSelector: "li" },
      { targetSelector: "li:first-line" },
      { targetSelector: "ul ol + li" },
      { targetSelector: "h1 + *[rel=up]" },
      { targetSelector: "ul ol li.red" },
      { targetSelector: "li.red.level" },
      { targetSelector: "ul li", inline: true },
      { targetSelector: "*", important: true, inline: true }
    ]);
  });
});
