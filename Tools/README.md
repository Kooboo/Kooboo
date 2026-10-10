# Kooboo AI assistant — function definitions

> **This directory is AI-facing.** It holds the 149 functions (tools) that the Kooboo
> AI assistant can call while working on a site, exported as OpenAI-style tool definitions. It
> is *not* a developer API and not end-user documentation — readers looking for Kooboo
> documentation should start at [`Docs/official-docs/`](../Docs/official-docs).
>
> Machine-readable map of the whole repository: [llms.txt](../../llms.txt)

One JSON file per function, with `name`, `description` and a JSON Schema `parameters` object
(required fields, patterns, enums and per-field descriptions). Categories follow the Kooboo
source layout under `Kooboo.Sites/AI/Functions/BuiltIn`.

## Categories

| Category | Functions |
| --- | --- |
| Core | 36 |
| AppBuilding | 29 |
| Commerce | 23 |
| History | 12 |
| Basic | 11 |
| Sandbox | 8 |
| Site | 7 |
| Analytic | 6 |
| SiteDevelopment | 6 |
| TaskJob | 6 |
| Utils | 3 |
| Settings | 2 |

## Functions

### Core (36)

| Function | Description |
| --- | --- |
| [create_api](create_api.json) | Create or replace the named site object. |
| [create_codeblock](create_codeblock.json) | Create or replace the named site object. |
| [create_content](create_content.json) | Create content in the specified content folder; data must comply with the content type schema. |
| [create_content_folder](create_content_folder.json) | Create a content folder using an existing content type name or ID. |
| [create_content_type](create_content_type.json) | Create a ContentType. |
| [create_database_table](create_database_table.json) | Create an internal SQLite table with explicit columns. |
| [create_htmlblock](create_htmlblock.json) | Create or replace the named site object. |
| [create_layout](create_layout.json) | Create or replace the named site object. |
| [create_page](create_page.json) | Create or replace the named site object. |
| [create_script](create_script.json) | Create or replace the named site object. |
| [create_style](create_style.json) | Create or replace the named site object. |
| [create_view](create_view.json) | Create or replace the named site object. |
| [delete_database_table](delete_database_table.json) | Delete an internal SQLite table and all its rows permanently. |
| [delete_site_object](delete_site_object.json) | Deletes a specific Kooboo object. |
| [get_content](get_content.json) | Retrieve all field values of a specified content item, with optional culture/language support. |
| [get_content_schema](get_content_schema.json) | Get the field schema of a specified content folder for building front-end forms or automated processing. |
| [get_database_table](get_database_table.json) | Get a table's columns and CREATE TABLE SQL from internal SQLite. |
| [get_htmlblock](get_htmlblock.json) | Retrieve the HTML content of a specified HtmlBlock, with optional culture/language support. |
| [get_label](get_label.json) | Retrieve label values by key array. |
| [list_content](list_content.json) | List content items in a specified folder with optional culture/language support, skip offset, and take limit. |
| [list_database_table](list_database_table.json) | List user tables in the site's internal SQLite database. |
| [list_site_object](list_site_object.json) | List all objects of a specific type (Page, Layout, View, Script, Style, etc.) from the current site. |
| [read_object_text](read_object_text.json) | Retrieve the text content of a site object (page, layout, view, script, style, api, codeblock). |
| [read_preview_element_context](read_preview_element_context.json) | Resolve a site-preview selected element to its current Page, View, or Layout source HTML. |
| [search_code](search_code.json) | Search names and source text across site pages, scripts, styles, menus, layouts, views, forms, APIs, and code blocks. |
| [search_site_object](search_site_object.json) | Search the current site search index by keyword with skip/take pagination. |
| [set_label](set_label.json) | Set one or more labels in batch. |
| [show_action_button](show_action_button.json) | Show a simple clickable action button in the AI chat with optional explanatory text and a target link. |
| [show_next_recommended_action](show_next_recommended_action.json) | Show recommended next actions in the AI chat. |
| [transfer_page](transfer_page.json) | Import one external web page and its resources into the current site. |
| [update_content](update_content.json) | Update content in the specified content folder; data must comply with the content type schema. |
| [update_content_type](update_content_type.json) | Update an existing content type by name. deleteProperties removes fields by exact name; appendProperties adds new fields. |
| [update_database_table](update_database_table.json) | Update internal SQLite schema atomically: rename columns, drop columns, add columns, then optionally rename the table. |
| [update_htmlblock](update_htmlblock.json) | Update the HTML content of a specified HtmlBlock by name and culture/language. |
| [update_object_route](update_object_route.json) | Update the route (URL) of a site object by specifying the old URL and the new URL. |
| [update_text_object](update_text_object.json) | Update a text object (Page, Layout, View, Script, Style, Api, CodeBlock). |

