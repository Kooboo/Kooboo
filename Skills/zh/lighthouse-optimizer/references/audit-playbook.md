# Lighthouse audit 定位手册

将 Audit ID 作为稳定证据。由于不同 Lighthouse 版本可能调整 audit 名称，定位时同时匹配完整 ID 和具有明确含义的片段。

## 目录

- Performance 与 Core Web Vitals
- Images 与 fonts
- JavaScript 与 CSS
- Accessibility
- SEO 与 best practices
- 源码定位规则
- 修复与验证规则

## Performance 与 Core Web Vitals

| Audit ID 或片段 | 优先检查 | 常见修复方向 | 验证方式 |
|---|---|---|---|
| `largest-contentful-paint`, `lcp-*` | DOM node、Page、View、Layout、Image、Style | 优先处理真实 LCP 资源，缩短 render delay，避免对 LCP asset 使用 lazy-loading，简化开销过高的 hero markup | LCP element 保持一致、Preview 视觉一致、新的 LCP |
| `cumulative-layout-shift`, `cls-*` | DOM nodes、View、Page、Layout、Style | 为媒体或组件预留空间，稳定 fonts 和异步内容，避免延迟插入布局 | 相同 viewport 和交互路径、新的 CLS |
| `total-blocking-time`, `interaction-to-next-paint`, long tasks | Script、View inline scripts、third parties | 减少同步任务，延迟非关键初始化，拆分页面专属行为 | 交互正常、Console/Network 无错误、新的 metric |
| `server-response-time`, `document-latency-*` | Page、View、Layout、API、CodeBlock、data binding | 移除重复工作，减少阻塞式远程或数据调用，简化 server rendering | 响应内容不变、无 server error、新的 latency |
| `render-blocking-resources`, `render-blocking-*`, critical request chains | Layout、Style、Script、共享 View | 延迟非关键资源，减少依赖，同时保留必要的 critical styles | 无 FOUC、功能完整、Network 顺序、FCP/LCP |
| `network-dependency-tree-*`, redirects | Page route、Layout 资源引用、外部集成 | 在不改变 canonical 或 authentication 行为的前提下移除不必要的 redirect 和依赖跳转 | 最终 URL、authentication、canonical、Network chain |
| `dom-size`, `dom-size-*` | Page 组合结构、重复 Views、Layout | 减少无意义的 wrapper 和过度重复的 DOM nodes | 视觉层级、bindings、交互 selector |

## Images 与 fonts

| Audit ID 或片段 | 优先检查 | 常见修复方向 | 验证方式 |
|---|---|---|---|
| `unsized-images`, `image-aspect-ratio` | DOM snippet、View、Page、Layout | 添加正确的 intrinsic dimensions 或 aspect ratio，同时保留 responsive CSS | 图片无变形、responsive layout、CLS |
| `modern-image-formats`, `uses-optimized-images`, `image-delivery-*` | Image 对象及所有引用它的 View/Page | 确认质量、transparency、animation 和共享范围后再调整尺寸或转换格式 | 视觉对比、文件大小、所有受影响页面 |
| `offscreen-images` | View/Page 的 image markup | 只对 below-fold 图片使用 lazy-loading，排除 LCP 和交互关键图片 | LCP 身份、滚动加载、gallery |
| `preload-lcp-image`, `lcp-discovery-*` | Layout/head、hero View、image markup | 让 LCP URL 尽早被发现；只 preload 已确认且稳定的 LCP 资源 | 无重复下载、responsive candidate 正确 |
| `font-display`, `font-display-*` | Style、Layout 中的 font declarations | 使用合适的 display policy，只 preload 关键本地 fonts | Typography、语言 glyphs、CLS、Network |

## JavaScript 与 CSS

