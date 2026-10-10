# YlTooltip

Import and register `{ YlTooltip }` from `/yuanli.js`. Install it with sync_design_runtime before importing.

Default scoped slot supplies aria-describedby: bind it to a focusable trigger. Hover/focus opens; Escape dismisses without moving focus. Tooltip remains open while its trigger is focused or either trigger/bubble is hovered (100ms gap allowance). Plain text only; use YlPopover for interactive panels. `placement: 'top'|'bottom'|'left'|'right' = 'top'` is preferred placement; flips when space is insufficient and stays within 8px of viewport edges. Native manual popover renders in the top layer; older browsers fall back to fixed positioning within ancestors. Resize/scroll repositions it. White text, maximum width 280px, long text wraps; content exceeding viewport height can scroll.

Props and events (Vue declaration; types/defaults are authoritative):
```js
{text:{type:String,required:true},placement:{type:String,default:'top'}}
```
