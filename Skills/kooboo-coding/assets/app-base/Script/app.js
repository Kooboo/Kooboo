/* kooboo-app-base:v2; role: app-root */
import { provide } from 'vue'

export default {
  name: 'App',
  koobooSource: { moduleUrl: import.meta.url },
  props: { bootstrap: { type: Object, required: true } },
  setup(props) {
    provide('app.bootstrap', props.bootstrap)
  },
  // Layouts belong to parent routes. Do not key this outlet by the leaf URL:
  // sibling page navigation must preserve the mounted layout and its state.
  template: '<RouterView />'
}
