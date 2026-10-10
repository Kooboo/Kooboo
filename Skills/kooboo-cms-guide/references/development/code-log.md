# Development Resources - Code Logs

## Page Purpose

- Admin page: `/development/code-log`
- This page views code logs actively written by site KScript to troubleshoot business-code execution. It is not a site admin-operation log and does not guarantee that every runtime error is collected.
- Each record contains level, time, Trace ID, category, and message. No record is automatically produced when code writes no log, logging is disabled, or the log level filters it out.

## Enabling and Scope

- The site-level Enable switch determines whether new code logs enter the recording flow. Enabling code logs does not backfill executions that were not previously recorded.
- Logs are also limited by the site's minimum level; messages below it are not saved. All, Debug, Information, Warning, Error, and Critical on the page are query conditions for saved records and do not change code execution.
- Log writing is asynchronous, so the list may briefly lag after code runs. Repeated queries do not execute code again or generate new logs.

## Weeks, Filters, and Search

- Logs are stored partitioned by year and week, and the page lists at most the eight most recent available weeks. After a week is selected, time range, level, Trace ID, and keyword query only data in that week.
- Time range filters by the exact record time; level and Trace ID use exact matching. Keywords search only category and message, using contains matching.
- A Trace ID can associate multiple logs carrying the same identifier, but the same Trace ID is not a complete distributed call chain and does not prove every related system recorded it.
- An empty result can mean the week or filters are wrong, logs have passed retention, code wrote no log, the level setting filtered logs, or writing is still queued; it does not directly establish that code did not execute.

## Retention and Diagnostic Boundaries

- The site has a code-log retention-days setting, and the admin periodically attempts to clean expired logs; the page cannot restore cleared data. The selectable weeks also do not mean all historical data is retained permanently.
- Log messages are determined by business code and may contain request parameters, external responses, or exception details. Avoid logging sensitive data such as passwords, tokens, and payment information.
- When troubleshooting, first confirm the log switch and minimum level, then select the correct week and time range, and finally combine frontend requests, API responses, or task logs to judge the actual result. Seeing one Information log does not mean the entire business flow succeeded.
