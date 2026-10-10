# Development Resources - Authentication

## Page Purpose

- Admin page: `/development/authentication`
- This page configures authentication rules for frontend site routes. Rules run after a site route matches and before a page or API is about to execute.
- Authentication rules do not create a login page, user system, or token-issuance flow, and are not admin-permission settings.

## Matching Rules

- When Matcher is `None`, the rule does not match. `Any` matches any site route entering the authentication stage. `Condition` evaluates the URL or request method.
- Conditions support equals, not equals, contains, does not contain, starts with, and does not start with. Comparisons ignore case and trim whitespace around conditions.
- Multiple conditions in one `Condition` rule must all be true for it to match.
- The system searches rules in stored order and uses the first matching rule. The page displays them by last-modified time, which is not the real matching priority. With overlapping rules, do not infer the result from list order.
- `Any` or an overly broad URL condition can cover many pages and APIs. Before enabling it, explicitly exclude login, token-acquisition, static-entry, and other paths that must remain anonymously accessible.

## Authentication Actions and Failure Handling

- The `None` action lets the request continue. `JwtAuth` validates the JWT in the request. `CustomCode` executes custom KScript and uses the response status to decide whether to continue.
- JWT validation first reads `Authorization: Bearer ...` and, when absent, tries the `jwt_access_token` Cookie. After validation succeeds, later code can read the token payload.
- JWT Secret, expiration, and other parameters are not configured here; set them separately under the site's Service Integrations → Others → JwtSetting. Creating a `JwtAuth` rule does not automatically generate a token or Secret.
- On JWT failure, the rule can return a specified HTTP status code, redirect to a specified URL, or output a validation error. The redirect includes the original address; the target login page must implement the return flow itself.
- Custom code can directly decide the response status, content, or redirect. A successful code save does not mean every request branch is safe; test anonymous, valid-credential, and invalid-credential requests separately.

## Scope and Verification

- Authentication runs only on frontend requests that have already matched a valid site route; it is not a complete 404 handler or admin menu-permission system.
- After changing rules, test the target page or API using the real domain and request method. Seeing a rule in the admin does not mean runtime matching behaves as expected.
- For an API, verify the status code, response body, CORS preflight, and token source together. For a page, verify login redirect, return address, and access after login.

## Deletion and Risks

- Deleting a rule immediately removes its protection scope and does not automatically migrate it to another rule or custom code.
- An overly broad rule, incorrect failure status, or improper redirect address can block site access or cause redirect loops.
- A rule name cannot be changed directly in the current interface after creation. Before changing or deleting one, record the URLs, request methods, and JWT/custom-code configuration it actually covers.
