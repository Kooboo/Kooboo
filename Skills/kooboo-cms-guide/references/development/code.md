# Development Resources - Code

## Page Purpose

- Admin list: `/development/code`; editor: `/development/code/edit`.
- This page centrally manages executable KScript resources. Current creatable types include API, page script, payment callback, and CodeBlock; their trigger modes differ.
- Type filtering only views different code resources; not every type can be treated as a directly accessible HTTP API.

## Types and Runtime Entry Points

- An API has a site URL and executes when that route is accessed. Creating an API establishes the corresponding route; changing its URL changes the runtime entry point.
- A page script executes at its rendering location only after a page, layout, or view references it. Saving a page script does not automatically insert it into any page.
- A CodeBlock is imported or called by other code and normally has no independent access URL. Saving it does not mean any page or API uses it.
- A payment callback executes as a callback in the payment flow. Saving callback code alone does not prove that the payment channel, callback URL, and order-status flow are configured successfully.
- Existing code names and some type-specific options cannot be changed directly in the current editor; a code type must not be casually converted like an ordinary field after creation.
- Protected code that cannot be decrypted is shown with an error, and the current editor provides no save entry. Inability to view source must not be interpreted as empty code or safe-to-overwrite code.

## API Routes, Requests, and Documentation

- An API URL must be usable and must not occupy the route of another valid site resource. After changing the URL, the old address and external callers do not automatically receive compatibility guarantees.
- An API's method, parameter locations, request body, and response structure depend on how the code actually reads the request and writes the response; saving does not validate every real request combination for you.
- The Swagger switch in API settings generates and displays API documentation. Token location and name describe authentication in the documentation and do not automatically implement real authentication logic.
- If an API needs access control, configure authentication rules separately or implement validation in code; JWT Secret must also be configured separately in Service Integrations.

## Saving, Execution, and Troubleshooting

- A successful save proves only that the code resource and necessary API route were persisted; it does not prove that the code executed successfully.
- Verify an API through its real URL, correct method, and representative input; verify a page script on the page that references it; verify a CodeBlock through a real caller.
- Runtime exceptions can make an API return an error status and be recorded as a code error. Check Code Logs when troubleshooting execution rather than only the editor's save message.
- Formatting and type hints in the editor help write code but cannot replace validation of runtime dependencies, permissions, data, and external services.

## Sharing and Deletion Risks

- Page scripts and CodeBlocks may be reused by multiple resources or other code. Changing shared code affects every caller.
- Used by information in the list mainly covers relationships identified by Kooboo; dynamic imports, name lookups, and external HTTP calls are not guaranteed to appear completely.
- Deleting an API makes its entry point unavailable; deleting a page script or CodeBlock can make a caller render or execute unsuccessfully. Search callers before deletion and verify the main runtime paths afterward.
