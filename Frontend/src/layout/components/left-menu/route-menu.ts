import type { Menu } from "@/global/types";
import type { RouteRecordRaw } from "vue-router";
import { cloneDeep } from "lodash-es";
import { newGuid } from "@/utils/guid";
import { useSiteStore } from "@/store/site";

const siteStore = useSiteStore();

export function toMenus(routes: RouteRecordRaw[]) {
  const menus: Menu[] = [];
  for (const route of routes) {
    if (route.meta?.menu) {
      const menu: Menu = {
        name: route.name?.toString(),
        items: [],
        display: route.meta.menu.display,
        id: newGuid(),
        permission: route.meta?.menu?.permission,
        advanced: route.meta?.advanced ? route.meta?.advanced : undefined,
        icon: route.meta?.menu?.icon,
        queryBuilder: route.meta?.menu?.queryBuilder,
        routeMenuName: route.meta?.menu.name,
      };

      if (route.children) {
        menu.items.push(...toMenus(route.children));
      }

      menus.push(menu);
    } else if (route.children) {
      menus.push(...toMenus(route.children));
    }
  }

  return menus;
}

export function toAllowMenus(menus: Menu[]) {
  const list: Menu[] = [];
  for (const menu of menus) {
    const item = cloneDeep(menu);

    if (
      !item.permission ||
      siteStore.hasAccess(item.permission.feature, item.permission.action)
    ) {
      list.push(item);
      item.items = toAllowMenus(item.items);
    }
  }
  return list;
}