### AppBuilding (29)

| Function | Description |
| --- | --- |
| [accept_blueprint](accept_blueprint.json) | Persist a user-reviewed Blueprint proposal, including Actors, Goals with their Actor lists, and Scenarios. |
| [create_app_base](create_app_base.json) | Initialize a client-rendered App using direct configuration when both Blueprint and App Plan are absent, or the current App Plan when planning exists. |
| [create_app_blueprint_version](create_app_blueprint_version.json) | Retired legacy kfile Blueprint creation entry point. |
| [create_app_plan_fast](create_app_plan_fast.json) | Retired App Plan writer; use the typed per-object save tools. |
| [create_app_plan_fast_with_design](create_app_plan_fast_with_design.json) | Retired App Plan writer; use the typed per-object save tools. |
| [create_app_plan_version](create_app_plan_version.json) | Retired App Plan writer; use the typed per-object save tools. |
| [delete_app_plan_objects](delete_app_plan_objects.json) | Delete exact current App Plan objects by type and ID. |
| [install_backend_templates](install_backend_templates.json) | Copy selected backend templates into the current site as application-owned CodeBlocks and routed APIs, without requiring App Base or an App Plan. |
| [propose_blueprint](propose_blueprint.json) | Prepare a reviewable application Blueprint proposal from structured business requirements. |
| [read_app_blueprint](read_app_blueprint.json) | Read the accepted Blueprint graph from AIApplicationStore. |
| [read_app_plan](read_app_plan.json) | Read the current strongly typed server App Plan with all settings, deliverySlices, screens, dataContracts, services and sourceFiles. |
| [save_actor](save_actor.json) | Save one reviewed Actor in the existing Blueprint. |
| [save_app_settings](save_app_settings.json) | Save reviewed complete App settings: designSystem, layout and navigation. |
| [save_blueprint](save_blueprint.json) | Save the reviewed initial Blueprint structure: root, Actors, Goals, Modules, Scenarios and their relations. |
| [save_business_object](save_business_object.json) | Save one reviewed enriched Business Object through the shared Blueprint save contract. |
| [save_business_rule](save_business_rule.json) | Save one reviewed Business Rule in the existing Blueprint. |
| [save_data_contract](save_data_contract.json) | Save reviewed complete contracts in the current server App Plan. |
| [save_delivery_slice](save_delivery_slice.json) | Save reviewed complete slices in the current server App Plan. |
| [save_fact](save_fact.json) | Save one complete reviewed FCO-IM Fact and optional Scenario link through the shared Blueprint save contract. |
| [save_goal](save_goal.json) | Save one reviewed Goal in the existing Blueprint. actorIds are stored Actor GUIDs; a new Goal requires at least one. |
| [save_module](save_module.json) | Save one reviewed Module and optional Scenario links through the shared Blueprint save contract. |
| [save_scenario](save_scenario.json) | Save one reviewed Scenario and optional Module links through the shared Blueprint save contract. |
| [save_screen](save_screen.json) | Save reviewed complete screens in the current server App Plan. |
| [save_service](save_service.json) | Save reviewed complete services in the current server App Plan. |
| [save_source_file](save_source_file.json) | Save reviewed complete files in the current server App Plan. |
| [save_user_story](save_user_story.json) | Save one reviewed User Story in the existing Blueprint. actorId references a saved beneficiary; goalId identifies its single primary saved Goal and is required for creation. scenarioIds reference saved Scenarios. |
| [sync_design_runtime](sync_design_runtime.json) | Preview or synchronize the installed Yuanli, Element Plus or Ant Design Vue UI framework. |
| [update_app_blueprint](update_app_blueprint.json) | Retired legacy kfile Blueprint update entry point. |
| [update_app_plan](update_app_plan.json) | Retired App Plan writer; use the typed per-object save tools. |

### Commerce (23)

