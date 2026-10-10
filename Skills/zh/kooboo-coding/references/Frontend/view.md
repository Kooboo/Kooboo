# Site Component 组件开发指南

## 概述

本指南提供站点组件开发的简洁参考，介绍常见结构、使用场景和示例代码，帮助快速构建可复用组件。

## 使用场景

创建或修改站点组件（包括视图和占位符）时使用。

## 示例

### 组件示例

```html
<div>
    <h1>Welcome</h1>
    <view id="other_view"></view>
</div>
```

## 特性

- `k-data` 与 `k-xx` 绑定不需要在 View 外层声明 `env="server"`；只有真正的服务端脚本才使用 `<script env="server">`
- View 应只包含一个可复用的页面区块，不应再次包裹 `<layout>`、`<placeholder>`、完整 `<html>` / `<body>` 或全站页面壳。
- Header / Footer 等 shared View 默认由 Layout 引用一次；Hero、Features、表单等 page-level View 由 Page 的 `<placeholder>` 编排。不要在 Layout 与 Page 中重复引用同一 View。
- 如果 View 在 Page 中没有出现，先检查 Page / Layout 装配关系并执行 [Preflight](../Preflight/index.md)：`PF-KOBOO-TEMPLATE-003/005/007/010/013/014` 用于装配问题，`PF-KOBOO-RESOURCE-001` 用于 `<view id>` 指向的对象不存在。用户显式选择 `/debug` 时，再按 finding 返回的结构化 `playbook` 读取对应 Debug reference。
