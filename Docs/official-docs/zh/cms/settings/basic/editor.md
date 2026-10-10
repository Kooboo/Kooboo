# 基础 · 编辑器 Tab

> Tab：**编辑器** · `name=editor`  
> 深链：`/_Admin/system/settings?SiteId={站点GUID}&name=editor`

配置后台与 **内联编辑** 相关体验：富文本（TinyMCE）工具栏、**内联设计** 默认选项、Monaco **代码补全库**，以及 **操作日志录屏**。

<DocImage src="/cms/settings/settings-basic-editor.png" alt="编辑器 Tab" width="1120" />

## 富文本编辑器

**RichTextEditorConfig**（`tinymce.vue`）：配置 TinyMCE 插件、工具栏按钮等站点级默认值，影响内容字段、多行文本等使用富文本控件的界面。

## 内联设计设置

| 项 | 说明 |
|----|------|
| **内联设计设置** | 点击 **编辑** 打开 `InlineDesignSettingsDialog`，配置页面内联编辑时的默认行为与面板选项（`inlineDesignSettings`） |

与 [页面内联编辑](../../site/pages-inline-edit.md) 配合使用。

## 附加代码建议

**附加代码建议**（`codeSuggestions`）多选：为 Monaco 编辑器注入额外类型/片段库，可选例如：

- `bootstrap-v3`
- `tailwind-v3.1`
- `element-plus-v2.2`
- `amp-v0`

影响 [开发 → 代码](../../development/code.md) 等编辑界面的智能提示。

## 操作日志录屏

**记录站点日志视频**（`recordSiteLogVideo`）：开启后操作日志可附带屏幕录制信息（与 [运维 · 操作日志](../../operations/site-logs.md) 相关，具体以产品为准）。

## 相关

- [基础设置概述](./index.md)  
- [内容 · 条目编辑](../../content/contents-entries.md)
