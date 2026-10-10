# ElTabPane

ElTabPane: native Element Plus component.

Kind: component. Import `ElTabPane` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/tabs.html#tab-pane

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| label | string | '' | title of the tab |
| disabled | boolean | false | whether Tab is disabled |
| name | string / number | not declared | identifier corresponding to the name of Tabs, representing the alias of the tab-pane, the default is ordinal number of the tab-pane in the sequence, e.g. the first tab-pane is '0' |
| closable | boolean | false | whether Tab is closable |
| lazy | boolean | false | whether Tab is lazily rendered |

## Slots

- `default`: Tab-pane's content
- `label`: Tab-pane's label
