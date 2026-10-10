# Site Layout 布局开发指南

## 概述

本指南提供站点布局开发的简洁参考，介绍常见的占位符、使用场景和示例代码，帮助快速构建页面骨架。

## 使用场景

创建或修改站点布局（页面骨架和占位符）时使用。

## 示例

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Document</title>
</head>
<body>
    <div k-placeholder="Main"></div>
    <div k-placeholder="Sider"></div>
</body>
</html>
```

## 必须包含的结构

- `k-placeholder="[Custom Name]"`: Layout 必须至少包含一个页面占位符，用于标识页面内容注入区域，如 `Main` 或 `Sider`。Page 中的 `<placeholder id="[Custom Name]">` 必须与这里的属性值逐字一致；大小写敏感，`Main` 与 `main` 不相同。详细用法示例参见 `page` 参考文档。
- 全站 chrome（header/footer）：默认在 Layout 中通过 `<view id="site_header">`、`<view id="site_footer">` 引用一次；**不要**在 Page 的 `<placeholder>` 中再次引用同名 view。
- Layout 普通元素不声明 `env="server"`；`k-placeholder`、`view` 和模板绑定均不依赖外层 server 环境

## 建站默认骨架（含 chrome）

建站 S4 步骤 ① 的 **Layout 骨架唯一出处**。其它 reference（含 SOP）只引用本节，不重复贴代码。

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="stylesheet" href="/kooboo-var.css" />
</head>
<body>
    <view id="site_header"></view>
    <div k-placeholder="Main"></div>
    <view id="site_footer"></view>
</body>
</html>
```
