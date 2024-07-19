# [🔙](./index.md) k-table

k-table 与 k-data 可以搭配出丰富交互的表格

```html
<k-data id="products">[{id:1,name:"surface"},{id:2,name:"macbook"}]</k-data>
<k-table data="products"></k-table>
```

## API

| 属性   | 类型   | 可空 | 备注                             |
| ------ | ------ | ---- | -------------------------------- |
| data   | string | 是   | 列表数据的来源 id                |
| edit   | string | 是   | [ 详见 edit demo](#editdemo)     |
| remove | string | 是   | [ 详见 remove demo](#removedemo) |
| create | string | 是   | [ 详见 create demo](#createdemo) |

<a id="createdemo"></a>

## Create

```html
<k-data id="products">[{id:1,name:"surface"},{id:2,name:"macbook"}]</k-data>
<k-data post="http://myapi.com/create_product" id="create_product">
  <k-data-item name="name"></k-data-item>
</k-data>

<k-table data="products" create="create_product"></k-table>
```

<a id="editdemo"></a>

## Edit

如果 table 中的列可以满足编辑需要可以采用如下方式

```html
<k-data id="products">[{id:1,name:"surface"},{id:2,name:"macbook"}]</k-data>
<k-data post="http://myapi.com/create_product" id="edit_product">
  <k-data-item name="id" hidden></k-data-item>
  <k-data-item name="name"></k-data-item>
</k-data>

<k-table data="products" create="edit_product"></k-table>
```

如果编辑需要而外字段,则建议配合 k-form,导航到新的页面进行编辑

```html
<k-data id="products">[{id:1,name:"surface"},{id:2,name:"macbook"}]</k-data>
<k-data  id="edit_product" to="/edit_product_page.html">
  <k-data-query name="id" ></k-data-item>
</k-data>

<k-table data="products" edit="edit_product"></k-table>
```

<a id="removedemo"></a>

## Remove

```html
<k-data id="products">[{id:1,name:"surface"},{id:2,name:"macbook"}]</k-data>
<k-data post="http://myapi.com/remove_product" id="remove_product">
  <k-data-query name="id" ></k-data-item>
</k-data>

<k-table data="products" remove="remove_product"></k-table>
```

# k-table-action

## API

| 属性    | 类型   | 可空 | 备注                                            |
| ------- | ------ | ---- | ----------------------------------------------- |
| get     | string | 是   | 如果有值,触发给定 id 的 data get 请求           |
| post    | string | 是   | 如果有值,触发给定 id 的 data post 请求          |
| to      | string | 是   | 如果有值,点击事件执行成功之后跳转到指定页面地址 |
| confirm | string | 是   | 如果有值,将在点击事件执行之前弹出确认框         |
| label   | string | 是   | button 的 text                                  |

## 生命周期

| 阶段                 | 可触发事件 |
| -------------------- | ---------- |
| 点击事件执行之前     | confirm    |
| 点击事件             | get,post   |
| 点击事件执行成功之后 | to         |
