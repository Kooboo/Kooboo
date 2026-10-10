# Site Observability - User Behavior Analysis

## Page Purpose

- Admin page: `/system/user-behavior-analysis`
- This page summarizes recorded page visits, transitions between pages, and configured goals into conversion overviews, path graphs, funnels, frequent transitions, and drop-off nodes.
- It is statistical analysis based on events captured by Kooboo, not browser recording, per-session replay, or a complete behavior trail for every real user.

## Enabling and Data Sources

- Enabling behavior analysis also enables access logs because page-path statistics depend on new frontend access records. It does not reconstruct historical paths from existing content or old server logs.
- After disabling, no new page transitions or goal events are calculated; existing statistic files are not rewritten to zero, but the page stops producing new data.
- Page paths process only human visits identified as site pages. Bots do not enter user-path calculations.
- Adjacent pages are mainly inferred from same-site Referer. Without a usable Referer, the system may use the identified user's most recent recorded page as the previous node. The path is therefore a statistical inference, not guaranteed to match the strict order of browser-tab visits.

## Visitors, Visits, and Conversion

- Total visits counts visits to page nodes; unique visitors are deduplicated by Kooboo's analytics identifier. A multi-day range combines daily statistics, so one visitor may be counted again across days.
- Total goal users is the unique-user count reaching configured goals; total goals is the number of goal triggers. One user can trigger multiple goals or trigger repeatedly, so these are different metrics.
- Total conversion rate is users reaching any goal divided by visitors; a single goal's conversion rate is unique users for that goal divided by visitors.
- Goal status compares only current conversion rate with that goal's Warning and Risky thresholds: below Risky is risk, below Warning is warning. It is not an anomaly-detection model and does not explain why conversion fell automatically.

## Goal Configuration

- Goals can come from pages, public APIs, site events, or custom tracking events. A goal can be selected only from options identifiable on the current site; its display name can be edited without changing its underlying identity.
- A page goal completes when the corresponding page is visited; an API goal is recorded when the public API executes; site events such as e-commerce and custom events are recorded only when actually triggered while behavior analysis is enabled.
- A new goal affects only later identification and reporting for that goal; it does not reinterpret every past visit as goal completion.
- Deleting or changing a goal does not undo business operations already performed or change the page, API, or e-commerce flow itself. Changing thresholds changes only status judgment in reports.

## Path Graphs, Funnels, and Drop-Off

- The path graph shows later nodes from configured starting points and step groups; steps can allow multiple pages or goals. Configured steps define an analytical view only and do not restrict real navigation or create pages or routes.
- A single-path funnel counts only recorded paths matching the selected starting path and eventually reaching the goal. Changing path selection changes query conditions and does not rewrite original visits.
- Transition count is recorded transitions from a source node to a target; unique users are deduplicated users completing the transition. Repeated transitions by one user make the two different.
- Drop-off means users in the currently constructed path who did not continue to the next node; it does not mean permanent departure, business failure, or dissatisfaction.
- Popular goals show only the top five sorted by completion count; a missing goal does not mean it has no completions.
