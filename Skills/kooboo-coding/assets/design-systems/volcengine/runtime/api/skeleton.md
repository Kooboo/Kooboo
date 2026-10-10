# YlSkeleton

Import `{ YlSkeleton } from '/yuanli.js'`. Props: loading=true, rows=4 (clamped to 1–20), title=true, animated=true, label="". No events. When loading=false, render the default slot; the consumer owns request/error state. Use an error message and retry action on failure, never an indefinite skeleton. Reduced-motion preference disables animation. Shapes are decorative; pass a localized status label only when the loading region needs one. This is a runtime extension built from Yuanli tokens, not an original source component.

```html
<YlSkeleton :loading="loading" :rows="5">
  <YlTable :rows="rows" :columns="columns" row-key="id" />
</YlSkeleton>
```

Do not simulate completion percentages or imply that planned/unbuilt screens are real data queries. PlannedScreen uses a preparation placeholder until replaced by an implemented route. Existing App layouts are not rewritten by a runtime upgrade.
