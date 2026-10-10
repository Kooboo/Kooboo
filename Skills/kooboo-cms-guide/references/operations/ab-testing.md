# Site Operations - A/B Testing

## Page Purpose

- Admin pages include the experiment list, experiment creation/editing, and reports.
- A/B testing assigns requests for the same site object to a control group or variants and observes exposure and effect metrics in the current report. It does not copy an independent site or act as publication approval or content-version management.
- The interface supports experiments for pages, views, layouts, HTML blocks, Labels, text content, and products. An object being selectable does not mean its replacement path is verified in every rendering path; especially for layout experiments, a successful save alone cannot establish that the frontend applies the variant.

## Activation Conditions and Time

- The site-level A/B-test switch and an individual experiment's status are independent. An experiment can affect frontend rendering only when the master switch is on, the experiment is Running, and current UTC time is within its start and end range.
- The page date picker uses browser local time and converts it to UTC on submit; when troubleshooting across time zones, check browser display time and actual UTC time together.
- Draft, Paused, and Completed experiments do not participate in normal assignment. Saving an experiment proves only that its definition was persisted, not that it started or generated exposure.
- Two running experiments still within their valid periods cannot start for the same object, preventing experiments from competing for one rendering object.

## Traffic Allocation and Conditions

- Each variant has a percentage weight; the control-group weight is 100% minus the sum of variant weights. The experiment cannot be saved when variant weights exceed 100%.
- Conditions combine with AND, so a visitor must satisfy all conditions to enter. Conditions such as network type, round-trip latency, pixel density, and viewport width depend on client-provided hints; missing fields must not be assumed to satisfy a condition.
- Grouping uses the current request's analytics identifier and stores assignment by statistical day. The same visitor usually keeps the group on repeated visits on one day but may be reassigned across days; the experiment group is not a long-term fixed user attribute.
- Pages, views, HTML blocks, and Labels use object-level variants; text content and products use field overrides. A field override replaces only when a value exists for the current language; otherwise the original field remains.

## Report Basis and Limitations

- The current feature can record exposure, but conversion and revenue data cannot yet be treated as a complete business loop. Reports therefore cannot directly be authoritative measurements of order, registration, or revenue conversion.
- Reports include only statistical records that can correspond to current variant configuration; control-group statistics may not fully enter the current report. A missing control group does not prove it received no traffic.
- Winner is simply the item with the highest conversion rate in current report data, not a long-term optimum automatically confirmed by rigorous statistical testing.
- Confidence uses simplified data-volume and lift rules, not a formal p-value, Z-test, or Bayesian conclusion; the p-value shown in the interface is currently a placeholder.
- Need More Data is mainly based on whether total exposure reaches 1000. Reaching the threshold does not mean business significance, and failing to reach it does not mean the experiment is misconfigured.

## Changes, Stopping, and Deletion

- Pausing an experiment or turning off the master switch stops later requests from being assigned by the experiment; it does not retract content already shown or cancel external side effects already triggered by users.
- Changing a running experiment changes later assignment and report interpretation. Record the change time when comparing data and avoid combining different configuration phases into one conclusion.
- Deleting an experiment deletes only its definition. Do not assume historical statistic files are deleted at the same time, and it does not retract exposure, conversions, or business operations already occurred.
