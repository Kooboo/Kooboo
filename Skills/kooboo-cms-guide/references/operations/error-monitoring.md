# Site Observability - Error Monitoring

## Page Purpose

- Admin pages include the error overview and error-log details.
- This module aggregates Page, API, KScript, and Custom errors already recorded at site runtime and shows counts, affected users, source composition, trends, type distribution, and frequent errors.
- It is not the browser console, external APM, all server logs, or an admin-operation audit. Problems that never enter Kooboo's error-recording flow do not appear here automatically.

## Dates and Statistical Basis

- All cards, trends, and lists are limited by the current date range. A statistical day is divided at 4:00 AM server time, so records near that time may belong to the previous statistical day.
- Total Errors is the number of recorded errors in the selected dates; repeated errors on one path are counted repeatedly.
- Error Rate is error count divided by human page visits in access logs. When access logs are disabled or the denominator is missing, error rate may be 0; this does not prove the site has no errors.
- Affected Users adds each day's number of analysis identifiers with errors across days. It is not logged-user count or strictly deduplicated people for the whole date range; one visitor failing across days may be counted repeatedly.
- Human/Bot source share compares human and bot requests only among recorded errors and does not represent total site traffic composition.

## Types, Trends, and Frequent Errors

- Page means page-request errors, API means public-interface execution errors, KScript means script-execution errors, and Custom means custom errors actively recorded by business code.
- Type distribution calculates share by error count across the four types; it describes record-count structure, not severity or business-impact ranking.
- When the date span exceeds 20 statistical days, the trend graph selects at most 20 dates from the range rather than combining all dates into 20 periods. Trends are therefore suitable for direction, not a complete day-by-day export.
- Frequent errors aggregate by “error type + status code + URL” and sort by count, showing at most the top 16. They are not grouped by exception stack or error message, so different internal causes on one URL may be combined.
- Frequent ranking is limited by date range and Top count; not appearing in the ranking does not mean an error does not exist.

## Error Log Details

- The details page reads individual records paginated by error type and date and shows time, URL, status code, and message; Custom errors can also show record level.
- Current details provide Page, API, and Custom category switches. KScript data enters the overview and trends, but the current details interface has no independent KScript switch.
- A status code describes only part of the request result. Business code may return success while recording a custom error, or a downstream failure may produce no Kooboo error record.
- Log writing is asynchronous, so a just-occurred problem may be absent briefly. Empty results can also come from type, date, or permission mismatch and do not directly prove that no problem occurred.

## Diagnostic Boundaries

- Error Monitoring is a read-only observation entry; it does not automatically repair pages, APIs, or scripts and does not reexecute failed requests because they are viewed or filtered.
- During troubleshooting, first confirm error type, date, and URL, then combine the corresponding page, API, KScript logs, and actual business results. A drop in error count alone does not prove the root cause was fixed; cache, lower traffic, or changed collection conditions can also change the number.
