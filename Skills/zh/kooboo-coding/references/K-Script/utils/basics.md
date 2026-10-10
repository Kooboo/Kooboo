# k.utils 基础方法

> `k.utils` 顶层方法 — 阻塞、字节与字符串互转

## 概述

除子模块外，`k.utils` 本身还提供若干直接可调用的方法，常用于脚本内短暂等待与编码转换。

## TypeScript 定义（部分）

```ts
interface KUtils {
  sleep(millisecondsTimeout: number): void;
  bytesToString(bytes: number[], encoding: string): string;
  stringToBytes(content: string, encoding: string): number[];
}
```

## sleep()

阻塞当前线程指定毫秒数。运行时会先将数值转换为 `Int32`，再调用 `Thread.Sleep`；无效、负数、超出范围或特殊数值可能抛出异常。该调用会占用 worker，因此只适用于 CodeBlock 或 API 中的短暂延时。

```ts
k.utils.sleep(1000); // 1 秒
```

## bytesToString()

将字节数组按指定编码解码为字符串。

```ts
const bytes = k.file.readBinary("data.txt"); // number[]
const text = k.utils.bytesToString(bytes, "utf-8");
```

常用编码包括运行时支持的 `utf-8`、`gb2312`、`gbk` 等。编码名称不受支持时，.NET 的编码查找可能抛出异常。

## stringToBytes()

将字符串按指定编码转为字节数组。

```ts
const bytes = k.utils.stringToBytes("你好", "utf-8");
k.file.writeBinary("out.txt", bytes);
```

## 相关模块

- 文件读写：[File 模块](../file/index.md)
- 网络工具：[Net 模块](../net/index.md)
- 字符串处理：[string-date-uri.md](string-date-uri.md) 中的 `k.utils.string`
