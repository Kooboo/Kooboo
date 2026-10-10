# UI 依赖与 Style 技术规范

客户端渲染 Vue 规范不定义页面必须采用哪一种视觉风格。配色、字体、密度、圆角、阴影、布局和动效遵循用户要求、既有 Site 设计系统或上游视觉设计。

本文件只约束这些设计如何安全地落成浏览器可直接加载的 CDN、ESM 和 Kooboo Style 对象。

## UI 依赖兼容性

UI 框架、图标、字体、图表等依赖在写入 Page 前必须记录并验证：

```text
name
version
loadMode              # script | css | esm | import-map
cdnAssets[]
importSpecifier
installMethod
vueCompatibility
componentCoverage[]
themeAdapter
smokeTest.status      # pending | passed | failed
smokeTest.pageUrl
smokeTest.assertions[]
```

- Vue 使用主规范指定的官方 CDN 通道，Tailwind 使用 Kooboo 托管的 v3 CDN；其它依赖记录明确且经过验证的版本策略；
- `importSpecifier` 与 import map 和 `main.js` 的实际导入一致；
- `installMethod` 使用当前版本官方用法，不猜测导出名；
- Vue UI 框架必须复用 import map 中的 Vue，不能通过自身 bundle 再加载 Vue；
- `componentCoverage[]` 只记录当前页面真实需要的组件；
- `themeAdapter` 说明 Kooboo CSS Variables 如何映射到框架公开变量，不使用框架时写 `none`；
- 只有最小冒烟通过后才进入业务组件实施。

视觉设计已经确定，不代表其中建议的 npm 包一定能通过 CDN/ESM 直接运行。依赖不兼容时，应调整技术实现或选择等价依赖，不得在组件中临时混用另一套加载方式。

## 依赖决策原则

减少依赖不是独立质量目标。应比较成熟库与自行实现的总成本，包括实现时间、交互完整性、响应式、可访问性、边界状态、维护成本、包体积和运行稳定性。一个兼容、许可清晰、版本固定的专项库能显著降低复杂度或提升专业度时，AI 应主动引入，不得为了宣称“纯 CSS”“零依赖”而选择明显较弱的实现。

- 图表包含坐标轴、Tooltip、图例、缩放、响应式、动态数据或多序列时，默认使用成熟图表库；纯 CSS 仅适合进度条、装饰性迷你柱或不承载精确数据读取的简单图形。
- 日期/时间选择、富文本或代码编辑、拖拽排序、虚拟列表、复杂表格、地图、文件预览和复杂动画编排等能力，优先评估成熟专项库，不重复实现通用基础设施。
- 简单、静态且实现范围明确的交互继续使用原生 HTML/CSS/JS，例如单个折叠区、基础 Tabs、短过渡和少量状态切换。
- 不因设计系统禁止 Tailwind、Bootstrap 或其它 CSS 框架，就推断其同时禁止图表、编辑器、日期等功能库；功能库只需通过兼容性、许可、版本和主题适配检查。
- 正常的实现级依赖选择由 AI 根据当前需求决定并记录，不需要为了“是否允许引入一个库”暂停询问用户。只有依赖会显著增加体积、引入付费/授权条件、改变技术路线、上传数据到第三方服务或带来长期平台约束时才需要用户确认。
- 选择满足需求的最小能力面：优先现有依赖，其次模块化专项库；不要因为可以引入库就叠加多个职责重复的库。

禁止把“少一个依赖”作为牺牲可读性、交互质量、可访问性或开发效率的理由。若决定自行实现一个已有成熟库覆盖的非简单能力，应能指出自行实现更合适的具体原因，而不是笼统写“避免引入依赖”。

## UI 框架选择

不强制使用 UI 框架。按以下顺序判断：

1. Site 已有框架：优先复用同版本，避免重复 CSS 和组件语义；
2. 用户或设计规范已指定：验证 CDN/ESM 与 Vue 兼容性后使用；
3. 原生 HTML + CSS 已足够：记录 `framework = none`；
4. 新增框架确有价值：只选择一套，并说明引入原因。

不能为了少量 Button、Input 或 Card 引入体积很大且加载链复杂的框架。

## 图标库

App 的导航、紧凑操作、搜索/筛选、展开收起、分页、行操作、状态或反馈中只要存在功能图标 slot，就必须选择图标库。禁止 Emoji、Unicode 符号和手绘 SVG 是来源约束，不是删除图标的理由：

- 优先复用 Site 已有图标库；
- UI 框架官方图标包满足需求时可直接复用；
- 否则选择一套许可证明确、可通过精确版本 CDN/ESM 加载的开源图标库，并记录库名、版本、加载方式和许可证；
- 一个应用保持一套主图标库，不为少数图标混入第二套；
- 操作、导航和状态图标不用 Emoji 代替；
- 不用 `★`、`→`、`×`、`⋮` 等 Unicode 文字符号冒充图标，也不因这些替代方式被禁止而把设计证据要求的图标 slot 删除或退化成纯文字；
- 写 UI 前列出当前 Screen 的语义图标清单；图标库缺少某个 glyph 时，先在同一库选择语义等价项，仍不满足则更换图标库；
- 仅图标按钮提供 `aria-label`，装饰图标提供 `aria-hidden="true"`。

