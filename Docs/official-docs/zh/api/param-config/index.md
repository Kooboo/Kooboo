# k.paramConfig

> 读取后台 **内容 → 参数配置** 中保存的结构化 JSON 数据

## 概述

`k.paramConfig` 读取后台 **内容 → 参数配置** 中保存的数据：每条配置有 **名称**（`name`）、**显示名**、**Schema**（字段结构）与 **data**（JSON 值）。在 Page、Layout、View 或 KScript 中按配置名与字段名访问，无需把电话、开关、链接对象等写死在模板里。

后台维护方式见 [参数配置](../../cms/content/user-options.md)。

::: tip 与其它 API 的区别
| API | 用途 |
|-----|------|
| **`k.paramConfig`** | 本页：按 Schema 定义的站点级 JSON 参数 |
| **`k.label` / `k.t`** | 多语言短文案，见 [k.label](../label/index.md) |
| **`k.content`** | 内容夹条目字段，见 [k.content](../content/index.md) |
| **元素属性（KConfig）** | 模板 `k-config` 绑定的 HTML 属性，见 [元素属性](../../cms/content/tag-attributes.md) |
:::

## 访问方式

### 属性访问（推荐）

配置在后台的 **名称** 作为 `k.paramConfig` 的一级属性；Schema 中的字段名作为其下的属性（对象可继续嵌套）。

```javascript
// 配置名 siteContact，Schema 含 phone、showBanner
var phone = k.paramConfig.siteContact.phone;
var show = k.paramConfig.siteContact.showBanner;
```

引擎会为当前站点的各配置生成 **KDefine** 类型定义，`k.paramConfig` 下可按配置名、字段名获得智能提示。


## 数据形状

- **Schema** 在后台定义，支持 `string`、`number`、`boolean`、`object`、`array`（及嵌套）。
- **data** 须与 Schema 一致；在后台 **编辑数据** 页或 JSON 编辑器中维护。
- 数组、嵌套对象的访问方式与普通 JavaScript 对象相同，例如 `k.paramConfig.footer.links[0].url`。

## 不存在或未填值时

- 配置名不存在，或该配置尚无 `data`：属性访问将得到 **`undefined`**。
- 脚本中应先判断再使用，避免直接访问深层属性时报错。

## 相关文档

- [参数配置（后台）](../../cms/content/user-options.md)
- [k.site](../site/index.md) — 站点资源
- [k.label](../label/index.md) — 多语言标签
