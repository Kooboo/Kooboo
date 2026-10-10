# k.google

> Google PageSpeed/Lighthouse and Firebase Admin services

Use `k.google` for PageSpeed/Lighthouse methods and `k.google.firebase` for Firebase Admin methods. Accessing `k.google` eagerly initializes Firebase; missing or invalid Firebase service-account settings can therefore fail even in a PageSpeed-only call.

## PageSpeed / Lighthouse

```ts
const json = k.google.lHRJson("https://www.kooboo.com", false);
const summary = k.google.parseJson(json).getSummary();

return {
  performance: summary.performance,
  accessibility: summary.accessibility,
  seo: summary.seo,
};
```

| Method | Contract |
| --- | --- |
| `lHRJson(absoluteUrl, mobileDevice)` | Requests remote PageSpeed JSON and returns the response string. `absoluteUrl` is concatenated into the request without explicit URL encoding; remote failures propagate. |
| `lHRUrl(absoluteUrl, mobileDevice)` | Calls `lHRJson()` and embeds its response in a Kooboo Lighthouse report URL. |
| `lHRUrlFromJson(json)` | Wraps the supplied text as a Lighthouse payload and compresses it into a report URL; it does not validate the JSON first. |
| `parseJson(json)` | Deserializes JSON into a Lighthouse result object; invalid JSON can throw. |
| `saveJson(json)` | Writes report text to temporary storage and returns a `Guid`. |
| `getJson(id)` | Reads the report for a `Guid`; returns `null` when the file is absent. |
| `shortLhrReportUrl(json)` | Saves the report and returns a site-local `/_api/LightHouse/ReadReport?id=...` URL. |

`mobileDevice` remains a required compatibility parameter; the current request does not switch strategy based on its value.

## Firebase

Available methods include `verifyIdToken`, `createCustomToken`, `createUser`, `deleteUser`, `updateUser`, `getUserByEmail`, `getUserByPhoneNumber`, `generateEmailVerificationLink`, `generateSignInWithEmailLink`, and `generatePasswordResetLink`.

The runtime requires valid Firebase service-account settings when `k.google` is created. Firebase failures, invalid tokens, and invalid user arguments can throw; do not treat these methods as returning a neutral failure value.

The overload `generatePasswordResetLink(email, settings)` currently calls Firebase's email-link sign-in API despite its name. Do not assume it generates a password-reset link; use the one-argument overload for the current password-reset behavior and `generateSignInWithEmailLink` for email-link sign-in.
