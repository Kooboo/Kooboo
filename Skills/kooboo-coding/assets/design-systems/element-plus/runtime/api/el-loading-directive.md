# ElLoadingDirective

Vue directive for v-loading; register locally as loading.

Kind: directive. Import `ElLoadingDirective` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

## Usage and state ownership

Use directives: { loading: ElLoadingDirective } on the consuming component, then v-loading="busy". The binding is a boolean owned by the business. Optional fullscreen/lock modifiers and element-loading-text attributes follow Element Plus. No full-library app.use is required.

```js
export default { directives: { loading: ElLoadingDirective }, setup() { return { busy: ref(false) }; }, template: '<section v-loading="busy">...</section>' };
```
