# k.utils.xml / k.utils.dom

> XML 文档构建与 HTML DOM 解析

## k.utils.xml

解析或创建 XML，序列化为字符串。

### TypeScript 定义（节选）

```ts
interface Xml {
  parse(xml: string): Document;
  create(rootName: string): Document;
}

interface Document {
  declaration: Declaration;
  root: Element;
  stringify(): string;
}

interface Declaration {
  version: string;
  encoding: string;
  standalone: string;
}

interface Element {
  name: string;
  value: string | null;
  children: Element[];
  attributes: Attribute[];
  addAttribute(name: string, value: string): Attribute;
  addChild(name: string): Element;
  addCData(content: string): void;
  removeAttribute(name: string): void;
  removeChild(name: string): void;
}

interface Attribute {
  name: string;
  value: string;
}
```

`parse()` 会在源 XML 包含声明时暴露 XML 声明。对于不带 XML 声明但仍然合法的 XML，`declaration` 仍会暴露包装对象，但底层声明缺失，读取 `version`、`encoding` 或 `standalone` 可能抛出异常。元素包含子元素时，`Element.value` 返回 `null`；设置它会替换元素现有的全部内容，包括子元素和 CDATA。

### parse()

```ts
const doc = k.utils.xml.parse("<root><item>1</item></root>");
const xml = doc.stringify();
```

### create()

```ts
const doc = k.utils.xml.create("root");
const child = doc.root.addChild("item");
child.addAttribute("id", "1");
child.value = "Text Value";
const xmlString = doc.stringify();
```

命名空间属性示例（见源码 CodeDescription）：

```ts
doc.root.addAttribute("{http://www.w3.org/2000/xmlns/}xlink", "http://www.kooboo.com/xlink");
doc.root.addAttribute("{http://www.kooboo.com/xlink}href", "http://www.kooboo.cn");
```

创建带命名空间的属性时可以使用 `{namespace}localName` 形式，但 `Element.name` 和 `Attribute.name` 只返回本地名称。`removeChild(name)` 只删除第一个匹配名称的直接子元素。

---

## k.utils.dom

解析 HTML、提取纯文本或枚举图片。

### TypeScript 定义

```ts
interface DocumentObjectModel {
  document: Kooboo.Dom.Document;
  hasImage: boolean;
  images: DomImage[];
  parse(html: string): DocumentObjectModel;
  getText(html: string): string;
}

interface DomImage {
  src: string;
  alt: string;
}
```

`hasImage` 将所有非空 `src` 视为图片，而 `images` 会排除空字符串或仅包含空白的 `src`；不要把两者当作完全相同的筛选结果。

### parse()

```ts
const dom = k.utils.dom.parse("<html><body><img src='/a.jpg' alt='x'/></body></html>");
const hasImg = dom.hasImage;
const imgs = dom.images; // [{ src, alt }, ...]
```

### getText()

去除 HTML 标签得到纯文本（不修改原文 DOM）。运行时还会解码 HTML 实体、移除 `script` 与 `style` 内容，并规范化空白。

```ts
const text = k.utils.dom.getText("<p>Hello <b>World</b></p>");
```

## 相关模块

- 字符串去 HTML：[string-date-uri.md](string-date-uri.md) 中的 `k.utils.string.removeHtml`
