# k.utils Basic Methods

> Top-level `k.utils` methods — sleeping and converting between bytes and strings

## Overview

In addition to its submodules, `k.utils` provides directly callable methods for short waits and encoding conversion.

## TypeScript definition (partial)

```ts
interface KUtils {
  sleep(millisecondsTimeout: number): void;
  bytesToString(bytes: number[], encoding: string): string;
  stringToBytes(content: string, encoding: string): number[];
}
```

## sleep()

Block the current thread for the specified number of milliseconds. The runtime first converts the numeric argument to `Int32` and then calls `Thread.Sleep`; invalid, negative, out-of-range, or special numeric values may throw. The call occupies a worker, so use it only for short delays in a CodeBlock or API.

```ts
k.utils.sleep(1000); // 1 second
```

## bytesToString()

Decode a byte array into a string using the specified encoding.

```ts
const bytes = k.file.readBinary("data.txt"); // number[]
const text = k.utils.bytesToString(bytes, "utf-8");
```

Common encodings include `utf-8`, `gb2312`, and `gbk` when supported by the runtime. An unsupported encoding name may throw from .NET's encoding lookup.

## stringToBytes()

Convert a string into a byte array using the specified encoding.

```ts
const bytes = k.utils.stringToBytes("你好", "utf-8");
k.file.writeBinary("out.txt", bytes);
```

## Related modules

- File I/O: [File module](../file/index.md)
- Network utilities: [Net module](../net/index.md)
- String handling: `k.utils.string` in [string-date-uri.md](string-date-uri.md)
