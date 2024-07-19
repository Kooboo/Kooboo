<script lang="ts">
import { i18n } from "@/modules/i18n";
import { copyText } from "@/hooks/use-copy-text";

const $t = i18n.global.t;

export const define = {
  name: "debug",
  icon: "icon-debugging",
  display: $t("common.debug"),
  order: 90,
  // permission: useSiteStore().hasAccess("code", "debug"),
  permission: "debug",
  actions: [
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
  ] as Action[],
} as Activity;
</script>

<script lang="ts" setup>
import Collapse from "../components/collapse.vue";
import { useCodeStore } from "@/store/code";
import FileItem from "../components/file-item.vue";
import type { Action, Activity } from "@/store/dev-mode";
import { useDevModeStore } from "@/store/dev-mode";
import type { Code } from "@/api/code/types";
import DebugPanel from "../tab-panels/debug-panel.vue";
import { DEBUG_TAB_PREFIX } from "@/constants/constants";
import { usePreviewUrl } from "@/hooks/use-preview-url";
import { useI18n } from "vue-i18n";
import { computed, ref, watch } from "vue";
import ContextMenu from "@/components/basic/context-menu.vue";
import ReSubMenu from "@/views/dev-mode/components/collapse/re-sub-menu.vue";

const { t } = useI18n();
const codeStore = useCodeStore();
const devModeStore = useDevModeStore();
const debugTab = ref();
const contextMenu = ref();

const codesByType = (type: string) =>
  codeStore.codes.filter(
    (f) => f.codeType?.toLowerCase() === type?.toLowerCase()
  );

const open = (item: Code) => {
  devModeStore.addTab({
    id: DEBUG_TAB_PREFIX + item.id,
    name: item.name,
    panel: DebugPanel,
    icon: "icon-debugging",
    actions: [
      {
        name: "refresh",
        display: t("common.refresh"),
        visible: true,
        icon: "icon-Refresh",
        invoke: async () => {
          const tab = await useDevModeStore().activeTab?.panelInstance?.promise;
          if (!tab) return;
          tab.reload();
        },
      },
      {
        name: "open",
        visible: false,
        display: t("common.preview"),
        icon: "icon-eyes",
        invoke: async () => {
          const tab = await useDevModeStore().activeTab?.panelInstance?.promise;
          if (!tab) return;
          const found = codeStore.codes.find((f) => f.id === tab.currentCode);
          if (found) {
            const { onPreview } = usePreviewUrl();
            onPreview!(found.previewUrl);
          }
        },
      },
    ],
  });
};

const load = () => {
  codeStore.loadCodes();
};

watch(
  () => devModeStore.debugTab,
  async (value) => {
    debugTab.value = await value?.panelInstance.promise;
  }
);

const actions = computed(() => {
  var list = [
    {
      name: t("common.copyTitle"),
      invoke: async (item: any) => {
        copyText(item.name);
      },
    },
  ];

  if (contextMenu.value?.payload?.codeType == "Api") {
    list.push({
      name: t("common.preview"),
      invoke: async (item: any) => {
        const { onPreview } = usePreviewUrl();
        onPreview!(item.previewUrl);
      },
    });
  }

  return list;
});

defineExpose({ load });
</script>

<template>
  <div>
    <template v-for="(value, key) of codeStore.types" :key="key">
      <Collapse
        v-if="codesByType(key).length"
        :title="value"
        :expand-default="true"
        permission="debug"
      >
        <template v-if="key === 'pageScript'">
          <FileItem
            v-for="item in codeStore.codes.filter(
              (f) => f.codeType === 'PageScript'
            )"
            :id="item.id"
            :key="item.id"
            :class="
              debugTab?.currentCode === item.id
                ? 'bg-blue/20 dark:text-fff/86'
                : ''
            "
            :title="item.name"
            :name="item.name"
            permission="debug"
            @click="open(item)"
            @contextmenu.prevent.stop="contextMenu?.openMenu($event, item)"
          />
        </template>

        <template v-for="item in codeStore.codesByType[key]" v-else>
          <FileItem
            v-if="!item.children"
            :id="item.id"
            :key="item.id"
            :class="
              debugTab?.currentCode === item.id
                ? 'bg-blue/20 dark:text-fff/86'
                : ''
            "
            :title="item.name"
            :name="item.name"
            permission="debug"
            :padding="12 + item.floor * 6"
            @contextmenu.prevent.stop="contextMenu?.openMenu($event, item)"
            @click="open(item)"
          />
          <ReSubMenu
            v-else
            :key="item.folderName + item.floor"
            :data="item"
            permission="debug"
            hidden-remove="true"
            @click-handle="open($event)"
            @click-context-menu="
              contextMenu?.openMenu($event.event, $event.item)
            "
          />
        </template>
      </Collapse>
    </template>
    <ContextMenu ref="contextMenu" :actions="actions" />
  </div>
</template>
