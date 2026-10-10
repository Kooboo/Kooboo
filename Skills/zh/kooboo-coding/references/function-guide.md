# Kooboo Function 路由

只读取当前操作需要的 reference：

| 当前任务 | Reference |
|---|---|
| 列举、读取、更新、删除对象；预览元素定位 | [读取与更新](Functions/read-update.md) |
| 创建 Page / Layout / View / Style / Script；Page Plan 装配 | [前端对象创建](Functions/frontend-create.md) |
| 创建 API / CodeBlock | [Backend 对象创建](Functions/backend-create.md) |
| 生成可加载的占位图片 URL | [ImagePlaceholder](Functions/image-placeholder.md) |

不要为了一个对象操作加载全部 Function 说明。参数和返回值以当前 Function 实现与对应 reference 为准；不确定时先读取目标 reference，再调用 Function。
