import { defineStore } from "pinia";
import { ref, watch } from "vue";
import type { Binding } from "@/api/binding/types";
import { getListBySite } from "@/api/binding";
import { useSiteStore } from "./site";

export const useBindingStore = defineStore("bindingStore", () => {
  const bindings = ref<Binding[]>([]);
  const siteStore = useSiteStore();

  const loadBindings = async () => {
    bindings.value = await getListBySite();
  };

  watch(
    () => siteStore.site?.id,
    () => {
      bindings.value = [];
    }
  );

  return {
    bindings,
    loadBindings,
  };
});
