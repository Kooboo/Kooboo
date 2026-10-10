# ElImage

Besides the native features of img, support lazy load, custom placeholder and load failure, etc.

Kind: component. Import `ElImage` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/image.html#image

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| src | string | '' | image source, same as native. |
| fit | '' \| 'fill' \| 'contain' \| 'cover' \| 'none' \| 'scale-down' | '' | indicate how the image should be resized to fit its container, same as [object-fit](https://developer.mozilla.org/en-US/docs/Web/CSS/object-fit). |
| hide-on-click-modal | boolean | false | when enabling preview, use this flag to control whether clicking on backdrop can exit preview mode. |
| loading | 'eager' \| 'lazy' | not declared | Indicates how the browser should load the image, same as [native](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/img#attr-loading). |
| lazy | boolean | false | whether to use lazy load. |
| scroll-container | string / HTMLElement | not declared | the container to add scroll listener when using lazy load. By default, the container to add scroll listener when using lazy load. |
| alt | string | not declared | native attribute `alt`. |
| referrerpolicy | string | not declared | native attribute [referrerPolicy](https://developer.mozilla.org/en-US/docs/Web/API/HTMLImageElement/referrerPolicy). |
| crossorigin | '' \| 'anonymous' \| 'use-credentials' | not declared | native attribute [crossorigin](https://developer.mozilla.org/en-US/docs/Web/HTML/Attributes/crossorigin). |
| preview-src-list | string[] | [] | allow big image preview. |
| z-index | number | not declared | set image preview z-index. |
| initial-index | number | 0 | initial preview image index, less than the length of `url-list`. |
| close-on-press-escape | boolean | true | whether the image-viewer can be closed by pressing ESC. |
| preview-teleported | boolean | false | whether to append image-viewer to body. A nested parent element attribute transform should have this attribute set to `true`. |
| infinite | boolean | true | whether the viewer preview is infinite. |
| zoom-rate | number | 1.2 | the zoom rate of the image viewer zoom event. |
| scale | number | 1 | the preview image scale. |
| min-scale | number | 0.2 | the min scale of the image viewer zoom event. |
| max-scale | number | 7 | the max scale of the image viewer zoom event. |
| show-progress | boolean | false | whether to display the preview image progress content. |

## Events

- `load`: same as native load.
- `error`: same as native error.
- `switch`: trigger when switching images.
- `close`: trigger when clicking on close button or when `hide-on-click-modal` enabled clicking on backdrop.
- `show`: trigger when the viewer displays

## Slots

- `placeholder`: custom placeholder content when image hasn't loaded yet.
- `error`: custom image load failed content.
- `[image viewer slots]`: when you allow big image preview, image viewer slots all can be used.
