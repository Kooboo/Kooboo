import assert from "node:assert/strict";
import test from "node:test";
import { createKFile, runTopics, topic } from "./planning-topics-test-helper.mjs";

test("Topics persist independently without creating Blueprint or App Plan", () => {
  const { k, files } = createKFile();
  assert.deepEqual(runTopics(k, "read", {}).items, []);
  assert.equal(runTopics(k, "update", { expectedUpdatedAt: runTopics(k, "read", {}).updatedAt, item: topic() }).ok, true);
  assert.deepEqual([...files.keys()], ["_ai/site-development/planning-topics.json"]);
  assert.equal(runTopics(k, "read", {}).items[0].suggestion, undefined);
  assert.deepEqual(runTopics(k, "read", { ids: [topic().id] }).items[0], topic());
  const nextSession = createKFile();
  files.forEach((value, key) => nextSession.files.set(key, value));
  assert.deepEqual(runTopics(nextSession.k, "read", { ids: [topic().id] }).items[0], topic());
});

test("Unconfirmed answers remain blocking; explicit confirmation closes the topic", () => {
  const { k } = createKFile();
  const answered = topic({ answer: "需要登录", status: "answered" });
  assert.equal(runTopics(k, "update", { expectedUpdatedAt: runTopics(k, "read", {}).updatedAt, item: answered }).ok, true);
  assert.equal(runTopics(k, "readiness", { stage: "implementation" }).items.length, 1);
  assert.equal(runTopics(k, "update", { expectedUpdatedAt: runTopics(k, "read", {}).updatedAt, item: { ...answered, status: "confirmed" } }).ok, false);
  assert.equal(runTopics(k, "update", { expectedUpdatedAt: runTopics(k, "read", {}).updatedAt, item: { ...answered, status: "confirmed", confirmationEvidence: "用户回复：确认，运营人员必须登录。" } }).ok, true);
  assert.equal(runTopics(k, "readiness", { stage: "implementation" }).items.length, 0);
});

test("Invalid replacements never alter the stored discussion", () => {
  const { k, files } = createKFile();
  runTopics(k, "update", { expectedUpdatedAt: runTopics(k, "read", {}).updatedAt, item: topic() });
  const before = [...files];
  for (const item of [topic({ id: "../bad" }), topic({ status: "confirmed" }), topic({ options: [{ id: "a", label: "x" }] }), topic({ unknown: true })]) {
    assert.equal(runTopics(k, "update", { item }).ok, false);
    assert.deepEqual([...files], before);
  }
});

test("Read filters and pagination preserve site-wide questions and stage boundaries", () => {
  const { k } = createKFile();
  for (let i = 0; i < 4; i++) runTopics(k, "update", { expectedUpdatedAt: runTopics(k, "read", {}).updatedAt, item: topic({ id: `question.q${i}`, resolveBefore: i === 0 ? "implementation" : "production-release", scope: { artifact: i === 0 ? "site" : "app-plan", relatedIds: [], milestoneIds: [] } }) });
  const first = runTopics(k, "read", { maxItems: 2 });
  const second = runTopics(k, "read", { cursor: first.nextCursor, maxItems: 2 });
  assert.equal(first.total, 4); assert.equal(second.nextCursor, null);
  assert.equal(new Set([...first.items, ...second.items].map(item => item.id)).size, 4);
  assert.equal(runTopics(k, "read", { artifact: "blueprint" }).items.length, 1);
  assert.equal(runTopics(k, "readiness", { stage: "app-plan-creation" }).items.length, 0);
  assert.equal(runTopics(k, "readiness", { stage: "implementation" }).items.length, 1);
  assert.equal(runTopics(k, "readiness", { stage: "production-release" }).items.length, 4);
});

test("Pagination errors identify the invalid field and allow a corrected read", () => {
  const { k, files } = createKFile();
  const items = Array.from({ length: 31 }, (_, i) => topic({ id: `question.q${String(i).padStart(2, "0")}` }));
  files.set("_ai/site-development/planning-topics.json", JSON.stringify({ siteId: "site-1", items, updatedAt: null }));
  const before = [...files];
  const failed = runTopics(k, "read", { ids: null, status: null, artifact: null, cursor: 0, maxItems: 50 });
  assert.equal(failed.error.code, "INVALID_INPUT");
  assert.match(failed.error.message, /maxItems.*1 and 30.*50/);
  for (const maxItems of [0, 31, 1.5, "20", null, NaN, Infinity]) {
    assert.match(runTopics(k, "read", { maxItems }).error.message, /maxItems/);
  }
  for (const cursor of [-1, 0.5, "0", null, NaN, Infinity]) {
    assert.match(runTopics(k, "read", { cursor }).error.message, /cursor/);
  }
  const first = runTopics(k, "read", {});
  assert.equal(first.items.length, 20);
  assert.equal(first.nextCursor, 20);
  assert.equal(runTopics(k, "read", { maxItems: 1 }).items.length, 1);
  const page = runTopics(k, "read", { cursor: 0, maxItems: 30 });
  const last = runTopics(k, "read", { cursor: page.nextCursor, maxItems: 30 });
  assert.deepEqual([...page.items, ...last.items].map(item => item.id), items.map(item => item.id));
  assert.equal(last.nextCursor, null);
  assert.deepEqual([...files], before);
});

