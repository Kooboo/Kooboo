# Kooboo 常见错误快速通道

本文件是 [Kooboo Debug SOP](triage.md) 的已知错误目录。它不替代通用排障闭环，而是在症状和静态证据命中高置信规则时，让 AI 停止开放式猜测，立即执行确定性的检查、最小修复和复验。

## 1. 路由契约

当出现白屏、HTTP 500、模板 SSR 失败、Page / Layout / View 未渲染等信号时：

1. 在首轮证据采集中通过 `run_skill_script("kooboo-coding", "scripts/preflight-scan.js", ...)` 运行 Preflight，并读取当前 Page、绑定 Layout 和挂载 View；不要等待 Preview 工具全部恢复后才运行 Preflight。
2. Browser Control 最多 reload 一次并有限等待；`frame_not_ready`、`agentReady=false` 或 Log / Network unavailable 是熔断信号。
3. Preflight finding 若包含 `knownError`，使用 `playbook.skillName` 和 `playbook.resourceName` 调用 `read_skill_resource`，再按 `playbook.anchor` 定位本文件章节并执行已知错误快速路径；不要继续并列生成 ContentType、缓存、数据为空等无证据假设。
4. 确定性规则命中，加上受影响对象中的直接源码证据，即可确认该类根因；有修复授权时实施最小修复。
5. 修改后必须对同一批对象重新运行 Preflight。Preview 可用时再做同路由运行态复验；Preview 不可用时只能报告 `changed_unverified`。

## 2. V1 目录

| ID | 错误 | 检测方式 | 当前状态 |
|---|---|---|---|
| `KOOB-CDATA-WRAPPER` | Page / Layout / View 正文被 XML CDATA 包裹 | `PF-UNIVERSAL-004` | 已接入快速通道 |
| `KOOB-LAYOUT-PAGE-ASSEMBLY` | Layout / Page / View 或 placeholder 装配错误 | `PF-KOBOO-TEMPLATE-003/005/007/010/013/014` | 已接入快速通道 |
| `KOOB-KDATA-STRUCTURE` | `k-data` query、export、分支、作用域或绑定结构错误 | `PF-KOBOO-TEMPLATE-006/011/012/015/016/017` | 已接入快速通道 |
| `KOOB-RESOURCE-REFERENCE` | Script / Style / Image / View 等资源引用失败 | `PF-KOBOO-RESOURCE-001/002` + Network | 已接入快速通道 |
| `KOOB-AUTH-HEADER-STALE` | 登录成功后 Header 未更新 | 登录场景重放 + Network + 状态源检查 | 待补 |

只有“已接入快速通道”的条目才保证 Preflight 返回结构化 `knownError` 路由。不要把“待补”条目描述为已经自动识别。

---

## 3. `KOOB-CDATA-WRAPPER`

### 3.1 典型信号

- 页面白屏、返回 500，或者 Page / Layout / View 整体没有渲染；
- `read_object_text` 返回的对象正文以 `<![CDATA[` 开头，通常以 `]]>` 结束；
- Preflight 返回 `PF-UNIVERSAL-004`，并携带：

```json
{
  "knownError": {
    "id": "KOOB-CDATA-WRAPPER",
    "priority": "immediate",
    "playbook": {
      "skillName": "debug",
      "resourceName": "references/common-errors.md",
      "anchor": "3-koob-cdata-wrapper"
    }
  }
}
```

### 3.2 根因

`create_page`、`create_layout`、`create_view` 和 `update_text_object` 通过 JSON 传递正文，不需要 XML CDATA 转义。把完整 HTML 包进 `<![CDATA[...]]>` 会把 XML 包装误写入 Kooboo 对象正文，进而破坏模板解析或渲染。

检测规则保持模型无关。即使问题在某个模型生成内容时更常见，也应依赖保存后 Preflight，而不是只依赖模型 Prompt。

### 3.3 确认步骤

1. 确定当前路由对应的 Page、绑定 Layout 和挂载 View。
2. 对这些对象运行 Preflight。
3. 读取所有命中 `PF-UNIVERSAL-004` 的真实对象正文。
4. 确认 CDATA 是对象正文的外层包装，或确认其确实作为错误模板文本写入；不要仅凭用户说“白屏”直接修改。
5. 记录完整受影响对象清单，避免只修 Page 而遗漏 Layout / View 中的同源包装。

