# Site Operations - Full-Text Search Index

## Page Purpose

- Admin page: `/system/search`
- This page manages the current site's full-text search switch, rebuild, and cleanup, and shows index size, recent search records, and weekly keyword statistics.
- It manages site full-text search, not admin code search, access logs, SEO keyword rankings, or external search-engine indexing.

## Enabling, Disabling, and Automatic Sync

- When enabling full-text search, choose an immediate rebuild or enable only subsequent automatic synchronization.
- If it is enabled without rebuilding, existing resources do not all enter the index immediately; the index syncs when supported resources are later added, changed, or deleted.
- The current index covers supported objects such as pages, views, Labels, layouts, HTML blocks, and text content. Scripts, styles, external data, runtime-generated content, and other unsupported objects are outside a full rebuild.
- Disabling stops full-text index sync when site resources change but does not explicitly clear existing index files. Disabling and cleanup are different operations.

## Rebuild and Cleanup

- Rebuild first deletes the current full-text index and regenerates it from supported, indexable online resources in the current site.
- Rebuild does not modify pages, content, or other source resources, but search results may be temporarily incomplete; a site with many resources may take a long time.
- Successful rebuild proves only that index generation returned success; it does not guarantee dynamic content, external data, language tokenization, permission scenarios, or the frontend search interface meet business expectations.
- Cleanup deletes current index data but does not automatically turn off Full-Text Search. The switch can remain on while index statistics are temporarily zero, then resource changes gradually write new data.

## Index Statistics and Search Logs

- Keyword count, document count, and disk size describe current index scale; they are not total site resources and do not prove every document matches a target keyword.
- Recent Searches returns at most 50 site-search records and may include IP, keyword, document count found, and time.
- Search statistics read weekly keyword occurrence counts and show at most 100 keyword counts for that week. They reflect site-search behavior, not visitor page views or search-engine source terms.
- Empty recent records or weekly statistics may mean no usable search logs, no records in the selected week, or that the frontend did not use this search entry; they do not alone prove the site had no visits.

## Acceptance Boundaries

- The index switch, statistics, or completed rebuild cannot replace acceptance through a real search.
- For critical business, use the actual frontend search entry and verify target language, keywords, result count, titles and summaries, permission boundaries, and visibility after resource updates separately.
