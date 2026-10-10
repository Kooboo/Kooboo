# k.utils.compression

> Gzip 解压与 Zip 打包/解包

## 概述

`k.utils.compression` 处理 gzip 文本解压，以及内存或 `k.file` 路径上的 zip 操作。

## TypeScript 定义

```ts
interface Compression {
  decompressGzip(path: string): string;
  decompressGzipBinary(bytes: number[]): string;
  zip(items: Entry[] | null): number[] | null;
  zip(folder: string, zipPath: string): void;
  unzip(binary: number[] | null): ZipItem[] | null;
  unzip(zipPath: string, folder: string): void;
}

interface Entry {
  name: string;
  binary: number[];
}

interface ZipItem {
  name: string;
  fullName: string;
  binary: number[];
  getString(): string;
}
```

## decompressGzip() / decompressGzipBinary()

```ts
const text = k.utils.compression.decompressGzip("archive/data.gz");

const bytes = k.file.readBinary("archive/data.gz");
const text2 = k.utils.compression.decompressGzipBinary(bytes);
```

解压结果为 UTF-8 字符串。`decompressGzip(path)` 要求 File I/O 文件存在；路径不存在或 gzip 字节无效时会失败，不会返回空字符串。

## zip()（内存条目）

```ts
const zipped = k.utils.compression.zip([
  { name: "a.txt", binary: k.utils.stringToBytes("hello", "utf-8") },
  { name: "b.txt", binary: k.utils.stringToBytes("world", "utf-8") },
]);
k.file.writeBinary("out/archive.zip", zipped);
```

## zip()（文件夹）

将 `k.file` 下已存在的目录打包为 zip 文件（参数为 fullName）。`zipPath` 应放在源目录之外：运行时先创建输出文件，再枚举源文件。空目录不会被保留。

```ts
k.utils.compression.zip("source-folder", "archives/backup.zip");
```

## unzip()（二进制）

```ts
const zipBytes = k.file.readBinary("archives/backup.zip");
const items = k.utils.compression.unzip(zipBytes);
return items?.map(item => ({
  name: item.name,
  fullName: item.fullName,
  size: item.binary.length
}));
```

`name` 是末尾文件名，`fullName` 保留条目的目录路径。目录条目可能有空名称或空字节。写入 File I/O 前，应校验 `fullName` 是限制在预期目标目录内的相对路径，拒绝绝对路径和 `..` 路径穿越，并跳过目录。不要直接拼接不可信的归档路径作为写入路径。

## unzip()（路径）

将 zip 解压到目标目录（两个参数均为 `k.file` fullName）。此路径重载仅用于可信归档：当前 Kooboo 代码中没有看到对归档条目的目标路径 containment 校验。

```ts
k.utils.compression.unzip("archives/backup.zip", "extracted");
```

## 注意事项

1. **路径** — 涉及文件系统的方法使用 **fullName**；写入后访问 URL 须用 `k.file.url()`，见 [File 模块](../file/index.md)。
2. **zip 条目** — 内存 `zip(items)` 中 `name` 为 zip 内相对路径。
3. **空值与大小边界** — `zip(null)` 与 `unzip(null)` 返回 `null`。内存打包/解包会缓冲整个归档，应限制输入大小；不要假设无效条目或损坏归档能得到正常的空结果。
