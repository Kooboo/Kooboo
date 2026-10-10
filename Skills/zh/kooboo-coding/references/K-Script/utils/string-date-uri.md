# k.utils.string / date / uri / mime

> 字符串、日期时间、URI 与 MIME 工具

四个子模块分别是 `string`、`date`、`uri`、`mime`，均通过 `k.utils.*` 访问。使用前先阅读对应的边界说明。

## k.utils.string

### TypeScript 定义

```ts
interface KText {
  removeHtml(input: string): string;
  sementicSubString(input: string, start: number, count: number): string | null;
  subString(input: string, start: number, count: number): string;
  replace(input: string, oldValue: string, newValue: string): string;
  remove(input: string, valueToRemove: string): string;
}
```

### removeHtml()

通过搜索工具移除 HTML 标记、解码 HTML 实体并规范文本空白。KScript 包装层没有启用工具的可选 script/style 排除标志，因此不要假设 `<script>` 或 `<style>` 内容一定会被移除。

```ts
const plain = k.utils.string.removeHtml("<p>Hello <b>World</b></p>");
```

### subString()

从 `start` 开始返回 `count` 个字符；负数 `start` 按 `0` 处理。输入为空或为 `null` 时原样返回。其他无效范围直接交给运行时的 substring 操作，可能抛出异常；方法不会自动截断 `count` 或超出范围的 `start`。

```ts
k.utils.string.subString("Kooboo CMS", 0, 6); // "Kooboo"
```

### sementicSubString()

方法名中的拼写错误是运行时固定名称。它会跳过 `start` 处的前导空白，在语义边界截取，并可能为了保持 ASCII 字母数字单词完整而超过 `count`。当 `start` 超出输入范围或剩余内容全为空白时返回 `null`；`count` 不是严格上限。

```ts
const summary = k.utils.string.sementicSubString("Kooboo 内容管理系统", 0, 10);
if (summary !== null) {
  // 使用 summary
}
```

### replace() / remove()

两者都忽略大小写，且只处理第一个匹配的子串。输入或目标为空时返回原输入；它们不是全局替换。

```ts
k.utils.string.replace("Hello World World", "world", "Kooboo"); // "Hello Kooboo World"
k.utils.string.remove("Hello World World", "world"); // "Hello  World"
```

## k.utils.date

```ts
interface KDate {
  utcNow: Date;
  now: Date;
  formate(dateTime: Date, formate?: string): string;
  addSeconds(dateTime: Date, seconds: number): Date;
  addMinutes(dateTime: Date, minutes: number): Date;
  addHours(dateTime: Date, hours: number): Date;
  addDays(dateTime: Date, days: number): Date;
  addMonths(dateTime: Date, months: number): Date;
  addYears(dateTime: Date, years: number): Date;
}
```

`formate` 是运行时拼写。`now` 使用服务器本地时间，`utcNow` 使用 UTC。省略格式时，`formate()` 使用 `yyyy-MM-dd HH:mm:ss`；自定义格式按服务器进程当前文化格式化。

```ts
const local = k.utils.date.now;
const utc = k.utils.date.utcNow;
const text = k.utils.date.formate(local, "yyyy/MM/dd");
const tomorrow = k.utils.date.addDays(local, 1);
```

## k.utils.uri

通过 `UriBuilder` 解析有效 URI 并读取或修改 query；这些方法不会发起 HTTP 请求。除非另有运行时验证，否则应使用绝对 URI。

```ts
interface KUri {
  parse(uri: string): KUriBuilder | null;
  setQueryParam(uri: string, name: string, value: string): string | null;
  getQueryParam(uri: string, name: string): string | null;
}

interface KUriBuilder {
  scheme: string;
  userName: string;
  password: string;
  host: string;
  port: number;
  path: string;
  query: string;
  setQueryParam(name: string, value: string): void;
  getQueryParam(name: string): string | null;
  toUri(): string;
}
```

空或空白输入返回 `null`。非空但无效的 URI 会传入 `UriBuilder`，可能抛出异常。参数不存在时 `getQueryParam()` 返回 `null`。query 更新使用 `HttpUtility.ParseQueryString`，可能解码并重新编码 query；未经测试不要假设重复键或 `null` 值的删除语义。

```ts
const builder = k.utils.uri.parse("https://www.kooboo.com/path?a=1&b=2");
if (builder) {
  builder.setQueryParam("c", "3");
  const url = builder.toUri();
}

const withPage = k.utils.uri.setQueryParam("https://www.kooboo.com/list?page=1", "page", "2");
const page = k.utils.uri.getQueryParam("https://www.kooboo.com/list?page=2", "page"); // "2"
```

## k.utils.mime

```ts
interface KMime {
  getMimeMapping(fileName: string): string;
  getExtensions(mimeType: string): string[];
}
```

根据文件名查询 MIME 类型，或根据 MIME 类型查询扩展名。未知输入的行为由底层 MIME 映射库决定；不要假设所有未知扩展名都会得到非空结果。

```ts
k.utils.mime.getMimeMapping("photo.jpg"); // image/jpeg
k.utils.mime.getExtensions("image/png"); // ["png", ...]
```

## 相关模块

- HTTP 请求：[k.request](../core/request.md)
- 安全哈希：[k.security](../core/security.md)
