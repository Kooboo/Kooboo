import type { RouteRecordRaw } from "vue-router";

const accountRoutes: Array<RouteRecordRaw> = [
  {
    path: "/login",
    name: "login",
    meta: {
      anonymous: true,
    },
    component: () => import("@/views/account/password-login.vue"),
  },
  {
    path: "/login/phone",
    name: "phone-login",
    meta: {
      anonymous: true,
    },
    component: () => import("@/views/account/phone-login.vue"),
  },
  {
    path: "/bind/name",
    name: "bind-name",
    meta: {
      anonymous: true,
    },
    component: () => import("@/views/account/bind-name.vue"),
  },
  {
    path: "/bind/account",
    name: "bind-account",
    meta: {
      anonymous: true,
    },
    component: () => import("@/views/account/bind-account.vue"),
  },
  {
    path: "/register",
    name: "register",
    meta: {
      anonymous: true,
    },
    component: () => import("@/views/account/register.vue"),
  },
  {
    path: "/retrieve-password",
    name: "retrieve-password",
    meta: {
      anonymous: true,
    },
    component: () => import("@/views/account/retrieve-password.vue"),
  },
  {
    path: "/profile",
    component: () => import("@/layout/blank.vue"),
    children: [
      {
        path: "",
        name: "profile",
        component: () => import("@/views/account/profile.vue"),
      },
    ],
  },
];

export default accountRoutes;
