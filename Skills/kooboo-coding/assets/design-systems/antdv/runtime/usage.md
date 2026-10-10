# Ant Design Vue for Kooboo

Design-system ID `antdv`; adapter 0.2.0; official Ant Design Vue 4.2.6 (MIT); App Base Vue 3.5.13. This is the Vue implementation, not React Ant Design. Native APIs remain available; `AButton` etc. are aliases of upstream `Button` etc., not replacement components.

## Install and consume

`create_app_base` always installs the full fixed runtime. Optional `components` only selects API guides. Read `runtime/component-selection.json` to discover guide names; use returned guides directly and read only missing guides needed now. New component usage requires no add/replace synchronization.

```json
{"appName":"订单管理","designSystemId":"antdv","authenticationMode":"mock","shellTemplate":"antdv-workspace","components":["ATable","AForm","AFormItem","AInput","AButton","AModal"]}
```

Import named exports from `/antdv.js`, register components locally. Load `/antdv.css` (official reset), then `/antdv.adapters.css`, then business `/app.css`. `/antdv.vendor.js` is built by maintainers from the complete pinned upstream ESM with one shared Day.js and locales; `/antdv.js` adds aliases and the thin platform adapter. Both share App Base `vue` via its import map. No second Vue, on-site npm/SFC build, or separate Ant Design CDN request is needed. Component styles are injected by native CSS-in-JS; reset CSS alone does not contain component styles.

```js
import { ref } from 'vue';
import { AConfigProvider, AButton, AInput, antZhCN } from '/antdv.js';
export default {
  components: { AConfigProvider, AButton, AInput },
  setup() { const name = ref(''); return { name, antZhCN }; },
  template: `<AConfigProvider :locale="antZhCN"><AInput v-model:value="name" placeholder="姓名"/><AButton :disabled="!name.trim()">继续</AButton></AConfigProvider>`
};
```

## Native contracts

Business Screens/services own data, requests and mutations. Auxiliary exports require the documented native parent composition.

- Text/Select/Date/Number use `v-model:value`; Checkbox/Switch use `v-model:checked`; Modal/Drawer/Popover use `v-model:open`; Upload uses `v-model:file-list`. Sizes are `large`, `middle`, `small` where supported. Do not copy Yuanli or Element Plus prop names.
- Forms use model, FormItem name/rules, finish/finishFailed. Validate before submitting, guard duplicate writes, and own async loading/errors in Screens/services. Permissions remain business/backend responsibilities.
- Dates are Dayjs by default. Prefer explicit `value-format="YYYY-MM-DD"` or `HH:mm:ss` for API strings. `format` is display only. Choose local/UTC semantics explicitly. `antZhCN`/`antEnUS` configure component language; exported `dayjs.locale('zh-cn')`/`dayjs.locale('en')` configures helper date text. The maintained vendor shares one extended Day.js between components and the exported helper; Chinese and English date locales are included.
- `message`, `notification`, `AModal.confirm` are callable APIs. Static calls do not inherit provider theme/locale. For contextual services, wrap with `AApp` inside `AConfigProvider`, then call `AApp.useApp()` in a descendant component. Confirm cancellation is separate from request failure.
- Popover uses a default-slot trigger and `#content`; placement is `bottomRight`, etc. Let the native trigger handle click/outside click rather than toggling twice.
- Upload `beforeUpload: () => false` means local selection only. Implement actual transport with customRequest, cancellation, retry and late-response guards; do not report selection as upload success.
- Native internal icons are included upstream. `KbAntIcon` and `antIcons` provide native Ant icons locally. Use AButton #icon for spacing, loading and icon-only sizes; default size is 1em. Existing KbLucideIcon remains compatible. Read runtime/icons.md for exact names and examples. Icon-only buttons need accessible labels.

## Layout, themes and updates

Use native ALayout/ALayoutHeader/ALayoutSider/ALayoutContent, ARow/ACol/AFlex for business layouts. `shellTemplate="antdv-workspace"` installs KbAntShell with bootstrap, route-scoped navigation, account/logout, optional notifications and RouterView. Desktop collapse is optional controlled state; mobile navigation is a separate Drawer. Empty planned routes show navigation skeletons. The main content owns scrolling and 24px padding (16px up to 767px); do not duplicate that inset. Before composing or modifying a route layout, read [App layout](../../../../references/Frontend/cdn-vue/app-layout.md) for shared route and bootstrap integration.

Theme with AConfigProvider `theme.token`, `theme.components` and native dark/compact algorithms. KbAntShell reads native theme tokens; there are no `--el-*` or Yuanli token guarantees. Read `runtime/extensions.md` for custom compositions. Charts remain a separate integration with a theme adapter.

Use `sync_design_runtime` mode `upgrade`: preview first, apply the exact expectedRevision. Modified assets and business code are protected. Upgrading the runtime preserves existing layouts and does not upgrade a remote site merely because local files were synchronized.

All official exports are retained; the guide index covers public component groups plus the listed helpers. Generated docs originate in pinned upstream metadata; auxiliary variants may require parent docs. Automated runtime/installer checks, manual component acceptance and real AI Chat App acceptance are separate. Browser and real business App acceptance remain pending.
