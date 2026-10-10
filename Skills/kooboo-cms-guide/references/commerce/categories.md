# E-commerce - Product Categories

## Page Purpose

- Admin pages: `/commerce/product-categories`, `/commerce/product-categories/create`, `/commerce/product-categories/edit`
- Product categories organize products, build a category tree, and provide category conditions for frontend product queries. Category titles, SEO names, descriptions, and images can be maintained per language.
- A category's parent relationship, sort order, matching mode, and product relationship are different concepts and cannot replace one another.

## Manual and Automatic Categories

- You can choose a manual or automatic category when creating it. An existing category's type cannot be changed on the current edit page.
- A manual category stores explicit product relationships. Products can be added or removed in the category-product dialog or on the product edit page.
- An automatic category does not store the same kind of product relationship; the system matches products against the conditions using their product and variant data each time. A product that still satisfies the conditions cannot be manually removed from the category.
- Automatic conditions can require any condition or all conditions. The parent category determines only the tree structure and does not participate in the AND or OR evaluation of these conditions.

## Hierarchy, Ordering, and Status

- The parent category forms paths and child categories. Dragging in the list changes only the sort value between categories; it does not change the parent. Changing the parent must be saved in the category's basic information.
- When deleting a parent category, do not assume that child categories will be cascade-deleted or automatically attached to another parent. Handle the child categories' parent relationships first.
- Active controls the category's own status and does not automatically modify or take products offline. Whether the frontend filters disabled categories also depends on the page or query logic.
- The SEO name must be unique among categories and does not allow spaces or various special characters. It is different from the category title, parent, and page URL; saving a category does not automatically create a frontend page or route.

## Deletion Boundaries

- Deleting a category deletes the category and its manual product relationships, but does not delete products.
- An automatic category has no equivalent manual relationships to clean up; its product set comes from condition matching.
- Deleting a category does not automatically repair frontend links, menus, code queries, child-category parents, or other dynamic references. Check the category tree and its consumers before deletion; do not rely only on the product count in the admin list.
