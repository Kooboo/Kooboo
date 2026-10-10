# Site Settings - AI Service

## Page Purpose and Current Entries

- This page currently provides Settings, AI Functions, Vector Search, llms.txt, Markdown, and JSON-LD. Vector Search shows a separate status page only after it is enabled.
- The current page does not expose a Pay-per-crawl tab; it must not be described as an available admin capability at this stage.
- Feature switches, saved configuration, and actual runtime results are different layers. A successful save does not mean a remote model, index, tool execution, page conversion, or structured-data output succeeded.

## MCP Server and AI Functions

- After MCP Server is enabled, the site's `/_mcp` entry accepts MCP requests. The connection address shown on the page prioritizes HTTPS.
- The displayed Authorization Header comes from the current login credential and is development connection information. The MCP entry must not therefore be treated as a public anonymous API, and the credential must not be sent to an untrusted client.
- Disable Built-in Tools controls only whether built-in tools are exposed; custom tools come from the current site's AI Functions. Adding, editing, or deleting a custom tool also updates the tool set of existing MCP sessions.
- Run Tool really executes the corresponding function and may read or modify the site and external systems. Saving a tool proves only that its definition exists, not that the script runs, permissions suffice, or downstream calls succeed.
- The built-in-tool list is read-only here and cannot be edited item by item.

## Vector Search

- Enabling vector search saves embedding-provider and model configuration only and does not automatically guarantee a complete index. A usable embedding provider and model are required for the asynchronous queue to generate vectors.
- The status page shows total, pending, queued, running, and error counts by indexable object type. An object's update time or a provider/model change can make existing objects appear pending again.
- Sync processes objects pending synchronization, expired, or changed by model. Rebuild first deletes all existing vectors of the selected type and queues them again; search results may be temporarily incomplete.
- When the site is unpublished, the index queue does not complete normally; after consecutive errors exceed the limit, new items also stop entering the queue. Successful button submission does not prove indexing completed.
- Search selects candidates by similarity, and multiple text chunks of the same object ultimately return one object result. Vector similarity is not a guarantee of business permissions, content correctness, or search ranking.

## llms.txt

- This feature actually maintains `/llms.txt` in the site root. Generate scans only online pages with routes and creates index text pointing to each page's Markdown address.
- The home page maps to `/index.html.md`; a path ending in / appends `index.html.md`, and other page paths append `.md`.
- Generate only fills the result into the editor and does not save automatically; click Save to overwrite the site's existing `/llms.txt` content.
- Automatic generation is off by default. Enable it and save to refresh the file in the background. Like sitemap, subsequent requests to `/llms.txt` check the site version, return the existing file, and regenerate it in the background when the version changes. A missing file is generated on the first request. Turn it off to edit manually.
- Content is stored as `llms.txt` in the site's FileIO directory; existing files from the AI directory are migrated on first read without overwriting FileIO content.

## Markdown

- When globally enabled, requesting a page's `.md` or `index.html.md` address maps to an existing page route, retains query parameters, and converts the rendered page result to Markdown. It does not create a Markdown page when the original page route does not exist.
- The conversion is neither raw template source nor a complete DOM snapshot after browser execution; comments, doctype, later client scripts, and some unsupported HTML structures may not remain.
- The page can save Match Path and Use Object override rules, but it is currently not confirmed that these rules participate in actual Markdown request processing. A successful rule save cannot establish that it overrides the default page source.

## JSON-LD

- JSON-LD can select a table or content type from content and configured data sources, then map fields to schema.org types and properties. When an external data source connection fails, the table or field list may be empty.
- Use an explicit `<k-json-ld>` element in the page template and inspect the rendered `application/ld+json` script to verify structured-data output.
- For template syntax, examples, and rendering limitations, load `kooboo-jsonld`.
- Mapping does not modify original data or automatically validate required fields, data quality, search-engine indexing, or SEO correctness. After changes, inspect real page output and verify with an independent structured-data tool.
