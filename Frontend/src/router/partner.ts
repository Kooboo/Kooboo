import type { RouteRecordRaw } from "vue-router";

const partnerRoutes: Array<RouteRecordRaw> = [
  {
    path: "/partner",
    meta: {
      oldPath: "/_Admin/Partner",
    },
    component: () => import("@/views/partner/index.vue"),
    children: [
      {
        path: "",
        name: "partner",
        component: () => import("@/views/partner/server/index.vue"),
      },
      {
        path: "dns",
        name: "dns",
        meta: {
          activeMenu: "dns",
        },
        component: () => import("@/views/partner/dns/index.vue"),
      },
      {
        path: "subAccount",
        name: "subAccount",
        meta: {
          activeMenu: "subAccount",
        },
        component: () => import("@/views/partner/sub-account/index.vue"),
      },
    ],
  },
];

export default partnerRoutes;
