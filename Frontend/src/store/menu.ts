import { defineStore } from "pinia";
import { computed, ref, watch } from "vue";
import { getList, deletes, getEdit, create } from "@/api/menu";
import type { MenuItem } from "@/api/menu/types";
import { useSiteStore } from "./site";
import { useDevModeStore } from "./dev-mode";

export const useMenuStore = defineStore("menuStore", () => {
  const siteStore = useSiteStore();
  const devModeStore = useDevModeStore();
  const list = ref<MenuItem[]>([]);

  const getMenuList = computed(() => {
    return devModeStore.getSplitNameData(list.value) as any[];
  });

  const load = async () => {
    list.value = await getList();
  };

  const deleteMenus = async (ids: string[]) => {
    await deletes(ids);
    devModeStore.deleteTabs(ids);
    load();
  };

  const createMenu = async (name: string) => {
    const result = await create(name);
    load();
    return result;
  };

  const getMenu = async (id: string) => {
    return await getEdit(id);
  };

  watch(
    () => siteStore.site?.id,
    () => {
      list.value = [];
    }
  );

  return { load, list, deleteMenus, getMenu, createMenu, getMenuList };
});
