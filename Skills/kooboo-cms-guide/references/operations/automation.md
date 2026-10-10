# Site Operations - Automation

## Page Purpose

- Admin page: `/system/automation`
- This page lists automation tasks registered by the current Kooboo service so an administrator can select one and execute it immediately.
- It is not a workflow designer and does not provide creation, editing, deletion, scheduling, or long-term execution history; scheduled code should use the Jobs page.

## Task List and Modification Hints

- Each task shows a name, description, and May modify site marker. Available tasks come from the running service and are not data created by the user on this page.
- Only one task can be selected for execution at a time.
- May modify site is a capability hint declared by the task, not proof that this execution modified anything or a list of actual changes.
- No modification marker means only that the task declares itself a checking task. Understand what it checked from the task description and output; do not extend an empty result into “the whole site has no problems.”

## Execution and Results

- Clicking Execute creates an asynchronous session and immediately returns a session identifier; a successful button request does not mean the task has completed or succeeded.
- The page then polls incremental results, up to 50 per request, until the session ends and remaining results are retrieved.
- Result rows are issues or explanations actively output by the task in this session, not a persisted audit log, and may not contain complete before-and-after differences.
- Task execution completed means only that the task thread ended. A runtime exception may also end the session without a clear error result, so a completion icon alone cannot establish success.
- No results usually displays No problems found, but this means only within the rules, resources, and execution environment actually covered by the task.

## Session and Side-Effect Boundaries

- Execution sessions are held in the service process and have an expiration. After a service restart, session expiration, or process-state change, the page may not be able to retrieve earlier results.
- A modifying task may directly change site images, styles, or other resources; judge final impact from site logs, object content, page preview, and necessary publication verification.
- Automation has no unified preview, transaction rollback, or “commit only when all succeed” guarantee. Confirm the description and impact before running a modifying task, then inspect actual changes rather than only the task declaration.
