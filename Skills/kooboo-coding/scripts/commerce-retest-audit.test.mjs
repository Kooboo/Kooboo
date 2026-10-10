import assert from "node:assert/strict";
import test from "node:test";
import { auditConversation } from "./commerce-retest-audit.mjs";

test("audit reports failed Preflight completion claims and unsafe commerce code", () => {
  const conversation = {
    title: "fixture",
    messages: [
      { role: "user", contents: [{ $type: "text", text: "修复分类和支付" }] },
      {
        role: "assistant",
        contents: [
          {
            $type: "functionCall",
            callId: "write-1",
            name: "update_text_object",
            arguments: {
              newContent:
                "k.commerce.order.pay(body.orderId); list.innerHTML = orders.map(order => order.title)",
            },
          },
          {
            $type: "functionCall",
            callId: "pf-1",
            name: "run_skill_script",
            arguments: { scriptName: "scripts/preflight-scan.js" },
          },
        ],
      },
      {
        role: "tool",
        contents: [
          { $type: "functionResult", callId: "write-1", result: "Update success" },
          { $type: "functionResult", callId: "pf-1", result: { result: "FAIL" } },
        ],
      },
      { role: "assistant", contents: [{ $type: "text", text: "支付已经接入，全部完成。" }] },
    ],
  };

  const result = auditConversation(conversation, "fixture.json");
  assert.equal(result.preflight.verdicts.FAIL, 1);
  assert.equal(result.preflight.completionClaimsAfterUnresolvedFailure, 1);
  assert.equal(result.unsafePatterns.directOrderPay, 1);
  assert.equal(result.unsafePatterns.persistedInnerHtml, 1);
});
