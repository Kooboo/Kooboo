# ElTabs

Divide data collections which are related yet belong to different types.

Kind: component. Import `ElTabs` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/tabs.html#tabs

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | string / number | not declared | binding value, name of the selected tab, the default value is the name of first tab |
| default-value | string / number | not declared | The value of the tab that should be active when initially rendered. (avoid initial transition) |
| type | '' \| 'card' \| 'border-card' | '' | type of Tab |
| closable | boolean | false | whether Tab is closable |
| addable | boolean | false | whether Tab is addable |
| editable | boolean | false | whether Tab is addable and closable |
| tab-position | 'top' \| 'right' \| 'bottom' \| 'left' | top | position of tabs |
| stretch | boolean | false | whether width of tab automatically fits its container |
| before-leave | (activeName: TabPaneName, oldActiveName: TabPaneName) => Awaitable<void \| boolean> | () => true | hook function before switching tab. If `false` is returned or a `Promise` is returned and then is rejected, switching will be prevented |
| tabindex | string / number | 0 | tabs tabindex |

## Events

- `tab-click`: triggers when a tab is clicked
- `tab-change`: triggers when `activeName` is changed
- `tab-remove`: triggers when tab-remove button is clicked
- `tab-add`: triggers when tab-add button is clicked
- `edit`: triggers when tab-add button or tab-remove is clicked

## Slots

- `default`: customize default content
- `add-icon`: customize add button icon
- `add-icon`: customize add button icon