test("Corrupt or foreign topic storage fails closed and is never overwritten", () => {
  const { k, files } = createKFile();
  for (const content of ["{bad", JSON.stringify({ siteId: "other", items: [], updatedAt: null })]) {
    files.set("_ai/site-development/planning-topics.json", content);
    assert.equal(runTopics(k, "readiness", { stage: "implementation" }).error.code, "CORRUPT_STORE");
    assert.equal(runTopics(k, "update", { expectedUpdatedAt: runTopics(k, "read", {}).updatedAt, item: topic() }).ok, false);
    assert.equal(files.get("_ai/site-development/planning-topics.json"), content);
  }
});

test("Panel answers preserve topic context and never change planning", () => {
  const { k, files } = createKFile();
  const original = topic();
  runTopics(k, "update", { expectedUpdatedAt: runTopics(k, "read", {}).updatedAt, item: original });
  let current = runTopics(k, "read", { ids: [original.id] });
  const saved = runTopics(k, "answer", { id: original.id, answer: "选择分类", confirm: false, expectedUpdatedAt: current.updatedAt });
  assert.equal(saved.ok, true);
  assert.equal(saved.item.status, "answered");
  assert.deepEqual(saved.item.scope, original.scope);
  assert.equal(saved.item.question, original.question);
  current = runTopics(k, "read", { ids: [original.id] });
  const confirmed = runTopics(k, "answer", { id: original.id, answer: "选择分类", confirm: true, expectedUpdatedAt: current.updatedAt });
  assert.equal(confirmed.ok, true);
  assert.equal(confirmed.item.status, "confirmed");
  assert.ok(confirmed.item.confirmationEvidence);
  assert.deepEqual([...files.keys()], ["_ai/site-development/planning-topics.json"]);
});
test("Panel refuses stale and invalid answers without overwriting topics", () => {
  const { k, files } = createKFile();
  runTopics(k, "update", { expectedUpdatedAt: runTopics(k, "read", {}).updatedAt, item: topic() });
  const current = runTopics(k, "read", {});
  const before = [...files];
  for (const input of [
    { id: topic().id, answer: "answer", confirm: true, expectedUpdatedAt: "stale" },
    { id: topic().id, answer: "", confirm: true, expectedUpdatedAt: current.updatedAt },
    { id: "missing", answer: "answer", confirm: true, expectedUpdatedAt: current.updatedAt }
  ]) { assert.equal(runTopics(k, "answer", input).ok, false); assert.deepEqual([...files], before); }
});

test("Stale AI replacement cannot erase a panel confirmation; fresh merge succeeds", () => {
  const { k, files } = createKFile();
  assert.equal(runTopics(k, "update", { item: topic() }).ok, true);
  const old = runTopics(k, "read", { ids: [topic().id] });
  assert.equal(runTopics(k, "answer", { id: topic().id, answer: "用户确认需要登录", confirm: true, expectedUpdatedAt: old.updatedAt }).ok, true);
  const confirmedFiles = [...files];
  const staleItem = { ...old.items[0], context: "AI补充说明" };
  for (const token of [undefined, null, old.updatedAt]) {
    const result = runTopics(k, "update", { item: staleItem, expectedUpdatedAt: token });
    assert.equal(result.error.code, "TOPICS_CHANGED");
    assert.deepEqual([...files], confirmedFiles);
  }
  const fresh = runTopics(k, "read", { ids: [topic().id] });
  const merged = runTopics(k, "update", { item: { ...fresh.items[0], context: staleItem.context }, expectedUpdatedAt: fresh.updatedAt });
  assert.equal(merged.ok, true);
  assert.equal(merged.item.status, "confirmed");
  assert.equal(merged.item.answer, "用户确认需要登录");
  assert.equal(merged.item.confirmationEvidence, fresh.items[0].confirmationEvidence);
  assert.deepEqual(merged.item.scope, old.items[0].scope);
});

test("Revision advances within one millisecond and duplicate creation cannot replace a topic", (t) => {
  t.mock.method(Date, "now", () => 1700000000000);
  const { k, files } = createKFile();
  const created = runTopics(k, "update", { item: topic() });
  const changed = runTopics(k, "update", { item: topic({ context: "changed" }), expectedUpdatedAt: created.updatedAt });
  assert.equal(changed.ok, true);
  assert.notEqual(changed.updatedAt, created.updatedAt);
  const before = [...files];
  assert.equal(runTopics(k, "update", { item: topic() }).error.code, "TOPICS_CHANGED");
  assert.equal(runTopics(k, "answer", { id: topic().id, answer: "stale", confirm: true, expectedUpdatedAt: created.updatedAt }).error.code, "TOPICS_CHANGED");
  assert.deepEqual([...files], before);
  const noop = runTopics(k, "update", { item: changed.item, expectedUpdatedAt: changed.updatedAt });
  assert.equal(noop.changed, false);
  assert.equal(noop.updatedAt, changed.updatedAt);
});
