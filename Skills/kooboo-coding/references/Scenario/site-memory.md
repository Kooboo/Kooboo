# Site memory and conversation handoff

Site memory is a small set of Markdown notes in the current site's kfile directory, `_ai/memory/` (entry: `_ai/memory/index.md`). The named tools execute `scripts/site-development/site-memory.js` with the current site context and use `k.file` for storage. Tool paths are plain filenames relative to that directory. It supports cross-conversation context, not a second planning or progress system. Existing App creation, review, authorization and implementation gates still apply.

## Read only what the task needs

Start a new site conversation with `read_site_memory` (defaults to `index.md`). A missing index is normal. A failed read is not absence. The index response includes a bounded filename list; read named notes or use `search_site_memory` with a literal phrase. Follow `nextOffset` only for needed content. Reuse unchanged notes already in the conversation.

## Save at meaningful boundaries

Use `update_site_memory` when the user asks to remember something, or when durable context, a special implementation convention/reason, or an important resource entry needs recording. Read the existing note first. Use `oldContent` for an exact unique text replacement, or replace the whole small note with `content` while preserving unrelated information. Plain filenames such as `index.md` and `inventory.md` are supported. Keep the index within 4000 characters and other notes within 24000; split only when needed and link relevant notes from the index.

Topics own questions, answers and confirmation; Milestones own progress; Blueprint/Plan own their definitions; source owns actual implementation. Update those records through their normal authorized workflow rather than copying them into memory. Notes may link to stable IDs and resource paths. Do not store a transcript, repeated tool output, credentials, speculative debugging, or a duplicate implementation checklist. If content moves into structured planning, verify it is saved there, then replace the duplicate memory text with a reference.

Save useful context when it becomes established, not only at conversation end. A successful write returns `receipt.path` and `receipt.sha256` after persisted readback. A hash proves the saved content, not business correctness. On failure, report that the note was not saved. Memory never overrides the user's current instruction or grants permission to implement. Distinguish desired behavior from what source currently implements.

## Review and correct saved memory

The Site memory entry in AI Chat settings lets users read existing notes, edit them, or delete a file. Management writes and deletions require the hash from the latest read; a conflict requires reloading. For tool writes, supply `expectedSha256` when replacing an existing file in full; an exact unique `oldContent` replacement remains available. A deleted note must not be recreated merely because an index still references it. Already loaded chat content and references in other notes are not automatically removed.

A successful tool write also returns `siteId`, `savedPreview` (the first 600 characters of the persisted note) and `previewTruncated`. The chat UI shows one compact, clickable saved-status message per execution, without displaying note contents or separate editing controls. Clicking it opens the saved note in the memory manager; users can also review and edit notes in AI Chat settings. The preview is a tool-result excerpt, not a generated summary or evidence of current implementation.

Save established business background and the reasons for special conventions. Do not promote proposals, experiments or the assistant's own suggestions into confirmed site conventions. Formal decisions and completion claims belong in their authoritative records.

## User-requested move to a new conversation

An ordinary request to remember information saves it without opening another conversation. The explicit handoff action uses a preparation turn limited to memory and relevant reads. Existing records remain authoritative; do not perform planning, business-resource or progress writes as part of this turn.

Read and save only missing durable context, then call `prepare_site_memory_handoff` with current `path`/`sha256` receipts for `index.md` and the notes needed for continuation. A read response also supplies a receipt hash, so unchanged notes do not need a write. Keep `continuation` within 1500 characters: current user intent, next step and necessary references, in the user's language. Preserve discussion versus implementation and the exact authorized scope. A completed milestone does not authorize the next one. Do not embed a transcript or detailed copies of Topics/Plan. Resolve failed saves first; call the handoff tool last and stop.

The UI verifies the structured result before creating the same site's new conversation. It sends a short continuation and preserves the original conversation. No history compression or arbitrary interruption recovery is performed. In the new conversation, read the index and only relevant authoritative records before continuing.
