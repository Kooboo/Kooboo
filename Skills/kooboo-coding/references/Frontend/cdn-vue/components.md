# Plain JavaScript Components

## Standard component format

Components directly export plain JavaScript objects and do not use `defineComponent()`:

The examples below assume no UI framework. If the project has already selected a UI framework, keep the component contract, state, and event boundaries unchanged, but use the framework's verified official usage for Button, Input, Dialog, and other controls.

```javascript
export default {
  name: 'TaskCard',
  koobooSource: { moduleUrl: import.meta.url },
  components: {},
  props: {
    task: {
      type: Object,
      required: true
    }
  },
  emits: {
    edit: id => (typeof id === 'string' && id.length > 0) || Number.isFinite(id),
    delete: id => (typeof id === 'string' && id.length > 0) || Number.isFinite(id)
  },
  setup(props, { emit }) {
    function requestEdit() {
      emit('edit', props.task.id)
    }

    function requestDelete() {
      emit('delete', props.task.id)
    }

    return {
      requestEdit,
      requestDelete
    }
  },
  template: `
    <article>
      <div>
        <div>
          <h3>{{ task.title }}</h3>
          <p>
            {{ task.description }}
          </p>
        </div>
        <span>{{ task.priority }}</span>
      </div>

      <div>
        <button
          type="button"
          :aria-label="'编辑任务：' + task.title"
          @click="requestEdit"
        >
          编辑
        </button>
        <button
          type="button"
          :aria-label="'删除任务：' + task.title"
          @click="requestDelete"
        >
          删除
        </button>
      </div>
    </article>
  `
}
```

## Component contract

Every component explicitly declares:

- `name`
- `koobooSource: { moduleUrl: import.meta.url }` for App-owned root, Layout, Screen, and business components, including named exports
- `components`
- `props`
- `emits`
- `setup()`
- Static `template`

Even when there are no child components, props, or events, write `components: {}`, `props: {}`, and `emits: []` so the component contract is immediately visible.

Do not omit `name`. The global `errorHandler`, Vue Devtools, and runtime warnings use it to identify the specific component.

### Preview source metadata

New App components declare `koobooSource` on the exported component object. `import.meta.url` identifies the actual module; do not handwrite a path or infer it from `name`. Each named component in a shared Script declares its own metadata and name. Preserve the scaffold's `/preview-source.js` installation before mount in `/main.js`; the plugin activates only in the editor preview iframe. The inspector records the current route separately from the render component chain and VNode declaration context, so shared layouts and teleported/slot content retain their own provenance. Metadata identifies source modules, not an instruction to edit the innermost component. Vue Sites may adopt the same plugin and metadata; server-rendered Site selection continues to use Page/View/Layout boundaries.

The object list for a complete application should record each component's responsibility, direct dependencies, hosted screen, and Script object. Imports, registration names, props, and emits in code must match the list and its consumers.

## Screen components

A screen component owns an identifiable user task and is not a Kooboo Page. It should:

- Receive the inputs required by the current screen or read explicit store state;
- Initiate domain actions through emits or store actions;
- Provide loading, empty, error, and normal states;
- Not modify the global navigation URL itself;
- Use server-returned state to control presentation, but never treat template conditions as real authorization;
- Treat the server response as the final result for permission-related actions.

The root `App` handles screen selection and shell assembly; screen components handle business areas; leaf components handle limited display or input. Do not make the root component carry every form, list, and rule.

## One module may export multiple simple components

Closely related display components without complex state may share one Script object and be exported as named exports:

```javascript
export const AppSidebar = {
  name: 'AppSidebar',
  koobooSource: { moduleUrl: import.meta.url },
  components: {},
  props: {
    title: {
      type: String,
      default: '应用'
    }
  },
  emits: ['toggle-theme'],
  setup(props, { emit }) {
    return {
      toggleTheme: () => emit('toggle-theme')
    }
  },
  template: `
    <aside>
      <div>
        <strong>{{ title }}</strong>
        <button type="button" @click="toggleTheme">
          切换主题
        </button>
      </div>
    </aside>
  `
}

export const TaskStats = {
  name: 'TaskStats',
  koobooSource: { moduleUrl: import.meta.url },
  components: {},
  props: {
    total: { type: Number, default: 0 },
    doing: { type: Number, default: 0 },
    done: { type: Number, default: 0 }
  },
  emits: [],
  setup() {
    return {}
  },
  template: `
    <section aria-label="任务统计">
      <article>总任务 {{ total }}</article>
      <article>进行中 {{ doing }}</article>
      <article>已完成 {{ done }}</article>
    </section>
  `
}
```

The parent component imports them with a root-absolute URL:

```javascript
import { AppSidebar, TaskStats } from '/component.dashboard.js'
```

## Complex components must be independent

The component should have its own Script object when any of the following applies:

- It has independent lifecycle hooks such as `onMounted`, `onUnmounted`, or `watch`;
- It contains form validation, dialogs, focus management, or keyboard interaction;
- It makes asynchronous requests or orchestrates multiple store actions;
- It has substantial local reactive state;
- Its template is long and errors are difficult to locate quickly;
- It is reused by multiple domain modules;
- Another complex component already exists in the same module.

For example, `TaskFormModal` should use an independent object:

```text
Name: component.task-form-modal.js
URL:  /component.task-form-modal.js
```

Do not put every component in a domain into one large file merely to “aggregate by domain.” Any syntax error in the module prevents all exported components in that module from loading.

