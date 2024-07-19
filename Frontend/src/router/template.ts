import type { RouteRecordRaw } from "vue-router";

const templateRoutes: Array<RouteRecordRaw> = [
  {
    path: "/",
    meta: {
      oldPath: "/_Admin/Sites",
    },
    component: () => import("@/layout/template.vue"),
    children: [
      {
        path: "/templates",
        name: "templates",
        component: () => import("@/views/template/index.vue"),
      },
      {
        path: "/template/detail",
        name: "template-detail",
        component: () => import("@/views/template/template-detail.vue"),
      },
    ],
  },
];

export default templateRoutes;
