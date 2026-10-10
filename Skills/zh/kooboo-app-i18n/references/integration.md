# 安装与接入

## 检查现状

先读站点 cultures/defaultCulture、index import map、main、router、layout、设计系统运行时和现有 Label 的原始语言值。保留认证、路由、定制外壳和启动流程。规划中的能力不等于已安装。需要时用已有规划描述和验收条件记录语言范围，不新增 App Plan 字段。

使用站点实际返回的语言代码。当前 DefaultCulture setter 会把地区语言缩成两位，例如 zh-CN 变为 zh；不要假定默认值为 zh-CN。默认语言必须在应用支持的站点语言中。安装前通过已有站点设置流程补齐所需语言，不顺带改动语言域名/路径策略。组件库语言要显式映射，例如 zh 对应 epZhCn。

模板面向同源 hash-router App。已有语言域名/路径的站点必须先检查其策略并适配导航和 API URL；此模板不实现跨域语言切换或多语言 SEO。业务 API 的 culture 仍由服务端控制，前端切换不会自动翻译业务数据或认证错误。

## 安装独立资产

### 共享服务端配置

App i18n 配置归模块 CodeBlock `ENV` 所有，通过 `ENV.i18n` 导出。先读取现有 ENV 源码，仅合并此分组，保留其他配置和定制逻辑。如果 ENV 使用 TypeScript 接口，同时补充对应字段类型。

```ts
// 放入现有导出的 ENV 对象内部：
i18n: {
  prefixes: ['app.orders.'],
  supportedLocales: ['zh', 'en']
}
```

`Services.i18n` 从 `./ENV` 导入 `{ ENV }`。支持语言和公开 Label 前缀只读取 `ENV.i18n`；默认语言仍来自站点 `defaultCulture`，不另设一份 i18n 默认语言。浏览器只接收筛选后的语言元数据和公开文案，不能输出整个 ENV 对象。

ENV 不存在时，安装器创建包含此分组并导出 ENV 的模块。已有 ENV 时，安装器逐字保留源码，只检查对象类型，不执行或重写任意 JS/TS。回执 `configuration` 返回 `object: "ENV"`、`field: "i18n"`、请求值和状态。`preserved-review-required` 表示使用 API 前，应对照目标配置检查实际 ENV.i18n，并显式合并必要改动。安装参数用于初始化缺失的 ENV，或描述已有 ENV 中待核对的分组，不是第二份运行时配置来源。配置缺失或无效时，服务拒绝读取文案。

### 复制资产

调用 `run_skill_script`：

```json
{
  "skillName": "kooboo-app-i18n",
  "scriptName": "scripts/install-runtime.js",
  "arguments": {
    "prefixes": ["app.orders."],
    "supportedLocales": ["zh", "en"],
    "dryRun": true
  }
}
```

按实际应用选择前缀，检查结果后用 dryRun=false 执行。前缀以 `app.` 开头、`.` 结尾。只把公开 UI 文案放进这些前缀：翻译接口可在认证启动前匿名读取。请求参数不能扩大前缀范围。

脚本通过现有 create Function 直接复制技能资产，不让模型读入再输出大体积第三方库。写入前预检全部名称与路由；同内容跳过，不同内容拒绝覆盖；每次写入后读回。多个对象不构成数据库事务，失败时检查已完成对象和未验证对象，不声称自动回滚。脚本不安装 Label，不改 main/index/router/layout 和站点设置。

| 对象 | 名称 / URL | 用途 |
| --- | --- | --- |
| CodeBlock | ENV | 共享服务端配置；支持语言和 Label 前缀统一归 ENV.i18n 所有 |
| CodeBlock | Services.i18n | 严格读取语言值，不使用 Label 隐式回退 |
| API | app.i18n / `/api/app-i18n/{action}` | 基础 GET `/api/app-i18n?locale=en` |
| Script | vue-i18n.js / `/vue-i18n.js` | 固定版本完整浏览器 ESM，保留消息编译器 |
| Script | service.i18n.js / `/service.i18n.js` | 初始化、会话缓存、切换与标题辅助 |

