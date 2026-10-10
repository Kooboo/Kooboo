# Extend Ant Design Vue in a business App

Use native props, slots and composition first. Keep custom components in business Scripts such as `/component.customer-picker.js`, with styles in `/app.css`; never edit managed antdv.js/vendor.js/css/adapters.css or the receipt.

Use AConfigProvider `:theme="{token:{colorPrimary:'#1677ff',borderRadius:6},components:{Button:{controlHeight:36}}}"`. In descendant setup, `const { token } = theme.useToken()` exposes reactive values such as colorText, colorBgContainer, colorBorderSecondary, colorError. Bind them to your component styles; avoid scraping generated CSS-in-JS class hashes. Use `theme.darkAlgorithm` or `theme.compactAlgorithm` when explicitly required. Place dialogs inside provider context; for imperative services use AApp.useApp() from descendants of AApp.

```js
import { ref } from 'vue';
import { AModal, ASelect } from '/antdv.js';
export default {
  components: { AModal, ASelect },
  props: { open: Boolean, customers: { type: Array, default: () => [] }, busy: Boolean },
  emits: ['update:open', 'select'],
  setup(props, { emit }) {
    const selected = ref();
    function confirm() { if (!props.busy && selected.value != null) emit('select', selected.value); }
    return { selected, confirm, close: () => emit('update:open', false) };
  },
  template: `<AModal :open="open" title="选择客户" :confirm-loading="busy" :ok-button-props="{disabled:selected == null}" @ok="confirm" @cancel="close"><ASelect v-model:value="selected" :options="customers" :disabled="busy" show-search style="width:100%"/></AModal>`
};
```

The parent owns persistence and closes only after successful selection. Provide loading, empty, error/retry and disabled states; preserve user input after failure and guard double submissions. Respect native keyboard/focus behavior and test first-open overlays, Escape and return focus manually. Upload needs honest transport state and cleanup of previews/late responses. Custom business components stay in the App; reusable framework additions require a separate maintenance decision.
