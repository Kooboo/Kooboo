# k.utils.office

> Office 文档处理（Excel）

## 概述

`k.utils.office` 当前暴露 **`excel`** 子对象，读写 `.xls` / `.xlsx`（站点文件或字节）。

```ts
const names = k.utils.office.excel.readSheetNames("data/report.xlsx")
const rows = k.utils.office.excel.readAsObjects("data/report.xlsx", "Sheet1")
```

## excel 常用方法

| 方法 | 说明 |
|------|------|
| `getWorkBook(fileName)` | 打开工作簿对象 |
| `getWorkBookFromBytes(buffer, extension)` | 从字节打开 |
| `createNewWorkbook(extension)` | 新建 `.xls` 或 `.xlsx` |
| `readSheetNames` / `readSheetNamesFromBytes` | 工作表名称列表 |
| `readAsObjects` / `readAsObjectsFromBytes` | 读为对象数组（首行作字段名） |
| `readAsArrays` / `readAsArraysFromBytes` | 读为二维数组 |

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `fileName` | `string` | 是 | 站点 Excel 文件路径 |
| `buffer` | `number[]` | 是 | Excel 文件字节 |
| `extension` | `string` | 是 | 扩展名，`.xls` 或 `.xlsx` |
| `sheetName` | `string` | 否 | 工作表名称；不传时读取第一个工作表 |
| `range` | `KExcelReadRange` | 否 | 读取范围 |

| 方法 | 返回 |
|------|------|
| `getWorkBook(fileName)` | `KWorkBook` |
| `getWorkBookFromBytes(buffer, extension)` | `KWorkBook` |
| `createNewWorkbook(extension?)` | `KWorkBook` |
| `readSheetNames(fileName)` / `readSheetNamesFromBytes(buffer, extension)` | `string[]` |
| `readAsObjects(...)` / `readAsObjectsFromBytes(...)` | `Record<string, any>[]` |
| `readAsArrays(...)` / `readAsArraysFromBytes(...)` | `any[][]` |

可选 `KExcelReadRange`：`firstRowIndex`、`lastRowIndex`、`firstColumnIndex`、`lastColumnIndex`。

```ts
const names = k.utils.office.excel.readSheetNames("data/report.xlsx")
const rows = k.utils.office.excel.readAsObjects("data/report.xlsx", names[0], {
    firstRowIndex: 0,
    firstColumnIndex: 0,
    lastColumnIndex: 3
})
```

从上传文件字节读取：

```ts
const file = k.request.files[0]
const rows = k.utils.office.excel.readAsArraysFromBytes(file.bytes, ".xlsx", "Sheet1")
```

## KWorkBook / KSheet

工作簿：`sheets`、`createSheet(name)`、`write(fileName)`、`readAsBytes()`。

工作表：`getObjectData(range)`、`fillObjectData(json)`、`fillArrayData(arrays)`、`rows` 等。

| 成员 / 方法 | 返回 | 说明 |
|-------------|------|------|
| `book.sheets` | `KSheet[]` | 工作表列表 |
| `book.numberOfSheets` | `number` | 工作表数量 |
| `book.createSheet(name)` | `KSheet` | 新建工作表 |
| `book.write(fileName)` | `void` | 保存到站点文件 |
| `book.readAsBytes()` | `number[]` | 读取工作簿字节 |
| `sheet.getObjectData(range?)` | `Record<string, any>[]` | 读取对象数组 |
| `sheet.getArrayData(range?)` | `any[][]` | 读取二维数组 |
| `sheet.fillObjectData(json)` | `void` | 用对象数组 JSON 填充 |
| `sheet.fillArrayData(arrays)` | `void` | 用二维数组填充 |

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

## 相关文档

- [k.utils.converter](./converter.md) — Office 转 HTML（云端）
- [k.file](../file/file.md)
