# YlSteps

Props: items=[] with {key?,title,description?,status?,disabled?}; current=0 (zero-based); direction=horizontal (horizontal/vertical); label=步骤. Status: completed/active/pending/disabled/error. Explicit item status overrides position-derived status; disabled overrides status. Parent owns business progress.

Ordered status display, not an implicit form workflow. Current step uses aria-current=step; all statuses have screen-reader text. Optional item-{key or index} slots receive {item,index,status}. No events.

```html
<YlSteps :items="steps" :current="step" direction="vertical" />
```
