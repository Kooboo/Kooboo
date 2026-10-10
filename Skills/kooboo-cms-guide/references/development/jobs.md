# Development Resources - Scheduled Jobs

## Page Purpose

- Admin page: `/development/jobs`
- This page creates and manages KScript jobs executed by the site. Jobs can run automatically after their start time and can also be triggered manually in the admin.
- Job code has real side effects corresponding to its KScript logic and may read or write site data or call external services; Run must not be treated as a read-only test.

## Scheduling Settings

- A job contains a name, start time, enabled status, repeat flag, repeat interval, and execution code. Existing job names cannot be changed directly in the current interface.
- The page date picker inputs and displays the user's browser local time; on submit it converts the value to UTC and saves it. The automatic scheduler compares the current UTC time with the saved start time to determine whether the start time has arrived.
- The same absolute moment therefore displays as different local times in admin sessions in different time zones. When investigating early or late execution, check the browser time zone, saved time, and server's current UTC time together; do not treat the page value directly as UTC.
- Enable controls eligibility for automatic scheduling; disabled jobs are not scheduled automatically. Manual Run uses an independent entry point, so disabled must not be understood as an absolute ban on every execution.
- A non-repeating job runs once automatically when the current UTC time reaches its saved start time and is marked complete. Editing a job resets its completed status, so a one-time job that already ran may become eligible again after editing.
- Repeating jobs use intervals of seconds, minutes, hours, days, weeks, or months. Month is calculated as a fixed 30-day interval, not the same calendar date each month.
- Automatic scheduling also requires the site to be published. Saving a job proves only that its definition was persisted, not that the server scheduler is enabled, the site is published, or the code will ultimately succeed.

## Manual Run and Status

- Clicking Run submits an asynchronous execution only; it does not mean the job completed successfully. When the API returns success, background work may have just started, still be running, or fail later.
- Running in the list means the current process recorded that the job is executing. Refreshing the list observes only a momentary state and cannot replace execution-result logs.
- The scheduler avoids starting the same job concurrently during automatic execution, but manual triggers and external side effects should still be designed for repeatable execution. Do not rely on the UI button to provide business idempotency inherently.

## Completion and Failure Logs

- Completed and Failed separately show up to the latest 100 corresponding execution results for the site, including job name, trigger description, execution time, and return or exception message.
- Automatic scheduling and manual triggers write logs after execution ends. To judge the result, inspect the corresponding log and verify target data or external-system status instead of only the button message or list status.
- Completed means the job code did not throw an unhandled exception; it does not necessarily mean downstream business succeeded. For example, if a remote service returns a business failure without the code throwing, the job must still check and record it.
- Logs and job definitions are stored separately. Deleting a job removes its future scheduling definition but does not automatically undo site data, external requests, or historical business results already produced.

## Change and Safety Boundaries

- After changing the start time, interval, enabled status, or code, first run it manually with controlled data, then check Completed/Failed logs and actual side effects.
- Deleting or disabling a job cannot cancel external requests already sent and does not guarantee that currently running code can be stopped. For payments, email, inventory, or batch updates, job code must handle repeated execution, timeouts, failed retries, and audit records itself.
