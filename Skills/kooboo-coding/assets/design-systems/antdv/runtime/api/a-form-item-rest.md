# AFormItemRest

Ant Design Vue 4.2.6; native export `FormItemRest`. Import `AFormItemRest` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Use :model, FormItem name and rules; native validate/validateFields/resetFields are instance methods. @finish runs after validation; @finishFailed handles invalid submissions. Async request state and server validation belong to business code.

## Props and defaults



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