接口返回 siteId、locale、currentLocale、defaultLocale、supportedLocales 和目标/默认语言 messages，没有 `data` 包装。省略 locale 时按 preferredLocale（URL 选择）、savedLocale（保存的选择）、服务端当前语言、默认语言选择第一个受支持的值；无效偏好会被忽略，显式传入不支持的 locale 仍返回 400。仅返回严格匹配且非空的 Label 值。不调用 k.t，不使用持久服务端缓存，刷新页面后读取 Label 更新和删除。

## 写入 Label

使用稳定语义 key，例如 `app.orders.common.save`，总长不超过 64 个 ASCII 字母、数字、点、下划线或连字符。判断是否缺少翻译前必须查看原始语言键：普通 get_label/k.label/GetCultureValue 都可能回退。KScript 检查单个 Label 时，先看 `label.keys`，找到确切语言键后才调用 `label.getCultureValue(exactKey)`。Services.i18n 提供了此写法。

使用现有 `set_label`，按明确 culture 批量写入，保留其它语言及已有翻译。检查后可写入：

```json
{"culture":"en","labels":[
  {"name":"app.orders.common.save","value":"Save"},
  {"name":"app.orders.language.label","value":"Language"},
  {"name":"app.orders.language.error","value":"Could not switch language. Try again."}
]}
```

同时补齐默认语言。覆盖启动/外壳/账户/通知操作、空/加载/失败状态、aria-label、placeholder、校验和按钮；历史通知 title/body 与业务内容不自动迁移。新后端错误可返回稳定 code/params，界面映射到 Label，保留旧 message 作为兼容兜底。

消息使用 vue-i18n 语法：命名插值 `{name}`，复数 `no items | one item | {count} items`。字面量示例：`user{'@'}example.com`、`left{'|'}right`、`{'{'}name{'}'}`。不要在服务端先插值，不拆句拼接，不把 Label 交给 v-html。此语法不等同于 k.label 的简单字符串替换。

## 局部接入启动代码

在现有 import map 增加 `"vue-i18n": "/vue-i18n.js"`，保留同一个 `/vue.js`。该产物外部引用 Vue，不引入另一份 Vue，也不用 npm 的 bundler 专用入口。

随包版本、校验和及许可证见[第三方库来源](../assets/vendor.md)。

将以下操作合并进当前 main.js 的 try 块，保留 bootstrap、router、错误处理、预览桥接和图标初始化：

```js
import { createAppI18n, bindTranslatedTitle } from '/service.i18n.js'

// 在现有 loadAppBootstrap 附近、挂载之前：
const languages = await createAppI18n()
// 在现有 createApp、createAppRouter 之后：
app.use(languages.i18n)
app.provide('app.i18n', languages)
// 必须在 createAppRouter 已注册旧标题 afterEach 后注册：
const stopTitle = bindTranslatedTitle(router, languages.i18n, {
  appTitleKey: 'app.orders.name', appTitle: document.title
})
// 保留 app.use(router)、await router.isReady()、app.mount(...)。
// 若应用支持卸载，在卸载生命周期调用 stopTitle。
```

初始化失败进入带重试/刷新的启动错误界面。语言包不可用时可保留一句静态紧急提示，但不维护第二套完整字典。createAppI18n 会抛出错误而非静默展示 key。

首次选择顺序：hash 前有效 `?lang=` → 站点隔离的已存选择 → 服务端选择 → 默认语言。hash 内的路由查询不是服务端语言参数。安装时在 service.i18n.js 写入站点 ID，首次请求前即可读取原有站点隔离偏好，一次响应同时取得配置和目标/默认语言包。两个返回的语言包都会缓存，切回默认语言无需再次请求。

切换调用 `languages.setLocale(value)`，不要直接赋值 locale ref。它加载目标/默认语言、准备组件库资源，只允许最新请求提交；失败可重试。成功更新 document.lang、可用时的 localStorage，并同步已有 URL lang，保留 hash/history state。语言包仅在页面会话内缓存，Label 更新/删除后刷新；运行时语言配置变化也需刷新。

## Vue 与切换入口

