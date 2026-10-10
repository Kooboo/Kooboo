// Site-owned Markdown notes. Topics, Milestones, planning and source remain authoritative.
var ROOT = "_ai/memory/";
var MAX_INDEX = 4000;
var MAX_NOTE = 24000;
var MAX_FILES = 32;
var FILE_NAME = /^[a-zA-Z0-9][a-zA-Z0-9_-]{0,79}\.md$/;

function arg(name) {
    if (k.request.model && k.request.model[name] !== undefined) return k.request.model[name];
    if (k.request[name] !== undefined) return k.request[name];
    return typeof k.request.get === "function" ? k.request.get(name) : null;
}
function fail(message) { throw new Error(message); }
function validName(path) {
    return typeof path === "string" && path === path.trim() && FILE_NAME.test(path)
        && (path.toLowerCase() !== "index.md" || path === "index.md");
}
function resolve(path) {
    if (!validName(path)) fail("Invalid memory path: use 1-80 ASCII English letters, digits, underscores or hyphens, starting with a letter or digit, followed by lowercase .md (for example asset-management.md). Chinese characters, spaces, directories and extra dots are not allowed. The reserved entry filename must be exactly index.md. When reading, use an existing filename returned in index.md files; renaming a guessed path does not make a note exist.");
    return ROOT + path;
}
function limit(path) { return path === "index.md" ? MAX_INDEX : MAX_NOTE; }
function hash(text) { return String(k.security.sha256(text)).toLowerCase(); }
function list() {
    var files = k.file.folderFiles(ROOT) || [];
    var names = [];
    for (var i = 0; i < files.length; i++) {
        if (validName(files[i].name)) names.push(String(files[i].name));
    }
    names.sort();
    if (names.length > MAX_FILES) fail("Memory file limit exceeded; consolidate existing notes.");
    return names;
}
function readText(path) {
    var fullName = resolve(path);
    if (!k.file.exists(fullName)) return null;
    var info = k.file.get(fullName);
    if (!info) fail("The memory file could not be read: " + path);
    if (info.size > MAX_NOTE * 4) fail("Memory file exceeds the size limit: " + path);
    var content = k.file.read(fullName);
    if (content === null || content === undefined) fail("The memory file could not be read: " + path);
    content = String(content);
    if (content.length > limit(path)) fail("Memory file exceeds the size limit: " + path);
    return content;
}
function number(value, fallback, minimum, maximum, name) {
    if (value === undefined) return fallback;
    if (typeof value !== "number" || !isFinite(value) || Math.floor(value) !== value || value < minimum || value > maximum)
        fail(name + " is outside the supported range.");
    return value;
}
function read(input) {
    var path = input.path === undefined ? "index.md" : input.path;
    var offset = number(input.offset, 0, 0, 2147483647, "offset");
    var maxChars = input.maxChars === undefined ? 4000 : input.maxChars;
    if (typeof maxChars !== "number" || !isFinite(maxChars) || Math.floor(maxChars) !== maxChars || maxChars < 1)
        fail("maxChars must be a positive integer. Default: 4000; maximum per page: 8000. Retry with maxChars=4000.");
    maxChars = Math.min(maxChars, 8000);
    var content = readText(path);
    var start = Math.min(offset, content === null ? 0 : content.length);
    var page = content === null ? "" : content.slice(start, start + maxChars);
    return {
        success: true, path: path, exists: content !== null, content: page,
        sha256: content === null ? null : hash(content), offset: start,
        nextOffset: content !== null && start + page.length < content.length ? start + page.length : null,
        files: path === "index.md" ? list() : []
    };
}
function update(input) {
    var path = input.path === undefined ? "index.md" : input.path;
    var fullName = resolve(path);
    var content = input.content;
    if (typeof content !== "string") fail("content must be text.");
    var existing = readText(path);
    if (existing !== null && !input.oldContent && !input.expectedSha256)
        fail("Supply expectedSha256 from the latest read when replacing an existing note, or use an exact oldContent replacement.");
    if (input.expectedSha256 !== undefined && input.expectedSha256 !== null)
        checkVersion(existing, input.expectedSha256);
    if (input.oldContent !== undefined && input.oldContent !== null) {
        var old = input.oldContent;
        if (typeof old !== "string" || !old.length || existing === null) fail("Read existing memory before replacing text.");
        var start = existing.indexOf(old);
        if (start < 0 || existing.indexOf(old, start + old.length) >= 0) fail("oldContent must match exactly once; reread the note.");
        content = existing.slice(0, start) + content + existing.slice(start + old.length);
    }
    if (content.length > limit(path)) fail("Keep index.md within 4000 characters and other notes within 24000 characters.");
    if (existing === null && list().length >= MAX_FILES) fail("Memory file limit reached; consolidate existing notes.");
    k.file.write(fullName, content);
    if (readText(path) !== content) fail("Memory readback did not match the saved content: " + path);
    return { success: true, receipt: { path: path, sha256: hash(content) },
        siteId: String(arg("siteId")), savedPreview: content.slice(0, 600), previewTruncated: content.length > 600 };
}
function checkVersion(content, expected) {
    if (typeof expected !== "string" || !/^[a-f0-9]{64}$/.test(expected)
        || content === null || hash(content) !== expected) {
        var error = new Error("Memory changed or was deleted. Reload it before saving or deleting.");
        error.code = "memory_conflict";
        throw error;
    }
}
function edit(input) {
    checkVersion(readText(input.path), input.expectedSha256);
    return update(input);
}
function remove(input) {
    checkVersion(readText(input.path), input.expectedSha256);
    k.file.delete(resolve(input.path));
    if (k.file.exists(resolve(input.path))) fail("Memory deletion could not be verified.");
    return { success: true, path: input.path };
}
function search(input) {
    if (typeof input.query !== "string" || !input.query.trim() || input.query.length > 200)
        fail("Use a nonempty search phrase of at most 200 characters.");
    var query = input.query.toLowerCase();
    var matches = [];
    list().forEach(function (path) {
        var content = readText(path);
        if (content === null) return;
        var offset = content.toLowerCase().indexOf(query);
        if (offset < 0) return;
        var start = Math.max(0, offset - 80);
        matches.push({ path: path, offset: start, excerpt: content.slice(start, start + 240) });
    });
    return { success: true, matches: matches.slice(0, 12), truncated: matches.length > 12 };
}
function prepare(input, siteId) {
    if (typeof input.continuation !== "string" || !input.continuation.trim() || input.continuation.length > 1500)
        fail("Provide a concise continuation of 1..1500 characters.");
    if (!Array.isArray(input.files) || !input.files.length || input.files.length > MAX_FILES)
        fail("Provide current read/write receipts for index.md and relevant notes.");
    var files = [], hasIndex = false, seen = {};
    input.files.forEach(function (file) {
        if (!file || !validName(file.path) || seen[file.path]) fail("Invalid or duplicate memory receipt.");
        seen[file.path] = true;
        var content = readText(file.path);
        if (content === null || !content.trim() || hash(content) !== file.sha256)
            fail("Memory receipt is missing or outdated; read or save the note again: " + file.path);
        if (file.path === "index.md") hasIndex = true;
        files.push({ path: file.path, sha256: file.sha256 });
    });
    if (!hasIndex) fail("Provide a receipt for index.md.");
    return { success: true, siteId: siteId, continuation: input.continuation, files: files };
}
try {
    var siteId = String(arg("siteId") || "");
    if (!siteId) fail("A current site is required.");
    var input = arg("data") || {};
    if (typeof input === "string") input = JSON.parse(input);
    if (!input || typeof input !== "object" || Array.isArray(input)) fail("An input object is required.");
    switch (arg("action")) {
        case "read": return read(input);
        case "update": return update(input);
        case "edit": return edit(input);
        case "delete": return remove(input);
        case "search": return search(input);
        case "prepare": return prepare(input, siteId);
        default: fail("Unsupported memory action.");
    }
} catch (error) {
    return { success: false, status: "error", code: error.code || "site_memory_failed", message: error.message || String(error) };
}
