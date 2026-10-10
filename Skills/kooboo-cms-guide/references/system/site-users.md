# Site Settings - Site Users

## Page Purpose

- Admin page: `/system/siteuser`
- This page adds existing accounts in the current organization to the current site and assigns one site role.
- It is not for registering users, inviting new accounts, changing passwords, or maintaining frontend member profiles. Selectable users come from the organization and exclude organization administrators and users already in the site.

## Users and Roles

- Adding requires selecting both a user and a role. A site-user record stores one site role only and does not merge permissions from multiple roles.
- The role list includes built-in roles and current-site custom roles. Actual capabilities come from feature and action permissions on the Roles page, not the role name alone.
- The current page has no direct entry to change an existing user's role. To change it, confirm the new role's permissions, then remove and add the user again.
- Username and email come from organization account data; this page does not create email, send invitations, or verify other login settings are complete.

## Permissions and Frontend Access

- For CMS admin operations, non-owner requests are checked against feature and action permissions from the user's site role. Site or organization owners have independent admin permissions and do not depend on ordinary site-user records in the list.
- Site-user identity can also be used by frontend site types restricted to site users. Adding a user satisfies that membership check, but the Roles page is not a JWT validation rule for frontend pages or APIs.
- Adding a user does not automatically grant organization-level administration, permissions on other sites, or customer identity in a frontend business system.

## Deletion Boundaries

- Deleting a site user removes only membership and role relationship with the current site; it does not delete the global account, organization membership, password, email, or permissions on other sites.
- Existing login credentials may not become invalid immediately after deletion, but later requests requiring site membership or admin permission fail the corresponding checks.
- Deleting a user does not undo content changes, publication, external calls, or other business side effects they completed earlier; use site logs and business records to trace history.

## Operation Suggestions

- Before adding, check the role under least privilege; seeing a menu does not mean every edit or delete permission exists.
- After deleting or changing a role, have the user re-enter the admin and verify view, edit, and delete actions separately. Refreshing the administrator's own list does not prove the other user's permissions meet expectations.
