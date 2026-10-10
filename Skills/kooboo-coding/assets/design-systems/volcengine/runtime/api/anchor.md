# YlAnchor

Three-level section navigation. Props: items=[] with {id,label,children?}; modelValue optional active id; container optional scrolling Element or getter (defaults to window); offset=16 pixels; label=本页导航.

Emits update:modelValue(id) as scrolling changes active section and navigate(id) on ordinary clicks. In an App Shell pass its actual scrolling container. Ordinary clicks scroll without changing the App hash route; modified clicks retain native anchor behavior. Targets must exist in the same document and have unique ids. Bind modelValue if the parent owns active state. No slots.

```html
<YlAnchor :items="sections" :container="getScrollContainer" />
```
