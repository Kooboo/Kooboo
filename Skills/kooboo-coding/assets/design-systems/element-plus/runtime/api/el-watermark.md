# ElWatermark

Add specific text or patterns to the page.

Kind: component. Import `ElWatermark` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/watermark.html#watermark

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| width | number | 120 | The width of the watermark, the default value of `content` is its own width |
| height | number | 64 | The height of the watermark, the default value of `content` is its own height |
| rotate | number | -22 | When the watermark is drawn, the rotation Angle, unit `°` |
| z-index | number | 9 | The z-index of the appended watermark element |
| image | string | not declared | Image source, it is recommended to export 2x or 3x image, high priority |
| content | string / string[] | Element Plus | Watermark text content |
| font | [Font] | [Font](#font) | Text style |
| gap | [number, number] | \[100, 100\] | The spacing between watermarks |
| offset | [number, number] | \[gap\[0\]/2, gap\[1\]/2\] | The offset of the watermark from the upper left corner of the container. The default is `gap/2` |

## Slots

- `default`: container for adding watermark
