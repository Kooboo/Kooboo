# Development Resources - Code Search

## Page Purpose

- Admin page: `/development/code-search`
- This page statically searches names and bodies of selected site text resources, suitable for locating HTML, CSS, JavaScript, KScript, menu text, or resource names.
- Code Search is a read-only locating tool. It does not replace text, modify references, execute code, or generate complete dependency relationships.

## Current Search Scope

- Current search resource types include pages, scripts, styles, menus, layouts, views, forms, and code.
- Scripts embedded in pages, embedded scripts, embedded styles, embedded forms, and task-type code are skipped.
- Code Search does not search every site object or runtime-generated content. Content data, HTML blocks, external files, binary resources, database values, remote-interface responses, and text dynamically concatenated at runtime are outside the main current scope.
- Therefore, no results means only that no match was found in currently supported static resources; it does not prove that text is absent from the whole site, external systems, or actual responses.

## Search Modes and Results

- By default, search uses ordinary text, with an option to enable regular expressions; case matching is controlled independently.
- Both ordinary text and regular expressions check resource names and bodies. An invalid regular expression does not start a valid search; an overly broad or complex expression can produce many results and increase search time.
- Results show resource name, type, and a matching-line summary. Displayed line numbers help locate text; they do not mean the line has run on the frontend or will necessarily be loaded by the current page.
- Clicking edit opens the corresponding editor by resource type and is restricted by view permission for that resource. Seeing a hit on the search page does not mean the current user can modify it.

## Usage Boundaries and Change Risks

- Search results are not a reference scan or dependency graph. A resource containing another resource's name does not mean a valid reference exists; dynamic references, variable concatenation, and external references may also be unrecognized.
- After changing a matched text, inspect its owning resource, callers, and real access path separately. Code Search does not synchronize other copies or repair references broken by name, deletion, or parameter changes.
- To verify that code actually executes, combine frontend requests, code logs, or task logs. To confirm whether a resource is used, inspect pages, layouts, views, and relationships; do not rely only on search results.
