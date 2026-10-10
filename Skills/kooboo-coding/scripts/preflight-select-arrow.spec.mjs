import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { dirname, resolve } from "node:path";
import { fileURLToPath } from "node:url";
import test from "node:test";

const scriptsDir = dirname(fileURLToPath(import.meta.url));
const packageDir = resolve(scriptsDir, "..");
const scannerSource = readFileSync(resolve(scriptsDir, "preflight-scan.js"), "utf8");

function runScanner(content) {
  const parameters = {
    taskType: "style",
    layers: ["frontend", "design", "forms"],
    mode: "capability",
    artifacts: [{ name: "theme", objectType: "Style", content }],
  };
  const k = {
    request: {
      get(name) {
        return parameters[name];
      },
    },
    ai: {
      callFunction(name, args) {
        if (name === "read_skill_resource") {
          return readFileSync(resolve(packageDir, args.resourceName), "utf8");
        }
        throw new Error(`Unexpected function call: ${name}`);
      },
    },
  };
  return new Function("k", scannerSource)(k);
}

function selectFinding(content) {
  return runScanner(content).findings.find((item) => item.id === "PF-FRONTEND-007");
}

test("reports the common shared input/select compact-padding pattern", () => {
  const finding = selectFinding(`
    .filter-field select,
    .filter-field input[type="search"] {
      border: 1px solid #999;
      background: #fff;
      padding: .5rem .6rem;
      min-height: 40px;
    }
  `);
  assert.ok(finding, "expected PF-FRONTEND-007");
  assert.equal(finding.severity, "MEDIUM");
});

test("reports the shared full-width form-control pattern from the incident", () => {
  const finding = selectFinding(`
    .form-field input,
    .form-field select,
    .form-field textarea {
      width: 100%;
      border: 1px solid #999;
      background-color: #fff;
      padding: .6rem .7rem;
    }
  `);
  assert.ok(finding, "expected PF-FRONTEND-007");
});

test("allows a dedicated same-scope select correction", () => {
  const finding = selectFinding(`
    .filter-field select,
    .filter-field input[type="search"] {
      border: 1px solid #999;
      background-color: #fff;
      padding: .5rem .6rem;
    }
    .filter-field select {
      padding-inline-end: 2rem;
    }
  `);
  assert.equal(finding, undefined);
});

test("allows a complete custom-arrow strategy", () => {
  const finding = selectFinding(`
    select {
      appearance: none;
      background-image: url("data:image/svg+xml,%3Csvg/%3E");
      background-repeat: no-repeat;
      background-position: right .75rem center;
      padding-inline-end: 2.25rem;
    }
  `);
  assert.equal(finding, undefined);
});

test("allows the incident fix with global arrow and scoped background-color rules", () => {
  const finding = selectFinding(`
    select {
      appearance: none;
      background-image: url("data:image/svg+xml,%3Csvg/%3E");
      background-repeat: no-repeat;
      background-position: right .7rem center;
      padding-right: 2.2rem;
    }
    .filter-field select {
      border: 1px solid #999;
      background-color: #fff;
      padding: .5rem 2.2rem .5rem .6rem;
    }
    .form-field select {
      border: 1px solid #999;
      background-color: #fff;
      padding: .6rem 2.2rem .6rem .7rem;
    }
  `);
  assert.equal(finding, undefined);
});

test("reports an explicitly flush custom arrow", () => {
  const finding = selectFinding(`
    select {
      appearance: none;
      background-image: url("data:image/svg+xml,%3Csvg/%3E");
      background-position: right center;
      padding-inline-end: 2rem;
    }
  `);
  assert.ok(finding, "expected PF-FRONTEND-007");
});

test("reports a more-specific background shorthand that clears the arrow", () => {
  const finding = selectFinding(`
    select {
      appearance: none;
      background-image: url("data:image/svg+xml,%3Csvg/%3E");
      background-position: right .75rem center;
      padding-right: 2rem;
    }
    .form-field select {
      background: #fff;
    }
  `);
  assert.ok(finding, "expected PF-FRONTEND-007");
});

test("allows a shorthand followed by an explicit replacement image", () => {
  const finding = selectFinding(`
    select {
      appearance: none;
      background-image: url("data:image/svg+xml,%3Csvg/%3E");
      background-position: right .75rem center;
      padding-right: 2rem;
    }
    .form-field select {
      background: #fff;
      background-image: var(--form-select-arrow);
      background-position: right .75rem center;
      padding-right: 2rem;
    }
  `);
  assert.equal(finding, undefined);
});

test("does not infer a base defect from state-only background changes", () => {
  const finding = selectFinding(`
    select {
      appearance: none;
      background-image: url("data:image/svg+xml,%3Csvg/%3E");
      background-position: right .75rem center;
      padding-right: 2rem;
    }
    .form-field select:disabled {
      background: #eee;
    }
  `);
  assert.equal(finding, undefined);
});

test("does not treat UI framework class names as native select selectors", () => {
  const finding = selectFinding(`
    .el-select,
    .ui-select,
    .form-select {
      border: 1px solid #999;
      background: #fff;
      padding: .5rem .5rem;
    }
  `);
  assert.equal(finding, undefined);
});

test("allows an untouched browser-native select", () => {
  const finding = selectFinding(".form { display: grid; gap: 1rem; }");
  assert.equal(finding, undefined);
});

test("does not guess about CSS variables or wrapper-component arrow contracts", () => {
  const finding = selectFinding(`
    .field select,
    .field input {
      border: 1px solid var(--field-border);
      background-color: var(--field-bg);
      padding: var(--field-padding);
    }
    .select-shell select {
      appearance: none;
    }
    .select-shell::after {
      content: "";
      background: var(--select-arrow);
    }
  `);
  assert.equal(finding, undefined);
});

test("real scanner dispatch loads frontend rules and reports no invalid regex", () => {
  const result = runScanner(`
    .field select,
    .field input {
      border: 1px solid #999;
      background: #fff;
      padding: 8px 10px;
    }
  `);
  assert.ok(result.findings.some((item) => item.id === "PF-FRONTEND-007"));
  assert.equal(result.invalidRegexFindings, undefined);
  assert.match(result.structuralReceipt.ruleVersion, /^\d{4}-\d{2}-\d{2}\.\d+$/);
  assert.equal(result.structuralReceipt.ruleVersion, result.scanSummary.ruleVersion);
});