Preflight finding 加上正文中的 `<![CDATA[` 是足够的直接证据，不需要继续等待 iframe 才能确认这一静态根因。

### 3.4 最小修复

有“排查并修复”等写入授权时：

1. 对完整外层 CDATA，只移除开头 `<![CDATA[` 和对应结尾 `]]>`；内部 HTML 原样保留。
2. 如果 CDATA 出现在正文中部或存在多段，先检查上下文，不执行全局字符串替换，避免误删 Script / SVG 或文本中的合法字符。
3. 使用 `update_text_object` 保存纯 HTML 正文。
4. 不同时重构模板、数据查询、视觉样式或 Layout 结构。
5. 对同一批 Page / Layout / View 重新运行 Preflight。

### 3.5 验证与结果状态

最低静态验证：

- 同一批对象不再出现 `PF-UNIVERSAL-004`；
- 本次 Preflight 没有其他 `BLOCKER`；
- 读回对象正文，确认没有残留外层 CDATA，且内部 HTML 未被截断。

完整运行态验证：

- reload 原路由；
- 页面实际渲染；
- 原 500 / 白屏不再出现；
- Runtime Log 和 Network 没有相关新错误。

结果措辞：

- 静态与运行态均通过：`fixed_verified`；
- 静态通过但 Preview 仍不可用：`changed_unverified`，剩余风险中写明缺少运行态复验；
- 用户只授权分析：`diagnosed`；
- 对象无法读取或无写入权限：`blocked`。

### 3.6 禁止行为

- 不要在已命中 `PF-UNIVERSAL-004` 后继续多次调用 Browser Control 等待相同 iframe。
- 不要通过注释、HTML entity 或再次包裹 CDATA 来隐藏规则命中。
- 不要只修第一个对象后就宣称整个页面已恢复。
- 不要在未重新运行 Preflight 时声称 BLOCKER 已清除。
- 不要因为 CDATA 高频出现于某个模型，就跳过保存后确定性扫描。

---

## 5. `KOOB-LAYOUT-PAGE-ASSEMBLY`

这一类错误处理的是对象已经存在，但 Layout、Page、placeholder 与 View 的职责或引用关系没有正确闭合。它与“引用的 View / Script / Style 根本不存在”不同；后者属于 `KOOB-RESOURCE-REFERENCE`。

### 5.1 正确装配契约

带 Layout 的 Page 必须同时满足：

1. Page 的 `layoutName` 指向一个真实可读取的 Layout。
2. Page content 使用 `<layout id="LayoutName">...</layout>` 作为外层容器，`id` 与 `layoutName`、Layout 对象名称一致。
3. Layout 使用 `k-placeholder="Main"` 等属性声明注入区域。
4. Page 在 `<layout>` 内使用 `<placeholder id="Main">...</placeholder>` 提供内容。
5. Page placeholder id 与 Layout `k-placeholder` 值逐字一致，大小写敏感。
6. Header / Footer 等全站 chrome View 默认只由 Layout 引用；Hero、Features 等页面级 View 只由 Page placeholder 编排。
7. View 本身只包含可复用区块，不再包裹 `<layout>`、`<placeholder>`、完整 `<html>` 或重复的全站 chrome。

最小正确结构：

```html
<!-- Layout: main -->
<body>
  <view id="site_header"></view>
  <main k-placeholder="Main"></main>
  <view id="site_footer"></view>
</body>
```

```html
<!-- Page: home, layoutName=main -->
<layout id="main">
  <placeholder id="Main">
    <view id="home_hero"></view>
    <view id="home_features"></view>
  </placeholder>
</layout>
```

### 5.2 规则与含义

| 规则 | 已确认的问题 |
|---|---|
| `PF-KOBOO-TEMPLATE-003` | Page 声明了 `layoutName`，但缺少正确的 `<layout id>` 或 `<placeholder>`，或者 `<layout id>` 与 `layoutName` 不一致 |
| `PF-KOBOO-TEMPLATE-005` | Layout 没有任何 `k-placeholder`，无法承载 Page 内容 |
| `PF-KOBOO-TEMPLATE-007` | Layout 与 Page placeholder 重复引用同一个 View，通常导致双 Header / Footer |
| `PF-KOBOO-TEMPLATE-010` | Page 的 `<placeholder>` 在 `<layout>` 外部，或 `<layout>` 被空闭合 |
| `PF-KOBOO-TEMPLATE-013` | Page placeholder id 在 Layout 中没有同名 `k-placeholder`，包括大小写不一致 |
| `PF-KOBOO-TEMPLATE-014` | Page 的 `layoutName` 指向不存在或无法读取的 Layout |