| Audit ID 或片段 | 优先检查 | 常见修复方向 | 验证方式 |
|---|---|---|---|
| `unused-javascript`, `legacy-javascript-*` | Script、Layout、Page/View inline code | 移除已确认的 dead code，限制页面专属代码的作用范围，延迟可选 modules | 所有依赖页面和交互 |
| `unminified-javascript` | Script delivery/configuration | 优先优化 delivery，不要把源码直接改成难以维护的压缩代码 | 源码可维护性和实际 delivery response |
| `unused-css-rules` | Style、Layout、共享 Views | 不得只根据一次 coverage 删除 selectors；确认 template、responsive、hover、locale 和动态状态 | 多个 routes、breakpoints、交互状态 |
| `unminified-css` | Style delivery/configuration | 在保持源码可维护性的前提下优化 delivery | CSS 正常加载且源码仍可编辑 |
| `third-party-*` | Layout、共享 View、Script、业务集成 | 延迟或移除前先确认归属和业务用途 | Analytics、consent、ads、chat、payment、authentication |
| `cache-*`, `uses-long-cache-ttl` | 资源类型和 site/server settings | 单独提出配置修改建议，不要用重写页面源码代替 | Response headers 和 cache invalidation |

## Accessibility

| Audit ID 或片段 | 优先检查 | 常见修复方向 | 验证方式 |
|---|---|---|---|
| `color-contrast` | DOM node、Style、View | 调整归属明确的 design token 或 scoped selector，不要修改无关的全局颜色 | 各种 states、themes、WCAG contrast、视觉设计 |
| `image-alt` | Image context、View/Page | 为信息型图片编写有意义的 alt；装饰性图片使用空 alt | Screen reader semantics 和重复内容 |
| `link-name`, `button-name`, ARIA name audits | DOM node、View/Page/Layout | 添加与操作一致的可见名称或 accessible name | Keyboard 和 assistive technology 行为 |
| `label`, form audits | View/Page form markup | 将 label 和 description 与真实 control 关联 | 提交、validation、keyboard flow |
| `heading-order`, `document-title` | Page 和组合后的 Views | 修正 semantic hierarchy，不要为了视觉字号选择 heading | Outline、样式、动态 title |
| `aria-*`, `role-*` | 精确 DOM node 和交互代码 | 优先使用 native semantics；理解 widget 后才能修改 ARIA | Keyboard 交互和状态播报 |
| `tap-targets`, viewport audits | Style、Layout、Page | 保留 zoom 能力和可用的 target size，检查 responsive breakpoints | Mobile viewport 和 touch 行为 |

## SEO 与 best practices

| Audit ID 或片段 | 优先检查 | 常见修复方向 | 验证方式 |
|---|---|---|---|
| `meta-description`, `document-title` | Page metadata 和 dynamic binding | 使用基于真实内容的页面专属文本 | Rendered head 和 locale variants |
| `crawlable-anchors` | View/Page links | 使用真实目标地址，不得编造 routes | Route 存在且 navigation 正常 |
| `canonical` | Page/head configuration | 保留有意设置的 cross-domain 和 localized canonical 策略 | Rendered canonical URL |
| `robots-txt`, `is-crawlable` | Site settings 和 deployment policy | 作为 configuration/policy 问题处理，修改前确认环境 | Production indexing 意图 |
| `errors-in-console`, `deprecations` | Script、API/资源失败、third parties | 编辑前先复现问题并确认归属源码 | 执行相同操作后的 Console 和 Network |
| `uses-http2`, security headers, CSP | Server/deployment settings | 除非存在明确的 site setting，否则作为 infrastructure/configuration 问题报告 | Response protocol 和 headers |

## 源码定位规则

1. 优先使用返回的 `selector`、`snippet`、资源 `url` 或 `sourceLocation`，不要根据 audit title 猜测。
2. 对于 DOM 证据，存在 Preview context 时调用 `read_preview_element_context`；否则在组合后的 Page/View/Layout 源码中搜索有区分度的 snippet 或资源 URL。
3. 对于资源 URL，先列出匹配的 Script、Style、Image 或 File，再搜索 Page/View/Layout 中的引用。
4. 将共享 Layout、Style、Script 和 View 视为 high-blast-radius 对象。提出修改前先列出可能受影响的 routes。
5. 无法确认归属时标记为“需人工复核”，禁止随机选择对象。

## 修复与验证规则

- 优化实际测量到的 bottleneck，不要只处理 score label。
- 确认业务用途前，禁止移除 third-party code。
- 禁止只根据一次 coverage 删除 CSS。
- 禁止对已确认的 LCP image 使用 lazy-loading。
- 未检查 keyboard 和 assistive behavior 前，禁止修改 accessibility semantics。
- 新测量结果返回前，禁止宣称指标已经改善；Preview 和 Preflight 用于检查 regression，不能证明 Lighthouse score 已提升。