需要记录：名称、版本、资产 URL、加载方式、使用方式以及冒烟结果。

## 最小冒烟

至少验证：

1. import map 可以解析 Vue 和其它 ESM；
2. 页面只存在一个 Vue runtime；
3. `createApp`、插件安装和 `mount` 成功；
4. 当前实际需要的最小 Button、Input、Dialog 和图标可以渲染和交互，设计证据要求的功能 icon slot 没有留空或被纯文字替代；
5. CSS、字体、模块、图标 sprite 和其它子资源无 CORS、4xx/5xx；
6. 控制台无版本不兼容、重复 Vue、组件解析或 hydration 警告。

冒烟失败必须保留失败原因并回到依赖选择，不能把 `failed` 改写为可用。

## `kooboo-var.css`

需要让 DesignPanel 管理共享设计变量时，创建 `kooboo-var.css`。文件只包含：

- 一个 `:root { ... }` 块；
- `--<name>: <value>;` CSS 自定义属性；
- `/* @group ... */` 分组注释；
- 变量声明后的行尾显示名注释。

禁止放入：

- 普通 CSS 属性或额外选择器；
- reset、布局和组件样式；
- 主题选择器；
- `@import`、`@media`、`@layer`、`@font-face`、`@keyframes` 等 at-rule。

格式遵循 [Design Variable Extraction](../../Scenario/design-variable-extraction.md)。没有可管理变量需求时，不创建空文件，并从 Page 移除对应 `<link>`。

## `app.css`

`app.css` 承载普通 CSS：

- reset 和基础规则；
- UI 框架公开变量映射；
- 主题选择器；
- 页面和组件样式；
- 响应式规则；
- 字体、动画和打印样式。

技术要求：

- 通过 `<link rel="stylesheet">` 加载，不使用 CSS `@import` 串联站内 Style；
- 引用的 CSS Variables 必须真实存在；
- 框架变量名来自对应版本官方 API；
- Page、组件 class 和 Style 对象名称保持一致；
- 包含 `[v-cloak] { display: none; }` 或等价规则；
- 包含可见且与组件结构匹配的 focus 样式；禁止使用通配选择器、裸的全局 `:focus-visible`，或 `button:focus-visible, a:focus-visible, input:focus-visible, select:focus-visible, textarea:focus-visible` 这类宽泛的元素类型选择器列表，不加区分地套用同一种 outline。把全局规则改写成元素列表，并不会使其具备组件感知能力；
- 独立控件可以通过组件专属 class 或选择器在自身绘制 `:focus-visible` 指示，但前提是被聚焦元素本身就是完整的视觉边界。图标 + 输入框搜索栏、输入框组、日期字段、自定义选择器触发器等组合控件，必须通过 `:has(:focus-visible)` 或 `:focus-within` 在真实视觉外壳上绘制且只绘制一个与外壳圆角、边界一致的焦点态；禁止同时显示外壳 focus ring 和内部控件 outline。只有外壳已提供清晰的替代焦点态时，才可移除内部控件的 outline。当 CSS 可能脱离 HTML 单独扫描时，内部移除 outline 的选择器必须带上同一个外壳身份，例如 `.search-box .search-input:focus-visible`，使补偿关系可以被静态验证；
- 出现以下任一结果即视为组合控件焦点样式失败：outline 切入前缀图标、后缀操作、分隔线或分组字段；出现内外嵌套的双重焦点环；焦点环沿内部 input 矩形绘制，而不是包围完整交互表面。完成前必须在渲染后的组件层级检查各组合控件的键盘焦点，不能只在 CSS 中搜索到 `:focus-visible` 就视为通过；
- 非必要动效遵循 `prefers-reduced-motion`；
- 启动、loading、error 和空状态有必要的基础样式。

例如，搜索框应把外壳视为一个完整的视觉控件：

```css
/* 避免：宽泛的元素列表仍会让 outline 切进组合控件内部。 */
button:focus-visible,
a:focus-visible,
input:focus-visible,
select:focus-visible,
textarea:focus-visible {
  outline: 2px solid var(--primary-6);
  outline-offset: 2px;
}

/* 推荐：统一焦点环沿搜索框外边界绘制。 */
.search-box:has(.search-input:focus-visible) {
  border-color: var(--primary-6);
  box-shadow: 0 0 0 2px color-mix(in srgb, var(--primary-6) 24%, transparent);
}

.search-box .search-input:focus-visible {
  outline: none;
}
```

## Tailwind v3 CDN

Tailwind 固定使用 Kooboo 托管的 v3 CDN：

```text
https://www.kooboo.com/tailwindcss.js
```

自定义主题必须在独立 `<script>` 中使用 `window.tailwind.config`，并放在 Tailwind CDN `<script>` 之后；该顺序不可颠倒，写在 CDN 之前不会生效。JavaScript 条件分支中的完整类名必须静态出现在源码中：

```javascript
const classesByState = {
  idle: 'control control--idle',
  active: 'control control--active'
}
```

禁止运行时拼接不完整类名：

```javascript
const className = `${prefix}-${state}-${level}`
```

Tailwind、普通 CSS 和 UI 框架的视觉职责由当前项目决定，本规范只要求加载方式、版本和类名可被浏览器可靠处理。
