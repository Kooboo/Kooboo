# YlButton

Import `{ YlButton } from '/yuanli.js'`; register in `components`.

Props: `icon?: string` (Lucide kebab-case, leading icon; suffixIcon supplies a trailing icon; loading replaces it with the spinner); `variant: 'primary'|'secondary'|'outline'|'text'|'danger'|'success'|'warning'|'dashed' = 'secondary'`; `size: 'lg'|'md'|'sm'|'mini' = 'md'`; `loading: boolean = false`; `disabled: boolean = false`; `type: 'button'|'submit'|'reset' = 'button'`.

Default slot: visible label/content. Event: `click(MouseEvent)`, suppressed during loading/disabled. Loading shows an aria-hidden spinner and sets aria-busy. Caller owns async work and errors; loading never indicates completed work. Native attributes such as `form`, `aria-label` and `title` pass through.

```html
<YlButton icon="save" variant="primary" :loading="saving" @click="save">保存</YlButton>
```

Use `type="submit"` with a form's `@submit.prevent`; loading prevents repeat submission through the button. Icon-only content needs an accessible name. Do not add alternate button CSS.