| Function | Description |
| --- | --- |
| [create_discount](create_discount.json) | Create a new discount. |
| [create_product](create_product.json) | Create a new product with a default variant and its manual Commerce category relations. |
| [create_product_category](create_product_category.json) | Create a new product category. |
| [daily_sale_stats](daily_sale_stats.json) | Get daily sales statistics including total sales amount, quantity, order count, and unique customer count for each day within a specified date range, with pagination support. |
| [delete_discount](delete_discount.json) | Delete a discount by its id. |
| [delete_product](delete_product.json) | Delete a product by its id. |
| [delete_product_category](delete_product_category.json) | Delete a product category by its id. |
| [get_cart](get_cart.json) | Get detailed information for a cart by cart id. |
| [get_customer](get_customer.json) | Get a customer by id, phone, or email. |
| [get_discount](get_discount.json) | Get a discount by id, including its usage count. |
| [get_order](get_order.json) | Get detailed information for an order by order id. |
| [get_product_category_detail](get_product_category_detail.json) | Get detailed information of a product category by its id or seoName, including ancestor path and direct children. |
| [get_product_detail](get_product_detail.json) | Get detailed information of a product by its id or seoName, including variants and categories. |
| [list_cart](list_cart.json) | List carts in descending update time, with optional pagination by skip/take. |
| [list_customer](list_customer.json) | List customers in descending last order time, with optional pagination by skip/take. |
| [list_discount](list_discount.json) | List all discounts with usage counts. |
| [list_order](list_order.json) | List orders in descending creation time, with optional status/customer filter and pagination by skip/take. |
| [list_product](list_product.json) | List all products. |
| [list_product_category](list_product_category.json) | List all product categories. |
| [order_sale_stats](order_sale_stats.json) | Get order sales statistics including detailed order line information with customer, shipping, and payment details for orders within a specified date range, with pagination support. |
| [product_sale_stats](product_sale_stats.json) | Get product sales statistics including sales amount, quantity, and order count for products sold within a specified date range, with pagination support. |
| [update_product](update_product.json) | Update basic information of an existing product by its id. |
| [update_product_category](update_product_category.json) | Update an existing product category by its id. |

### History (12)

| Function | Description |
| --- | --- |
| [checkout_history](checkout_history.json) | Create another website containing the current website's state at a history log ID. |
| [compare_history_versions](compare_history_versions.json) | Compare two history versions of the same CMS object. |
| [compare_history_with_latest](compare_history_with_latest.json) | Compare a history version with the latest version of the same object. |
| [get_history_item](get_history_item.json) | Get one CMS history item by its log ID. |
| [list_history](list_history.json) | List CMS history using paging and optional filters. |
| [list_history_versions](list_history_versions.json) | List all recorded versions of the object referenced by a history log ID. |
| [list_object_history_versions](list_object_history_versions.json) | List all recorded versions of a CMS object by its name or ID and its store or table name. |
| [list_recent_history](list_recent_history.json) | List the most recent CMS history items. |
| [restore_history_to_log](restore_history_to_log.json) | Restore the current website to the state at a history log ID. |
| [restore_history_to_time](restore_history_to_time.json) | Restore the current website to the last recorded state at or before a date/time. |
| [undo_history_item](undo_history_item.json) | Undo one CMS history change. |
| [undo_history_items](undo_history_items.json) | Undo multiple CMS history changes after validating every log ID. |

### Basic (11)

| Function | Description |
| --- | --- |
| [function_discover](function_discover.json) | Discover callable functions. |
| [get_site_info](get_site_info.json) | Returns site settings, resource counts and appPlanning presence (present, missing). |
| [list_todos](list_todos.json) | Read the todo list for current conversation |
| [load_skill](load_skill.json) | Load the entry instructions for an installed skill. |
| [read_skill_resource](read_skill_resource.json) | Read a resource inside an installed skill, such as a reference document or asset. |
| [run_kscript](run_kscript.json) | Runs KScript code and returns the execution result. |
| [run_skill_script](run_skill_script.json) | Runs a JavaScript file associated with a skill. |
| [update_site_milestones](update_site_milestones.json) | Update site milestones. |
| [update_site_settings](update_site_settings.json) | Update settings for the current site. |
| [update_todos](update_todos.json) | Update the todo list for current conversation |
| [use_function](use_function.json) | Load authoritative InputSchema and ReturnSchema before invoking a function found through function_discover. |

### Sandbox (8)

