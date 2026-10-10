# AUpload

Ant Design Vue 4.2.6; native export `Upload`. Import `AUpload` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Use v-model:fileList and native customRequest({file,onSuccess,onError,onProgress,...}). Returning false from beforeUpload selects locally without upload. Explicitly implement cancel/retry and prevent late responses from reviving removed files. File selection is not storage success.

## Props and defaults

- `accept`: string; default -. File types that can be accepted. See [input accept Attribute](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/file#accept)
- `action`: string|(file) => `Promise`; default -. Uploading URL
- `beforeUpload`: (file, fileList) => `boolean` | `Promise`; default -. Hook function which will be executed before uploading. Uploading will be stopped with `false` or a rejected Promise returned.
- `customRequest`: function; default -. override for the default xhr behavior allowing for additional customization and ability to implement your own XMLHttpRequest
- `data`: object|function(file); default -. Uploading params or function which can return uploading params.
- `directory`: boolean; default false. Support upload whole directory（[caniuse](https://caniuse.com/#feat=input-file-directory)）
- `disabled`: boolean; default -. disable upload button
- `downloadIcon`: v-slot:iconRender="{file: UploadFile}"; default -. custom download icon
- `fileList`: object[]; default -. List of files that have been uploaded (controlled). Here is a common issue [#2423](https://github.com/ant-design/ant-design/issues/2423) when using it
- `headers`: object; default -. Set request headers, valid above IE10.
- `iconRender`: v-slot:iconRender="{file: UploadFile, listType?: UploadListType}"; default -. Custom show icon
- `isImageUrl`: (file: UploadFile) => boolean; default -. Customize if render &lt;img /> in thumbnail
- `itemRender`: v-slot:itemRender="{originNode: VNode, file: UploadFile, fileList: object[], actions: { download: function, preview: function, remove: function }"; default -. Custom item of uploadList
- `listType`: string; default `text`. Built-in stylesheets, support for three types: `text`, `picture` or `picture-card`
- `maxCount`: number; default -. Limit the number of uploaded files. Will replace current one when `maxCount` is `1`
- `method`: string; default `post`. http method of upload request
- `multiple`: boolean; default false. Whether to support selected multiple file. `IE10+` supported. You can select multiple files with CTRL holding down while multiple is set to be true
- `name`: string; default `file`. The name of uploading file
- `openFileDialogOnClick`: boolean; default true. Click open file dialog
- `previewFile`: (file: File | Blob) => Promise&lt;dataURL: string>; default -. Customize preview file logic
- `previewIcon`: v-slot:iconRender="{file: UploadFile}"; default -. custom preview icon
- `progress`: [ProgressProps](/components/progress/#api) (support `type="line"` only); default { strokeWidth: 2, showInfo: false }. Custom progress bar
- `removeIcon`: v-slot:iconRender="{file: UploadFile}"; default -. custom remove icon
- `showUploadList`: boolean | { showPreviewIcon?: boolean, showRemoveIcon?: boolean, showDownloadIcon?: boolean }; default true. Whether to show default upload list, could be an object to specify `showPreviewIcon`, `showRemoveIcon` and `showDownloadIcon` individually
- `supportServerRender`: boolean; default false. Need to be turned on while the server side is rendering.
- `withCredentials`: boolean; default false. ajax upload with cookie sent

## Events

- `change`: A callback function, can be executed when uploading state is changing. See [change](#change)
- `download`: Click the method to download the file, pass the method to perform the method logic, do not pass the default jump to the new TAB.
- `drop`: A callback function executed when files are dragged and dropped into upload area
- `preview`: A callback function, will be executed when file link or preview icon is clicked.
- `reject`: A callback function, will be executed when drop files is not accept.
- `remove`: A callback function, will be executed when removing file button is clicked, remove event will be prevented when return value is false or a Promise which resolve(false) or reject

## Slots

- `downloadIcon`: custom download icon
- `iconRender`: Custom show icon
- `itemRender`: Custom item of uploadList
- `previewIcon`: custom preview icon
- `removeIcon`: custom remove icon

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AUpload v-model:file-list="files" :before-upload="() => false"><AButton>选择文件</AButton></AUpload><p>仅本地选择，未上传服务器。{{ files.length }} 个文件</p>
```

Source: pinned upstream es/components.js, es/upload, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
