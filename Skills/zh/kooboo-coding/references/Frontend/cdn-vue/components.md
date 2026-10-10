# 普通 JavaScript 组件

## 标准组件格式

组件直接导出普通 JavaScript 对象，不使用 `defineComponent()`：

下列示例以不使用 UI 框架为前提。若项目已经选择 UI 框架，组件契约、状态与事件边界保持不变，但 Button、Input、Dialog 等应使用该框架当前版本已经验证的官方用法。

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

## 组件契约

每个组件显式声明：

- `name`
- App 自有根组件、Layout、Screen、业务组件（包括具名导出）声明 `koobooSource: { moduleUrl: import.meta.url }`
- `components`
- `props`
- `emits`
- `setup()`
- 静态 `template`

即使没有子组件、props 或事件，也写 `components: {}`、`props: {}`、`emits: []`，让组件契约一眼可见。

`name` 不得省略。全局 `errorHandler`、Vue Devtools 和运行时警告需要它定位具体组件。

### 预览来源元数据

新 App 的组件在导出的组件对象上声明 `koobooSource`。`import.meta.url` 标识真实模块，不手写路径或根据 `name` 推测文件。同一个 Script 中的每个具名组件分别声明元数据和名称。保留脚手架 `/main.js` 在挂载前安装 `/preview-source.js` 的逻辑；插件仅在编辑器预览 iframe 中启用。选择器分别记录当前路由、渲染组件链和 VNode 内容声明上下文，让共享布局以及 Teleport／插槽内容保留自身来源。元数据用于定位模块，不代表应当修改最内层组件。Vue Site 可接入相同插件和元数据；服务端渲染 Site 的选择继续使用 Page/View/Layout 边界。

完整应用的对象清单应记录组件职责、直接依赖、承载的 screen 和所在 Script 对象。代码中的 import、注册名、props、emits 必须与清单和消费者一致。

## screen 组件

screen 组件负责一个可识别的用户任务，不等于 Kooboo Page。它应：

- 接收当前 screen 所需输入，或读取明确的 store state；
- 通过 emits 或 store action 发起领域动作；
- 提供 loading、empty、error 和正常状态；
- 不自行修改全局导航 URL；
- 可以根据服务端返回状态控制展示，但不能把 template 条件判断当成真实鉴权；
- 权限相关动作必须以服务端响应为最终结果。

根 `App` 负责 screen 选择和 shell 组装；screen 组件负责业务区域；叶子组件负责有限展示或输入。不要让根组件承载所有表单、列表和规则。

## 一个模块可以导出多个简单组件

紧密相关、无复杂状态的展示组件可以放在同一个 Script 对象中，以 named export 导出：

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

父组件使用根绝对 URL 导入：

```javascript
import { AppSidebar, TaskStats } from '/component.dashboard.js'
```

## 复杂组件必须独立

出现以下任意情况时，组件应拥有独立 Script 对象：

- 有 `onMounted`、`onUnmounted`、`watch` 等独立生命周期
- 包含表单校验、模态框、焦点管理或键盘交互
- 发起异步请求或编排多个 store actions
- 有较多本地响应式状态
- 模板较长，错误发生时难以快速定位
- 被多个领域模块复用
- 同一模块中已经存在另一个复杂组件

例如 `TaskFormModal` 应使用独立对象：

```text
Name: component.task-form-modal.js
URL:  /component.task-form-modal.js
```

不要因为“按领域聚合”把一个领域的所有组件都塞入同一个大文件。模块内任何语法错误都会导致该模块的所有导出组件无法加载。

## 代码规模

- 单个 Script 推荐保持在约 100～250 行。
- 超过约 300 行时，必须重新检查是否混入多个职责。
- 一个模块最多承载少量紧密相关的简单组件。
- 有生命周期、表单、异步状态的组件优先独立，不依赖行数阈值。
- 不拆分只有十几行、没有独立职责的静态片段。

