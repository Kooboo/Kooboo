import assert from "node:assert/strict";
import fs from "node:fs";
import test from "node:test";

const script = fs.readFileSync(new URL("./search.js", import.meta.url), "utf8");

function run(model) {
  const k = {
    request: {
      model,
      get(name) {
        return this.model[name];
      },
    },
  };
  return new Function("k", script)(k);
}

test("help exposes the current Site Building design modes", () => {
  const result = run({ mode: "help" });

  assert.deepEqual(result.modes, [
    "page-plan",
    "design-system",
    "page-item",
    "view-batch",
    "prompt-recommendations",
    "search",
    "stack",
    "help",
  ]);
});
