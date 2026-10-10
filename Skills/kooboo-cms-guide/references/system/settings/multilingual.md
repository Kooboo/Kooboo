# Kooboo Multilingual Settings

## Multilingual Switch

- When enabled, a site can configure multiple languages, a default language, and corresponding access modes.
- When disabled, Kooboo retains the default language and disables language-path mode.
- This feature does not translate content automatically; pages, Labels, content fields, and other copy still need language versions.

## Languages and Default Language

- The language list determines which languages the site recognizes.
- The default language supplies base content and address behavior when no language is specified.
- Keep language codes stable. Changing a code after publishing many multilingual addresses can affect existing links, indexing, and external references.

## Parameter and Route Modes

- Parameter mode does not require language as part of the URL path.
- Route mode generates language paths for different languages and suits sites wanting language visible in URLs.
- In route mode, Default Language Prefix determines whether the default-language address must also include a language prefix.
- Plan redirects from old addresses before changing address mode to avoid duplicate pages or broken links.

## Automatic Language Detection

- When enabled, Kooboo tries to select a suitable language from the current access context.
- Automatic detection does not replace a clear language switcher and does not guarantee the same result in all browsers, crawlers, or proxy environments.

## Hreflang

- When enabled, Kooboo declares relationships between language versions for corresponding multilingual routes.
- Hreflang depends on language pages and routes actually existing; turning on the switch does not fill missing translations or pages.

## Suggestions

- Decide default language, language codes, and URL scheme before production release.
- Maintain real content for each language; do not treat the multilingual switch as browser translation.
- After changing language paths, check internal links, Canonical, sitemap, and search-engine indexing together.
