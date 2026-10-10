# Add Content

> Create a content model and content items so pages can retrieve managed data from Admin.

## Create a Content Type First

| Item | Details |
|------|---------|
| Menu path | Site Admin -> Content -> Content Types |
| Deep link | `/_Admin/contents/types?SiteId={site-guid}` |
| Detailed documentation | [CMS: Content Types](../cms/content/content-types.md) |

A content type defines the fields available to content. For your first model, start with a simple structure:

| Scenario | Suggested fields |
|----------|------------------|
| News | Title, summary, cover image, body, and publication date |
| Product showcase | Name, image, description, price, and order |
| FAQ | Question, answer, category, and pinned status |

## Create a Content Folder

| Item | Details |
|------|---------|
| Menu path | Site Admin -> Content -> Contents |
| Deep link | `/_Admin/contents?SiteId={site-guid}` |
| Detailed documentation | [CMS: Content List](../cms/content/contents-folders-list.md) |

A content folder turns a content type into a maintainable data collection. After creating it, open the folder and add several content items as test data for the page.

## Use Content on a Page

| Method | Recommended use | Documentation |
|--------|-----------------|---------------|
| Template binding | Render lists, details, and fields directly on a page | [Template Binding Syntax](../templateEngine/binding/index.md) |
| `k-data` | Declare queries, mappings, and exports in a template | [k-data](../templateEngine/k-data/index.md) |
| KScript API | Let the frontend retrieve content through a JSON endpoint | [k.content](../api/content/index.md) |

If you are not ready to write scripts, begin with template binding or `k-data`. Create an API when you need filtering, pagination, permissions, or data shared across clients.

## Common Checks

| Problem | What to check |
|---------|---------------|
| The page has no data | Confirm that the content folder contains items and the field names match the template |
| Fields render incorrectly | Check content-type field types, required settings, and default values |
| The list order is incorrect | Check content item order, publication time, or query conditions |
| Multilingual content is missing | Check the content language and site language settings |

## Next Step

Continue to [Publish and Domains](./publish-domain.md), or go directly to [Write Your First KScript API](./add-script-api.md).
