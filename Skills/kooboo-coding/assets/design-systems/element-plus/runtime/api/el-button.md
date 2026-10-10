# ElButton

Commonly used button.

Kind: component. Import `ElButton` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/button.html#button

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| size | 'large' \| 'default' \| 'small' | not declared | button size |
| type | 'default' \| 'primary' \| 'success' \| 'warning' \| 'danger' \| 'info' \| '' \| 'text' | not declared | button type, when setting `color`, the latter prevails |
| plain | boolean | false | determine whether it's a plain button |
| text | boolean | false | determine whether it's a text button |
| bg | boolean | false | determine whether the text button background color is always on |
| link | boolean | false | determine whether it's a link button |
| round | boolean | false | determine whether it's a round button |
| circle | boolean | false | determine whether it's a circle button |
| dashed | boolean | false | determine whether it's a dashed button |
| loading | boolean | false | determine whether it's loading |
| loading-icon | string / Component | Loading | customize loading icon component |
| disabled | boolean | false | disable the button |
| icon | string / Component | not declared | icon component |
| autofocus | boolean | false | same as native button's `autofocus` |
| native-type | 'button' \| 'submit' \| 'reset' | button | same as native button's `type` |
| auto-insert-space | boolean | false | automatically insert a space between two chinese characters(this will only take effect when the text length is 2 and all characters are in Chinese.) |
| color | string | not declared | custom button color, automatically calculate `hover` and `active` color. Works with `link`/`text` buttons since |
| dark | boolean | false | dark mode, which automatically converts `color` to dark mode colors |
| tag | string / Component | button | custom element tag |

## Slots

- `default`: customize default content
- `loading`: customize loading component
- `icon`: customize icon component
