# ElAvatarGroup

ElAvatarGroup: native Element Plus component.

Kind: component. Import `ElAvatarGroup` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/avatar.html#avatargroup

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| size | number / 'large' \| 'default' \| 'small' | not declared | control the size of avatars in this avatar-group |
| shape | 'circle' \| 'square' | not declared | control the shape of avatars in this avatar-group |
| collapse-avatars | boolean | false | whether to collapse avatars |
| collapse-avatars-tooltip | boolean | false | whether show all collapsed avatars when mouse hover text of the collapse-avatar. To use this, `collapse-avatars` must be true |
| max-collapse-avatars | number | 1 | the max avatars number to be shown. To use this, `collapse-avatars` must be true |
| effect | 'dark' \| 'light' / string | light | tooltip theme, built-in theme: `dark` / `light` |
| placement | 'top' \| 'top-start' \| 'top-end' \| 'bottom' \| 'bottom-start' \| 'bottom-end' \| 'left' \| 'left-start' \| 'left-end' \| 'right' \| 'right-start' \| 'right-end' | top | placement of tooltip |
| popper-class | string | '' | custom class name for tooltip |
| popper-style | string / object | not declared | custom style for tooltip |
| collapse-class | string | '' | custom class name for the collapse-avatar |
| collapse-style | string / object | not declared | custom style for the collapse-avatar |
