# [🔙](./index.md) k-form

k-page 可以根据 k-data 中的描述生成表单,点击提交按钮将调用 k-data 中的 post 方法

新建记录表单

```html
<k-data post="http://myapi.com/create_product" id="create_product">
  <k-data-item name="name" label="名称"></k-data-item>
  <k-data-item name="count" type="number" label="数量"></k-data-item>
</k-data>
<k-form data="create_product"></k-form>
```

编辑记录表单

```html
<k-data
  post="http://myapi.com/create_product"
  get="http://myapi.com/product"
  id="create_product"
  init-get
>
  <k-data-query name="id">1</k-data-query>
  <k-data-item name="id" hidden></k-data-item>
  <k-data-item name="name" label="名称"></k-data-item>
  <k-data-item name="count" type="number" label="数量"></k-data-item>
</k-data>
<k-form data="create_product"></k-form>
```

## API

| 属性 | 类型   | 可空 | 备注                                                |
| ---- | ------ | ---- | --------------------------------------------------- |
| data | string | 是   | 绑定的 data id                                      |
| to   | string | 是   | 提交表单成功后后跳转的 url,如果为空则会刷新当前页面 |
