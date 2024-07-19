<script lang="ts">
import { i18n } from "@/modules/i18n";
import { router } from "@/modules/router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { useRoute } from "vue-router";
import type { ScriptModule } from "@/api/module/types";
import { ref, watch } from "vue";
import { useSiteStore } from "@/store/site";

const $t = i18n.global.t;

export const define = {
  name: "modules",
  icon: "icon-classification",
  display: $t("common.modules"),
  order: 100,
  permission: "module",
  actions: [
    {
      name: "back",
      display: $t("common.back"),
      icon: "icon-go-back",
      visible: false,
      invoke() {
        router.push(
          useRouteSiteId({
            name: "dev-mode",
            query: {
              activity: "modules",
            },
          })
        );

        useModuleStore().exitEditModule();
      },
    },
    {
      name: "refresh",
      display: $t("common.refresh"),
      icon: "icon-Refresh",
      visible: true,
      async invoke() {
        const activity = useDevModeStore().activeActivity;
        if (!activity) return;
        const instance = await activity.panelInstance.promise;
        instance.load();
      },
    },
    {
      name: "more",
      display: $t("common.more"),
      icon: "icon-more",
      visible: true,
      invoke() {
        router.push(useRouteSiteId({ name: "modules" }));
      },
    },
  ],
} as Activity;
</script>

<script lang="ts" setup>
import type { Activity } from "@/store/dev-mode";
import { useDevModeStore } from "@/store/dev-mode";
import { useModuleStore } from "@/store/module";
import ListPanel from "../components/module/list-panel.vue";
import EditPanel from "../components/module/edit-panel.vue";
import { getList } from "@/api/module";

const moduleStore = useModuleStore();
const devModeStore = useDevModeStore();
const siteStore = useSiteStore();
const route = useRoute();
const list = ref<ScriptModule[]>([]);

const load = () => {
  moduleStore.load();
  changeModule();
};
const changeModule = async () => {
  if (!siteStore.hasAccess("module", "view")) return;
  list.value = await getList();
  if (route.query.moduleId && route.query.activity === "modules") {
    moduleStore.editModule(route.query.moduleId as string);
    if (devModeStore.activeActivity) {
      devModeStore.activeActivity.panelDisplay = list.value.find(
        (f) => f.id === route.query.moduleId
      )?.name;
    }
  }
};

watch(
  () => route.query.moduleId,
  () => {
    if (route.query.moduleId) {
      changeModule();
    }
  },
  { immediate: true }
);

defineExpose({ load });
</script>

<template>
  <div>
    <EditPanel v-if="moduleStore.editingModule && moduleStore.types.length" />
    <ListPanel v-if="!moduleStore.editingModule" />
  </div>
</template>
