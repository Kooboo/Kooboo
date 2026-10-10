# YlTopNav

Import and register `{ YlTopNav }` from `/yuanli.js` after installation.

Props: title required; theme=light (light/dark); links=[] and secondary=[] with {path,label,disabled?}; activePath optional; products=[] with {key,label,disabled?}; product optional primitive; secondaryTitle optional; searchable=false; searchValue=""; searchLabel=搜索. Emits navigate(path), update:product(key), update:searchValue(text), search(text) on Enter. brand, actions, secondaryActions and navigationControl slots are optional. The App supplies routing, authentication, notifications and any sidebar toggle; none is inserted by default. Product switching is a controlled selection; secondary links are route navigation. Header theme does not change the rest of the App theme.
