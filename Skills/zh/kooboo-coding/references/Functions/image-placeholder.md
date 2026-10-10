# ImagePlaceholder

占位图仅用于展示，不下载成上传资产或纳入文件清理。只需配图时使用本文，无需实现上传后端；实际图片编辑功能遵循 [图片与文件管理](../Frontend/images-and-files.md)。

需要可加载的占位配图时，直接拼接站内 URL，不调用 Function：

```text
/_api/v2/AiSiteBuilder/ImagePlaceholder?keyword=<encoded>&orientation=<orientation>&size=<size>&page=<page>&color=<encoded>
```

| 参数 | 约束 |
|---|---|
| `keyword` | 必填；具体英文画面描述并 URL 编码 |
| `orientation` | 可选：`landscape` / `portrait` / `squarish` |
| `size` | 可选：`thumb` / `small` / `regular` |
| `page` | 可选，1–20；相似列表图片递增 |
| `color` | 可选；颜色名或编码后的十六进制值，`#` 编码为 `%23` |

可用于 `<img src>`、CSS `url()`、商品图和内容封面字段。关键词必须结合当前内容和视觉方向，不在全站重复同一行业泛词；不自行上网找图，不编造外链图床或随机 URL。
