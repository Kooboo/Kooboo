# YlDropdown

Import and register `{ YlDropdown }` from `/yuanli.js` after installation.

Props: modelValue=null (single primitive or primitive array when multiple); open=undefined for internal popup state, or bind v-model:open; items=[] with globally unique {value,label,description?,disabled?,icon?,group?,dividerBefore?,children?}; multiple=false; searchable=false; disabled=false; label=选择操作; width=220. update:modelValue reflects leaf selection. Single selection emits select(item) and update:open(false); multiple selection keeps the panel open and emits the selected values. Default trigger is a labeled button; trigger slot receives {open,toggle,triggerAttrs,disabled}. Bind its attributes and disabled state. Columns support Arrow/Home/End and Left/Right traversal through the shared Cascader. Group headings, separators and descriptions stay non-interactive. Caller owns commands/API calls.
