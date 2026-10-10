import { readFileSync } from "node:fs";

const script = readFileSync(new URL("./planning-topics.js", import.meta.url), "utf8");

export function runTopics(k, action, data, format) {
  const request = k.request;
  try {
    k.request = { action, siteId: "site-1", data, format };
    return new Function("k", script)(k);
  } finally { k.request = request; }
}

export function createKFile() {
  const files = new Map();
  const k = {
    request: {},
    file: {
      exists(path) { return files.has(path); },
      read(path) { return files.get(path); },
      write(path, value) { files.set(path, String(value)); }
    }
  };
  k.ai = { readPlanningTopicReadiness(stage) { return runTopics(k, "readiness", { stage }, "json"); } };
  return { k, files };
}

export function topic(overrides = {}) {
  return {
    id: "question.order-auth", kind: "open-question", question: "订单管理是否需要登录？",
    context: "影响订单访问权限。", suggestion: "运营人员登录后使用。", options: [],
    answer: null, status: "open", confirmationEvidence: null, impact: "high", resolveBefore: "app-plan-creation",
    scope: { artifact: "blueprint", relatedIds: ["module.order"], milestoneIds: [] }, ...overrides
  };
}
