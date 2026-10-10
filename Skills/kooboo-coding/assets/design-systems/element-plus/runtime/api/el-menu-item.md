# ElMenuItem

ElMenuItem: native Element Plus component.

Kind: component. Import `ElMenuItem` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/menu.html#menu-item

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| index | string | not declared | unique identification |
| route | string / object | not declared | Vue Router Route Location Parameters |
| disabled | boolean | false | whether disabled |

## Events

- `click`: callback function when menu-item is clicked, the param is menu-item instance

## Slots

- `default`: customize default content
- `title`: customize title content
