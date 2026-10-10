# 能力与证据边界

Preflight 有两个独立边界：adapter 权威选择的 Site/App 场景，以及从真实对象识别的能力。场景不能替代能力识别。即使业务界面使用 JavaScript 和 API，App 也可以使用 Kooboo Page/Layout/View 装配。k-data 和 Commerce+k-data 当前仅用于 Site。

## 选择规则

64 条规则按能力组织为 14 个文件。两个 reference 目录使用相同的可执行 YAML；本页说明对应的证据边界。

扫描器读取已保存对象内容，从平台取得 Page LayoutName 和对象真实 URL，并通过 `get_site_info` 读取平台多语言配置。源码标签、CSS、import、API 操作和显式的正向 Commerce 上下文决定适用能力。模型省略 `layers` 不能关闭检测到的检查。`orders`、`login` 等名称本身不能证明对象属于 Commerce。

| 能力 | 触发条件 | 范围 |
| --- | --- | --- |
| 通用 | 所有扫描对象 | Site、App |
| 前端 / 表单 | 可渲染对象；实际原生表单控件 | Site、App |
| 模板装配 | Layout 对象、Page 绑定、layout/placeholder/view 标签或 Kooboo 绑定 | Site、App |
| k-data | k-data 操作/绑定，包括错误地位于容器外的操作 | Site |
| KScript | API/CodeBlock 或服务器 API 用法 | Site、App |
| 数据契约 | Script、API/CodeBlock 或内嵌脚本 | Site、App；作为建议复核 |
| 安全 | 代码或可渲染 HTML | Site、App；明确的私钥阻断，推断风险需复核 |
| 多语言 | 平台多语言配置已启用 | Site、App |
| 资源 / 模块 | 渲染资源、Script import、import map | Site、App |
| Commerce | 实际 Commerce API/query 或显式 Commerce 上下文 | Site、App |
| Commerce 模板 | 同时使用 Commerce 和 k-data | Site |
| 支付渠道 | 实际支付操作；支付宝规则要求实际支付宝用法 | Site、App |

客户端框架语法在对应渲染器中合法。Page 脚本位置和 import map 契约以平台实现为准：普通内联 import map 与受支持的 Header 服务端脚本均合法。资源检查验证字面量本地模块地址和扫描范围内的映射；计算型 import、缺少所属 Page/映射或清单不可用时需要补充证据。

## 结构问题与建议

`findings` 是可限定的结构断言。`advisories` 是关于用户意图、视觉表现、疑似凭据、净化范围、响应/数据契约或实现约定的核实事项。建议不等于违规，不改变结构 PASS，也不意味着该事项已被验证解决。

尊重用户和品牌明确选择的 emoji、颜色、外部图片。三列布局合法，重复 View 和空客户端挂载点可能是有意设计。HTML 字符串、接口响应格式和模块用法应符合实际运行环境及业务契约。

App 的 `mock-api` fixture 合法。逐项比对已接受的数据策略；源码模式无法证明生产持久化或业务正确性。安全建议同样不能证明被调用服务中的字段校验和授权。明确的私钥材料仍是脱敏后的结构 BLOCKER，任何凭据候选都不能出现在报告片段中。

## 证据

结构 PASS 表示已检查范围内没有活动的 BLOCKER。必要的元数据或资源检查无法完成时，返回 `skippedChecks`，通过 `structuralReceipt.complete=false` 保持完成门禁关闭。UI 运行证据与 Commerce/支付证据仍然独立；payment-runtime 阶段继续要求 Commerce 运行证据，return URL 不能证明支付成功。

结合实际用户要求和代码路径核实建议，获取任务特定的运行/业务证据，并报告剩余不确定性。不要增加模型可随意控制的通用豁免列表，也不要为了让 Preflight 通过而关闭已使用的能力。
