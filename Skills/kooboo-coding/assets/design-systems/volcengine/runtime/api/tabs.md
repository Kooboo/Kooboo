# YlTabs

Import and register `{ YlTabs }` from `/yuanli.js` after installation.

Props: modelValue required string, items=[] with {key,label,disabled?,icon?,count?,badge?}; label=选项卡; variant=line (line/card/capsule/button/segmented/vertical); size=md (lg/md/sm); scrollable=false; addable=false; addLabel=新增选项卡. update:modelValue(key) is controlled; provide a valid enabled key and update it when items change. Arrow/Home/End activate/focus enabled tabs (Up/Down for vertical). Named item-key slots render corresponding panels; inactive panel content unmounts. extra slot supplies contextual actions. add() asks the caller to create a new item; scrollable enables horizontal scroll buttons.

`bordered=false` adds the source bordered content-panel treatment when enabled.
