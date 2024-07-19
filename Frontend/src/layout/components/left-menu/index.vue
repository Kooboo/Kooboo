<script lang="ts" setup>
import SubMenu from "./sub-menu.vue";
import TopBar from "./top-bar.vue";
import { useRoute } from "vue-router";
import { computed, onMounted, onUnmounted, ref } from "vue";
import routes from "@/router/site";
import type { Menu } from "@/global/types";
import { toMenus, toAllowMenus } from "./route-menu";
import { useSiteStore } from "@/store/site";
import { useI18n } from "vue-i18n";
import { cloneDeep } from "lodash-es";
import { updateAdvancedMenus } from "@/api/site";

const route = useRoute();
const siteStore = useSiteStore();
const { t } = useI18n();
const showAdvancedSetting = ref(false);
const editMenuBox = ref();

const menus = computed(() => {
  const list = toMenus(routes);
  for (const moduleMenu of siteStore.moduleMenus) {
    let parent: Menu[] | undefined = list;
    if (moduleMenu.parent) {
      parent = list.find((f) => f.routeMenuName == moduleMenu.parent)?.items;
    }
    if (parent) {
      parent.push({
        id: moduleMenu.id,
        display: moduleMenu.name,
        name: "module menu",
        routeMenuName: moduleMenu.id,
        items: [],
        icon: moduleMenu.icon,
        queryBuilder() {
          return {
            url: moduleMenu.url,
            SiteId: route.query.SiteId,
          };
        },
        params: {
          module: moduleMenu.id,
        },
      });
    }
  }
  return list;
});

const userMenus = computed(() => {
  if (siteStore.isAdmin) return menus.value;
  if (!siteStore.permissions) return [];
  return toAllowMenus(menus.value);
});

const activeName = computed(() => {
  if (siteStore.firstActiveMenu) return siteStore.firstActiveMenu;
  if (route.meta.activeMenu) {
    return route.meta.activeMenu;
  }
  if (route.name == "module menu") {
    return route.name + route.params.module;
  }
  return route.name as string;
});

const getAdvancedMenusStatus = (menu: Menu) => {
  if (siteStore.site.visibleAdvancedMenus === null) return true;
  if (siteStore.site.visibleAdvancedMenus?.includes(menu.name!)) return true;
  if (siteStore.site.visibleAdvancedMenus?.length === 0) return false;
  if (!menu.advanced) return true;
};

const changeVisible = (name: string) => {
  if (siteStore.site.visibleAdvancedMenus === null) {
    let list = userMenus.value.filter((f) => f.advanced === true);
    let advancedMenuList = [] as Menu[];
    list.forEach((f) => {
      advancedMenuList = advancedMenuList.concat(f.items);
    });

    siteStore.site.visibleAdvancedMenus = advancedMenuList
      .filter((f) => f.name !== name && f.advanced === true)
      .map((m) => m.name!);
  } else {
    if (siteStore.site.visibleAdvancedMenus?.includes(name)) {
      siteStore.site.visibleAdvancedMenus =
        siteStore.site.visibleAdvancedMenus.filter((f) => f !== name);
    } else {
      siteStore.site.visibleAdvancedMenus?.push(name);
    }
  }
  onSave();
};

const onSave = async () => {
  await updateAdvancedMenus(siteStore.site.visibleAdvancedMenus!);
};

const clickMenuOutside = (e: any) => {
  if (!editMenuBox.value.contains(e.target) && showAdvancedSetting.value) {
    showAdvancedSetting.value = false;
  }
};

onMounted(() => {
  window.addEventListener("click", clickMenuOutside);
});
onUnmounted(() => {
  window.removeEventListener("click", clickMenuOutside);
});
</script>

<template>
  <aside class="w-202px h-full relative bg-fff z-10 dark:bg-[#252526]">
    <el-scrollbar class="pb-14">
      <TopBar />
      <el-menu :default-active="activeName">
        <SubMenu v-for="menu of userMenus" :key="menu.id" :item="menu" />
      </el-menu>
    </el-scrollbar>
    <div
      class="absolute bottom-0 left-0 right-0 h-[56px] flex items-center px-5 hover:bg-[#d3eafd] dark:hover:bg-[#18222c] cursor-pointer text-444 dark:text-999"
      :class="showAdvancedSetting ? 'bg-[#d3eafd]  dark:bg-[#18222c]' : ''"
      data-cy="edit-menu"
      @click.stop="showAdvancedSetting = !showAdvancedSetting"
    >
      <el-icon
        class="iconfont icon-a-writein3 text-18px text-999 text-opacity-60 mr-8"
      />
      <span class="text-[#303133] dark:text-999">{{
        t("common.editMenu")
      }}</span>
    </div>
    <div
      ref="editMenuBox"
      class="absolute inset-0 left-210px w-250px h-full bg-fff shadow-s-10"
      :class="showAdvancedSetting ? 'block' : 'hidden'"
    >
      <el-scrollbar class="dark:bg-[#252526]">
        <div class="p-26px pb-16">
          <div class="flex justify-between items-center mb-8 dark:text-999">
            <h1>{{ t("common.editMenu") }}</h1>
            <el-icon
              class="iconfont icon-close cursor-pointer hover:text-blue text-14px"
              :title="t('common.close')"
              @click.stop="showAdvancedSetting = !showAdvancedSetting"
            />
          </div>
          <span class="text-12px text-666 dark:text-999">
            {{ t("common.editMenuTips") }}
          </span>
        </div>

        <el-collapse class="editMenu pt-8">
          <el-collapse-item
            v-for="item in userMenus.filter((f) => f.advanced === true)"
            :key="item.id"
            :title="item.display"
          >
            <div
              v-for="itm in cloneDeep(item.items).sort(
                (a, b) =>
                  Number(a.advanced ?? false) - Number(b.advanced ?? false)
              )"
              :key="itm.id"
              class="py-8 px-44px flex items-center justify-between dark:text-999"
              :data-cy="itm.name"
            >
              <el-checkbox
                :model-value="getAdvancedMenusStatus(itm)"
                :disabled="itm.advanced ? false : true"
                @change="changeVisible(itm.name!)"
                >{{ itm.display }}</el-checkbox
              >
            </div>
          </el-collapse-item>
        </el-collapse>
      </el-scrollbar>
    </div>
  </aside>
</template>
<style>
.editMenu .el-collapse-item__content {
  padding-bottom: 0px;
  font-size: 14px;
  border: none;
}

.editMenu .el-collapse-item .el-collapse-item__header {
  font-size: 14px;
  height: 40px;
}
</style>
<style scoped>
:deep(.el-collapse .el-collapse-item .el-collapse-item__header.is-active) {
  box-shadow: none;
}

:deep(.el-collapse-item__header) {
  padding-left: 24px;
  border: none;
}

:deep(.el-collapse-item__wrap) {
  border: none;
}
</style>
