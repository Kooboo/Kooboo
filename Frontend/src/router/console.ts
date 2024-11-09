import type { RouteRecordRaw } from "vue-router";

const systemRoutes: Array<RouteRecordRaw> = [
  {
    path: "/kconsole",
    meta: {
      oldPath: "/_Admin/Console",
    },
    component: () => import("@/views/console/index.vue"),
    children: [
      {
        path: "",
        name: "kconsole",
        component: () => import("@/views/console/domains/index.vue"),
      },
      // {
      //   path: "domainDNS",
      //   name: "domainDNS",
      //   meta: {
      //     activeMenu: "kconsole",
      //   },
      //   component: () => import("@/views/console/domains/domain-dns.vue"),
      // },
      {
        path: "DomainBinding",
        name: "domain-binding",
        meta: {
          activeMenu: "kconsole",
        },
        component: () => import("@/views/console/domains/domain-binding.vue"),
      },
      {
        path: "domainSearch",
        name: "domain-search",
        meta: {
          activeMenu: "kconsole",
        },
        component: () => import("@/views/console/domains/domain-search.vue"),
      },
      {
        path: "balance",
        name: "domain-balance",
        meta: {
          activeMenu: "domain-balance",
        },
        component: () => import("@/views/console/balance/index.vue"),
      },
      //11.15
      {
        path: "organization",
        name: "organization",
        meta: {
          activeMenu: "organization",
        },
        component: () => import("@/views/console/organization/index.vue"),
      },
      {
        path: "departments",
        name: "departments",
        meta: {
          activeMenu: "organization",
        },
        component: () => import("@/views/console/departments/index.vue"),
      },
      {
        path: "department",
        name: "department",
        meta: {
          activeMenu: "organization",
        },
        component: () => import("@/views/console/departments/department.vue"),
      },
      {
        path: "serviceDataCenter",
        name: "serviceDataCenter",
        meta: {
          activeMenu: "serviceDataCenter",
        },
        component: () =>
          import("@/views/console/service-data-center/index.vue"),
      },
      {
        path: "bandwidth",
        name: "bandwidth",
        meta: {
          activeMenu: "bandwidth",
        },
        component: () => import("@/views/console/bandwidth-center/index.vue"),
      },
      {
        path: "cdn",
        name: "cdn",
        meta: {
          activeMenu: "cdn",
        },
        component: () => import("@/views/console/cdn/index.vue"),
      },
      {
        path: "resourceCDN",
        name: "resourceCDN",
        meta: {
          activeMenu: "resourceCDN",
        },
        component: () => import("@/views/console/resource-cdn/index.vue"),
      },
      {
        path: "topupBandwidth",
        name: "topupBandwidth",
        meta: {
          activeMenu: "bandwidth",
        },
        component: () =>
          import("@/views/console/bandwidth-center/topup-bandwidth.vue"),
      },
      {
        path: "consoleOrder",
        name: "consoleOrder",
        meta: {
          activeMenu: "consoleOrder",
        },
        component: () => import("@/views/console/order/index.vue"),
      },
      {
        path: "checkOrder",
        name: "checkOrder",
        meta: {
          activeMenu: "consoleOrder",
        },
        component: () => import("@/views/console/order/check-order.vue"),
      },
      {
        path: "searchConsole",
        name: "searchConsole",
        meta: {
          activeMenu: "searchConsole",
        },
        component: () => import("@/views/console/search-console/index.vue"),
      },
      {
        path: "member",
        name: "member",
        meta: {
          activeMenu: "member",
        },
        component: () => import("@/views/console/member/index.vue"),
      },
    ],
  },
];

export default systemRoutes;
