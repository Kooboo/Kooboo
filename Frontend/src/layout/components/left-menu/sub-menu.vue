<script lang="ts" setup>
import type { Menu } from "@/global/types";

import { useRoute } from "vue-router";
import { useSiteStore } from "@/store/site";
import { computed } from "vue";

const route = useRoute();
const siteStore = useSiteStore();

defineProps<{ item: Menu }>();
const visibleAdvancedMenus = computed(() => (meta: Menu) => {
  if (meta.advanced === undefined) return true;
  let list = JSON.parse(JSON.stringify(siteStore.site.visibleAdvancedMenus));
  if (list === null) {
    return true;
  } else {
    if (list.includes(meta.name)) {
      return true;
    } else {
      return false;
    }
  }
});

const isShowSubMenu = computed(() => (item: Menu) => {
  if (!siteStore.site.visibleAdvancedMenus) {
    return true;
  }

  if (item.items.some((s) => !s.advanced)) {
    return true;
  }

  return item.items.some((s) => {
    return siteStore.site.visibleAdvancedMenus!.includes(s.name!);
  });
});

function getDefaultQuery(item: Menu) {
  if (typeof item.queryBuilder === "function") {
    return item.queryBuilder(item, route);
  }

  return {
    SiteId: route.query.SiteId,
  };
}
</script>

<template>
  <template v-if="item.items.length && isShowSubMenu(item)">
    <el-sub-menu :index="item.id">
      <template #title>
        <el-icon class="iconfont" :class="item.icon" />
        <span>{{ item.display }} </span>
      </template>
      <template v-for="i of item.items" :key="i.id">
        <SubMenu :item="i" />
      </template>
    </el-sub-menu>
  </template>
  <template v-else-if="item.name">
    <router-link
      :to="{
        name: item.name.toLowerCase(),
        query: getDefaultQuery(item),
        params: item.params,
      }"
    >
      <el-menu-item
        v-if="visibleAdvancedMenus(item)"
        :index="
          item.routeMenuName
            ? item.name.toLowerCase() + item.routeMenuName
            : item.name.toLowerCase()
        "
      >
        <img
          v-if="item.routeMenuName && item.icon"
          class="w-18px h-18px mr-8"
          :src="item.icon as any"
        />
        <el-icon
          v-else
          class="iconfont"
          :class="[
            item.icon,
            {
              '!w-18px': !item.icon,
            },
          ]"
        />
        {{ item.display }}
      </el-menu-item>
    </router-link>
  </template>
</template>
