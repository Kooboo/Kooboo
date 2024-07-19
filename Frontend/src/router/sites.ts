import type { RouteRecordRaw } from "vue-router";

const sitesRoutes: Array<RouteRecordRaw> = [
  {
    path: "/",
    meta: {
      oldPath: "/_Admin/Sites",
    },
    component: () => import("@/layout/blank.vue"),
    children: [
      {
        path: "",
        name: "home",
        component: () => import("@/views/home/index.vue"),
      },
      {
        path: "Sites",
        redirect: { name: "home" },
      },
      {
        path: "transferring",
        name: "transferring",
        component: () => import("@/views/create-site/transferring.vue"),
      },
      {
        path: "create",
        component: () => import("@/views/create-site/index.vue"),
        children: [
          {
            path: "",
            name: "create-site",
            component: () => import("@/views/create-site/blank.vue"),
          },
          {
            path: "import",
            name: "create-site-import",
            component: () => import("@/views/create-site/import.vue"),
          },
          {
            path: "clone",
            name: "create-site-clone",
            component: () => import("@/views/create-site/clone.vue"),
          },
        ],
      },
    ],
  },
];

export default sitesRoutes;
