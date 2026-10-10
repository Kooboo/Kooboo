# k.utils.compression

> Gzip decompression and Zip archive creation/extraction

## Overview

`k.utils.compression` handles gzip text decompression and zip operations in memory or at `k.file` paths.

## TypeScript definition

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

The decompression result is a UTF-8 string. `decompressGzip(path)` requires an existing File I/O file; a missing path or invalid gzip bytes fail rather than returning an empty string.

## zip() (in-memory entries)

```ts
const zipped = k.utils.compression.zip([
  { name: "a.txt", binary: k.utils.stringToBytes("hello", "utf-8") },
  { name: "b.txt", binary: k.utils.stringToBytes("world", "utf-8") },
]);
k.file.writeBinary("out/archive.zip", zipped);
```

## zip() (folder)

Pack an existing directory under `k.file` into a zip file (the parameters are fullNames). Place `zipPath` outside the source directory: the runtime creates the output file before enumerating source files. Empty directories are not preserved.

```ts
k.utils.compression.zip("source-folder", "archives/backup.zip");
```

## unzip() (binary)

```ts
const zipBytes = k.file.readBinary("archives/backup.zip");
const items = k.utils.compression.unzip(zipBytes);
return items?.map(item => ({
  name: item.name,
  fullName: item.fullName,
  size: item.binary.length
}));
```

`name` is the leaf filename; `fullName` retains the entry's directory path. Directory entries can have empty names or bytes. Before writing an entry to File I/O, validate its `fullName` as a relative path contained within the intended destination, reject absolute paths and `..` traversal, and skip directories. Do not concatenate untrusted archive paths directly into a write path.

## unzip() (path)

Extract a zip into the target folder (both arguments are `k.file` fullNames). Use this path overload only for trusted archives: the current Kooboo code does not show a destination-containment check for archive entry paths.

```ts
k.utils.compression.unzip("archives/backup.zip", "extracted");
```

## Notes

1. **Paths** — File-system methods use **fullName**; after writing, use `k.file.url()` to access the URL. See the [File module](../file/index.md).
2. **Zip entries** — In in-memory `zip(items)`, `name` is the relative path inside the zip.
3. **Null and size boundaries** — `zip(null)` and `unzip(null)` return `null`. In-memory zip/unzip buffer entire archives; keep inputs bounded, and do not assume invalid members or corrupted archives return a clean empty result.
