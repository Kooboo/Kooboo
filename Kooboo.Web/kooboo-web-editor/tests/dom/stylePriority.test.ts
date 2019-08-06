import { StyleSelector, getStylePriority, compareStylePriority, sortStylePriority } from "@/dom/stylePriority";

describe("stylePriority", () => {
  test("getPriority ", () => {
    let styleSelector: StyleSelector;
    styleSelector = { selector: "button.button > i.icon:before", important: true, inline: false, styleSequence: 88 };
    let priority = getStylePriority(styleSelector);
    expect(priority).toEqual([1, 0, 0, 2, 3, 88]);
  });
  test("compareStylePriority ", () => {
    let styleSelector1: StyleSelector;
    styleSelector1 = { selector: "button.button > i.icon:before", important: true, inline: false, styleSequence: 88 };
    let priority1 = getStylePriority(styleSelector1);
    let styleSelector2: StyleSelector;
    styleSelector2 = { selector: "li:first-line", important: false, inline: true, styleSequence: 88 };
    let priority2 = getStylePriority(styleSelector2);
    let compareResult = compareStylePriority(priority1, priority2);
    expect(compareResult).toBe(1);
  });
  test("stylePriority ", () => {
    let styleSelectorList: StyleSelector[];
    styleSelectorList = [
      { selector: "ul ol li.red" },
      { selector: "li" },
      { selector: "li:first-line" },
      { selector: "ul li" },
      { selector: "ul ol + li" },
      { selector: "h1 + *[rel=up]" },
      { selector: "*" },
      { selector: "li.red.level" }
    ];

    let sortStylePriorityResult = sortStylePriority(styleSelectorList);
    expect(sortStylePriorityResult).toEqual([
      { selector: "*" },
      { selector: "li" },
      { selector: "li:first-line" },
      { selector: "ul li" },
      { selector: "ul ol + li" },
      { selector: "h1 + *[rel=up]" },
      { selector: "ul ol li.red" },
      { selector: "li.red.level" }
    ]);
  });
  test("sortStylePriority_changeIndex ", () => {
    let styleSelectorList: StyleSelector[];
    styleSelectorList = [
      { selector: "h1 + *[rel=up]" },
      { selector: "ul ol li.red" },
      { selector: "li" },
      { selector: "li:first-line" },
      { selector: "ul li" },
      { selector: "ul ol + li" },
      { selector: "*" },
      { selector: "li.red.level" }
    ];

    let sortStylePriorityResult = sortStylePriority(styleSelectorList);
    expect(sortStylePriorityResult).toEqual([
      { selector: "*" },
      { selector: "li" },
      { selector: "li:first-line" },
      { selector: "ul li" },
      { selector: "ul ol + li" },
      { selector: "h1 + *[rel=up]" },
      { selector: "ul ol li.red" },
      { selector: "li.red.level" }
    ]);
  });
  test("sortStylePriority_inline ", () => {
    let styleSelectorList: StyleSelector[];
    styleSelectorList = [
      { selector: "h1 + *[rel=up]" },
      { selector: "ul ol li.red" },
      { selector: "li" },
      { selector: "li:first-line" },
      { selector: "ul li" },
      { selector: "ul ol + li" },
      { selector: "*", inline: true },
      { selector: "li.red.level" }
    ];

    let sortStylePriorityResult = sortStylePriority(styleSelectorList);
    expect(sortStylePriorityResult).toEqual([
      { selector: "li" },
      { selector: "li:first-line" },
      { selector: "ul li" },
      { selector: "ul ol + li" },
      { selector: "h1 + *[rel=up]" },
      { selector: "ul ol li.red" },
      { selector: "li.red.level" },
      { selector: "*", inline: true }
    ]);
  });
  test("sortStylePriority_inline_impartant ", () => {
    let styleSelectorList: StyleSelector[];
    styleSelectorList = [
      { selector: "h1 + *[rel=up]" },
      { selector: "ul ol li.red" },
      { selector: "li" },
      { selector: "li:first-line" },
      { selector: "ul li", inline: true },
      { selector: "ul ol + li" },
      { selector: "*", important: true, inline: true },
      { selector: "li.red.level" }
    ];

    let sortStylePriorityResult = sortStylePriority(styleSelectorList);
    expect(sortStylePriorityResult).toEqual([
      { selector: "li" },
      { selector: "li:first-line" },
      { selector: "ul ol + li" },
      { selector: "h1 + *[rel=up]" },
      { selector: "ul ol li.red" },
      { selector: "li.red.level" },
      { selector: "ul li", inline: true },
      { selector: "*", important: true, inline: true }
    ]);
  });
});
