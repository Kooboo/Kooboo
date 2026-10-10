# AForm

Ant Design Vue 4.2.6; native export `Form`. Import `AForm` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Use :model, FormItem name and rules; native validate/validateFields/resetFields are instance methods. @finish runs after validation; @finishFailed handles invalid submissions. Async request state and server validation belong to business code.

## Props and defaults

- `colon`: boolean; default true. change default props colon value of Form.Item (only effective when prop layout is horizontal)
- `disabled`: boolean; default false. Set form component disable, only available for antdv components
- `hideRequiredMark`: Boolean; default false. Hide required mark of all form items
- `labelAlign`: 'left' | 'right'; default 'right'. text align of label of all items
- `labelCol`: [object](/components/grid/#col); default unspecified. The layout of label. You can set `span` `offset` to something like `{span: 3, offset: 12}` or `sm: {span: 3, offset: 12}` same as with `<Col>`
- `labelWrap`: boolean; default false. whether label can be wrap
- `layout`: 'horizontal'|'vertical'|'inline'; default 'horizontal'. Define form layout
- `model`: object; default unspecified. data of form component
- `name`: string; default -. Form name. Will be the prefix of Field `id`
- `noStyle`: boolean; default false. No style for `true`, used as a pure field control
- `rules`: object; default unspecified. validation rules of form
- `scrollToFirstError`: boolean | [options](https://github.com/stipsan/scroll-into-view-if-needed/#options); default false. Auto scroll to first failed field when submit
- `validateOnRuleChange`: boolean; default true. whether to trigger validation when the `rules` prop is changed
- `validateTrigger`: string | string[]; default `change`. Config field validate trigger
- `wrapperCol`: [object](/components/grid/#col); default unspecified. The layout for input controls, same as `labelCol`

## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AForm :model="form" layout="vertical" @finish="event = '验证通过：' + form.name"><AFormItem label="姓名" name="name" :rules="[{required:true,message:'请输入姓名'}]"><AInput v-model:value="form.name"/></AFormItem><AButton type="primary" html-type="submit">验证</AButton></AForm><p role="status">{{ event }}</p>
```

Source: pinned upstream es/components.js, es/form, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
