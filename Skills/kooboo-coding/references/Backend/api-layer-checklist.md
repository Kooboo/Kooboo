# API Layer Checklist

Confirm each item before writing API code:

## 1. Module identification

- [ ] Which business module does this API belong to? (commerce / other)
- [ ] If it is a commerce module, first use the [Commerce module](../K-Script/commerce/index.md) to confirm the corresponding submodule skill.

## 2. create_api parameters

- [ ] Is `Name` a **concise API identifier** (such as `product`, `auth`, or `init-sqlite`)? **Do not** set an API `Name` to `Services.xxx` (that form is only for the Service / Utils CodeBlock created by `create_codeblock`).
- [ ] Does `Url` begin with `/`?
- [ ] Does `Url` include `{action}`? (when multiple subroutes are needed)
- [ ] Are line breaks in `Content` escaped as `\\n`?

## 3. Route design

- [ ] If dynamic subroutes are needed, does `Url` include `{action}`?
- [ ] Is the wildcard route `k.api.method(()=>{})` placed at the end of the file?

## 4. Parameter access

- [ ] Are GET parameters obtained through function parameters (such as `(page, size)`)?
- [ ] Are POST/PUT parameters obtained through function parameters (such as `(body)`)?
- [ ] Are required parameters validated?

## 5. Service calls

- [ ] Is business logic called only through **`import { ... } from "./Services.xxx"`** (or `./Utils.xxx`)? **Do not use** `k.req.services`, `k.request.services`, or any “call a Service by name from the request side” pattern not present in the documentation.
- [ ] Are Service functions exposed with **`export function`**, with an import path beginning with `./` and matching the actual CodeBlock name?

## 6. Responses

- [ ] Is error handling present (404 / 400 / 401)?
- [ ] Is the response format consistent?

---

**Call `create_api` to write code only after all checks pass.**
