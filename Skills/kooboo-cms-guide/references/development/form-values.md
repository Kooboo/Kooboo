# Development Resources - Form Values

## Page Purpose

- Admin page: `/development/form-values?id={formId}`
- This page views submission records saved by a specified form in the site's internal FormValue storage and supports paginated viewing and deletion.
- It is not a unified audit log for every form submission mode and does not show the processing results of content writes, site APIs, direct page submissions, external addresses, or other downstream systems.

## Which Submissions Appear Here

- Submission fields are saved as records on this page only when the form is enabled and selects the internal FormValue submission mode.
- When another mode such as page redirect, content, API, or external address is selected, its result is not automatically copied to the Form Values page just because it belongs to the same form.
- Internal submission saves values from form fields or query parameters and ignores internal fields used by Kooboo; no empty record is created when there are no savable fields.
- Switching submission modes does not migrate, fill, or reclassify earlier records. An empty page alone does not prove the form was never submitted; the current form may use another submission mode.

## Pagination, Sorting, and Dynamic Columns

- The page reads only internal records for the FormId specified in the URL and returns them paginated; the admin prioritizes newer records.
- Table columns are dynamically merged from field names that actually appear in records on the current page. Different pages can therefore show different columns.
- A field absent from the current page does not mean the form never collected it or that it was deleted from the form definition.
- Changing form fields does not automatically migrate existing submissions. Old records may retain old field names, and old and new structures can coexist.
- Values saved here are the field values received at submission time; they do not mean the values passed business validation, anti-spam checks, or downstream processing.

## Deletion Boundaries

- Deletion removes only the selected internal FormValue records, not the form definition.
- Deleting internal records does not retract content already created or updated, site APIs already executed, requests already submitted directly to pages or external addresses, or their downstream side effects.
- The page does not treat deletion as a unified rollback across submitters. To handle external results, check and handle them separately in the corresponding downstream system.
- Reload the current page after deletion to confirm the list state; do not infer from the button message alone that data in other systems was deleted too.
