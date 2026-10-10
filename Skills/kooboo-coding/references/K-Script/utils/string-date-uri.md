# k.utils.string / date / uri / mime

> String, date/time, URI, and MIME utilities

The four submodules are `string`, `date`, `uri`, and `mime`, all under `k.utils.*`. Read the relevant boundary below before using an operation.

## k.utils.string

### TypeScript definition

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

Removes HTML markup through the search utility, decodes HTML entities, and normalizes text spacing. The KScript wrapper does not enable the utility's optional script/style exclusion flag, so do not assume `<script>` or `<style>` content is always removed.

```ts
const plain = k.utils.string.removeHtml("<p>Hello <b>World</b></p>");
```

### subString()

Returns `count` characters starting at `start`; negative `start` is treated as `0`. Empty or `null` input is returned unchanged. Other invalid ranges are passed to the runtime substring operation and may throw; the method does not clamp `count` or an out-of-range `start`.

```ts
k.utils.string.subString("Kooboo CMS", 0, 6); // "Kooboo"
```

### sementicSubString()

The misspelled method name is fixed by the runtime. It skips leading whitespace at `start`, stops at a semantic boundary, and may extend beyond `count` to keep ASCII alphanumeric words together. It returns `null` when `start` is outside the input or the remaining content is only whitespace; `count` is not a strict maximum.

```ts
const summary = k.utils.string.sementicSubString("Kooboo 内容管理系统", 0, 10);
if (summary !== null) {
  // use summary
}
```

### replace() / remove()

Both operations are case-insensitive and affect only the first matching substring. If the input or target is empty, the original input is returned; they are not global replacements.

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

`formate` is the runtime spelling. `now` uses the server's local time; `utcNow` uses UTC. If the format is omitted, `formate()` uses `yyyy-MM-dd HH:mm:ss`; custom formats use the server process's current culture.

```ts
const local = k.utils.date.now;
const utc = k.utils.date.utcNow;
const text = k.utils.date.formate(local, "yyyy/MM/dd");
const tomorrow = k.utils.date.addDays(local, 1);
```

## k.utils.uri

Parse a valid URI and read or update its query through `UriBuilder`; these methods do not make an HTTP request. Use absolute URIs unless the runtime has been separately verified to support a relative URI.

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

Empty or whitespace input returns `null`. A non-empty invalid URI is passed to `UriBuilder` and may throw. `getQueryParam()` returns `null` when the parameter is absent. Query updates use `HttpUtility.ParseQueryString` and may decode/re-encode the query; do not assume duplicate-key or `null`-value deletion semantics without testing.

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

Look up a MIME type from a filename or extensions from a MIME type. Unknown-input behavior is delegated to the underlying MIME mapping library; do not assume every unknown extension has a non-empty result.

```ts
k.utils.mime.getMimeMapping("photo.jpg"); // image/jpeg
k.utils.mime.getExtensions("image/png"); // ["png", ...]
```

## Related modules

- HTTP requests: [k.request](../core/request.md)
- Secure hashing: [k.security](../core/security.md)