```js
import { computed, inject, ref } from 'vue'
import { useI18n } from 'vue-i18n'
// 放在现有 setup() 中：
const { t, locale } = useI18n({ useScope: 'global' })
const languages = inject('app.i18n')
const switching = ref(false)
const languageError = ref(false)
async function changeLanguage(event) {
  switching.value = true
  languageError.value = false
  try { await languages.setLocale(event.target.value) }
  catch { languageError.value = true }
  finally {
    event.target.value = locale.value // 失败时还原原生 select 的选择
    switching.value = false
  }
}
const rules = computed(() => ({
  name: [{ required: true, message: t('app.orders.nameRequired'), trigger: 'blur' }]
}))
// 向模板暴露 t、locale、languages、switching、languageError、changeLanguage、rules。
```

按当前设计系统适配有可访问标签的入口，例如：

```html
<label>
  {{ t('app.orders.language.label') }}
  <select :value="locale" :disabled="switching" @change="changeLanguage">
    <option v-for="code in languages.supportedLocales" :key="code" :value="code">{{ code }}</option>
  </select>
</label>
<p v-if="languageError" role="alert">{{ t('app.orders.language.error') }}</p>
```

可用语言的本地名称或翻译名称替代代码展示。示例中所有 key 均需写入 Label。按钮用 `{{ t(key) }}`，属性用 `:placeholder="t(key)"`、`:aria-label="t(key)"`。

菜单在 computed 内根据稳定的 `meta.navigationLabelKey` / `navigationGroupKey` 翻译，旧文本作为兼容回退；不要只在创建路由时翻译一次。路由增加 `meta.titleKey`，标题辅助同时处理导航和语言切换。日期/数字用当前 locale 的 Intl 或 vue-i18n 格式化，时区单独处理。已经显示的表单错误按表单行为在切换后重新校验或清除。

## 设计系统

Element Plus 的 `/element-plus.js` 已导出 epZhCn、epEn。启动时准备共享 shallowRef：

```js
import { shallowRef } from 'vue'
import { epZhCn, epEn } from '/element-plus.js'
const elementLocale = shallowRef(epZhCn)
const elementLocales = { zh: epZhCn, 'zh-CN': epZhCn, en: epEn }
const languages = await createAppI18n({
  prepareLocale: async code => {
    if (!elementLocales[code]) throw new Error('Component language unavailable')
    return elementLocales[code]
  },
  commitLocale: (_code, prepared) => { elementLocale.value = prepared }
})
app.provide('app.elementLocale', elementLocale)
```

layout 注入 app.elementLocale，把固定 `:locale="epZhCn"` 改为 `:locale="elementLocale"`。更多语言需实际提供组件库语言对象并扩展映射。prepareLocale 可异步，commitLocale 必须简短、同步且不抛错。KbElementShell 的导航/菜单标签、外围 popover 也要翻译，ElConfigProvider 不处理业务文案。

Volcengine 没有通用 locale provider。外壳文案局部迁移；组件有 label props 时显式传翻译，例如 YlPagination 的 label/previousLabel/nextLabel。分页总数、跳页等部分内部选项仍有硬编码：检查实际使用的组件，局部适配已安装源码或组合应用级翻译控件。不能只切换外壳就声称组件内部已完成国际化，也不要复制整套设计运行时。以后同步设计系统前需检查并保留这些应用修改。

## 维护与移除

验证后在站点记忆记录实际 service/main 注册、语言与前缀约定。后续有实际接入时继续加载本技能。安装脚本不会升级覆盖定制对象，版本注释只说明来源。移除时先去掉翻译消费者和注册，再删无引用资产。默认保留 Label；现有引用扫描不能证明 Vue Script 中的 key 未被使用。

维护已接入的 App 时，一起检查应用拥有的 service.i18n.js、Services.i18n、app.i18n，保留定制代码、ENV 和 Label。确认 Services.i18n 读取 ENV.i18n，并将前端 service 的 installedSiteId 绑定到目标站点 ID（或向 createAppI18n 传 siteId）。不要盲目重跑安装器覆盖定制对象；复制到另一站点时必须重新绑定站点 ID。移除 i18n 时只删除已无引用的 ENV.i18n 分组，保留共享 ENV 对象及其他模块配置。