这些规则命中时会路由到：

```json
{
  "knownError": {
    "id": "KOOB-LAYOUT-PAGE-ASSEMBLY",
    "priority": "immediate",
    "playbook": {
      "skillName": "debug",
      "resourceName": "references/common-errors.md",
      "anchor": "5-koob-layout-page-assembly"
    }
  }
}
```

### 5.3 确认步骤

1. 读取当前 Page 的真实 `layoutName` 和正文，不只根据路由名猜测 Layout。
2. 读取该 Layout 正文，并把 Page 与 Layout 作为同一次 Preflight 的 artifacts；Page artifact 必须携带 `layoutName`。
3. 列出 Layout 中的所有 `k-placeholder`，以及 Page 中的所有 `<placeholder id>`，按原始大小写逐项对应。
4. 列出 Layout 和 Page placeholder 中引用的 View，检查同名 View 是否被双方重复托管。
5. 对重复 View 判断职责：全站 chrome 默认保留在 Layout；页面级区块默认保留在 Page。
6. 若 `<view id>` 指向的对象不存在，不在本 playbook 中猜测创建内容，转入 `KOOB-RESOURCE-REFERENCE`。

Preflight 已经返回确定性 finding 时，不需要通过反复刷新 Preview 来确认标签嵌套或名称不匹配。

### 5.4 按规则最小修复

#### `003`：Layout 绑定不完整或名称不一致

- 以 Page 实际 `layoutName` 和真实 Layout 对象名称为准。
- 修正 Page 外层 `<layout id>`，并补齐嵌套的 `<placeholder>`。
- 不同时重命名 Layout，除非用户需求明确要求重命名。

#### `005`：Layout 缺少注入区域

- 在 Layout 的实际页面主体位置添加 `k-placeholder="..."`。
- 不要机械添加到 `<body>` 末尾；注入位置必须符合页面骨架。
- 随后让 Page 使用完全相同的 placeholder 名称。

#### `007`：View 重复引用

- Header / Footer、全站导航、全局通知等 shared View 默认保留在 Layout，从 Page placeholder 删除重复引用。
- 页面专属 View 默认保留在 Page；如果 Layout 中误引入，才从 Layout 删除。
- 不通过 CSS 隐藏其中一个副本。

#### `010`：placeholder 不在 layout 内

- 把 `<placeholder>` 移入对应 `<layout>` 内部。
- 删除空闭合 `<layout></layout>` 与并列结构，不复制 placeholder 内容。

#### `013`：placeholder 名称不匹配

- 默认修正 Page 的 `<placeholder id>` 以匹配 Layout 的 `k-placeholder`。
- 只有 Layout 区域命名本身确实错误并影响多个 Page 时，才修改 Layout，同时检查所有绑定 Page。
- 名称大小写必须原样一致。

#### `014`：Layout 不存在或无法读取

- 先确认是名称错误、对象尚未创建还是读取权限问题。
- 名称错误时修正 Page 的 `layoutName` 与 `<layout id>`。
- 不要为了让 Preflight 通过而创建一个同名空 Layout；只有需求中确实需要新 Layout 时才创建完整骨架。

### 5.5 验证

静态验证必须包括：

- 对 Page、绑定 Layout 和本次涉及的 View 重新运行同一次 Preflight；
- `003/005/007/010/013/014` 均不再出现；
- Page `layoutName`、`<layout id>` 和真实 Layout 名称一致；
- Page placeholder 与 Layout `k-placeholder` 一一对应且大小写一致；
- shared View 和 page-level View 没有重复托管。

运行态验证至少检查：

- 原路由实际渲染；
- Header / Footer 只出现一次；
- Page 主体进入正确 placeholder；
- 页面级 View 顺序正确且没有缺失；
- Preview Log 没有新增模板异常，Network 没有相关资源失败。

静态通过但 Preview 不可用时报告 `changed_unverified`。只有实际页面装配和关键 View 均正确显示，才能报告 `fixed_verified`。

### 5.6 禁止行为

