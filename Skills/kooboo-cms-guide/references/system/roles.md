# Site Settings - Roles and Permissions

## Page Purpose

- Admin page: `/system/roles`
- This page defines roles for the current site and controls CMS admin permissions by “feature + action.” Permissions are site-level and do not automatically apply to other sites in the organization.
- Role permissions are not frontend login, JWT validation, or page-access rules; configure those in corresponding site-access and authentication features.

## Permission Model

- Each permission item consists of a feature and action. Common actions are view, edit, and delete; some features also provide debug or other dedicated actions.
- The system precisely matches the feature and action required by a request. Checking View for a feature does not automatically grant edit or delete.
- When Edit, Delete, or Debug is checked, the current interface also retains View for that feature; View cannot be deselected while dependent actions remain selected.
- Select All changes every permission in the current scope. Expand features with multiple actions and review them individually before saving to avoid overly broad permissions based only on feature names.

## Built-In and Custom Roles

- The system provides four built-in roles: master, developer, contentmanager, and member, and allows site custom roles.
- The current interface does not allow built-in roles to be deleted. Changing a built-in role's permissions saves a site-specific definition and uses it first; it does not change the same-named built-in role on other sites.
- A new role name must be unique and at most 50 characters. After creation, the current interface cannot directly rename it; create a new role, migrate users, and then delete the old role to rename.
- Site or organization owners bypass ordinary role-permission checks, so an owner account cannot verify a restricted role's configuration.

## Assignment and Deletion

- Saving a role does not assign it to any user automatically; select it explicitly on the Site Users page.
- A role used by any site user cannot be deleted. Migrate every user to another role or remove their site membership first.
- Deleting a role does not rewrite users, undo historical actions, or migrate them to a default role; the system directly blocks deletion while the role is in use.

## Verification Suggestions

- After changing permissions, verify with a non-owner account actually assigned the role. Test menu visibility, list reads, save changes, and deletion separately at minimum.
- A hidden menu is only frontend presentation; actual permission should be based on whether the corresponding admin request is allowed. Opening a page does not mean all actions within it are permitted.
