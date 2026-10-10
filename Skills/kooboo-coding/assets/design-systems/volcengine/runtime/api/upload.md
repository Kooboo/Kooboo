# YlUpload

Native file input/drop adapter with Yuanli Button and Progress. Import from `/yuanli.js`.

- `v-model`: file-entry array (default []); bind for parent ownership, or omit for internal use. Updates are immutable. Replace the array for external changes; keep ids unique and stable. Entries: `{id,name,size,type,raw?,status,progress,error?,response?}`. Status is ready/uploading/success/error; progress 0–100. Do not persist `raw` File objects. Preloaded records can omit raw; they cannot be uploaded/retried and have no local preview.
- `request(file, {signal, onProgress}) => Promise<response>`: optional business adapter. Without it, selection never claims upload success. Resolve only after server acceptance; reject failures. Progress accepts percentage numbers, clamped to 0–99 until success. Adapter should honor AbortSignal; stale callbacks are ignored even if it does not.
- `autoUpload=true`; `multiple=false` controls files per selection/drop. Use `limit=1` for a single-file list. `limit=Infinity`, `maxSize=Infinity` (bytes), `accept=''` (comma-separated extensions, MIME types or MIME wildcards).
- `listType`: text (default) / picture. Local PNG/JPEG/GIF/WebP/AVIF/BMP preview URLs are revoked on removal/unmount. SVG is not previewed. Server response URLs are not automatically rendered.
- `disabled=false` blocks user actions; ongoing requests continue. `label=选择文件`, `hint=也可以将文件拖到这里`. Default slot replaces the hint.
- Events: `update:modelValue(list)`, `reject({file,reason})`, `success(entry)`, `error({file,error})`, `remove(entry)`.
- Component ref exposes `selectFiles(FileList|File[])`, `upload(id)`, `cancel(id)`, `remove(id)`. Cancel returns to ready; removal/unmount abort active requests. Cancellation does not undo a server-side write.

```html
<YlUpload v-model="files" :request="uploadFile" accept="image/png,image/jpeg"
  list-type="picture" :limit="4" :max-size="5242880" multiple />
```

Business adapter, with `uploadEndpoint` supplied by the actual App API:

```js
async function uploadFile(file, { signal }) {
  const body = new FormData()
  body.append('file', file)
  const response = await fetch(uploadEndpoint, { method: 'POST', body, signal })
  if (!response.ok) throw new Error('上传失败')
  return response.json()
}
```

fetch does not provide upload progress; supply an XHR or service adapter that calls onProgress when required. Authentication, endpoint, server validation/storage and deletion belong to the App. Client accept/size checks are convenience checks, not server validation. No implicit Kooboo media/admin upload, resumable transfer, cropper, folder upload or server deletion. Browser selection/drop and previews require manual acceptance.
