# ElAvatar

Avatars can be used to represent people or objects. It supports images, Icons, or characters.

Kind: component. Import `ElAvatar` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/avatar.html#avatar

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| icon | string / Component | not declared | representation type to icon, more info on icon component. |
| size | number / 'large' \| 'default' \| 'small' | not declared | avatar size. |
| shape | 'circle' \| 'square' | not declared | avatar shape. |
| src | string | not declared | the source of the image for an image avatar. |
| src-set | string | not declared | native attribute `srcset` of image avatar. |
| alt | string | not declared | native attribute `alt` of image avatar. |
| fit | 'fill' \| 'contain' \| 'cover' \| 'none' \| 'scale-down' | cover | set how the image fit its container for an image avatar. |

## Events

- `error`: trigger when image load error.

## Slots

- `default`: customize avatar content.
