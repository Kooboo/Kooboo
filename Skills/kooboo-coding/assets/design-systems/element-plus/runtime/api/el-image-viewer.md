# ElImageViewer

ElImageViewer: native Element Plus component.

Kind: component. Import `ElImageViewer` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/image.html#image-viewer

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| url-list | string[] | [] | preview link list. |
| z-index | number / string | not declared | preview backdrop z-index. |
| initial-index | number | 0 | the initial preview image index, less than or equal to the length of `url-list`. |
| infinite | boolean | true | whether preview is infinite. |
| hide-on-click-modal | boolean | false | whether user can emit close event when clicking backdrop. |
| teleported | boolean | false | whether to append image itself to body. A nested parent element attribute transform should have this attribute set to `true`. |
| zoom-rate | number | 1.2 | the zoom rate of the image viewer zoom event. |
| scale | number | 1 | the preview image scale. |
| min-scale | number | 0.2 | the min scale of the image viewer zoom event. |
| max-scale | number | 7 | the max scale of the image viewer zoom event. |
| close-on-press-escape | boolean | true | whether the image-viewer can be closed by pressing ESC. |
| show-progress | boolean | false | whether to display the preview image progress content |

## Events

- `close`: trigger when clicking on close button or when `hide-on-click-modal` enabled clicking on backdrop.
- `error`: same as native error.
- `switch`: trigger when switching images.
- `rotate`: trigger when rotating images.

## Slots

- `viewer`: custom content
- `progress`: custom progress content (Priority is higher than `show-progress` prop)
- `toolbar`: custom toolbar content
- `viewer-error`: custom image load failed content.
