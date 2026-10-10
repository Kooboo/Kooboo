# YlLayout
Import and register from /yuanli.js.
Props: direction: 'horizontal' | 'vertical' = 'vertical'; heightMode: 'flow' | 'fill' | 'viewport' = 'flow'. Default slot accepts any content including nested YlLayout.
flow grows with content. viewport bounds the outer App to 100dvh. fill consumes remaining space in a bounded flex parent; it requires the parent to have a definite available height. Only the outer root normally uses viewport.
No implicit navigation, authentication, routing, padding or breakpoint. Use scroll on YlMain/YlAside to designate scroll owners. Nested layouts never need another viewport.
```html
<YlLayout height-mode="viewport">
  <YlHeader>App header</YlHeader>
  <YlLayout direction="horizontal" height-mode="fill">
    <YlAside :width="220">Directory or filters</YlAside>
    <YlMain scroll>Business content</YlMain>
    <YlAside :width="280">Properties</YlAside>
  </YlLayout>
  <YlFooter>Footer</YlFooter>
</YlLayout>
```
For ordinary content pages omit height-mode to use document scrolling. Regions are optional. Main-only, header/main/footer and sidebar/main are supported through composition.
For mobile, the consuming App must choose its aside behavior (hide, drawer or stack) in composition CSS; containers do not silently remove content. YlShell supplies this policy for its navigation preset.
