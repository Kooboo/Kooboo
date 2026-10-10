# ElUpload

Upload files by clicking or drag-and-drop.

Kind: component. Import `ElUpload` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/upload.html#upload

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| action | string | # | request URL. |
| headers | Headers / Record<string, any> | not declared | request headers. |
| method | string | post | set upload request method. |
| multiple | boolean | false | whether uploading multiple files is permitted. |
| data | Record<string, any> / Awaitable<Record<string, any>> / (rawFile: UploadRawFile) => Awaitable<Record<string, any>> | {} | additions options of request. support `Awaitable` data and `Function` since v2.3.13. |
| name | string | file | key name for uploaded file. |
| with-credentials | boolean | false | whether cookies are sent. |
| show-file-list | boolean | true | whether to show the uploaded file list. |
| drag | boolean | false | whether to activate drag and drop mode. |
| accept | string | '' | accepted [file types](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input#attr-accept), will not work when `thumbnail-mode === true`. |
| crossorigin | '' \| 'anonymous' \| 'use-credentials' | not declared | native attribute [crossorigin](https://developer.mozilla.org/en-US/docs/Web/HTML/Attributes/crossorigin). |
| on-preview | (uploadFile: UploadFile) => void | not declared | hook function when clicking the uploaded files. |
| on-remove | (uploadFile: UploadFile, uploadFiles: UploadFiles) => void | not declared | hook function when files are removed. |
| on-success | (response: any, uploadFile: UploadFile, uploadFiles: UploadFiles) => void | not declared | hook function when uploaded successfully. |
| on-error | (error: Error, uploadFile: UploadFile, uploadFiles: UploadFiles) => void | not declared | hook function when some errors occurs. |
| on-progress | (evt: UploadProgressEvent, uploadFile: UploadFile, uploadFiles: UploadFiles) => void | not declared | hook function when some progress occurs. |
| on-change | (uploadFile: UploadFile, uploadFiles: UploadFiles) => void | not declared | hook function when select file or upload file success or upload file fail. |
| on-exceed | (files: File[], uploadFiles: UploadUserFile[]) => void | not declared | hook function when limit is exceeded. |
| before-upload | (rawFile: UploadRawFile) => Awaitable<void \| undefined \| null \| boolean \| File \| Blob> | not declared | hook function before uploading with the file to be uploaded as its parameter. If `false` is returned or a `Promise` is returned and then is rejected, uploading will be aborted. |
| before-remove | (uploadFile: UploadFile, uploadFiles: UploadFiles) => Awaitable<boolean> | not declared | hook function before removing a file with the file and file list as its parameters. If `false` is returned or a `Promise` is returned and then is rejected, removing will be aborted. |
| file-list | UploadUserFile[] | [] | default uploaded files. |
| list-type | 'text' \| 'picture' \| 'picture-card' | text | type of file list. |
| auto-upload | boolean | true | whether to auto upload file. |
| http-request | (options: UploadRequestOptions) => XMLHttpRequest / Promise<unknown> | ajaxUpload [see](https://github.com/element-plus/element-plus/blob/dev/packages/components/upload/src/ajax.ts#L55) | override default xhr behavior, allowing you to implement your own upload-file's request. |
| disabled | boolean | false | whether to disable upload. |
| limit | number | not declared | maximum number of uploads allowed. |
| directory | boolean | false | whether to support uploading directory. After enabling it, only folders can be selected, and after selecting a folder, the files within the folder will be flattened. |

## Slots

- `default`: customize default content.
- `trigger`: content which triggers file dialog.
- `tip`: content of tips.
- `file`: content of thumbnail template.

## Usage and state ownership

Business owns file-list, validation and the upload endpoint. Use v-model:file-list, before-upload and http-request for custom transport; report onSuccess/onError/onProgress according to the native UploadRequestOptions contract. A selected file is not a successful upload. For manual submission use auto-upload=false and the component ref submit()/abort()/clearFiles(). Validate again on the server. Simulated examples must be labelled and must not write real files. Guard late responses/cancel/retry in a custom transport.

```js
export default { components: { ElUpload, ElButton }, setup: () => ({ files: ref([]), request: uploadTransport }), template: '<ElUpload v-model:file-list="files" :http-request="request"><ElButton>Select file</ElButton></ElUpload>' };
```
