---
name: browser
description: Browser control for the expanded right-side site preview, including observation, interaction, navigation, upload, and verification.
allowed-tools: read_preview_diagnostics read_preview_network browser_control navigate_iframe set_iframe_device observe_iframe observe_iframe_changes set_iframe_files find_iframe_elements inspect_iframe_element act_on_iframe_element wait_for_iframe
metadata:
  explicit-only: true
---

# Browser control

The user explicitly selected `/browser`. This is an instruction to perform the requested user-visible task through the browser UI in the expanded right-side site preview on `/ai-chat/overview`.

## Mandatory intent

- Treat `/browser` as explicit browser-operation intent, even when the user's text could also be interpreted as a content, database, configuration, commerce, or account task.
- Start with a browser observation or navigation tool. Do not start by listing or mutating backend records.
- Complete actions such as login, form filling, settings changes, cart operations, navigation, uploads, and verification through the visible UI.
- If the required UI is missing, broken, or unavailable, explain that limitation. Do not make the expected result appear successful through a backend shortcut.

## Browser workflow

- Use `browser_control` as the unified entry point, or use the specialized iframe tools when more precise.
- Begin with `observe_iframe`, or use `find_iframe_elements` with role, name, text, or tag filters.
- Use only `nodeId` values returned by the latest observation. Prefer semantic queries over guessed CSS selectors.
- Supported interactions include click, fill, select, check, keypress, scroll, and hover.
- Use `navigate_iframe` for a same-site URL or preview reload, and `set_iframe_device` for responsive checks.
- Use `wait_for_iframe` for one explicit readiness, element, text, or URL condition wait.
- After an action, prefer `observe_iframe_changes` for incremental results.
- For file upload, call `ask_user_input` with `kind: "upload"` and `upload.target: "iframe"`, then pass the approved opaque `fileId` values to `set_iframe_files`.

## Navigation and failure boundaries

- Navigation, reload, and framework re-render invalidate all previous `nodeId` values.
- When an action returns `navigated: true` and `agentReady: false`, the action already succeeded and triggered navigation. Do not repeat it. Call `wait_for_iframe` once with `state: "ready"`, then observe the new document.
- Browser and iframe failures are returned as non-terminal structured results with `status: "error"`, a stable `code`, and one human-readable `message`. Read the reason and continue reasoning; the current task is not automatically terminated by that tool result.
- Do not automatically retry browser commands, reload as recovery, or repeat an action whose result is uncertain.
- If the workspace, iframe Agent, or target element is unavailable, report that browser control cannot continue.

## Backend boundary

Browser control is UI observation and interaction. It does not authorize backend mutation as a substitute for UI work.

- Backend source, configuration, logs, and data may be read only to diagnose what the browser shows.
- Do not use `run_kscript`, custom functions, database writes, content mutations, or backend mutation APIs to perform the user's browser task.
- Never reset or replace a password, manufacture login/session state, or directly insert cart, order, customer, settings, or content records.
- If login fails, ask the user to confirm credentials, register, verify the account, or take over. Encrypted passwords cannot be recovered.
- Only modify source code when the user explicitly asks to implement or fix code. After that change, verify it through the browser.
