import type { RouteRecordRaw } from "vue-router";

const mailRoutes: Array<RouteRecordRaw> = [
  {
    path: "/kmail",
    meta: {
      oldPath: "/_Admin/Emails/Inbox",
    },
    component: () => import("@/views/mail/index.vue"),
    children: [
      {
        path: "",
        redirect: { name: "inbox" },
      },
      {
        path: "inbox",
        name: "inbox",
        component: () => import("@/views/mail/inbox/index.vue"),
      },
      {
        path: "content",
        name: "mail-content",
        component: () => import("@/views/mail/content/index.vue"),
      },
      {
        path: "compose",
        name: "compose",
        component: () => import("@/views/mail/compose/index.vue"),
      },
      {
        path: "sent",
        name: "sent",
        component: () => import("@/views/mail/sent/index.vue"),
      },
      {
        path: "draft",
        name: "drafts",
        component: () => import("@/views/mail/draft/index.vue"),
      },
      {
        path: "trash",
        name: "trash",
        component: () => import("@/views/mail/trash/index.vue"),
      },
      {
        path: "spam",
        name: "spam",
        component: () => import("@/views/mail/spam/index.vue"),
      },
      {
        path: "calendar",
        name: "calendar",
        component: () => import("@/views/mail/calendar/index.vue"),
      },
      {
        path: "folder",
        name: "folder",
        component: () => import("@/views/mail/folder/index.vue"),
      },
      {
        path: "searchEmail",
        name: "searchEmail",
        component: () => import("@/views/mail/search/index.vue"),
      },
    ],
  },
  {
    path: "/original-message",
    name: "original message",
    component: () => import("@/views/mail/show-original-message.vue"),
  },
  {
    path: "/kmail-settings",
    component: () => import("@/views/mail/settings/index.vue"),
    meta: {
      oldPath: "/_Admin/Emails/Inbox",
    },
    children: [
      {
        path: "",
        redirect: { name: "addresses" },
      },
      {
        path: "addresses",
        name: "addresses",
        component: () => import("@/views/mail/settings/addresses/index.vue"),
      },
      {
        path: "contacts",
        name: "contacts",
        component: () => import("@/views/mail/settings/contacts/index.vue"),
      },
      {
        path: "security-report",
        name: "security-report",
        component: () =>
          import("@/views/mail/settings/sucurity-report/index.vue"),
      },
      {
        path: "mail-module",
        name: "mail-module",
        component: () => import("@/views/mail/settings/module/index.vue"),
      },
      {
        path: "imap-setting",
        name: "imap-setting",
        component: () => import("@/views/mail/settings/imap-setting/index.vue"),
      },
      {
        path: "inbox-logo",
        name: "inbox-logo",
        component: () => import("@/views/mail/settings/inbox-logo/index.vue"),
      },
      {
        path: "migration",
        name: "mail-migration",
        component: () => import("@/views/mail/settings/migration/index.vue"),
      },
      {
        path: "smtp-setting",
        name: "smtp-setting",
        component: () => import("@/views/mail/settings/smtp-setting/index.vue"),
      },
    ],
  },
  {
    path: "/mail-module-editor",
    meta: {
      oldPath: "/_Admin/Emails/Inbox",
    },
    name: "mail-module-editor",
    component: () => import("@/views/mail/settings/module-editor/index.vue"),
  },
];

export default mailRoutes;
