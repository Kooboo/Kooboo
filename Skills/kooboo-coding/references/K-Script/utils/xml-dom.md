# k.utils.xml / k.utils.dom

> XML document construction and HTML DOM parsing

## k.utils.xml

Parse or create XML and serialize it as a string.

### TypeScript definition (excerpt)

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

`parse()` exposes the XML declaration when the source contains one. For valid XML without an XML declaration, `declaration` is still exposed as a wrapper, but reading its `version`, `encoding`, or `standalone` can throw because the underlying declaration is missing. `Element.value` returns `null` when the element has child elements. Setting it replaces the element's existing content, including child elements and CDATA.

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

Namespace-attribute example (see the source CodeDescription):

```ts
doc.root.addAttribute("{http://www.w3.org/2000/xmlns/}xlink", "http://www.kooboo.com/xlink");
doc.root.addAttribute("{http://www.kooboo.com/xlink}href", "http://www.kooboo.cn");
```

Names in `{namespace}localName` form are accepted when creating namespaced attributes, but `name` on an `Element` or `Attribute` returns only the local name. `removeChild(name)` removes only the first direct child with that name.

---

## k.utils.dom

Parse HTML, extract plain text, or enumerate images.

### TypeScript definition

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

`hasImage` treats any non-empty `src` as an image, while `images` excludes empty or whitespace-only `src` values. Do not use them as identical filters.

### parse()

```ts
const dom = k.utils.dom.parse("<html><body><img src='/a.jpg' alt='x'/></body></html>");
const hasImg = dom.hasImage;
const imgs = dom.images; // [{ src, alt }, ...]
```

### getText()

Remove HTML tags to obtain plain text (without modifying the source DOM). The runtime also decodes HTML entities, removes `script` and `style` blocks, and normalizes whitespace.

```ts
const text = k.utils.dom.getText("<p>Hello <b>World</b></p>");
```

## Related modules

- Remove HTML from strings: `k.utils.string.removeHtml` in [string-date-uri.md](string-date-uri.md)