行数只是诊断信号，职责边界优先于机械限制。

## store 与 emits 边界

- 只影响父组件组合的 UI 事件使用 emits，例如 `close`、`select`、`navigate`；
- 修改共享领域数据使用明确 store action，例如 `createRequest`、`approveRequest`；
- 叶子展示组件不直接导入整个 store，优先通过 props/emits 保持可复用；
- screen 组件可以调用与自身场景对应的 store actions，但不能直接任意修改共享数组；
- 不在多个组件复制同一业务规则，规则集中在 action 或纯函数。

## 子组件注册

组件不会自动注册。父组件必须显式导入并放入 `components`：

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

- props 是只读输入，禁止直接修改 `props.task.title`。
- Object、Array 的默认值必须使用工厂函数。
- 必填字段写 `required: true`。
- 有明确枚举时使用 validator。
- 编辑表单应复制输入对象，提交时 emit 新数据或调用 store action，不直接双向修改父级对象。
- 作为 form draft 的副本必须保留不可变字段，并在 props 输入变化时决定重置、合并或提示冲突；不能静默覆盖用户未提交输入。

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

事件名使用清晰的领域动作。载荷重要时使用对象式声明校验：

```javascript
const componentOptions = {
  emits: {
    save: payload => Boolean(payload?.title?.trim()),
    close: null
  }
}
```

template 中监听 kebab-case 事件，JavaScript 中 emit 相同的规范名称。父子两端必须逐项核对，避免静默失效。

## ref 与 reactive

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

JavaScript 中 `ref` 使用 `.value`；template 中禁止 `.value`。

## template 规则

- template 必须是模块内的静态字符串。
- 禁止 fetch template、字符串拼接 template、`v-html` 和运行时编译用户输入。
- 用户内容使用 `{{ value }}`，让 Vue 进行文本转义。
- 组件标签统一成对闭合，减少嵌套和解析歧义。
- `v-for` 使用稳定领域 ID，不使用 index。
- 复杂表达式移入 `computed()` 或函数，template 保持易读。
- 不在 template 中声明赋值、递增或其他有副作用的表达式。
- `v-if` 切换复杂 screen 或 modal 时，确认卸载行为不会丢失必须保留的 draft；需要保留时明确存入父级或 store。

## class 绑定规则

组件使用用户要求或上游视觉规范提供的 class、UI 框架组件和样式结构；本目录不规定配色、间距、圆角、卡片、阴影或布局风格。

Tailwind 类名需要条件切换时，完整字符串必须静态出现在源码中，避免运行时拼接导致类名无法识别。具体类名由视觉方案决定：

```javascript
const statusClasses = {
  todo: 'task-status task-status--todo',
  doing: 'task-status task-status--doing',
  done: 'task-status task-status--done'
}
```

禁止运行时拼接不完整类名：

```javascript
const className = `${prefix}-${status}-${level}`
```

## 图标

界面需要图标时，先按 [ui-foundation.md](ui-foundation.md) 选择并验证一套主图标库。不得用 Emoji 临时代替操作图标，也不得在单个组件中私自加载另一套图标。

仅图标按钮必须提供 `aria-label`；装饰性图标添加 `aria-hidden="true"`。图标库需要初始化、注册或在更新后重新扫描时，把调用放入明确的 Vue 生命周期，并确保卸载时清理对应监听或实例。

## 生命周期与清理

组件注册的副作用必须成对清理：

- `addEventListener` / `removeEventListener`；
- `setInterval` / `clearInterval`；
- `ResizeObserver` / `disconnect`；
- `MutationObserver` / `disconnect`；
- 图标、Dialog、Popover 或其它库创建的实例销毁；
- 可取消的异步任务在卸载后不再修改状态。

不要为了表现“像真实接口”加入无业务价值的随机延迟。使用真实 HTTP API 时由请求生命周期驱动 loading/error；使用静态演示数据时，模拟状态必须可重复触发并明确与真实接口无关。
