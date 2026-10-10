# SPA Routing in Kooboo

## Actual Behavior

When an access address matches no normal Kooboo route, SPA mode tries to use the site's default home page as the app shell:

1. The site must have a default home page.
2. Kooboo renders that home page in the current request context.
3. The browser address remains the address originally requested.
4. After loading, the frontend router decides which interface to display.

Therefore, SPA mode “renders the default home page at an unmatched address”; it does not redirect the user to the home page.

## Effect on 404

- Without SPA, an unmatched address normally enters ordinary 404 handling.
- With SPA enabled and a default home page present, the address can first receive the app shell, supporting direct access and refresh for frontend routes.
- A truly invalid address may also appear to return a successful page. The app should provide its own not-found page or monitoring tools and search engines may not identify broken links accurately.

## Suitable Cases

- The site explicitly uses a single-page-app architecture.
- It uses History routing and requires deep addresses to open and refresh directly.
- The default home page loads the complete app and frontend router.

Using JavaScript, Vue, or React alone does not mean the site needs SPA. A multi-page site using normal Kooboo page routes usually does not need this fallback.

## Troubleshooting Suggestions

- When a deep address still fails, first confirm the site has a default home page.
- Confirm the frontend router recognizes the current address.
- If every invalid address shows the home page, add a not-found route in the frontend or disable unnecessary SPA mode.
- SPA fallback handles page addresses only and does not repair invalid script, style, API, or image addresses.
