# SPA 多语言（前台接口）

> 为单页应用提供 JSON 词典；词条在 [CMS：SPA 多语言](../../cms/development/spa-multilingual.md) 维护

## 概述

Kooboo **SPA 多语言**把翻译存为站点资源（键名 + 多语言 `value` 字典），通过固定 URL 一次性输出某语言下的 key→文案 映射，供 Vue、React 等前端框架加载。

::: warning 无 KScript 管理 API
当前版本**没有** `k.spaMultilingual` 或类似 `k.*` 接口在脚本中增删改词条。请在后台 **开发 → SPA 多语言** 导入/编辑；脚本侧若需同类文案请用 [k.label() / k.t()](../label/index.md)。
:::

::: tip 与 k.label 的分工
| 能力 | SPA 多语言 | k.label / k.t |
|------|------------|----------------|
| 数据维护 | 后台 SPA 多语言页 | 后台 **内容 → 标签** |
| 消费 | 浏览器 `fetch('/_spa/lang/en')` | 模板 `k-label`、Code 中 `k.t('key')` |
| 语言来源 | URL 路径段 `{语言代码}` | 站点多语言 / 当前请求文化 |

---

## 获取词典

### 请求

```http
GET /_spa/lang/{lang}
```

| 部分 | 说明 |
|------|------|
| `{lang}` | 语言代码，与后台列一致（小写），如 `en`、`zh-cn`、`zh-tw` |

无需 `SiteId` 查询参数：由当前访问的**站点域名**解析站点上下文。

### 响应

- **状态码**：200  
- **Content-Type**：`application/json`  
- **Body**：对象，key 为词条名，value 为字符串

```json
{
  "welcome": "Welcome",
  "nav.home": "Home"
}
```

仅包含在该语言（或 fallback 后）有值的词条；无译文且默认语言也无值的 key 不会出现在结果中。

### 回退（fallback）

对每个词条：

1. 取 `value[lang]`（请求路径中的语言）；  
2. 若为空，取该项在后台配置的 **默认语言**（`defaultLang`）对应文案；  
3. 仍有值则写入响应，否则跳过该 key。

默认语言对应列表中标记 **（默认）** 的那一列。

### 缓存

`SpaMultilingualMiddleware` 按 **站点 Id** 缓存序列化后的 JSON 约 **3 秒**，减轻高频刷新压力。后台保存词条后，最坏情况下前台可能短暂读到旧数据，可稍后重试或在前端自行缓存。

### CORS

该路径会设置 CORS 相关响应头（`CorsHelper.HandleHeaders`），便于 SPA 开发服务器跨域调试；生产环境仍建议与站点 **同源** 部署。

---

## 前端接入示例

### 原生 fetch

```javascript
const lang = "zh-cn" // 与 vue-i18n locale 等一致
const dict = await fetch(`/_spa/lang/${lang}`).then((r) => r.json())
document.querySelector("h1").textContent = dict.welcome ?? "welcome"
```

### Vue 3（示意）

```javascript
import { createApp, ref } from "vue"

const locale = ref("zh-cn")
const messages = ref({})

async function loadLocale(code) {
  messages.value = await fetch(`/_spa/lang/${code}`).then((r) => r.json())
  locale.value = code
}

createApp({
  setup() {
    loadLocale("zh-cn")
    return { locale, messages, loadLocale }
  },
  template: `<p>{{ messages.welcome }}</p>`,
}).mount("#app")
```

实际项目可封装为插件，在切换语言时重新 `loadLocale`。

### 与站点多语言联动

若站点同时启用 Kooboo **路由级多语言**（URL 前缀或 `?lang=`），建议 SPA 使用的 `{lang}` 与当前 UI 语言代码**统一约定**（均用 `zh-cn` 或均用 `zh`），避免词典语言与路由文化不一致。

---

## 导入 JSON 格式（与后台一致）

批量维护时可按此结构准备文件，在 CMS **导入**：

```json
{
  "keyName": {
    "en": "English text",
    "zh-cn": "简体中文"
  }
}
```

- `keyName`：最长 50 字符。  
- 内层 key：语言代码（导入后存为小写）。

---

## 相关文档

- [SPA 多语言（CMS）](../../cms/development/spa-multilingual.md) — 导入、导出、语言列、编辑  
- [k.label / k.site.labels](../label/index.md) — 服务端模板多语言  
- [k-data · culture](../../templateEngine/k-data/query.md#culture) — 模板内多语言 URL
