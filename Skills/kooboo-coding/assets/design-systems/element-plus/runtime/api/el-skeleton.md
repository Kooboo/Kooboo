# ElSkeleton

When loading data, and you need a rich experience for visual and interactions for your end users, you can choose `skeleton`.

Kind: component. Import `ElSkeleton` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/skeleton.html#skeleton

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| animated | boolean | false | whether showing the animation |
| count | number | 1 | how many fake items to render to the DOM |
| loading | boolean | false | whether showing the real DOM |
| rows | number | 3 | numbers of the row, only useful when no template slot were given |
| throttle | number / { leading?: number, trailing?: number, initVal?: boolean } | 0 | rendering delay in milliseconds. Numbers represent delayed display, and can also be set to delay hide, for example `{ leading: 500, trailing: 500 }`. When needing to control the initial value of loading, you can set `{ initVal: true }` |

## Slots

- `default`: real rendering DOM
- `template`: content as rendering skeleton template
