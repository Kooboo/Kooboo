# AProgress

Ant Design Vue 4.2.6; native export `Progress`. Import `AProgress` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `format`: function(percent, successPercent); default (percent) => percent + `%`. The template function of the content
- `percent`: number; default 0. To set the completion percentage
- `showInfo`: boolean; default true. Whether to display the progress value and the status icon
- `status`: string; default -. To set the status of the Progress, options: `success` `exception` `normal` `active`(line only)
- `strokeColor`: string; default -. The color of progress bar
- `strokeLinecap`: `round` | `butt` | `square`, see [stroke-linecap](https://developer.mozilla.org/docs/Web/SVG/Attribute/stroke-linecap); default `round`. To set the style of the progress linecap
- `success`: { percent: number, strokeColor: string }; default -. Configs of successfully progress bar
- `title`: string; default -. html dom title
- `trailColor`: string; default -. The color of unfilled part
- `type`: string; default `line`. To set the type, options: `line` `circle` `dashboard`
- `size`: number | [number, number] | "small" | "default"; default "default". Progress size

## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASpace><AProgress :percent="count" style="width:200px"/><AProgress type="circle" :percent="count" :width="90"/><AButton @click="count = (count + 10) % 110">增加进度</AButton></ASpace>
```

Source: pinned upstream es/components.js, es/progress, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
