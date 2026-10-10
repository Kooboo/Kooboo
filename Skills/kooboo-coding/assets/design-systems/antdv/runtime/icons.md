# Ant Design Vue icons

Use native Ant icons with the component library. The pinned `@ant-design/icons-vue` namespace is bundled locally as `antIcons`; it needs no extra network request. `KbAntIcon` accepts exact public names (`UserOutlined`, `ReloadOutlined`, `CheckCircleFilled`, `HeartTwoTone`) or their kebab forms (`user-outlined`). Shell semantic names such as `user-round`, `bell`, `layout-grid`, `refresh-cw` also map to native icons. Unknown names display QuestionCircleOutlined and set data-icon-missing instead of silently disappearing.

```js
import { AButton, KbAntIcon } from '/antdv.js';
export default {
  components: { AButton, KbAntIcon },
  template: `<AButton><template #icon><KbAntIcon name="ReloadOutlined"/></template>刷新数据</AButton>`
};
```

For an icon-only action, omit button text and supply aria-label. Use the `#icon` slot so native loading and icon-only sizing work correctly. Default icon size is 1em, inheriting button size; don't force 18/24px inside every small control. For a standalone illustration pass size explicitly. Two-tone icons accept twoToneColor; application semantic colors belong to theme tokens. `antIcons.UserOutlined` can also be registered directly without a wrapper.

`KbLucideIcon` remains available for existing business code and now uses the same `.anticon` container required by Ant Button's native icon/text spacing. It still uses the App Base Lucide loader. New Ant workspace presets use KbAntIcon for native names and supported semantic aliases; other existing Lucide navigation names keep their Lucide rendering rather than being discarded.

AAlert's default icon belongs to the native component: a short message uses a filled status icon; a description uses a larger outlined status icon. Do not globally resize `.anticon` or `.ant-alert-icon` to match a toolbar icon. Use the native icon slot only for an explicit design requirement.
