# Development Resources - Forms

## Page Purpose

- Admin list: `/development/forms`; HTML editor: `/development/form/edit`.
- This page manages form HTML, form submission settings, and form data saved by Kooboo, divided into external and embedded forms.
- A form's interface structure and submission behavior are separate: editing form HTML does not mean a data-submission target has been configured.

## External and Embedded Forms

- An external form is an independent form resource reusable by multiple pages or components; an embedded form belongs to another site object such as a page, layout, or view.
- A name can be specified when creating an external form. The names of existing external and embedded forms cannot be changed directly in the current editor.
- Changing an embedded form affects its owning site object. Copying or creating an external form does not automatically replace existing form references on pages.
- Saving form HTML updates only the form structure; it does not automatically enable submission or select a storage location, API, or external address.
- Formatting in the HTML editor adjusts markup layout only and does not automatically generate correct field names, submit buttons, validation rules, or submission settings.

## Submission Settings

- To form a submission loop handled by Kooboo, enable form settings and select a submitter. A form may still render normally when disabled or without a submitter, but that does not establish that Kooboo receives and processes data.
- `FormValue` saves submitted fields in the current form's data list; the Data count and details represent only these written form records.
- `ContentFolder` writes fields to the specified content folder. Field names, content type, and multilingual values must still match the target content structure.
- `Api` executes the selected API code within the site; the API's own logic, permissions, exceptions, and external dependencies require separate verification.
- `SubmitToPage` or `SubmitToExternal` makes the browser submit the form directly to the selected page or address. Kooboo does not prove successful processing by the target page or external service.
- The submission method can be GET or POST. Redirect is used after a non-Ajax submission; Refresh Current Page depends on the current access address carried during submission.

## Success Boundaries and Data Viewing

- A successful settings save proves only that submission configuration was saved; it does not prove a visitor submission, content write, API execution, or external-system process succeeded.
- After calling a submitter, the current submission entry mainly continues based on whether the response ended or an error status appeared. Do not infer completed downstream business only from browser navigation or a generic success response.
- The Data page shows records saved by the current form in Kooboo form-data storage; it is not a unified audit log for every submitter. Data sent to a content folder, API, page, or external address may not appear here.
- Form assembly and existing `action` and `method` in HTML can affect final submission behavior. After changing settings, inspect the actual frontend form attributes and network request.

## Deletion and Change Risks

- Deleting a form does not automatically repair references in pages, layouts, views, or components, and must not be assumed to clean up all historical submissions and downstream data.
- Deleting records on the Data page removes only the selected form data and does not undo results already written to a content folder, API, or external system.
- After changing field names, submission targets, request methods, or redirects, perform one complete submission on the real page and confirm the browser response and target-system result separately.
