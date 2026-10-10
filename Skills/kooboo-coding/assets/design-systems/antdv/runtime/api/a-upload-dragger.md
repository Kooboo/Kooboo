# AUploadDragger

Ant Design Vue 4.2.6; native export `UploadDragger`. Import `AUploadDragger` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Use v-model:fileList and native customRequest({file,onSuccess,onError,onProgress,...}). Returning false from beforeUpload selects locally without upload. Explicitly implement cancel/retry and prevent late responses from reviving removed files. File selection is not storage success.

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AUpload v-model:file-list="files" :before-upload="() => false"><AButton>选择文件</AButton></AUpload><p>仅本地选择，未上传服务器。{{ files.length }} 个文件</p>
```

Source: pinned upstream es/components.js, es/upload, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
