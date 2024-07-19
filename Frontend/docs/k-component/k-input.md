# [🔙](./index.md) k-input

k-input 作为统一的数据输入控件

```html
<k-data>
  <k-data-query name="name"></k-data-query>
  <k-data-query name="age"></k-data-query>
</k-data>
<k-input data="name"></k-input>
<k-input data="age" type="number"></k-input>
```

## API

| 属性    | 类型                             | 可空 | 备注                                                                                                                          |
| ------- | -------------------------------- | ---- | ----------------------------------------------------------------------------------------------------------------------------- |
| type    | [DataType](./types.md#datat-ype) | 是   |                                                                                                                               |
| data    | string                           | 是   | 绑定的 data id                                                                                                                |
| options | string                           | 是   | 此值的类型为全局状态的 id,id 所对应的 data 类型需为`any[]`,或`{value:any,label:string}[]`,如果提供此值则控件类型会变为 select |
