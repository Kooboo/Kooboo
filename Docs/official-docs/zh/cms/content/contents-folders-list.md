# 内容列表

> 菜单：**内容 → 内容**  
> 深链：`/_Admin/content/contents?SiteId={站点GUID}`

在 [数据类型](./content-types.md) 建好后，在此创建 **内容文件夹**（Content Folder），再进入夹内维护条目。内容文件夹的 **名称** 即 `k.content` 上的属性名。

::: tip 前置
须先完成 [数据类型](./content-types.md)。
:::

## 如何打开

左侧 **内容 → 内容**，地址为 `/_Admin/content/contents?SiteId=...`。

<DocImage src="/cms/content/contents-folders.png" alt="内容文件夹列表" width="1120" />

## 新建：文件夹 vs 单条内容

工具栏 **新建** 下拉两项，对应站点里两种不同的内容文件夹形态：

| 类型 | 后台标签 | 含义 | 典型场景 |
|------|----------|------|----------|
| **文件夹** | 文件夹 | 可包含 **多条** 内容条目 | 文章、资讯、产品说明列表 |
| **单条内容** | 单条内容 | 全站 **仅一条** 记录，无条目列表页 | 站点公告、首页配置块、全局 SEO 文案 |

### 行为差异（后台）

| | 文件夹 | 单条内容 |
|---|--------|----------|
| 点击列表 **名称** | 进入 **条目列表**，可新建多条 | 直接进入 **条目编辑**（唯一一条） |
| 设置对话框 | 可有分页、排序、关联数据等 | 部分选项不适用（如无分页） |
| 列表页 **类型** 列 | 橙色「文件夹」 | 绿色「单条内容」 |

创建后 **名称** 不可改；**数据类型** 在新建时选定，保存后锁定。

## 列表操作

| 列 / 操作 | 说明 |
|-----------|------|
| 名称 | 进入条目列表或单条编辑 |
| 引用 | 被页面、View 等引用 |
| 类型 | 文件夹 / 单条内容 |
| **设置** | 打开 [内容文件夹设置](./contents-folder-settings.md)（三 Tab） |
| **编辑数据类型** | 跳转该夹绑定的数据类型 |
| 拖动 | 调整内容文件夹在列表中的排序 |
| **显示全部** | 列出被标记为隐藏的内容文件夹 |

新建或编辑内容文件夹配置，见 [内容文件夹设置](./contents-folder-settings.md)。条目维护见 [内容条目](./contents-entries.md)。

## k.content 用法差异（重要）

访问键均为内容文件夹 **名称**（如 `Article`、`SiteNotice`），但 **脚本 API 形状不同**：

### 文件夹（`IsContent = false`）

`k.content.{名称}` 返回 **文件夹对象**，用于列表与按 id 读取：

```typescript
// 全部在线条目（可传选项字典）
const list = k.content.Article.all()

// 按 UserKey 或 Id
const item = k.content.Article.get('my-article-id')

// 其它常见方法见 API 文档
const one = k.content.Article.find({ Title: 'Hello' })
```

### 单条内容（`IsContent = true`）

`k.content.{名称}` **直接返回** 该唯一条目的内容对象（`KTextContent`），**没有** `.all()`：

```typescript
// 直接读字段（属性名为数据类型里的字段名）
const title = k.content.SiteNotice.title
k.content.SiteNotice.summary = '更新后的摘要'

// 错误示例：单条内容文件夹没有 all()
// k.content.SiteNotice.all()  // 不适用
```

::: warning
模板或 KScript 里先确认内容文件夹是「文件夹」还是「单条内容」，再选 `.all()/.get()` 或属性直读。混淆会导致运行时报错或取不到数据。
:::

关联字段（类别/内嵌）在 **单条条目** 上通过别名访问，见 [内容文件夹设置 · 关联数据](./contents-folder-settings.md#关联数据)。

## 相关

| 文档 | 说明 |
|------|------|
| [内容设置](./contents-folder-settings.md) | 基本信息、关联数据、字段顺序 |
| [内容条目](./contents-entries.md) | 条目列表与编辑 |
| [内容模块概述](./index.md) | 菜单总览 |
| [k.content](../../api/content/index.md) | 完整 API |
