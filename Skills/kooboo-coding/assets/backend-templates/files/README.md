# FileIO helpers

The optional `files` template copies `Services.file` and shared `ENV` (when absent) into the site. Install it with `install_backend_templates({"templates":["files"]})`, or select it explicitly in `create_app_base.backendTemplates`. It creates no tables, API routes, identity integration, reference registry, hooks or scheduler. No content hashing or upload deduplication is performed.

## Configuration

The shared [ENV source](../ENV.ts) defines `files.maxBytes`, `files.allowedExtensions`, `files.temporaryHours` and `files.cleanupBatchSize`. Defaults are 10 MiB, an explicit extension allowlist, 24 hours and 10 entries. Existing ENV is preserved; add the `files` section explicitly if missing. Helpers do not read `ENV.authMode` or depend on App Base users.

`saveTemporary(file, { maxBytes?, allowedExtensions? })` overrides upload defaults independently for one call. An omitted field uses its ENV default; a supplied extension list replaces the default list rather than merging with it. Limits may be stricter or more permissive: ENV.files provides defaults, not a global cap. Explicitly invalid values (including zero bytes or an empty allowlist) are rejected, not replaced by defaults. Extensions are case-insensitive names without a leading dot, such as `png` or `pdf`. Neither ENV nor the supplied options are mutated.

Keep reusable scenario policies in application-owned ENV groups, extending its TypeScript interface as needed. For example, configure an `ENV.uploadPolicies.avatar` group with `maxBytes: 2 * 1024 * 1024` and `allowedExtensions: ['jpg', 'png']`, and a document group with `maxBytes: 20 * 1024 * 1024` and `allowedExtensions: ['pdf', 'docx']`. These groups are business-defined; the helper does not install a scenario catalog. The authorized server endpoint selects and passes its policy. Never forward upload limits or allowlists from the request body; validate a client scenario selector against the operations the caller is allowed to perform.

The caller authenticates and authorizes operations, applies business quotas and decides whether real storage is appropriate. Mock previews must not call persistent helpers. Configure host request-body limits too: byte size is checked after upload parsing. Extension checks are not content verification or malware scanning.

## Exports

| Method | Behavior |
|---|---|
| `saveTemporary(file, options?)` | Accept `{ fileName, bytes }`, validate it against server-selected options or ENV defaults and save under a fresh UUID-based key in `app-files/tmp` |
| `promoteFile(fileKey)` | Move into `app-files/data`; repeated calls return the existing permanent file without overwriting it |
| `getFile(fileKey)` | Return metadata, preferring permanent storage; return `null` if absent |
| `deleteFile(fileKey)` | Explicitly delete temporary and permanent locations; missing files are harmless |
| `cleanupTemporary(options?)` | Inspect a batch of temporary keys and delete expired files; return counts, failures and a continuation cursor |

Save, promotion and lookup return `{ fileKey, state, path, url, size, lastModified }`. `fileKey` stays stable across promotion; the physical path and URL change. Refresh returned metadata after promotion and persist the stable key in business records. Original upload names are not stored separately; retain a display/download name in the caller if needed.

Keys are generated basenames, never client-supplied paths or URLs. Helpers operate only under their two fixed directories. Keep those directories reserved for these helpers. The service returns plain values and throws errors; APIs own input parsing, authentication, error mapping and HTTP output. Do not expose a generic deletion endpoint that accepts any key without business authorization.

## Explicit business integration

```ts
import { saveTemporary, promoteFile } from './Services.file'
import { successResponse } from './Utils.response'

// Inside an authorized business API callback, after validating exactly one upload:
const upload = k.request.files[0]
const temporary = saveTemporary(upload)
const permanent = promoteFile(temporary.fileKey)

// Persist permanent.fileKey in the business record using the application's own logic.
// Resolve business save failures before deciding whether permanent can be deleted.
const result = { fileKey: permanent.fileKey, url: permanent.url }
return successResponse(result)
```

Several business records can store the same key. The caller tracks references and confirms a permanent file is unused before calling `deleteFile`. The helper neither discovers nor modifies business records. It does not automatically delete unused permanent files or compensate for failed business saves. Failed/partial disk writes may leave temporary files that later cleanup can reclaim.

## Temporary cleanup

`cleanupTemporary({ retentionHours?, batchSize?, afterKey? })` uses the file's last-modified time. It returns `{ inspected, deleted, skipped, failures, nextCursor }`; failure entries contain `fileKey` and `message`. A missing/empty directory returns zero counts. Unknown filenames and nested directories are left alone.

A server job or another explicit caller controls execution. Pass `nextCursor` as `afterKey` until it is `null`; start the next sweep without a cursor. Failed files remain on disk for a later sweep, and do not prevent processing subsequent keys. Files added before the current cursor are visited on the next sweep. There is no automatic retry loop or scheduled deadline.

`batchSize` bounds inspected entries and deletion attempts, not directory enumeration: the current `k.file.folderFiles` primitive eagerly lists the directory. Very large temporary folders need a different enumeration strategy.

Cleanup never visits the permanent directory. It rechecks each candidate before deletion, but this is not a transaction with upload, promotion or business saves. Use a retention period longer than active upload/edit sessions, and coordinate cleanup with outstanding file operations where required. A promotion racing cleanup can fail if the temporary file was already removed; handle that failure before saving a business reference. Do not rewrite temporary files in place or treat them as durable business attachments.

## Access and removal

Returned URLs are ordinary FileIO URLs governed by platform/site access rules and `EnableFileIOUrl`. They do not provide per-attachment privacy. Confidential files need a protected-storage design and authorized delivery implemented by the application. The template does not change site settings or access control.

To remove the helper, stop callers and remove their imports and `Services.file`; remove its ENV section only when unused. Removing code does not delete files. Never recursively remove `app-files` while business records still reference its contents.
