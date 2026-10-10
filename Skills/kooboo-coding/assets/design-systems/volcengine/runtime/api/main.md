# YlMain
Import and register from /yuanli.js. Props: scroll: boolean = false; as: 'main' | 'section' | 'div' = 'main'. Default slot. Flexible content region with min-width/min-height zero.
scroll enables independent overflow and a focusable region (tabindex=0); its parent must have bounded height. Use one main landmark per document; nested panels use as="section" or as="div" and a suitable aria-label.
Exposed method: resetScroll() resets both axes on the root element. There is no implicit router dependency. YlShell invokes this on activePath changes. Ordinary flow pages can omit scroll.
