# k.utils.office

> Excel (`.xls` / `.xlsx`) read/write and export

## Overview

`k.utils.office.excel` and `k.office.excel` both expose the Excel module. It reads spreadsheets from Site files or byte arrays and creates workbooks that can be exported as bytes or written through `k.file`.

Typical flows:

- **Read**: `readAsObjects` / `readAsArrays` → process the data
- **Write**: `createNewWorkbook` → `createSheet` → `fillObjectData` / `fillArrayData` → `readAsBytes` or `k.file.writeBinary`

## TypeScript definition (excerpt)

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
  getExtentionName(): string; // runtime spelling; returns ".xls" or ".xlsx"
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

`getExtentionName` is intentionally spelled this way by the runtime. `getSheet()` returns `null` when no sheet matches; `getSheetAt()` returns `null` for an index beyond the last sheet, but negative indexes are not guarded.

## Reading

### readSheetNames()

```ts
const names = k.utils.office.excel.readSheetNames("data/report.xlsx");
```

### readAsObjects()

The first selected row supplies column names; subsequent rows become objects. `firstRowIndex`, `lastRowIndex`, `firstColumnIndex`, and `lastColumnIndex` are inclusive. If `sheetName` is omitted, the first worksheet is read.

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

Returned rows can be `null` for missing physical rows. Missing physical cells become `null`; blank cells usually become an empty string. Repeated column names are suffixed as `_1`, `_2`, and so on. Numeric cells are returned as numbers, Boolean cells as booleans, dates as `yyyy-MM-dd HH:mm:ss` strings, and formula cells use their cached result rather than recalculating the formula.

The current object reader has a sparse-range defect when `firstColumnIndex > 0` and a selected row contains missing physical cells. Avoid relying on that combination until the runtime is fixed.

### readAsArrays()

Each selected row is returned as an array; the header is not converted into field names. Missing physical rows are `null`, and missing physical cells are `null`.

```ts
const matrix = k.utils.office.excel.readAsArrays("data/report.xlsx", "Sheet1");
```

### Reading from bytes

```ts
const buffer = k.file.readBinary("upload/report.xls");
const names = k.utils.office.excel.readSheetNamesFromBytes(buffer, ".xls");
const rows = k.utils.office.excel.readAsObjectsFromBytes(buffer, ".xls", "Sheet1");
```

`.xls` and `.xlsx` are the supported formats. The runtime chooses the format from the filename or extension string and may retry the other workbook format when the supplied extension does not match the bytes.

### Fine-grained access with getWorkBook()

```ts
const book = k.utils.office.excel.getWorkBook("data/report.xlsx");
const sheet = book.sheets[0];
const rowValue = sheet.rows[1]?.data;
const cellValue = sheet.rows[1]?.cells[0]?.value;
```

`sheet.rows` includes only physical rows; its array index is not necessarily the original worksheet row number when rows are missing.

## Writing and exporting

### createNewWorkbook() + fillObjectData()

`createNewWorkbook()` defaults to `.xls`; pass `.xlsx` explicitly for an XSSF workbook. `fillObjectData` accepts a JSON string. Every row must contain the keys from the first row; a missing first-row key throws, while extra keys in later rows are ignored.

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

For direct HTTP downloads, use the response API described in [Response](../core/response.md); otherwise write the returned bytes with `k.file.writeBinary`.

### Writing to k.file

`fileName` is a File I/O `fullName`, not a Web URL:

```ts
book.write("exports/report.xlsx");
const relativeUrl = k.file.url("exports/report.xlsx");
```

Until the runtime `write()` buffer handling is fixed, prefer `readAsBytes()` followed by `k.file.writeBinary()`; `write()` currently passes the underlying `MemoryStream` buffer instead of its exact used length and may include trailing bytes.

## Notes

1. **File paths** — `read*` and `getWorkBook` use `k.file` `fullName`, not a Web URL.
2. **Empty or missing input** — A missing File I/O path can produce `null` bytes before workbook parsing; validate the input before calling the Excel methods.
3. **Byte output** — `readAsBytes()` returns `number[]`; use it with `k.file.writeBinary` or the response API reference.

## Related modules

- File operations: [File module](../file/index.md)
- Response handling: [Response](../core/response.md)
- Office to HTML: [converter.md](converter.md)
