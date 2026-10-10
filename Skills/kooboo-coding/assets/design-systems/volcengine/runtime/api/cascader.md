# YlCascader

Inline multi-column panel following the source Cascader preview. Props: options=[] tree nodes {value:string|number,label,children?,disabled?,icon?,group?}; modelValue=[]; multiple=false; searchable=false; disabled=false; label=级联选择; searchPlaceholder=搜索选项; emptyText=暂无选项.

Single modelValue is a leaf path array; multiple modelValue is an array of leaf paths. Sibling values must be unique. Parent checkboxes select/deselect enabled leaves only; disabled ancestors disable descendants. Search matches full label paths. Groups are string headings. Arrow up/down and Home/End move within a column; Right opens children, Left returns to the parent. Emits update:modelValue(next) immutably. The caller may wrap this inline panel in YlPopover. No lazy loading or server search is implied. No slots.

```html
<YlCascader v-model="paths" :options="regions" multiple searchable />
```
