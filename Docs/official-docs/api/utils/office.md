# k.utils.office

> Office document processing (Excel)

## Overview

`k.utils.office` currently exposes **`excel`** child objects, read and write `.xls` / `.xlsx` (site files or bytes).

```ts
const names = k.utils.office.excel.readSheetNames("data/report.xlsx")
const rows = k.utils.office.excel.readAsObjects("data/report.xlsx", "Sheet1")
```

## Excel common methods

| method | illustrate |
|------|------|
| `getWorkBook(fileName)` | Open workbook object |
| `getWorkBookFromBytes(buffer, extension)` | open from bytes |
| `createNewWorkbook(extension)` | Create new `.xls` or `.xlsx` |
| `readSheetNames` / `readSheetNamesFromBytes` | List of worksheet names |
| `readAsObjects` / `readAsObjectsFromBytes` | Read as an object array (the first line is used as the field name) |
| `readAsArrays` / `readAsArraysFromBytes` | Read as a two-dimensional array |

| Parameter | Type | Required | Description |
|------|------|------|------|
| `fileName` | `string` | Yes | Site Excel file path |
| `buffer` | `number[]` | Yes | Excel file bytes |
| `extension` | `string` | Yes | Extension, `.xls` or `.xlsx` |
| `sheetName` | `string` | No | Sheet name; reads the first sheet when omitted |
| `range` | `KExcelReadRange` | No | Read range |

| Method | Return |
|------|------|
| `getWorkBook(fileName)` | `KWorkBook` |
| `getWorkBookFromBytes(buffer, extension)` | `KWorkBook` |
| `createNewWorkbook(extension?)` | `KWorkBook` |
| `readSheetNames(fileName)` / `readSheetNamesFromBytes(buffer, extension)` | `string[]` |
| `readAsObjects(...)` / `readAsObjectsFromBytes(...)` | `Record<string, any>[]` |
| `readAsArrays(...)` / `readAsArraysFromBytes(...)` | `any[][]` |

Optional `KExcelReadRange`: `firstRowIndex`, `lastRowIndex`, `firstColumnIndex`, `lastColumnIndex`.

```ts
const names = k.utils.office.excel.readSheetNames("data/report.xlsx")
const rows = k.utils.office.excel.readAsObjects("data/report.xlsx", names[0], {
    firstRowIndex: 0,
    firstColumnIndex: 0,
    lastColumnIndex: 3
})
```

Read from uploaded file bytes:

```ts
const file = k.request.files[0]
const rows = k.utils.office.excel.readAsArraysFromBytes(file.bytes, ".xlsx", "Sheet1")
```

## KWorkBook/KSheet

Workbooks: `sheets`, `createSheet(name)`, `write(fileName)`, `readAsBytes()`.

Worksheets: `getObjectData(range)`, `fillObjectData(json)`, `fillArrayData(arrays)`, `rows`, etc.

| Member / Method | Return | Description |
|-------------|------|------|
| `book.sheets` | `KSheet[]` | Sheet list |
| `book.numberOfSheets` | `number` | Sheet count |
| `book.createSheet(name)` | `KSheet` | Create a sheet |
| `book.write(fileName)` | `void` | Save to a site file |
| `book.readAsBytes()` | `number[]` | Read workbook bytes |
| `sheet.getObjectData(range?)` | `Record<string, any>[]` | Read object rows |
| `sheet.getArrayData(range?)` | `any[][]` | Read array rows |
| `sheet.fillObjectData(json)` | `void` | Fill from object-array JSON |
| `sheet.fillArrayData(arrays)` | `void` | Fill from a two-dimensional array |

```ts
const book = k.utils.office.excel.createNewWorkbook(".xlsx")
const sheet = book.createSheet("Orders")
sheet.fillArrayData([
    ["OrderNo", "Amount"],
    ["SO20260702001", 129.00]
])

book.write("exports/orders.xlsx")
return k.file.get("exports/orders.xlsx")
```

## Related Docs

- [k.utils.converter](./converter.md) — Office to HTML (cloud)
- [k.file](../file/file.md)
