# Site Operations - Site Operation Logs

## Page Purpose

- Admin pages include the log list, object versions, and version comparison.
- Site operation logs record editing versions of site objects and some table data, showing editor, time, action, and object history. They are not login records, HTTP request logs, KScript execution logs, or a unified audit log for system jobs.
- The list filters by object type, action, user, time, and name. Filtering changes only the current view scope and does not delete or modify logs.

## Versions and Comparison

- Versions reads historical versions by the same object and storage location; records with similar names but different objects or data tables are not merged into one history line automatically.
- Compare can compare two historical versions or one historical version with the current latest version. Text objects show summary differences; image objects use the corresponding version resources for image comparison.
- A difference view means version content differs; it does not mean either side is published, still referenced, or correct in business results.
- An operation video may appear only for logs with corresponding recording data; not every site log has replayable video.

## Undo, Restore, and Checkout

- Undo or Revert rolls an object back for one selected version or change; Undo Changes or Blame can roll back multiple selected logs. Rollback really changes the current site and only changes log display.
- Restore restores site changes after a selected log point as a whole and usually affects more than one Undo. Confirm target time, affected objects, and current unpublished changes first.
- Checkout creates an independent new site, restores the source site's state at a specified log point into it, and marks the new site as published. It is not switching a code branch of the current site and does not move the current site back to that point.
- Export Changes and Export Items generate temporary ZIPs for download; they do not automatically push remotely, create a sync connection, or import into another site.

## Clean Log

- Clean Log is an asynchronous rebuild, not merely deleting list rows. During execution the site is temporarily unpublished, core objects and some additional data are copied to new storage, old operation logs are removed, and the original publication state is restored at the end.
- The page polls cleanup-task status. A successful button request means only that the task was submitted, not that rebuild finished or the site resumed service.
- Do not assume every runtime environment creates a recoverable backup; after cleanup there is no entry to retrieve deleted logs one by one.
- Cleanup can briefly affect frontend availability and later version tracking. Run it in a low-traffic window and verify publication status, key objects, and frontend access afterward.

## Risk Boundaries

- Rollback, whole-site restore, Checkout, and cleanup have different scopes and are not one Undo button. Export required objects or confirm an independent backup before real writes.
- Dynamic references, external side effects, and requests already sent are not automatically undone by rolling back site objects. Successful log restoration does not mean external business state was restored.
