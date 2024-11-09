import type { RouteLocationNormalizedLoaded, RouteRecordRaw } from "vue-router";

import type { Menu } from "@/global/types";
import { i18n } from "@/modules/i18n";

const t = i18n.global.t;

const siteRoutes: Array<RouteRecordRaw> = [
  {
    path: "/site",
    component: () => import("@/layout/site-menu.vue"),
    children: [
      {
        path: "",
        name: "dashboard",
        component: () => import("@/views/dashboard"),
      },
      {
        path: "media",
        name: "media",
        meta: {
          recentVisits: "common.mediaLibrary",
          menu: {
            display: t("common.mediaLibrary"),
            permission: { feature: "mediaLibrary" },
            icon: "icon-media1",
            queryBuilder(menu: Menu, route: RouteLocationNormalizedLoaded) {
              return {
                SiteId: route.query.SiteId,
                listType: menu.name === "media" ? "grid" : "",
                folder: "/",
                keyword: "",
                provider: "default",
              };
            },
          },
        },
        component: () => import("@/views/content/media/index.vue"),
      },
      {
        path: "editImage",
        name: "editImage",
        component: () => import("@/views/content/media/image-editor.vue"),
        meta: {
          activeMenu: "media",
        },
      },
      {
        path: "pages",
        name: "pages",
        meta: {
          recentVisits: "common.pages",
          menu: {
            display: t("common.pages"),
            permission: { feature: "pages" },
            icon: "icon-page3",
          },
        },
        component: () => import("@/views/pages/list/index.vue"),
      },
      {
        path: "modules",
        name: "modules",
        meta: {
          recentVisits: "common.modules",
          menu: {
            display: t("common.modules"),
            permission: { feature: "module" },
            icon: "icon-module1",
          },
        },
        component: () => import("@/views/modules/index.vue"),
      },
      {
        path: "module-menu/:module",
        name: "module menu",
        component: () => import("@/views/modules/module-index.vue"),
      },
    ],
  },
  {
    path: "/page",
    component: () => import("@/layout/site-blank.vue"),
    children: [
      {
        path: "edit",
        name: "page-edit",
        component: () => import("@/views/pages/normal-page/edit.vue"),
      },
      {
        path: "setting",
        name: "page-setting",
        component: () => import("@/views/pages/normal-page/setting.vue"),
      },
      {
        path: "design",
        name: "page-design",
        component: () => import("@/views/pages/normal-page/design.vue"),
      },
      {
        path: "layout-edit",
        name: "layout-page-edit",
        component: () => import("@/views/pages/layout-page/edit.vue"),
      },
      {
        path: "layout-setting",
        name: "layout-page-setting",
        component: () => import("@/views/pages/layout-page/setting.vue"),
      },
      {
        path: "layout-design",
        name: "layout-page-design",
        component: () => import("@/views/pages/layout-page/design.vue"),
      },
      {
        path: "rich",
        name: "rich-page-edit",
        component: () => import("@/views/pages/rich-page/index.vue"),
      },
    ],
  },
  // system start
  {
    path: "/system",
    component: () => import("@/layout/site-menu.vue"),
    meta: {
      menu: {
        name: "system",
        display: t("common.system"),
        icon: "icon-system",
      },
      advanced: true,
    },
    children: [
      {
        path: "settings",
        name: "settings",
        meta: {
          recentVisits: "common.settings",
          menu: {
            display: t("common.settings"),
            permission: { feature: "site", action: "edit" },
          },
        },
        component: () => import("@/views/system/settings/index.vue"),
      },
      {
        path: "config",
        name: "config",
        meta: {
          recentVisits: "common.config",
          menu: {
            display: t("common.config"),
            permission: { feature: "config" },
          },
          activeMenu: "config",
          advanced: true,
        },
        component: () => import("@/views/system/config/index.vue"),
      },
      {
        path: "domains",
        name: "domains",
        meta: {
          recentVisits: "common.domains",
          menu: {
            display: t("common.domains"),
            permission: { feature: "domain" },
          },
        },
        component: () => import("@/views/system/domains/index.vue"),
      },
      {
        path: "sync",
        name: "sync",
        meta: {
          recentVisits: "common.sync",
          menu: {
            display: t("common.sync"),
            permission: { feature: "sync" },
          },
          advanced: true,
        },
        component: () => import("@/views/system/sync/list/index.vue"),
      },
      {
        path: "sync/list",
        name: "sync-list",
        meta: {
          activeMenu: "sync",
        },
        component: () => import("@/views/system/sync/publishing/index.vue"),
      },
      {
        path: "sync/server",
        name: "sync-server",
        meta: {
          activeMenu: "sync",
        },
        component: () => import("@/views/system/sync/server/server-list.vue"),
      },
      {
        path: "site-logs",
        name: "sitelogs",
        meta: {
          recentVisits: "common.siteLogs",
          menu: {
            display: t("common.siteLogs"),
            permission: { feature: "site", action: "log" },
          },
        },
        component: () => import("@/views/system/site-logs/index.vue"),
      },
      {
        path: "site-logs/versions",
        name: "log-versions",
        component: () => import("@/views/system/site-logs/log-versions.vue"),
        meta: {
          activeMenu: "sitelogs",
        },
      },
      {
        path: "visitor-logs",
        name: "visitorlogs",
        meta: {
          menu: {
            display: t("common.visitorLogs"),
            permission: { feature: "visitorLog" },
          },
        },
        component: () => import("@/views/system/visitor-logs/index.vue"),
      },
      {
        path: "jobs",
        name: "jobs",
        meta: {
          recentVisits: "common.jobs",
          menu: {
            display: t("common.jobs"),
            permission: { feature: "job" },
          },
          advanced: true,
        },
        component: () => import("@/views/system/jobs/index.vue"),
      },
      {
        path: "siteuser",
        name: "siteuser",
        meta: {
          recentVisits: "common.siteUser",
          menu: {
            display: t("common.siteUser"),
            permission: { feature: "siteUser" },
          },
        },
        component: () => import("@/views/system/user/index.vue"),
      },
      {
        path: "roles",
        name: "roles",
        meta: {
          recentVisits: "common.roles",
          menu: {
            display: t("common.roles"),
            permission: { feature: "role" },
          },
          advanced: true,
        },
        component: () => import("@/views/system/role/index.vue"),
      },
      {
        path: "front-events",
        name: "frontevents",
        meta: {
          recentVisits: "common.frontEvents",
          menu: {
            display: t("common.frontEvents"),
            permission: { feature: "frontEvents" },
          },
          advanced: true,
        },
        component: () => import("@/views/system/front-events/index.vue"),
      },
      {
        path: "front-events/edit",
        name: "frontevents-edit",
        meta: {
          activeMenu: "frontevents",
        },
        component: () => import("@/views/system/front-events/edit.vue"),
      },
    ],
  },
  {
    path: "/system",
    component: () => import("@/layout/site-blank.vue"),
    children: [
      {
        path: "site-logs/version-compare",
        name: "version-compare",
        component: () => import("@/views/system/site-logs/version-compare.vue"),
      },
    ],
  },
  // system end

  // content start
  {
    path: "/content",
    component: () => import("@/layout/site-menu.vue"),
    meta: {
      menu: {
        name: "content",
        display: t("common.content"),
        icon: "icon-content1",
      },
      advanced: true,
    },
    children: [
      {
        path: "contents",
        name: "contents",
        meta: {
          recentVisits: "common.content",
          menu: {
            display: t("common.content"),
            permission: { feature: "content" },
          },
        },
        component: () => import("@/views/content/contents/index.vue"),
      },
      {
        path: "textContentsByFolder",
        name: "textcontentsbyfolder",
        component: () =>
          import("@/views/content/contents/contents-by-folder.vue"),
        meta: {
          activeMenu: "contents",
        },
      },
      {
        path: "content",
        name: "content",
        component: () => import("@/views/content/contents/content.vue"),
        meta: {
          activeMenu: "contents",
        },
      },
      {
        path: "contentTypes",
        name: "contenttypes",
        meta: {
          recentVisits: "common.contentTypes",
          menu: {
            display: t("common.contentTypes"),
            permission: { feature: "contentType" },
          },
        },
        component: () => import("@/views/content/content-types/index.vue"),
      },
      {
        path: "contentType",
        name: "contenttype",
        component: () =>
          import("@/views/content/content-types/content-type.vue"),
        meta: {
          activeMenu: "contenttypes",
        },
      },
      {
        path: "labels",
        name: "labels",
        meta: {
          recentVisits: "common.labels",
          menu: {
            display: t("common.labels"),
            permission: { feature: "label" },
          },
        },
        component: () => import("@/views/content/labels/index.vue"),
      },
      {
        path: "htmlblocks",
        name: "htmlblocks",
        meta: {
          recentVisits: "common.htmlBlocks",
          menu: {
            display: t("common.htmlBlocks"),
            permission: { feature: "htmlBlock" },
          },
        },
        component: () => import("@/views/content/html-blocks/index.vue"),
      },
      {
        path: "htmlBlock/edit",
        name: "htmlBlock-edit",
        component: () => import("@/views/content/html-blocks/html-block.vue"),
        meta: {
          activeMenu: "htmlblocks",
        },
      },
      {
        path: "files",
        name: "files",
        meta: {
          recentVisits: "common.files",
          menu: {
            display: t("common.files"),
            permission: { feature: "file" },
            queryBuilder(menu: Menu, route: RouteLocationNormalizedLoaded) {
              return {
                SiteId: route.query.SiteId,
                folder: "/",
                keyword: "",
                provider: "default",
              };
            },
          },
          advanced: true,
        },
        component: () => import("@/views/content/files/index.vue"),
      },
      {
        path: "text",
        name: "text",
        meta: {
          recentVisits: "common.text",
          menu: {
            display: t("common.text"),
            permission: { feature: "text" },
          },
          advanced: true,
        },
        component: () => import("@/views/content/text/index.vue"),
      },
      {
        path: "useroptions",
        name: "useroptions",
        meta: {
          recentVisits: "common.userOptions",
          menu: {
            display: t("common.userOptions"),
            permission: { feature: "userOptions" },
          },
          advanced: true,
        },
        component: () => import("@/views/content/user-options/index.vue"),
      },
      {
        path: "useroptions/create",
        name: "useroptions create",
        meta: {
          activeMenu: "useroptions",
        },
        component: () => import("@/views/content/user-options/create.vue"),
      },
      {
        path: "useroptions/setting",
        name: "useroptions setting",
        meta: {
          activeMenu: "useroptions",
        },
        component: () => import("@/views/content/user-options/setting.vue"),
      },

      {
        path: "useroptions/edit",
        name: "useroptions edit",
        meta: {
          activeMenu: "useroptions",
        },
        component: () => import("@/views/content/user-options/edit.vue"),
      },
    ],
  },
  // content end

  //E-Commerce start
  {
    path: "/commerce",
    component: () => import("@/layout/site-menu.vue"),
    meta: {
      menu: {
        name: "commerce",
        display: t("common.commerce"),
        icon: "icon-a-Electronicbusiness",
      },
      advanced: true,
    },
    children: [
      {
        path: "product-management",
        name: "product management",
        meta: {
          title: t("common.productManagement"),
          activeMenu: "product management",
          recentVisits: "common.productManagement",
          menu: {
            display: t("common.productManagement"),
            permission: { feature: "productManagement" },
          },
        },
        component: () =>
          import("@/views/commerce/products-management/index.vue"),
      },
      {
        path: "product-management/create",
        name: "product management create",
        meta: {
          title: t("common.productManagement"),
          activeMenu: "product management",
        },
        component: () =>
          import("@/views/commerce/products-management/create.vue"),
      },
      {
        path: "product-management/edit",
        name: "product management edit",
        meta: {
          title: t("common.productManagement"),
          activeMenu: "product management",
        },
        component: () =>
          import("@/views/commerce/products-management/edit.vue"),
      },
      {
        path: "product-types",
        name: "product types",
        meta: {
          title: t("common.productTypes"),
          activeMenu: "product types",
          recentVisits: "common.productTypes",
          menu: {
            display: t("common.productTypes"),
            permission: { feature: "productTypes" },
          },
        },
        component: () => import("@/views/commerce/types/index.vue"),
      },
      {
        path: "product-categories",
        name: "product categories",
        meta: {
          title: t("common.productCategories"),
          activeMenu: "product categories",
          recentVisits: "common.productCategories",
          menu: {
            display: t("common.productCategories"),
            permission: { feature: "productCategories" },
          },
        },
        component: () => import("@/views/commerce/categories/index.vue"),
      },
      {
        path: "product-categories/create",
        name: "product category create",
        component: () => import("@/views/commerce/categories/create.vue"),
        meta: {
          activeMenu: "product categories",
        },
      },
      {
        path: "product-categories/edit",
        name: "product category edit",
        component: () => import("@/views/commerce/categories/edit.vue"),
        meta: {
          activeMenu: "product categories",
        },
      },
      {
        path: "carts",
        name: "carts",
        meta: {
          title: t("common.carts"),
          activeMenu: "carts",
          recentVisits: "common.carts",
          menu: {
            display: t("common.carts"),
            permission: { feature: "carts" },
          },
        },
        component: () => import("@/views/commerce/carts/index.vue"),
      },
      {
        path: "cart/create",
        name: "cart create",
        component: () => import("@/views/commerce/carts/create.vue"),
        meta: {
          activeMenu: "carts",
        },
      },
      {
        path: "cart/edit",
        name: "cart edit",
        component: () => import("@/views/commerce/carts/edit.vue"),
        meta: {
          activeMenu: "carts",
        },
      },
      {
        path: "cart/checkout",
        name: "cart checkout",
        component: () => import("@/views/commerce/carts/checkout.vue"),
        meta: {
          activeMenu: "carts",
        },
      },
      {
        path: "customers",
        name: "customers",
        meta: {
          title: t("common.customers"),
          activeMenu: "customers",
          recentVisits: "common.customers",
          menu: {
            display: t("common.customers"),
            permission: { feature: "customers" },
          },
        },
        component: () => import("@/views/commerce/customers/index.vue"),
      },
      {
        path: "orders",
        name: "orders",
        meta: {
          title: t("common.orders"),
          activeMenu: "orders",
          recentVisits: "common.orders",
          menu: {
            display: t("common.orders"),
            permission: { feature: "orders" },
          },
        },
        component: () => import("@/views/commerce/orders/index.vue"),
      },
      {
        path: "order-detail",
        name: "order detail",
        meta: {
          title: t("common.orders"),
          activeMenu: "orders",
        },
        component: () => import("@/views/commerce/orders/detail.vue"),
      },
      {
        path: "sale-stats",
        name: "sale stats",
        meta: {
          title: t("common.saleStats"),
          activeMenu: "sale stats",
          recentVisits: "common.saleStats",
          advanced: true,
          menu: {
            display: t("common.saleStats"),
            permission: { feature: "saleStats" },
          },
        },
        component: () => import("@/views/commerce/sale-stats/index.vue"),
      },
      {
        path: "discounts",
        name: "discounts",
        meta: {
          title: t("common.discounts"),
          activeMenu: "discounts",
          recentVisits: "common.discounts",
          advanced: true,
          menu: {
            display: t("common.discounts"),
            permission: { feature: "discounts" },
          },
        },
        component: () => import("@/views/commerce/discounts/index.vue"),
      },
      {
        path: "discounts/create",
        name: "discount create",
        component: () => import("@/views/commerce/discounts/create.vue"),
        meta: {
          activeMenu: "discounts",
        },
      },
      {
        path: "discounts/edit",
        name: "discount edit",
        component: () => import("@/views/commerce/discounts/edit.vue"),
        meta: {
          activeMenu: "discounts",
        },
      },
      {
        path: "shippings",
        name: "shippings",
        meta: {
          title: t("common.shippings"),
          activeMenu: "shippings",
          recentVisits: "common.shippings",
          advanced: true,
          menu: {
            display: t("common.shippings"),
            permission: { feature: "shipping" },
          },
        },
        component: () => import("@/views/commerce/shippings/index.vue"),
      },
      {
        path: "shippings/create",
        name: "shipping create",
        component: () =>
          import("@/views/commerce/shippings/express/create.vue"),
        meta: {
          activeMenu: "shippings",
        },
      },
      {
        path: "shippings/edit",
        name: "shipping edit",
        component: () => import("@/views/commerce/shippings/express/edit.vue"),
        meta: {
          activeMenu: "shippings",
        },
      },
      {
        path: "digital-shippings/create",
        name: "digital shipping create",
        component: () =>
          import("@/views/commerce/shippings/digital/create.vue"),
        meta: {
          activeMenu: "shippings",
        },
      },
      {
        path: "digital-shippings/edit",
        name: "digital shipping edit",
        component: () => import("@/views/commerce/shippings/digital/edit.vue"),
        meta: {
          activeMenu: "shippings",
        },
      },
      {
        path: "loyalty",
        name: "loyalty",
        meta: {
          title: t("common.loyalty"),
          activeMenu: "loyalty",
          recentVisits: "common.loyalty",
          advanced: true,
          menu: {
            display: t("common.loyalty"),
            permission: { feature: "loyalty" },
          },
        },
        component: () => import("@/views/commerce/loyalty/index.vue"),
      },
      {
        path: "notification",
        name: "commerce notification",
        meta: {
          title: t("common.notifications"),
          activeMenu: "commerce notification",
          recentVisits: "common.notifications",
          advanced: true,
          menu: {
            display: t("common.notifications"),
            permission: { feature: "commerceSettings" },
          },
        },
        component: () => import("@/views/commerce/notifications/index.vue"),
      },
      {
        path: "settings",
        name: "commerce settings",
        meta: {
          title: t("common.settings"),
          activeMenu: "commerce settings",
          recentVisits: "common.settings",
          menu: {
            display: t("common.settings"),
            permission: { feature: "commerceSettings" },
          },
        },
        component: () => import("@/views/commerce/settings/index.vue"),
      },
    ],
  },
  //E-Commerce end
  // development start
  {
    path: "/development",
    component: () => import("@/layout/site-menu.vue"),
    meta: {
      menu: {
        name: "development",
        display: t("common.development"),
        icon: "icon-development1",
      },
      advanced: true,
    },
    children: [
      {
        path: "layouts",
        name: "layouts",
        meta: {
          recentVisits: "common.layouts",
          menu: {
            display: t("common.layouts"),
            permission: { feature: "layout" },
          },
        },
        component: () => import("@/views/development/layouts/index.vue"),
      },
      {
        path: "views",
        name: "views",
        meta: {
          recentVisits: "common.views",
          menu: {
            display: t("common.views"),
            permission: { feature: "view" },
          },
        },
        component: () => import("@/views/development/views/index.vue"),
      },
      {
        path: "forms",
        name: "forms",
        meta: {
          recentVisits: "common.forms",
          menu: {
            display: t("common.forms"),
            permission: { feature: "form" },
          },
          advanced: true,
        },
        component: () => import("@/views/development/forms/index.vue"),
      },
      {
        path: "form-values",
        name: "form-values",
        component: () => import("@/views/development/forms/values.vue"),
        meta: {
          activeMenu: "forms",
        },
      },
      {
        path: "menus",
        name: "menus",
        meta: {
          recentVisits: "common.menus",
          menu: {
            display: t("common.menus"),
            permission: { feature: "menu" },
          },
          advanced: true,
        },
        component: () => import("@/views/development/menus/index.vue"),
      },
      {
        path: "menu/edit",
        name: "menu-edit",
        component: () => import("@/views/development/menus/edit.vue"),
        meta: {
          activeMenu: "menus",
        },
      },
      {
        path: "scripts",
        name: "scripts",
        meta: {
          recentVisits: "common.scripts",
          menu: {
            display: t("common.scripts"),
            permission: { feature: "script" },
          },
        },
        component: () => import("@/views/development/scripts/index.vue"),
      },
      {
        path: "styles",
        name: "styles",
        meta: {
          recentVisits: "common.styles",
          menu: {
            display: t("common.styles"),
            permission: { feature: "style" },
          },
        },
        component: () => import("@/views/development/styles/index.vue"),
      },
      {
        path: "code",
        name: "code",
        meta: {
          recentVisits: "common.codes",
          menu: {
            display: t("common.codes"),
            permission: { feature: "code" },
          },
        },
        component: () => import("@/views/development/code/index.vue"),
      },
      {
        path: "code-log",
        name: "codelog",
        meta: {
          recentVisits: "common.codeLogs",
          menu: {
            display: t("common.codeLogs"),
            permission: { feature: "code", action: "log" },
          },
          advanced: true,
        },
        component: () => import("@/views/development/code-log/index.vue"),
      },
      {
        path: "code-search",
        name: "code search",
        meta: {
          recentVisits: "common.codeSearch",
          menu: {
            display: t("common.codeSearch"),
            permission: { feature: "code", action: "view" },
          },
          advanced: true,
        },
        component: () => import("@/views/development/code-search/index.vue"),
      },
      {
        path: "urls",
        name: "urls",
        meta: {
          recentVisits: "common.urls",
          menu: {
            display: t("common.urls"),
            permission: { feature: "link" },
          },
          advanced: true,
        },
        component: () => import("@/views/development/urls/index.vue"),
      },
      {
        path: "authentication",
        name: "authentication",
        meta: {
          recentVisits: "common.authentication",
          menu: {
            display: t("common.authentication"),
            permission: { feature: "authentication" },
          },
          advanced: true,
        },
        component: () => import("@/views/development/authentication/index.vue"),
      },
      {
        path: "openapis",
        name: "openapis",
        meta: {
          recentVisits: "common.openApi",
          menu: {
            display: t("common.openApi"),
            permission: { feature: "openApi" },
          },
          advanced: true,
        },
        component: () => import("@/views/development/openapis/index.vue"),
      },
      {
        path: "openapi/authorizes",
        name: "openapi-authorizes",
        component: () => import("@/views/development/openapis/authorizes.vue"),
        meta: {
          activeMenu: "openapis",
          advanced: true,
        },
      },
      {
        path: "search",
        name: "search",
        meta: {
          recentVisits: "common.search",
          menu: {
            display: t("common.search"),
            permission: { feature: "search" },
          },
          advanced: true,
        },
        component: () => import("@/views/development/search/index.vue"),
      },
      {
        path: "spamultilingual",
        name: "spamultilingual",
        meta: {
          recentVisits: "common.spaMultilingual",
          menu: {
            display: t("common.spaMultilingual"),
            permission: { feature: "spaMultilingual" },
          },
          advanced: true,
        },
        component: () => import("@/views/development/spa-multilingual"),
      },
      {
        path: "automation",
        name: "automation",
        meta: {
          recentVisits: "common.automation",
          menu: {
            display: t("common.automation"),
            // permission: { feature: "automation" },
          },
          advanced: true,
        },
        component: () => import("@/views/development/automation/index.vue"),
      },
    ],
  },
  {
    path: "/development",
    component: () => import("@/layout/development.vue"),
    children: [
      {
        path: "layout/edit",
        name: "layout-edit",
        component: () => import("@/views/development/layouts/edit.vue"),
      },
      {
        path: "view/edit",
        name: "view-edit",
        component: () => import("@/views/development/views/edit.vue"),
      },
      {
        path: "form/edit",
        name: "form-edit",
        component: () => import("@/views/development/forms/edit.vue"),
      },
      {
        path: "script/edit",
        name: "script-edit",
        component: () => import("@/views/development/scripts/edit.vue"),
      },
      {
        path: "style/edit",
        name: "style-edit",
        component: () => import("@/views/development/styles/edit.vue"),
      },
      {
        path: "code/edit",
        name: "code-edit",
        component: () => import("@/views/development/code/edit.vue"),
      },
      {
        path: "openapi/edit",
        name: "openapi-edit",
        component: () => import("@/views/development/openapis/edit.vue"),
      },
    ],
  },
  // development end

  // database start
  {
    path: "/database",
    component: () => import("@/layout/site-menu.vue"),
    meta: {
      menu: {
        name: "database",
        display: t("common.database"),
        icon: "icon-database1",
      },
      advanced: true,
    },
    children: [
      {
        path: "table",
        name: "table",
        meta: {
          title: t("common.database"),
          activeMenu: "table",
          recentVisits: "common.indexedDBTable",
          menu: {
            display: t("common.indexedDBTable"),
            permission: { feature: "database" },
          },
          advanced: true,
        },
        component: () => import("@/views/database/table/index.vue"),
      },
      {
        path: "table-relation",
        name: "table relation",
        meta: {
          recentVisits: "common.indexedDBRelation",
          menu: {
            display: t("common.indexedDBRelation"),
            permission: { feature: "tableRelation" },
          },
          advanced: true,
        },
        component: () => import("@/views/database/table-relation/index.vue"),
      },
      {
        path: "key-value",
        name: "key-value",
        meta: {
          recentVisits: "common.keyValueStore",
          menu: {
            display: t("common.keyValueStore"),
            permission: { feature: "keyValue" },
          },
          advanced: true,
        },
        component: () => import("@/views/database/key-value/index.vue"),
      },
      {
        path: "sqlite-table",
        name: "sqlite.table",
        meta: {
          title: t("common.sqliteTables"),
          activeMenu: "sqlite.table",
          recentVisits: "common.sqliteTables",
          menu: {
            display: t("common.sqliteTables"),
            permission: { feature: "database" },
          },
          advanced: true,
        },
        component: () => import("@/views/database/table/index.vue"),
      },
      {
        path: "mysql-table",
        name: "mysql.table",
        meta: {
          title: t("common.mysqlTables"),
          activeMenu: "mysql.table",
          recentVisits: "common.mysqlTables",
          menu: {
            display: t("common.mysqlTables"),
            permission: { feature: "database" },
          },
          advanced: true,
        },
        component: () => import("@/views/database/table/index.vue"),
      },
      {
        path: "sqlserver-table",
        name: "sqlserver.table",
        meta: {
          title: t("common.sqlServerTables"),
          activeMenu: "sqlserver.table",
          recentVisits: "common.sqlServerTables",
          menu: {
            display: t("common.sqlServerTables"),
            permission: { feature: "database" },
          },
          advanced: true,
        },
        component: () => import("@/views/database/table/index.vue"),
      },
      {
        path: "table/columns",
        name: "table-columns",
        component: () => import("@/views/database/table/columns.vue"),
        meta: {
          activeMenu: "table",
        },
        beforeEnter(to) {
          if (to.query.dbType) {
            to.meta.activeMenu =
              (to.query.dbType as string).toLocaleLowerCase() + ".table";
          }
        },
      },
      {
        path: "table/data",
        name: "table-data",
        component: () => import("@/views/database/table/data.vue"),
        meta: {
          activeMenu: "table",
        },
        beforeEnter(to) {
          if (to.query.dbType) {
            to.meta.activeMenu =
              (to.query.dbType as string).toLocaleLowerCase() + ".table";
          }
        },
      },
      {
        path: "table/edit-data",
        name: "table-edit-data",
        component: () => import("@/views/database/table/edit-data.vue"),
        meta: {
          activeMenu: "table",
        },
        beforeEnter(to) {
          if (to.query.dbType) {
            to.meta.activeMenu =
              (to.query.dbType as string).toLocaleLowerCase() + ".table";
          }
        },
      },

      {
        path: "sql-logs",
        name: "sql logs",
        meta: {
          recentVisits: "common.sqlLogs",
          menu: {
            display: t("common.sqlLogs"),
            permission: { feature: "database", action: "log" },
          },
          advanced: true,
        },
        component: () => import("@/views/database/sql-logs/index.vue"),
      },
    ],
  },
  // database end
];

export default siteRoutes;
