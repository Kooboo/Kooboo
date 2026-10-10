# k.site.labels

> 管理站点的多语言标签

## 概述

`k.site.labels` 是标签（Label）的管理器，标签用于存储多语言文本内容。通过标签，你可以：

- 集中管理站点的多语言文本
- 在模板中引用标签而不是硬编码文本
- 方便地进行国际化

## 前置要求

::: tip 多语言配置
使用多语言功能前，需要先在 Kooboo 后台完成以下配置
:::
### 1. 启用多语言

**配置路径**：站点设置 → 基础设置 → 基础 → 多语言

1. 进入 Kooboo CMS 后台
2. 选择目标站点
3. 进入 站点设置 → 基础设置 → 基础 tab
4. 在 多语言 选项中添加需要的语言（如 `zh` 中文、`en` 英文）
5. 保存设置

### 2. 配置标签内容

**配置路径**：内容 → 标签

在该页面可以创建和管理标签，配置每个标签在不同语言下的翻译文本。操作说明见 [后台：标签](../../cms/content/labels.md)。

例如创建一个 `Cancel` 标签：
- `zh`: 取消
- `en`: Cancel

## k.label() / k.t()

`k.label()` 和 `k.t()` 是全局函数，功能完全一致，用于获取指定标签的文本内容。它们会根据当前站点的语言设置返回对应的翻译。

### 参数

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| NameOrId | string | 是 | 标签的名称或 ID |
| params? | object | 否 | 替换参数对象 |

### 返回值

`string` - 标签的文本内容

### 代码示例

```ts
// 根据标签名称获取当前语言的文本
const text = k.label("Cancel");
// 返回: "取消" (中文环境) 或 "Cancel" (英文环境)

// 使用 k.t() (功能相同)
const text2 = k.t("Cancel");
// 返回: "取消"

// 带参数替换
const text3 = k.t("count:{count}个", { count: 123 });
// 返回: "count:123个"

const text4 = k.label("Welcome {name}", { name: "张三" });
// 返回: "Welcome 张三"
```

## labels 方法

`k.site.labels` 提供了以下方法：

### all()

获取所有标签列表。

**参数**：无。

**返回值**：`object[]`。每个对象包含各语言版本的键值对。

```ts
const labels = k.site.labels.all();
// 返回: [{ zh: "取消", en: "Cancel" }, ...]
```

返回的每个标签对象包含各语言版本的键值对。

### get(nameOrId)

根据名称获取单个标签。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | 标签名称或标签 ID。 |

**返回值**：`object | null`。匹配到的标签对象；不存在时为空值。

```ts
const label = k.site.labels.get("Cancel");
// 返回: { zh: "取消", en: "Cancel" }
```

## 相关文档

- [k.site](../site/index.md) - 站点概述
