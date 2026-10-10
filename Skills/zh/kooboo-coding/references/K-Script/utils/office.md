# k.utils.office

> Excel（`.xls` / `.xlsx`）读写与导出

## 概述

`k.utils.office.excel` 和 `k.office.excel` 都暴露 Excel 模块。它支持从站点文件或字节数组读取表格，以及创建工作簿后导出为字节或写入 `k.file`。

典型流程：

- **读取**：`readAsObjects` / `readAsArrays` → 处理数据
- **写入**：`createNewWorkbook` → `createSheet` → `fillObjectData` / `fillArrayData` → `readAsBytes` 或 `k.file.writeBinary`

## TypeScript 定义（节选）

```ts
interface Office {
  excel: Excel;
}

interface Excel {
  readSheetNames(fileName: string): string[];
  readSheetNamesFromBytes(buffer: number[], extensionName: string): string[];
  readAsObjects(fileName: string, sheetName?: string, range?: KExcelReadRange): (Record<string, any> | null)[];
  readAsObjectsFromBytes(buffer: number[], extensionName: string, sheetName?: string, range?: KExcelReadRange): (Record<string, any> | null)[];
  readAsArrays(fileName: string, sheetName?: string, range?: KExcelReadRange): (any[] | null)[];
  readAsArraysFromBytes(buffer: number[], extensionName: string, sheetName?: string, range?: KExcelReadRange): (any[] | null)[];
  getWorkBook(fileName: string): KWorkBook;
  getWorkBookFromBytes(buffer: number[], extensionName: string): KWorkBook;
  createNewWorkbook(extensionName?: string): KWorkBook;
}

interface KExcelReadRange {
  firstRowIndex?: number;
  lastRowIndex?: number;
  firstColumnIndex?: number;
  lastColumnIndex?: number;
}

interface KWorkBook {
  sheets: KSheet[];
  numberOfSheets: number;
  createSheet(sheetName: string): KSheet;
  getSheetAt(index: number): KSheet | null;
  getSheet(sheetName: string): KSheet | null;
  getSheetIndex(sheetName: string): number;
  readAsBytes(): number[];
  write(fileName: string): void;
  getExtentionName(): string; // 运行时固定拼写；返回 ".xls" 或 ".xlsx"
}

interface KSheet {
  name: string;
  firstRowNum: number;
  lastRowNum: number;
  rows: KExcelRow[];
  getObjectData(range?: KExcelReadRange): (Record<string, any> | null)[];
  getArrayData(range?: KExcelReadRange): (any[] | null)[];
  fillObjectData(jsonObject: string): void;
  fillArrayData(arrays: any[][]): void;
}

interface KExcelRow {
  firstCellNum: number;
  lastCellNum: number;
  physicalNumberOfCells: number;
  cells: (KExcelCell | null)[];
  data: any[];
}

interface KExcelCell {
  value: any;
  type: string;
}
```

`getExtentionName` 是运行时固定的拼写。没有匹配工作表时 `getSheet()` 返回 `null`；索引超过最后一张表时 `getSheetAt()` 返回 `null`，但负索引未受到保护。

## 读取

### readSheetNames()

```ts
const names = k.utils.office.excel.readSheetNames("data/report.xlsx");
```

### readAsObjects()

选中范围的首行作为列名，后续行转换为对象。`firstRowIndex`、`lastRowIndex`、`firstColumnIndex`、`lastColumnIndex` 都是包含边界；省略 `sheetName` 时读取第一个工作表。

```ts
const rows = k.utils.office.excel.readAsObjects("data/report.xlsx", "Sheet1");
// [{ Id: 1, Name: "A" }, ...]

const partial = k.utils.office.excel.readAsObjects("data/report.xlsx", "Sheet1", {
  firstColumnIndex: 0,
  lastColumnIndex: 3,
  firstRowIndex: 0,
  lastRowIndex: 10,
});
```