- 不要只修改 Page content，却忘记同步传入或保留正确的 `layoutName`。
- 不要把 `<layout>` 当作与 `<view>` 相同的声明标签；它必须包裹 `<placeholder>`。
- 不要把 Page placeholder 名称转成统一小写；placeholder 匹配大小写敏感。
- 不要同时在 Layout 和 Page 引用 Header / Footer，再用样式隐藏重复部分。
- 不要在 Layout 无法读取时直接创建空 Layout。
- 不要把 View 引用不存在误判为 placeholder 装配错误；应转入资源引用快速通道。

---

## 6. `KOOB-KDATA-STRUCTURE`

KData 的很多错误不会产生明确异常，而是静默地“不生成变量”或“不暴露到模板”，因此经常被误判成数据库为空、接口失败或 `k-for` 不工作。Debug 必须先排除结构契约，再调查真实数据。

### 6.1 运行时关键契约

- DataEvaluator 只解析 `<k-data>` 的内部 HTML；放在外部的 `query` 等标签不会进入 KData 流程。
- `query` 缺少非空 `as` 时直接结束，不写入变量。
- `source` / `action` 找不到已注册查询时直接结束，不写入变量。
- `export` 是 `query` / `let` 的布尔属性；无值或 `true` 表示暴露，`false` 或省略表示不暴露。
- 最终只有真实生成且由当前 `query` / `let` 的 `export` 属性暴露的值才进入模板 DataContext。

因此“模板变量为空”至少有三种不同原因：查询没有执行、查询执行但未声明 `export`、查询正常且已声明 `export` 但真实结果为空。必须按顺序区分。

### 6.2 规则与含义

| 规则 | 已确认的问题 |
|---|---|
| `PF-KOBOO-TEMPLATE-006` | `k-attribute` 插值直接做数学运算，应先在 KData 中通过 `let use` 预计算 |
| `PF-KOBOO-TEMPLATE-011` | `<query ... />` 自闭合，可能吞入后续 KData 节点 |
| `PF-KOBOO-TEMPLATE-012` | `when` / `else-if` / `else` 被写到 KData 外部 |
| `PF-KOBOO-TEMPLATE-015` | query 缺少 `as/source/resource/action`，或使用不支持的 source/action |
| `PF-KOBOO-TEMPLATE-016` | `query` / `let` 的 `export` 属性缺少 `as`，或使用了无效布尔值 |
| `PF-KOBOO-TEMPLATE-017` | query/try/catch/redirect 位于 `<k-data>` 外部 |

结构化路由：

```json
{
  "knownError": {
    "id": "KOOB-KDATA-STRUCTURE",
    "priority": "immediate",
    "playbook": {
      "skillName": "debug",
      "resourceName": "references/common-errors.md",
      "anchor": "6-koob-kdata-structure"
    }
  }
}
```

### 6.3 确认顺序

1. 读取包含 KData 的真实 Page / Layout / View 正文并运行 Preflight。
2. 先修复所有 `006/011/012/015/016/017` 确定性 finding，不先修改数据库查询条件。
3. 画出变量链：

```text
let/query 的 as
→ 后续 from / where / page-number / page-size 引用
→ 当前标签的 export 属性
→ 模板层 k-for / k-content / k-attribute
```

4. 确认名称逐字一致，特别是 query/let 的 `as`、map 的 `from` 和模板变量。
5. 静态结构通过后，才检查 source/resource/action 是否符合对应 [Query Reference](../../kooboo-coding/references/Frontend/k-data/query.md)，以及真实数据、筛选条件、权限和请求参数。

不要把 `action="list"` 的数组当作单对象读取。已知唯一标识并且要取详情时，应改为 `action="item"`；只有业务语义确实是“有序结果的第一项”时，才使用 `<let from="products[0]" as="product"></let>`。分页查询的列表通常从 `result.list` 读取，不凭其它框架经验猜 `items`。

### 6.4 最小正确示例

```html
<k-data>
  <query export
    as="products"
    source="commerce"
    resource="product"
    action="list"
  ></query>
</k-data>

<div>
  <article k-for="product in products">
    <h3 k-content="product.title"></h3>
  </article>
</div>
```

### 6.5 按规则最小修复

