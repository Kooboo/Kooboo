# Engineering Quality and Acceptance

Completion for client-rendered Vue is not “the objects have been created.” Page, Style, and Script dependencies must be closed, the actual Page URL must load and complete the target interactions, and frontend capabilities must not be misrepresented as server-side security guarantees.

## State and derived data

- Keep raw shared data in the store; derive search, filtering, sorting, and statistics with `computed()`;
- Use local `ref()` / `reactive()` for temporary component state instead of putting all state in the store;
- Use domain verbs for actions, such as `createTask`, `updateTask`, and `loadTasks`;
- Access refs with `.value` in JavaScript and do not write `.value` in templates;
- Use stable domain IDs for `v-for`; array-index keys are prohibited;
- Copy arrays before sorting to avoid accidentally modifying the store.

## Data sources

### Static data and localStorage

- Refresh behavior for static data must be explicit;
- localStorage keys include the app name and schema version;
- Use `try/catch` with `JSON.parse` and validate the data shape;
- Provide corruption fallback and reset;
- Do not continue showing “saved” after a write fails;
- Do not store secrets, long-lived tokens, real credentials, or unnecessary sensitive information.

### HTTP API

- URL, method, headers, body, and response structure come from the real contract; do not guess;
- Check `response.ok` and never treat 4xx/5xx as successful JSON;
- Use explicit loading / error / empty / success states;
- Use `AbortController` as needed when a component unmounts or a request is replaced;
- Disable duplicate submissions while saving and preserve user input on failure;
- 401/403 reflect only the server result; hiding a frontend button cannot replace permission control;
- Do not swallow errors or return fake success data from a service.

## Forms and business actions

- Apply `trim()` consistently at the submission boundary;
- Validate required fields, dates, numeric ranges, and enumerations before the action;
- Place errors near the relevant control and associate them with `aria-describedby`;
- Preserve the draft after a failed submission; clean up and close only after success;
- Reuse a form component when create and edit fields are the same;
- Destructive actions need confirmation and clear result feedback;
- Do not fake a network with random delays; when loading must be demonstrated, use a repeatable, clearly marked state.

## Dialogs and focus

A dialog must at least provide:

- `role="dialog"`;
- `aria-modal="true"`;
- `aria-labelledby` pointing to a visible title;
- Focus on an appropriate control after opening;
- Close on Escape;
- Remove keyboard listeners on unmount;
- Return focus to the trigger where possible after closing;
- An overlay-close policy must not unexpectedly lose unsaved content.

Complex dialogs need a focus trap. With a UI framework, prefer its official Dialog and verify the same focus and close behavior.

## Accessibility

- Every form control has a visible label, with matching `for` and `id`;
- Write `type="button"` or `type="submit"` explicitly on buttons;
- Icon buttons have `aria-label`; decorative icons have `aria-hidden="true"`;
- Use native `<button>` for actions instead of simulating it with a clickable `<div>`;
- Do not express state through color alone;
- Use `aria-busy` for loading regions and `aria-live` for asynchronous messages as needed;
- Touch targets are about 40×40 CSS pixels or larger;
- Focus is visible;
- Respect `prefers-reduced-motion`.

## Responsive behavior

- Main actions remain possible at 320px width;
- On narrow screens, tables use horizontal scrolling, cards, or collapsed fields instead of cutting off key actions;
- The sidebar has an explicit mobile open/close behavior;
- Dialogs stay within the viewport, can scroll, and keep the close button visible;
- Long titles, labels, URLs, and errors have wrapping or truncation strategies;
- Check phone, tablet, and desktop widths.

## Error handling

- Set `app.config.errorHandler` in `main.js`;
- Keep an explicit `name` on every component;
- Restore loading in `finally` for asynchronous actions;
- Empty `catch {}` blocks are not allowed;
- On startup failure, keep actionable console information and show a static fallback on the page;
- A complex module failure should not take down unrelated features; split Scripts by responsibility.

## Security boundaries

- Do not use `v-html` for untrusted content;
- Render user text through ordinary Vue interpolation;
- Do not concatenate untrusted text into templates, event expressions, styles, or script URLs;
- Add `rel="noopener noreferrer"` to external links using `target="_blank"`;
- Restrict URL inputs to allowed protocols and purposes;
- Do not keep server secrets in frontend source;
- Frontend validation, route guards, and hidden buttons are not authentication or authorization implementations.

## Module and dependency consistency

- Names do not contain `/`; each newly created Script URL equals `/` plus its exact Name, while existing routes are preserved unless migration is explicitly in scope;
- The object's create/update operation matches the actual write;
- Import, export, component registration, template tags, event names, and action names match;
- No circular imports exist;
- Vue uses the unpkg Vue 3 full-build channel recommended by the official documentation;
- Tailwind uses the Kooboo-hosted v3 CDN;
- Vue uses the specified official channel and Tailwind is fixed at v3; UI framework, icon, and other CDN assets use a verified version strategy;
- The page does not load a second Vue;
- JavaScript conditional classes use complete static strings rather than concatenated Tailwind fragments;
- No empty store, service, or component is created.

## Preflight

- Use focused mode for local changes and capability mode for complete applications;
- Artifacts cover the Page, Style, and Script objects actually changed;
- Do not pass `layoutName` for a Page without a Layout;
- Handle active BLOCKERs rather than quoting PASS alone;
- Preflight PASS does not replace the required read-only Preview diagnostics; browser operation remains opt-in.

## Acceptance through the actual Page URL

Browser acceptance covers at least:

1. Page, Style, Script, CDN, and API requests;
2. Vue mount and the default interface;
3. Navigation or screen switching;
4. Lists, empty states, and error states;
5. Form validation, submission, and duplicate-submission protection;
6. Loading, success, and failure feedback;
7. localStorage restore after refresh and corruption fallback when used;
8. HTTP failure, 401/403, cancellation, or timeout policy when used;
9. Keyboard, focus, Escape, and labels;
10. Phone, tablet, and desktop widths;
11. No module-resolution, template-compilation, or runtime errors in the console.

When actual CDN or API network requests are unavailable, do not claim completion by checking only the source URLs.

## Final checklist

- [ ] Page is complete HTML, with no Layout binding or server-template directives
- [ ] Name, URL, object type, and dependencies are correct; each new Script uses `/<Name>`
- [ ] Vue full build and Tailwind use the specified official CDN channels, with a clear version strategy for external dependencies
- [ ] The Page does not load a second Vue
- [ ] `main.js`, `app.js`, store, service, and component responsibilities are clear
- [ ] A service is created only when a real HTTP call exists
- [ ] Components declare name, props, emits, setup, and a static template
- [ ] State, forms, errors, empty states, and loading work
- [ ] Security and permissions are not implemented only in the frontend
- [ ] Responsive and keyboard interactions work
- [ ] Preflight has no active BLOCKER
- [ ] Actual Page URL, Network, and Console have been verified

## AI delivery behavior

When the user authorizes writes to a Kooboo Site:

1. Confirm the object scope and create/update operation first;
2. Call the corresponding Functions to create or update objects;
3. Read back key objects after every write batch;
4. Complete Preflight and actual-URL acceptance;
5. Report objects, entry point, data sources, and verification results instead of pasting complete source by default.

Only when the user explicitly requests “output code only, do not write to the Site” should you list each object type, Name, and URL and output complete code.