缺少物理行时，返回结果可能包含 `null` 行；缺少物理单元格时返回 `null`，空单元格通常转换为空字符串。重复列名会追加 `_1`、`_2` 等后缀。数字单元格返回数字，布尔单元格返回布尔值，日期返回 `yyyy-MM-dd HH:mm:ss` 字符串，公式单元格使用缓存结果，不会重新计算公式。

当前对象读取器在 `firstColumnIndex > 0` 且选中行包含缺少物理单元格时存在索引缺陷；运行时修复前不要依赖这一组合。

### readAsArrays()

每个选中行返回一个数组，不会把表头解析为字段名。缺少物理行时返回 `null`，缺少物理单元格时返回 `null`。

```ts
const matrix = k.utils.office.excel.readAsArrays("data/report.xlsx", "Sheet1");
```

### 从字节读取

```ts
const buffer = k.file.readBinary("upload/report.xls");
const names = k.utils.office.excel.readSheetNamesFromBytes(buffer, ".xls");
const rows = k.utils.office.excel.readAsObjectsFromBytes(buffer, ".xls", "Sheet1");
```

支持 `.xls` 和 `.xlsx`。运行时根据文件名或扩展名字符串选择格式；当扩展名与字节内容不匹配时，可能尝试另一种工作簿格式。

### getWorkBook() 细粒度访问

```ts
const book = k.utils.office.excel.getWorkBook("data/report.xlsx");
const sheet = book.sheets[0];
const rowValue = sheet.rows[1]?.data;
const cellValue = sheet.rows[1]?.cells[0]?.value;
```

`sheet.rows` 只包含物理存在的行；如果工作表中有缺失行，数组下标不一定等于原工作表的行号。

## 写入与导出

### createNewWorkbook() + fillObjectData()

`createNewWorkbook()` 默认创建 `.xls`；需要 XSSF 工作簿时显式传入 `.xlsx`。`fillObjectData` 接收 JSON 字符串。每行都必须包含首行的字段；缺少首行字段会抛错，后续行的额外字段会被忽略。

```ts
const book = k.utils.office.excel.createNewWorkbook(".xlsx");

const objects = [
  { Id: "1", Name: "My Name", Age: 12 },
  { Id: "2", Name: "His Name", Age: 22 },
];

book.createSheet("Sheet1").fillObjectData(JSON.stringify(objects));
const buffer = book.readAsBytes();
k.file.writeBinary("exports/report.xlsx", buffer);
```

### fillArrayData()

```ts
const book = k.utils.office.excel.createNewWorkbook(".xlsx");
const arrays = [
  ["Id", "Name", "Age"],
  ["1", "test", 3],
  ["2", "test2", 33],
];
book.createSheet("Sheet1").fillArrayData(arrays);
const buffer = book.readAsBytes();
```

如需直接下载，请按[Response](../core/response.md)中的响应 API 使用；否则用 `k.file.writeBinary` 写入返回的字节。

### 写入 k.file

`fileName` 是 File I/O 的 `fullName`，不是 Web URL：

```ts
book.write("exports/report.xlsx");
const relativeUrl = k.file.url("exports/report.xlsx");
```

在运行时修复 `write()` 的缓冲区处理前，建议优先使用 `readAsBytes()` 再调用 `k.file.writeBinary()`；当前 `write()` 传递的是 `MemoryStream` 的底层缓冲区而非实际使用长度，可能包含尾部多余字节。

## 注意事项

1. **文件路径** — `read*` 和 `getWorkBook` 使用 `k.file` 的 `fullName`，不是 Web URL。
2. **空或缺失输入** — File I/O 路径不存在时，解析前可能得到 `null` 字节；调用 Excel 方法前应先校验输入。
3. **字节输出** — `readAsBytes()` 返回 `number[]`；可配合 `k.file.writeBinary` 或响应 API reference 使用。

## 相关模块

- 文件操作：[File 模块](../file/index.md)
- 响应处理：[Response](../core/response.md)
- Office 转 HTML：[converter.md](converter.md)