- `006`：使用 `<let use="add|subtract|multiply|divide|modulo">` 预计算，模板只绑定结果变量。
- `011`：把 query 改成显式 `</query>`，并检查后续节点是否曾被错误嵌套。
- `012`：数据流程分支移入 `<k-data>`；模板 DOM 分支改用 `k-if/k-else-if/k-else`。
- `015`：补齐 query 的四个核心属性；根据 Query reference 修正 source/action，不通过拼写近似值试错。
- `016`：把 `export` 写在需要暴露结果的 `query` / `let` 上，并确保标签有非空 `as`；布尔值只使用无值、`true` 或 `false`。
- `017`：把 KData 元素移入同一个 `<k-data>` 容器，保持执行顺序和变量依赖不变。

一次只修复已命中的结构问题。结构通过后如果仍无数据，再进入 Network、服务端日志、真实数据和条件表达式排查。

### 6.6 验证

静态验证：

- 对受影响对象重新运行 Preflight，`006/011/012/015/016/017` 均不再出现；
- 每个 query 都有有效的 `as/source/resource/action`；
- 每个带 `export` 的 query/let 都有非空 `as`，且布尔值合法；
- query 和控制流标签都位于 `<k-data>` 内；
- 模板消费的变量名称与带 `export` 标签的 `as` 完全一致。

运行态验证至少覆盖：

- 无匹配数据：页面显示正确空状态，而不是变量未定义造成的结构消失；
- 单条数据：字段绑定正确；
- 多条或分页数据：列表数量、分页字段和排序符合真实结果；
- 请求参数变化后，queryString/path/form 对应的 `let` 和查询结果同步变化；
- Preview Log、Network 或服务端日志没有相关错误。

只有观察到真实数据链路和预期 UI，才能报告 `fixed_verified`。静态结构通过但无法访问数据或 Preview 时，应报告 `changed_unverified`。

### 6.7 禁止行为

- 不要看到数据为空就把 KData 改写成 `<script env="server">` + 猜测的 `k.*` API。
- 不要把 `export` 写成独立标签，也不要给 `map` 添加当前运行时不支持的 `export` 属性。
- 不要通过给所有变量添加默认假数据来掩盖 query 未执行。
- 不要把 list、paged、item 的返回形状混用。
- 不要在结构 finding 未清除前修改 where、权限或数据库内容。

---

## 7. `KOOB-RESOURCE-REFERENCE`

这一类错误包含两个证据强度不同的子问题：

1. **引用的 View 不存在**：可以由 Page / Layout / View 源码和站点 View 清单确定，属于立即处理的静态根因。
2. **本地 Script / Style / Image / CmsFile URL 不存在或写错**：Preflight 可以确认引用没有匹配当前站点对象的真实 Route，但还要结合 Preview Network 的失败请求确认运行态影响。

不要把“View 没有显示”都归入本错误。如果 View 对象存在，但被放在错误的 Layout、Page 或 placeholder 中，应转入 `KOOB-LAYOUT-PAGE-ASSEMBLY`。

### 7.1 运行时关键契约

- 普通站点模板中的 `<view id="...">` 按 View 名称或 ID 解析；对象找不到时，渲染器记录 View 404 并输出空内容。
- Style、Script、Image 和 CmsFile 通过站点 Route URL 对外提供；HTML / CSS 应引用对象清单返回的真实 `Url`。
- 站内对象 URL 应使用根绝对形式，例如 `/app.css`、`/main.js`、`/images/hero.webp`，避免当前 Page 路由改变相对 URL 的解析结果。
- 外部 `https://...`、协议相对 URL、`data:` / `blob:`、模板动态值以及 `/_api/` 等系统或接口地址，不由本静态 Route 规则判断。

### 7.2 规则与含义

| 规则 | 严重程度 | 已确认的问题 |
|---|---|---|
| `PF-KOBOO-RESOURCE-001` | `BLOCKER` | `<view id>` 在站点 View 名称/ID 清单中不存在；会立即路由到本 playbook |
| `PF-KOBOO-RESOURCE-002` | `HIGH` | `<script src>`、stylesheet `<link href>`、媒体 `src/srcset/poster` 或 CSS `url()` 使用相对 URL，或没有匹配 Script / Style / Image / CmsFile 的真实 Route |

View 缺失的结构化路由优先级是 `immediate`：

