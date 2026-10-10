// Requirement discussions belong to the site, independently of adopted planning documents.
var STORE_PATH = "_ai/site-development/planning-topics.json";
var ID = /^[A-Za-z][A-Za-z0-9]*(?:[._-][A-Za-z0-9]+)*$/;
var STAGES = ["app-plan-creation", "implementation", "production-release"];

function arg(name) {
    if (k.request.model && k.request.model[name] !== undefined) return k.request.model[name];
    if (k.request[name] !== undefined) return k.request[name];
    return typeof k.request.get === "function" ? k.request.get(name) : null;
}
function reject(code, message) { var error = new Error(message); error.code = code; throw error; }
function object(value) { return value && typeof value === "object" && !Array.isArray(value); }
function keys(value, allowed) {
    if (!object(value)) reject("INVALID_INPUT", "An object is required.");
    Object.keys(value).forEach(function (key) { if (allowed.indexOf(key) < 0) reject("INVALID_INPUT", "Unsupported property: " + key); });
}
function text(value, name, maximum) {
    if (typeof value !== "string" || !value.trim() || value.length > maximum) reject("INVALID_INPUT", name + " must be nonempty text, at most " + maximum + " characters.");
}
function oneOf(value, allowed, name) {
    if (allowed.indexOf(value) < 0) reject("INVALID_INPUT", "Invalid " + name + ".");
}
function ids(value, name, maximum) {
    if (!Array.isArray(value) || value.length > maximum) reject("INVALID_INPUT", "Invalid " + name + ".");
    var seen = {};
    value.forEach(function (id) {
        if (typeof id !== "string" || id.length > 120 || !ID.test(id) || seen[id]) reject("INVALID_INPUT", "Invalid or duplicate " + name + ".");
        seen[id] = true;
    });
}
function validate(item) {
    keys(item, ["id", "kind", "question", "context", "suggestion", "options", "answer", "status", "confirmationEvidence", "impact", "resolveBefore", "scope"]);
    ["suggestion", "answer", "confirmationEvidence"].forEach(function (key) { if (item[key] === undefined) item[key] = null; });
    if (item.options === undefined) item.options = [];
    ids([item.id], "id", 1);
    oneOf(item.kind, ["assumption", "open-question"], "kind");
    text(item.question, "question", 2000);
    text(item.context, "context", 4000);
    if (item.suggestion !== null) text(item.suggestion, "suggestion", 4000);
    if (!Array.isArray(item.options) || item.options.length > 20) reject("INVALID_INPUT", "Supply at most 20 options.");
    item.options.forEach(function (option) {
        keys(option, ["id", "label", "description"]);
        text(option.label, "option.label", 200);
        text(option.description, "option.description", 2000);
    });
    ids(item.options.map(function (option) { return option.id; }), "option ids", 20);
    if (item.answer !== null) text(item.answer, "answer", 4000);
    oneOf(item.status, ["open", "answered", "confirmed", "dismissed"], "status");
    if ((item.status === "answered" || item.status === "confirmed") && item.answer === null) reject("ANSWER_REQUIRED", "Answered and confirmed items require an answer.");
    if (item.status === "open" && item.answer !== null) reject("INVALID_INPUT", "Use answered when recording an unconfirmed answer.");
    if (item.status === "confirmed" || item.status === "dismissed") text(item.confirmationEvidence, "confirmationEvidence", 4000);
    else if (item.confirmationEvidence !== null) reject("INVALID_INPUT", "Only confirmed or dismissed items carry confirmation evidence.");
    oneOf(item.impact, ["low", "medium", "high"], "impact");
    oneOf(item.resolveBefore, STAGES, "resolveBefore");
    keys(item.scope, ["artifact", "relatedIds", "milestoneIds"]);
    oneOf(item.scope.artifact, ["site", "blueprint", "app-plan"], "scope.artifact");
    ids(item.scope.relatedIds, "relatedIds", 100);
    ids(item.scope.milestoneIds, "milestoneIds", 100);
}
function load(siteId) {
    if (!k.file.exists(STORE_PATH)) return { siteId: siteId, items: [], updatedAt: null };
    var raw = String(k.file.read(STORE_PATH));
    if (raw.length > 2 * 1024 * 1024) reject("STORE_TOO_LARGE", "Planning topics exceed the storage limit.");
    try {
        var store = JSON.parse(raw);
        keys(store, ["siteId", "items", "updatedAt"]);
        if (store.siteId !== siteId || !Array.isArray(store.items) || store.items.length > 500) throw new Error("Invalid site or items.");
        ids(store.items.map(function (item) { validate(item); return item.id; }), "item ids", 500);
        return store;
    } catch (_) { reject("CORRUPT_STORE", "Planning topics cannot be read; repair the stored document before continuing."); }
}
function dispatch() {
    var siteId = String(arg("siteId") || "");
    if (!siteId) reject("SITE_REQUIRED", "A site is required.");
    var action = arg("action") || "read";
    var input = arg("data") || {};
    if (typeof input === "string") input = JSON.parse(input);
    var store = load(siteId);
    if (action === "readiness") {
        keys(input, ["stage"]);
        oneOf(input.stage, STAGES, "stage");
        var stage = STAGES.indexOf(input.stage);
        return { ok: true, items: store.items.filter(function (item) {
            return (item.status === "open" || item.status === "answered") && STAGES.indexOf(item.resolveBefore) <= stage;
        }).map(function (item) { return { id: item.id, milestoneIds: item.scope.milestoneIds }; }) };
    }
    if (action === "answer") {
        keys(input, ["id", "answer", "confirm", "expectedUpdatedAt"]);
        ids([input.id], "id", 1);
        text(input.answer, "answer", 4000);
        if (typeof input.confirm !== "boolean") reject("INVALID_INPUT", "confirm must be boolean.");
        if (input.expectedUpdatedAt !== store.updatedAt) reject("TOPICS_CHANGED", "Topics changed; reload before saving your answer.");
        var existing = store.items.find(function (item) { return item.id === input.id; });
        if (!existing) reject("NOT_FOUND", "Topic not found.");
        var answered = JSON.parse(JSON.stringify(existing));
        answered.answer = input.answer;
        answered.status = input.confirm ? "confirmed" : "answered";
        answered.confirmationEvidence = input.confirm ? input.answer : null;
        input = { item: answered, expectedUpdatedAt: input.expectedUpdatedAt };
        action = "update";
    }
    if (action === "update") {
        keys(input, ["item", "expectedUpdatedAt"]);
        validate(input.item);
        var index = -1;
        store.items.forEach(function (item, i) { if (item.id === input.item.id) index = i; });
        if ((index >= 0 || input.expectedUpdatedAt != null) && input.expectedUpdatedAt !== store.updatedAt)
            reject("TOPICS_CHANGED", "Topics changed or expectedUpdatedAt is missing. Read the topic again, preserve the current answer and confirmation evidence, merge your intended changes, and retry with the returned updatedAt. Do not blindly retry the old item.");
        if (index >= 0 && JSON.stringify(store.items[index]) === JSON.stringify(input.item)) return { ok: true, changed: false, item: store.items[index], updatedAt: store.updatedAt };
        if (index < 0) store.items.push(input.item); else store.items[index] = input.item;
        if (store.items.length > 500) reject("STORE_TOO_LARGE", "Supply at most 500 planning topics.");
        store.updatedAt = new Date(Math.max(Date.now(), (Date.parse(store.updatedAt) || 0) + 1)).toISOString();
        var serialized = JSON.stringify(store, null, 2);
        if (serialized.length > 2 * 1024 * 1024) reject("STORE_TOO_LARGE", "Planning topics exceed the storage limit.");
        k.file.write(STORE_PATH, serialized);
        if (String(k.file.read(STORE_PATH)) !== serialized) reject("WRITE_FAILED", "Planning topic write could not be verified.");
        return { ok: true, changed: true, item: input.item, updatedAt: store.updatedAt };
    }
    if (action !== "read") reject("INVALID_ACTION", "Use read or update.");
    keys(input, ["ids", "status", "artifact", "cursor", "maxItems"]);
    var selectedIds = input.ids || [];
    ids(selectedIds, "ids", 30);
    if (input.status) oneOf(input.status, ["open", "answered", "confirmed", "dismissed"], "status");
    if (input.artifact) oneOf(input.artifact, ["site", "blueprint", "app-plan"], "artifact");
    var cursor = input.cursor === undefined ? 0 : input.cursor;
    var maximum = input.maxItems === undefined ? 20 : input.maxItems;
    if (typeof cursor !== "number" || !isFinite(cursor) || cursor < 0 || cursor % 1) reject("INVALID_INPUT", "cursor must be a nonnegative integer; received " + JSON.stringify(cursor) + ". Omit or use 0 on the first request; use nextCursor for subsequent pages and stop when it is null.");
    if (typeof maximum !== "number" || !isFinite(maximum) || maximum < 1 || maximum > 30 || maximum % 1) reject("INVALID_INPUT", "maxItems must be an integer between 1 and 30; received " + JSON.stringify(maximum) + ". Omit or use 20, then follow nextCursor for remaining items.");
    var items = store.items.filter(function (item) {
        return (!selectedIds.length || selectedIds.indexOf(item.id) >= 0) && (!input.status || item.status === input.status) &&
            (!input.artifact || item.scope.artifact === input.artifact || item.scope.artifact === "site");
    }).sort(function (left, right) { return left.id < right.id ? -1 : left.id > right.id ? 1 : 0; });
    var page = [], size = 0;
    for (var i = cursor; i < items.length && page.length < maximum; i++) {
        var row = selectedIds.length ? items[i] : { id: items[i].id, kind: items[i].kind, question: items[i].question, status: items[i].status, impact: items[i].impact, resolveBefore: items[i].resolveBefore };
        var length = JSON.stringify(row).length;
        if (page.length && size + length > 16000) break;
        page.push(row); size += length;
    }
    var next = cursor + page.length;
    return { ok: true, items: page, total: items.length, nextCursor: next < items.length ? next : null, updatedAt: store.updatedAt };
}
try {
    var result = dispatch();
    return arg("format") === "json" ? JSON.stringify(result) : result;
} catch (error) {
    var failure = { ok: false, error: { code: error.code || "PLANNING_TOPICS_FAILED", message: error.message || String(error) } };
    return arg("format") === "json" ? JSON.stringify(failure) : failure;
}
