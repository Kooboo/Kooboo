# [🔙](./index.md) k-button

k-button 有触发 data 事件或导航的作用.

```html
<k-page title="Page demo">
  <k-data get="http://myapi.com/products" id="products"></k-data>
  <k-button get="products">Get product</k-button>
  <k-button
    confirm="Do you want go to create product page?"
    to="/create_product_page.html"
    >Go to create product page</k-button
  >
</k-page>
```

## API

| 属性    | 类型   | 可空 | 备注                                   |
| ------- | ------ | ---- | -------------------------------------- |
| get     | string | 是   | 如果有值,触发给定 id 的 data get 请求           |
| post    | string | 是   | 如果有值,触发给定 id 的 data post 请求          |
| to      | string | 是   | 如果有值,点击事件执行成功之后跳转到指定页面地址 |
| confirm | string | 是   | 如果有值,将在点击事件执行之前弹出确认框          |

## 生命周期

| 阶段                 | 可触发事件 |
| -------------------- | ---------- |
| 点击事件执行之前     | confirm    |
| 点击事件             | get,post   |
| 点击事件执行成功之后 | to         |