| Function | Description |
| --- | --- |
| [copy_sandbox_objects](copy_sandbox_objects.json) | Copy selected object types from the current site into an existing sandbox site. |
| [create_sandbox](create_sandbox.json) | Create a new sandbox site. |
| [delete_sandbox](delete_sandbox.json) | Delete a sandbox site created for the current site by name. |
| [list_sandbox](list_sandbox.json) | List sandbox sites created for the current site. |
| [sandbox_eval_kdata](sandbox_eval_kdata.json) | Execute a KData block and return its exported variables. |
| [sandbox_eval_script](sandbox_eval_script.json) | Execute KScript in a sandbox site and return the script result. |
| [sandbox_render_template](sandbox_render_template.json) | Render template HTML in a sandbox site with optional model values. |
| [sandbox_request_url](sandbox_request_url.json) | Render a relative URL inside a sandbox site and return its status, body, and execution time. |

### Site (7)

| Function | Description |
| --- | --- |
| [add_site_user](add_site_user.json) | Add a user to current site by username. |
| [delete_site_user](delete_site_user.json) | Delete a site user by username. |
| [execute_automation](execute_automation.json) | Execute an automation task synchronously and return its response lines. |
| [internalize_url](internalize_url.json) | Internalize external URLs that saved site objects already reference (recorded as site external resources) into local site resources, rewriting those references. |
| [list_automation](list_automation.json) | List available automation tasks for the current site context. |
| [list_external_url](list_external_url.json) | List external URLs from current site. |
| [list_site_user_roles](list_site_user_roles.json) | List available site user roles. |

### Analytic (6)

| Function | Description |
| --- | --- |
| [get_error_monitoring_overview](get_error_monitoring_overview.json) | Get the current site's complete error analytics summary, including trends, affected users, error types, and high-frequency errors. |
| [get_resource_guardian_overview](get_resource_guardian_overview.json) | Get the current site's complete resource analytics summary. |
| [get_visitor_traffic_overview](get_visitor_traffic_overview.json) | Get the current site's complete visitor analytics summary. |
| [list_error_logs](list_error_logs.json) | List paginated error analytics logs for the current site. |
| [list_resource_logs](list_resource_logs.json) | List recent resource analytics logs for the current site. |
| [list_visitor_logs](list_visitor_logs.json) | List paginated visitor analytics logs for the current site. |

### SiteDevelopment (6)

| Function | Description |
| --- | --- |
| [prepare_site_memory_handoff](prepare_site_memory_handoff.json) | Finish a user-requested move to a new conversation after saving necessary memory. |
| [read_planning_topics](read_planning_topics.json) | Read site-level assumptions, open questions and answers independently of Blueprint and App Plan. |
| [read_site_memory](read_site_memory.json) | Read site-owned Markdown memory. |
| [search_site_memory](search_site_memory.json) | Search site memory by a literal phrase; returns bounded excerpts and offsets. |
| [update_planning_topic](update_planning_topic.json) | Create or replace one site-level planning topic by stable ID. |
| [update_site_memory](update_site_memory.json) | Save a small site memory note and return a verified readback receipt. |

### TaskJob (6)

| Function | Description |
| --- | --- |
| [add_ai_task_job](add_ai_task_job.json) | Create a new AI task job for the current site. |
| [ai_task_finish](ai_task_finish.json) |  |
| [delete_ai_task_job](delete_ai_task_job.json) | Delete an AI task job by its id or name. |
| [get_ai_task_job_detail](get_ai_task_job_detail.json) | Get detailed information of an AI task job by its id or name. |
| [list_ai_task_jobs](list_ai_task_jobs.json) | List AI task jobs in the current site, including scheduling settings and running status. |
| [update_ai_task_job](update_ai_task_job.json) | Update an existing AI task job by its id or name. |

### Utils (3)

| Function | Description |
| --- | --- |
| [add_image_watermark](add_image_watermark.json) | Add an image watermark to an image stored as a site Image, CmsFile, or FileIO file. |
| [change_image_format](change_image_format.json) | Convert an image stored as a site Image, CmsFile, or FileIO file to another image format. |
| [image_resize](image_resize.json) | Resize an image stored as a site Image, CmsFile, or FileIO file while preserving its aspect ratio. |

### Settings (2)

| Function | Description |
| --- | --- |
| [get_cookie_management_overview](get_cookie_management_overview.json) | Unavailable: this tool is not implemented and must not be called. |
| [list_cookie_definitions](list_cookie_definitions.json) | Unavailable: this tool is not implemented and must not be called. |
