# Element Plus runtime for Kooboo

Design-system ID: `element-plus`. Adapter: **0.3.0**. Upstream: **Element Plus 2.14.5** (MIT). Uses App Base **Vue 3.5.13**. This package keeps native `El*` APIs and theme-chalk; it is not a Yuanli theme or a Yl-compatible replacement.

## Install and read

The installer always provides the complete fixed official Element Plus runtime. `create_app_base.components` is optional and only requests API guides in usage-priority order; omitted or empty lists still install all components. Discover names in `assets/design-systems/element-plus/runtime/component-selection.json` when needed. Reuse returned guides and read only deferred APIs needed for the task. New component usage requires no add/replace operation.

```json
{"appName":"库存管理","designSystemId":"element-plus","authenticationMode":"mock","shellTemplate":"element-plus-workspace","components":["ElTable","ElTableColumn","ElInput","ElButton","ElDialog","ElForm","ElFormItem","ElPagination","ElMessage","ElMessageBox"]}
```

Import named exports from `/element-plus.js`, register components locally and load `/element-plus.css` followed by `/element-plus.adapters.css`, before business `/app.css`. The first stylesheet is unchanged upstream theme-chalk; the second contains only readable Kooboo Shell, navigation skeleton and icon styles. `/element-plus.vendor.js` is the complete official browser ESM distribution, copied unchanged from the pinned upstream package. The small `/element-plus.js` entry re-exports it and provides Kooboo adapters and Chinese/English locales. Both use the same App Base Vue via its `vue` import map. No separate CDN access for Element Plus, second Vue or on-site npm build is required. Maintenance sources, dependency packages, API docs and examples are not installed into business Apps.

```js
import { ref } from 'vue';
import { ElInput, ElButton, ElMessage } from '/element-plus.js';
export default {
  components: { ElInput, ElButton },
  setup() {
    const name = ref('');
    const busy = ref(false);
    async function save() {
      if (busy.value || !name.value.trim()) return;
      busy.value = true;
      try { await saveName(name.value.trim()); ElMessage.success('保存成功'); }
      catch (error) { ElMessage.error(error.message || '保存失败'); }
      finally { busy.value = false; }
    }
    return { name, busy, save };
  },
  template: `<ElInput v-model="name" aria-label="名称" /><ElButton :loading="busy" @click="save">保存</ElButton>`
};
// saveName is a business service imported by the consuming module.
```

## API distinctions

- Components use native Element Plus props, slots and events. Bind `model-value` through `v-model` when exposed. Sizes are `large`, `default`, `small`; input textarea is `ElInput type="textarea"`. Do not translate Yuanli prop names.
- `ElMessage`, `ElMessageBox`, `ElNotification`, `ElLoadingService` are callable services, not template components. Handle confirmation cancellation separately from a failed request. Close loading services in finally.
- `ElLoadingDirective` is a directive: register `directives: { loading: ElLoadingDirective }`, then use `v-loading="busy"`. No full plugin registration is required.
- `epZhCn` and `epEn` are locale objects. Use one with `ElConfigProvider`, expose it from setup, and pass `:locale="epZhCn"`. The optional Shell supplies Chinese control text by default; adapt it to the App's language when appropriate.
- Models, validation rules, request/busy states, permissions and business mutations belong to Screens/services. A provider or component never supplies business authorization.
- Date/time: explicitly choose `value-format` and local/UTC semantics. Without value-format date pickers return Date values. Display `format` does not determine serialization.
- Upload: provide the real transport or a clearly labelled mock through `http-request`; selected files are not uploaded files. See the upload API for native request callbacks and cancellation.
- Upstream internal icons are bundled unchanged. For business navigation/actions, `KbLucideIcon` uses the existing App Base Lucide loader and Vue rendering. Give icon-only controls accessible labels. Its CDN availability follows the App Base environment.

## Layout and extensions

Use `ElContainer`, `ElHeader`, `ElAside`, `ElMain`, `ElFooter`, `ElRow`, `ElCol` to compose layouts. Allocate height and scrolling explicitly in business `/app.css`; not every App needs a viewport sidebar shell. `ElMenu`, `ElSubMenu` and menu items support native nested navigation independently of layout containers.

`shellTemplate="element-plus-workspace"` installs a Kooboo adapter using `KbElementShell`, bootstrap, account/logout, notifications and RouterView. It provides an independent main scroller and mobile navigation. Desktop collapse is optional controlled behavior; use the actions slot/methods to add a trigger where the business needs it. Preserve the generated adapter's infrastructure when changing composition. Omit shellTemplate for a custom layout. Existing layouts are preserved during runtime updates.

The Shell's `.kb-ep-shell-main` owns outer RouterView padding: 24px on desktop and 16px at widths up to 767px. Business Screens must not duplicate that outer inset. For Shell styling questions, read the small `assets/design-systems/element-plus/runtime/element-plus.adapters.css` (installed as `/element-plus.adapters.css`); the minified upstream stylesheet is not the Shell styling source.

For a business component, read `assets/design-systems/element-plus/runtime/extensions.md`. Use native slots, public CSS variables and component composition. Keep custom code in business Scripts and `/app.css`; do not edit `/element-plus.js`, `/element-plus.css`, `/element-plus.adapters.css` or the installation receipt.

Use `sync_design_runtime` with `mode="upgrade"` to preview a fixed-distribution version update and apply its exact `expectedRevision`. Local edits, route ownership and business resources are protected. Component add/replace is unnecessary. Experimental split/assembled installations must be recreated for this distribution; no automatic chunk migration or deletion is provided. Updates preserve business code and the existing Shell composition; they do not prove visual acceptance.

## Coverage

API contracts are based on the bundled upstream implementation and web-types metadata with explicit adapter corrections. Automated smoke evidence is separate; native focus, geometry and visual variants require manual acceptance.

The component-selection index lists the 119 documented exports, including Kooboo adapters and locales; it is a guide index, not an installation allowlist. The official distribution retains its complete upstream exports. Prefer documented native component APIs and local registration; inspect upstream documentation for additional public capabilities.

`runtime/examples.js` provides executable form/table/date/layout compositions for maintenance or acceptance. Bundled upstream behavior and automated DOM tests do not constitute manual layout, focus, touch, resize, overlay or real AI Chat business acceptance. Theme defaults are light; `runtime/dark.css` is an optional upstream dark-token reference, not a default installed resource. Chart engines remain a separate integration; adapt chart themes with `--el-*` tokens instead of duplicating ECharts under this package.

Before composing or modifying a route layout, read [App layout](../../../../references/Frontend/cdn-vue/app-layout.md). It owns the shared route, bootstrap and access-control integration contract.