## Code size

- Keep an individual Script around 100–250 lines when possible.
- When it exceeds about 300 lines, recheck whether multiple responsibilities have been mixed in.
- A module should carry only a small number of closely related simple components.
- Components with lifecycle, forms, or asynchronous state take priority for extraction regardless of line count.
- Do not split a static fragment that is only a few lines and has no independent responsibility.

Line count is only a diagnostic signal; responsibility boundaries take priority over mechanical limits.

## Store and emits boundary

- Use emits for UI events that affect only parent composition, such as `close`, `select`, and `navigate`;
- Use explicit store actions to modify shared domain data, such as `createRequest` and `approveRequest`;
- Leaf display components should not import the entire store directly; prefer props/emits for reuse;
- A screen component may call store actions for its scenario, but must not arbitrarily modify shared arrays;
- Do not duplicate the same business rule across components; keep it in an action or pure function.

## Child-component registration

Components are not registered automatically. The parent must explicitly import them and place them in `components`:

```javascript
import TaskCard from '/component.task-card.js'

export default {
  name: 'TaskList',
  koobooSource: { moduleUrl: import.meta.url },
  components: { TaskCard },
  props: {
    tasks: {
      type: Array,
      default: () => []
    }
  },
  emits: ['edit', 'delete'],
  setup(props, { emit }) {
    return {
      editTask: id => emit('edit', id),
      deleteTask: id => emit('delete', id)
    }
  },
  template: `
    <div v-if="tasks.length">
      <TaskCard
        v-for="task in tasks"
        :key="task.id"
        :task="task"
        @edit="editTask"
        @delete="deleteTask"
      ></TaskCard>
    </div>
    <div v-else>
      <p>暂无匹配数据</p>
    </div>
  `
}
```

## props

- Props are read-only inputs; do not modify `props.task.title` directly.
- Object and Array defaults must use factory functions.
- Mark required fields with `required: true`.
- Use a validator when there is a defined enumeration.
- An edit form should copy its input object and emit new data or call a store action on submit rather than modifying the parent object bidirectionally.
- A form-draft copy must preserve immutable fields and decide whether to reset, merge, or report a conflict when props change; it must not silently overwrite unsubmitted user input.

```javascript
const componentOptions = {
  props: {
    status: {
      type: String,
      default: 'todo',
      validator: value => ['todo', 'doing', 'done'].includes(value)
    },
    tags: {
      type: Array,
      default: () => []
    }
  }
}
```

## emits

Use clear domain actions for event names. When the payload matters, use object-form declaration validation:

```javascript
const componentOptions = {
  emits: {
    save: payload => Boolean(payload?.title?.trim()),
    close: null
  }
}
```

Listen for kebab-case events in templates and emit the corresponding canonical names in JavaScript. Check both parent and child sides item by item to avoid silent failures.

## ref and reactive

```javascript
const keyword = ref('')
const form = reactive({ title: '', description: '' })

function reset() {
  keyword.value = ''
  form.title = ''
  form.description = ''
}
```

```html
<input v-model="keyword">
<input v-model="form.title">
```

Use `.value` for `ref` in JavaScript; do not write `.value` in templates.

## Template rules

- The template must be a static string inside the module.
- Do not fetch templates, concatenate template strings, use `v-html`, or compile user input at runtime.
- Render user content with `{{ value }}` so Vue escapes it as text.
- Close component tags consistently in pairs to reduce nesting and parsing ambiguity.
- Use stable domain IDs for `v-for`, not indexes.
- Move complex expressions into `computed()` or functions so the template stays readable.
- Do not declare assignments, increments, or other side-effecting expressions in a template.
- When `v-if` switches a complex screen or modal, confirm that unmounting will not lose a draft that must be retained; explicitly store it in the parent or store when retention is required.

## Class-binding rules

Components use the classes, UI framework components, and style structure required by the user or upstream visual guidance; this directory does not prescribe colors, spacing, radii, cards, shadows, or layout styles.

When Tailwind classes switch conditionally, complete strings must appear statically in the source so runtime concatenation does not make them unrecognizable. The exact classes come from the visual design:

```javascript
const statusClasses = {
  todo: 'task-status task-status--todo',
  doing: 'task-status task-status--doing',
  done: 'task-status task-status--done'
}
```

Do not concatenate incomplete class names at runtime:

```javascript
const className = `${prefix}-${status}-${level}`
```

## Icons

When the interface needs icons, select and verify one primary icon library according to [ui-foundation.md](ui-foundation.md). Do not temporarily replace action icons with Emoji or privately load another icon library in one component.

Icon-only buttons must provide `aria-label`; decorative icons must add `aria-hidden="true"`. When an icon library needs initialization, registration, or rescanning after updates, place the call in an explicit Vue lifecycle hook and clean up the corresponding listeners or instances on unmount.

## Lifecycle and cleanup

Side effects registered by a component must be cleaned up in pairs:

- `addEventListener` / `removeEventListener`；
- `setInterval` / `clearInterval`；
- `ResizeObserver` / `disconnect`；
- `MutationObserver` / `disconnect`；
- Destroy instances created by icons, Dialog, Popover, or other libraries;
- Cancellable asynchronous tasks must not update state after unmount.

Do not add random delays with no business value merely to look like a real API. With a real HTTP API, drive loading/error from the request lifecycle; with static demo data, simulated states must be repeatable and clearly unrelated to a real interface.
