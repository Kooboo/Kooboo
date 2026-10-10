# YlDialog

Import `{ YlDialog } from '/yuanli.js'`; register in `components`.

Props: `modelValue: boolean = false` (`v-model`); `title: string` required; `size: 'sm'|'md'|'lg' = 'md'`; `busy: boolean = false`; `closeLabel = '关闭'`.
Event: `update:modelValue(false)` requests close. Default slot: body. `footer` slot receives `{close}` to request close under the same busy guard. No automatic confirm/save button; caller owns validation and mutation.

```html
<YlDialog v-model="editing" title="编辑记录" :busy="saving">
  <label>名称 <YlInput v-model="draft.name" /></label>
  <p v-if="error" role="alert">{{ error }}</p>
  <template #footer="{close}">
    <YlButton :disabled="saving" @click="close">取消</YlButton>
    <YlButton variant="primary" :loading="saving" @click="save">保存</YlButton>
  </template>
</YlDialog>
```

Native `showModal()` provides modal focus containment and inert background. Escape/close button request close unless busy; close restores the connected opener. Backdrop clicks do not close. Busy does not prevent the parent from explicitly setting modelValue=false. Content stays mounted while closed, so reset draft/errors on opening a different record. Native HTML dialog support is required; browser focus behavior needs manual acceptance.

The header close control is a Lucide x icon button; closeLabel is its accessible name and tooltip, not visible text. Busy disables it.
