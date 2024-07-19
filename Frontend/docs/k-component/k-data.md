# [🔙](./index.md) k-data

k-data 用于请求,提交和描述数据,其内部可以嵌套使用`k-data-query`,`k-data-init`,`k-data-item`.

```html
<k-data get="http://myapi.com/products" init-get></k-data>
```

也可以直接书写模拟数据

```html
<k-data>[{id:1,name:"surface"},{id:2,name:"macbook"}]</k-data>
```

## API

| 属性      | 类型   | 可空 | 备注                                                          |
| --------- | ------ | ---- | ------------------------------------------------------------- |
| id        | string | 是   | 如果提供 id 属性,此 data 中的 response 数据将被注册为全局状态 |
| get       | string | 是   | 发起 get 请求的目标地址                                       |
| post      | string | 是   | 发起 post 请求的目标地址                                      |
| init-get  | string | 是   | 在页面初始化的时候调用 get 请求                               |
| init-post | string | 是   | 在页面初始化的时候调用 post 请求                              |
| to        | string | 是   | 在 post 或 get 请求后 页面导航到指定的 url                    |
| from      | string | 是   | 值为已注册的 id,表示将 id 的数据 mapping 到此 data            |
| confirm   | string | 是   | 如果有值,将在 get 或 post 请求发起前弹出确认框                |
| loading   | string | 是   | 如果有值,将在 get 或 post 请求发起前弹出 loading 遮罩         |

## 生命周期

| 阶段       | 可触发事件         |
| ---------- | ------------------ |
| 页面初始化 | init-get,init-post |
| 请求发起前 | confirm            |
| 请求时     | get,post           |
| 请求成功后 | to                 |

# k-data-query

k-data-query 用于在请求时描述 queryString 信息.

```html
<k-data get="http://myapi.com/products" init-get>
  <k-data-query name="category">laptop</k-data-query>
</k-data>
```

## API

| 属性           | 类型                                 | 可空 | 备注                                              |
| -------------- | ------------------------------------ | ---- | ------------------------------------------------- |
| id             | string                               | 是   | 如果提供 id 属性,此 data 的数据将被注册为全局状态 |
| name           | string                               | 否   | queryString item 的 Key                           |
| from           | string                               | 是   | 数据来源名称                                      |
| source         | [DataSource](./types.md#data-source) | 是   |                                                   |
| get-on-change  | boolean                              | 是   | 是否在字段值变化的时候调用 get 请求               |
| post-on-change | boolean                              | 是   | 是否在字段值变化的时候调用 post 请求              |

# k-data-init

k-data-init 用于初始化 data 的 response 数据.

```html
<k-data get="http://myapi.com/products" init-get>
  <k-data-init>[{id:1,name:"surface"},{id:2,name:"macbook"}]</k-data-init>
  <k-data-query name="category">laptop</k-data-query>
</k-data>
```

## API

| 属性 | 类型               | 可空 | 备注                                            |
| ---- | ------------------ | ---- | ----------------------------------------------- |
| type | "json" \| "string" | 是   | 格式化方式,默认为 json 之后可以在考虑其他的格式 |

# k-data-item

k-data-item 有两个职能.

1. 描述请求 response 的 mapping,(不用担心下面例子 response 返回的是 object 还是 array,mapping 的时候将会进行自动判断匹配)

```html
<k-data get="http://myapi.com/products" init-get>
  <k-data-init>[{id:1,name:"surface"},{id:2,name:"macbook"}]</k-data-init>
  <k-data-query name="category">laptop</k-data-query>
  <k-data-item name="key" from="id"></k-data-item>
  <k-data-item name="name"></k-data-item>
</k-data>
```

2. 描述 post 时候的 body

```html
<k-data post="http://myapi.com/create_product">
  <k-data-item name="name"></k-data-item>
  <k-data-item name="category"></k-data-item>
</k-data>
```

## API

| 属性    | 类型                                 | 可空 | 备注                                                                                                                          |
| ------- | ------------------------------------ | ---- | ----------------------------------------------------------------------------------------------------------------------------- |
| name    | string                               | 否   | 字段名                                                                                                                        |
| type    | [DataType](./types.md#data-type)     | 是   | 数据类型,在被 form 或者 table 使用的时候还充当控件类型的职能                                                                  |
| id      | string                               | 是   | 将此字段注册为全局状态                                                                                                        |
| hidden  | boolean                              | 是   | 在被 form 或者 table 使用的时候描述显示或隐藏                                                                                 |
| label   | string                               | 是   | 在被 form 或者 table 使用的时候充当 label,为空时使用 name 作为 label                                                          |
| from    | string                               | 是   | mapping 的时候描述来源字段名                                                                                                  |
| options | string                               | 是   | 此值的类型为全局状态的 id,id 所对应的 data 类型需为`any[]`,或`{value:any,label:string}[]`,如果提供此值则控件类型会变为 select |
| source  | [DataSource](./types.md#data-source) | 是   |                                                                                                                               |
