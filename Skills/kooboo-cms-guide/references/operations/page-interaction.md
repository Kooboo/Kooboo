# Site Observability - Page Interactions

## Page Purpose

- Admin pages include the page-interaction overview and single-page anchor details.
- This module counts element triggers through tracking anchors in pages and shows counts, visit ratios, and trends by page or anchor. It is not a heatmap recording, arbitrary DOM-click collection, or complete user-session replay.
- After Page Interactions is enabled, Kooboo adds the collection script to frontend page headers; after disabling it, normal pages no longer receive the script. Enabling does not backfill past clicks.

## Tracking Anchors

- An anchor is a tracking name written to a page element. Only elements actually carrying a tracking marker and identifiable by the current page enter anchor statistics.
- Adding, renaming, or deleting an anchor in the visual editor changes the tracking attribute in page content and the site's saved anchor list; it is not only a display-name change in the analytics panel.
- The same anchor name identifies one event; after a name changes, old and new names are different identifiers. Use a stable, readable, business-unique name.
- Deleting an anchor does not delete the page element or erase historical events already collected underneath, but reports associate only current anchors, so an old name may no longer display.
- Visual editing requires the page to load normally and locate editable site elements. Dynamically generated, cross-origin embedded, unbound, or failed-to-load elements may not accept an anchor.

## Statistical Basis

- Trigger Count is the number of page or anchor events triggered in the selected dates; repeated clicks by one visitor are counted repeatedly.
- User Count/UV shown for a page is actually that page's visit count used as the trigger-rate denominator, not precise deduplication of unique visitors who clicked the anchor.
- Trigger Rate is trigger count divided by page visits and can exceed 100% when one visit triggers repeatedly; do not interpret it directly as the share of users who clicked.
- Trends split the selected dates into earlier and later halves, compare “trigger count ÷ page visits,” and clamp the result to -100% through 100%. It is not automatic year-over-year comparison with a prior natural period and does not represent causality.
- Page ranking includes only pages with interaction events that can be associated with current anchor configuration. An empty list may mean the switch is off, anchors are not configured, there are no events in the dates, the page was not visited, or the collection script did not run normally.

## Details and Visual Editing

- Single-page details load a design preview of the selected page and overlay anchor statistics for the current date range. Preview success does not mean public routes, all devices, or all language versions have been verified.
- Changing dates rereads statistics only and does not modify anchors. Changing pages reloads the target page and its anchors.
- The Mark as Goal entry on the current details page is not open to users; a page-interaction anchor cannot be treated as a user-behavior-analysis goal from the current interface alone.
- After changing an anchor, trigger it once on the real frontend page and return to the correct date range to verify the count. Save notice, preview marker, and statistic growth are three separate verification steps.
