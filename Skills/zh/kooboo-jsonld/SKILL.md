---
name: kooboo-jsonld
description: "用于在 Kooboo 页面、视图或布局中添加、检查和排查 JSON-LD 结构化数据：k-json-ld、schema.org、Product、Article、Organization、BreadcrumbList、富媒体搜索结果、字段映射以及 application/ld+json 未输出。涵盖模板语法和当前渲染限制。"
---

# Kooboo JSON-LD

## 工作流程

1. 修改站点代码前先加载 `kooboo-coding`，遵守其读取、编辑和验证流程。后台配置任务加载 `kooboo-cms-guide`，读取 `references/operations/ai-service.md`。
2. 检查目标页面、视图与布局、已有结构化数据脚本以及渲染上下文中的数据。选择符合可见内容的 schema.org 类型，不虚构评分、价格、库存或商家信息。
3. 页面级结构化数据优先使用下方显式模板元素。不要假设保存字段映射就会生成页面输出。
4. 检查渲染后的 HTML，将每个 `application/ld+json` 脚本解析为 JSON，并与页面可见内容对比。检查页面与布局之间是否存在重复或冲突的实体。
5. 使用 Schema.org Validator 验证语义；涉及 Google 支持的搜索展示时，再使用 Rich Results Test。JSON 合法不代表满足富媒体结果条件，也不保证收录。

## 显式模板元素

使用 `<k-json-ld>` 元素。直接在其中放声明，不添加普通 HTML 包装，也不放原始 JSON。

```html
<k-json-ld>
  <type name="Product">
    <property name="name" value="'Laptop'"></property>
    <property name="description" value="'Lightweight laptop'"></property>
    <map as="offers">
      <type name="Offer">
        <property name="price" value="99"></property>
        <property name="priceCurrency" value="'USD'"></property>
      </type>
    </map>
  </type>
</k-json-ld>
```

以上仅为示例值，不应当作真实站点信息直接照搬。输出的 JSON 等价于：

```json
{
  "@context": "https://schema.org",
  "@type": "Product",
  "name": "Laptop",
  "description": "Lightweight laptop",
  "offers": { "@type": "Offer", "price": 99, "priceCurrency": "USD" }
}
```

模板元素会被替换为 `<script type="application/ld+json">`。每个根级 `type` 生成独立脚本，不会自动组成 `@graph`。

## 取值与嵌套

- `type name` 设置 `@type`；根类型还会获得 `@context`。
- `property name` 是输出属性名，`value` 从属性读取，不读取元素文本。
- `value="product.name"` 从渲染数据上下文解析。必须先确认 `product` 存在；这不是数据库查询，也不是通用 JavaScript 表达式。
- 上下文取值失败后，会尝试解析 null、布尔、整数、浮点数或去掉成对引号的字符串。不存在的 `product.name` 可能变成字面字符串 `"product.name"`，不要假设缺失字段会被省略。
- 字符串常量使用内层引号，例如 `value="'USD'"`。引号和特殊字符仍需遵守 HTML 属性转义规则。
- 用不带 `from` 的 `map as="offers"` 构造嵌套对象。`as` 是输出属性名，不是变量别名。
- 不要直接嵌套 `type` 来生成具名属性。子类型的键会合并到父对象，可能覆盖父对象的 `@type`；`type as` 虽会被解析，但渲染时没有使用。
- `map from` 查找已有上下文值，缺失时输出 null。虽然接受 JSON 对象、数组及非字符串可枚举集合，但当前渲染器不会把当前项绑定到子上下文。不要把它当作逐项循环，否则可能重复输出同一外层上下文的值。需要按项取值的数组时，应按站点编码流程采用已验证的替代方案并安全序列化，不要手工向脚本插入未经转义的 JSON。
- 仅识别 `type`、`property` 和 `map`，不会遍历未知包装元素。根级只有 property/map 不会生成文档。

## 字段映射配置

后台 JSON-LD 配置支持选择来源、表或内容类型，并将源字段映射到 schema.org 类型和属性。来源包括 content、IndexedDB、SQLite、MySQL、SQL Server 和 MongoDB。外部连接失败可能导致列表为空。映射不会修改原始数据，也不会验证 schema 必填字段。页面输出应使用上述显式模板元素进行验证。

## 排查顺序

1. 没有脚本：检查实际渲染路由、模板是否被包含、`<k-json-ld>` 元素、直接根级 `type`，以及是否禁用了数据求值器。忽略 `EnumEvaluator.Data` 时会跳过该元素。
2. JSON 出现路径字面量：检查上下文变量名称以及渲染时数据是否可用。
3. 嵌套类型错误或集合对象重复：检查直接嵌套 `type` 的写法以及上述 `map from` 当前项限制。
4. 输出异常：检查服务端渲染后的 HTML 与错误，而不仅是模板源码。渲染异常可能以文本形式出现在页面中。渲染器不校验 schema.org 名称或必填字段。
5. JSON 可解析但验证失败：核对类型、必填属性、URL、真实值及其与页面可见内容的一致性。说明剩余警告，不承诺搜索引擎展示效果。