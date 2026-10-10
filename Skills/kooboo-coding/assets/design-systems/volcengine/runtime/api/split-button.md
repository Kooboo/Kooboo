# YlSplitButton

Import and register `{ YlSplitButton }` from `/yuanli.js` after installation.

Props: label required, items=[] (YlDropdown item contract); variant=primary (primary/secondary); size=md (lg/md/sm/mini); disabled=false; loading=false; moreLabel=更多操作. Main action emits click(event); dropdown emits select(item). Busy/disabled guard both actions. The second trigger is independently labeled and keyboard reachable.
