import type { RouteRecordRaw } from "vue-router";

const commonRoutes: Array<RouteRecordRaw> = [
  {
    path: "/404",
    name: "404",
    component: () => import("@/views/common/error.vue"),
  },
  {
    path: "/dev-mode",
    component: () => import("@/layout/site-blank.vue"),
    children: [
      {
        path: "",
        name: "dev-mode",
        component: () => import("@/views/dev-mode/index.vue"),
      },
    ],
  },
  {
    path: "/inline-design",
    name: "inline-design",
    component: () => import("@/views/inline-design"),
  },
];

export default commonRoutes;