```json
{
  "knownError": {
    "id": "KOOB-RESOURCE-REFERENCE",
    "priority": "immediate",
    "playbook": {
      "skillName": "debug",
      "resourceName": "references/common-errors.md",
      "anchor": "7-koob-resource-reference"
    }
  }
}
```

仅命中本地静态 URL 时，优先级是 `normal`。这表示 AI 应先读取对应 Network 记录，而不是直接根据文件后缀修改对象。

### 7.3 确认顺序

1. 对当前 Page、绑定 Layout 和挂载 View 运行 Preflight；不要只扫描用户最后提到的一个对象。
2. 命中 `001` 时调用 `list_site_object` 获取 View 清单，按名称或 ID 确认引用确实不存在。
3. 区分名称拼写错误、引用了已删除对象、对象尚未创建三种情况；不要把它误判为 placeholder 装配错误。
4. 命中 `002` 时读取 issues 模式的 Preview Network，找到相同 URL 的失败、超时或异常状态。
5. 分别读取 `list_site_object(Style|Script|Image|CmsFile)` 返回的真实 `Url`，检查：
   - 引用是否错误增加了 `/src/js/`、`/src/css/`、`/assets/<project>/` 等假想目录；
   - 是否使用了会随 Page 路由变化的相对 URL；
   - 同名对象是否实际发布在另一个 Route；
   - 请求 URL 是否经过 query/hash 后仍对应同一个资源 Route。
6. 如果 Network 请求成功，或者失败 URL 属于 API、外部 CDN、动态模板值，不按 `002` 修改，转入对应的 Network、CSP、认证或外部依赖排查。

`001` 的 Preflight finding 加上 View 清单是足够的静态根因证据。`002` 单独命中只说明“当前引用不匹配站点资源清单”，完整根因仍需要失败 Network 证据或同名对象的真实 Route 作为佐证。

### 7.4 最小修复

#### View 不存在

- 只是拼写或大小写差异：把 `<view id>` 修正为现有 View 的真实名称。
- View 已重命名：更新引用方，不复制旧 View 或创建兼容空壳。
- 需求中确实缺少该区块：先明确它属于 shared chrome 还是 page-level block，再创建有完整职责和内容的 View，并放回正确的 Layout 或 Page。
- 不要为了让 Preflight 通过创建同名空 View。

#### 本地资源 URL 错误

- 使用对象清单返回的真实 `Url`，并写成根绝对 URL。
- 只修改发生错误的 `src` / `href` / `srcset` / `poster` / CSS `url()`，不顺带重命名资源或调整目录结构。
- 若引用 `/src/js/main.js`，而 Script 实际 URL 是 `/main.js`，应修改引用为 `/main.js`，不要再创建一份 `/src/js/main.js`。
- 如果真实对象本身没有预期 Route，先确认对象创建参数和路由配置，再决定修正对象；不要只在模板中继续猜路径。

### 7.5 验证

静态验证：

- 对原 Page / Layout / View / Style 重新运行 Preflight；
- `PF-KOBOO-RESOURCE-001/002` 不再出现；
- 每个 `<view id>` 都能在 View 清单中找到；
- 每个站内静态 URL 都与对应对象的真实 Route 一致，并使用根绝对形式。

运行态验证：

- reload 原路由并重放原操作；
- 缺失 View 的实际内容和位置正确出现，而不是只消除 404；
- 原 Script / Style / Image 请求返回成功状态；
- Console 不再出现资源加载或脚本解析错误；
- Network 没有因为修正路径引入重复请求、重定向循环或错误 MIME 类型。

只有静态检查和运行态资源加载都通过，才能报告 `fixed_verified`。如果 Preview 不可用，或 `002` 没有可读取的 Network 证据，应报告 `changed_unverified` 或 `diagnosed`，并明确缺失的验证证据。

### 7.6 禁止行为

- 不要看到 `<view id>` 没有显示就直接创建同名空 View。
- 不要把 Layout / Page 装配错误当作资源不存在。
- 不要因为 URL 看起来像文件路径就猜测 `/css/`、`/js/` 或 `/assets/` 前缀。
- 不要修改外部 CDN、API、动态模板 URL 来满足站点对象 Route 清单。
- 不要只看到 Preflight `002` 就宣称已确认 404；必须读取 Network 或找到同名对象的真实 Route。
- 不要只验证 HTTP 状态；Script / Style 还要确认 MIME、执行和样式实际生效。
