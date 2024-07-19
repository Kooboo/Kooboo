<script lang="ts" setup>
import { ref, watch, onMounted, nextTick } from "vue";
import type { Tab } from "@/store/dev-mode";
import { useDevModeStore } from "@/store/dev-mode";
import type { ElScrollbar } from "element-plus";
import Sortable from "sortablejs";
import { showConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";
import IconButton from "@/components/basic/icon-button.vue";
import ContextMenu from "@/components/basic/context-menu.vue";

const { t } = useI18n();
const devModeStore = useDevModeStore();
const scrollbar = ref<InstanceType<typeof ElScrollbar>>();
const sortableWrap = ref<HTMLElement>();
const contextMenu = ref();

onMounted(() => {
  if (sortableWrap.value) {
    Sortable.create(sortableWrap.value, {
      forceFallback: true,
      filter: ".icon-close",
      dragClass: "sortable-drag",
      //鼠标移动5px才触发拖拽
      fallbackTolerance: 5,
    });
  }
});

watch(
  () => devModeStore.tabs,
  () => {
    nextTick(() => {
      if (scrollbar.value) {
        scrollbar.value.update();
      }
    });
  },
  { deep: true }
);

watch(
  () => devModeStore.activeTab,
  () => {
    focusTab();
  }
);

function setActiveTab(tab: Tab) {
  // 点击当前tab，需要聚焦到tab (如tab只显示一半的情况时，使其显示完整)
  if (devModeStore.activeTab === tab) {
    focusTab();
    return;
  }
  devModeStore.activeTab = tab;
}

// 鼠标滚轮滚动监听
function handleWheel(e: WheelEvent) {
  const wrap = scrollbar.value?.$refs.wrap$ as HTMLElement;
  if (wrap) {
    wrap.scrollLeft = wrap.scrollLeft + (e.deltaX + e.deltaY);
  }
}

function focusTab() {
  nextTick(() => {
    // 当前激活的tab
    const activeTabDom = sortableWrap.value?.querySelector(
      ".activeTab"
    ) as HTMLElement;
    if (activeTabDom && scrollbar.value) {
      // 滚动条组件真实宽度
      const scrollbarWidth = scrollbar.value.$el.offsetWidth;
      const TabStart = activeTabDom.offsetLeft;
      const TabEnd = activeTabDom.offsetLeft + activeTabDom.offsetWidth;
      // 滚动条容器Dom
      const scrollWrap = scrollbar.value?.$refs.wrap$ as HTMLElement;
      // 当前滚动条滚动距离
      const scrollLeft = scrollWrap.scrollLeft;
      // 是否在视野范围内
      const isVisible =
        TabStart >= scrollLeft && TabEnd <= scrollbarWidth + scrollLeft + 1;
      if (!isVisible) {
        // 滚动到视野范围内, +50可以知道后面有没有其他Tab
        scrollWrap.scrollLeft = TabEnd - scrollbarWidth + 50;
      }
    }
  });
}

const onRemoveTab = async (item: Tab) => {
  if (item.changed) {
    await showConfirm(t("common.unsavedChangesLeaveTips"));
  }

  devModeStore.deleteTab(item.id);
};

const batchClose = async (tabs: Tab[], ids: string[]) => {
  const hasSaved = tabs.some((item) => {
    return item.changed;
  });
  if (hasSaved) {
    await showConfirm(t("common.unsavedChangesLeaveTips"));
  }
  devModeStore.deleteTabs(ids);
};

const actions = [
  {
    name: t("common.close"),
    invoke: async (item: any) => {
      if (item.changed) {
        await showConfirm(t("common.unsavedChangesLeaveTips"));
      }
      devModeStore.deleteTab(item.id);
    },
  },
  {
    name: t("common.closeToRight"),
    invoke: async (item: any) => {
      const currentIndex = devModeStore.tabs.findIndex((i) => {
        return i.id === item.id;
      });
      const rightTabs = devModeStore.tabs.filter(
        (i, index) => index > currentIndex
      );
      const rightIds = rightTabs.map((item) => {
        return item.id;
      });
      batchClose(rightTabs, rightIds);
    },
  },
  {
    name: t("common.closeOthers"),
    invoke: async (item: any) => {
      const ids = devModeStore.tabs.map((item) => {
        return item.id;
      });
      const otherTabs = devModeStore.tabs.filter((i) => i.id != item.id);
      const otherIds = ids.filter((i) => i != item.id);
      batchClose(otherTabs, otherIds);
    },
  },
  {
    name: t("common.closeSaved"),
    invoke: async () => {
      const tabSaved = devModeStore.tabs.filter((item) => {
        return !item.changed;
      });
      const savedId = tabSaved.map((item) => {
        return item.id;
      });
      devModeStore.deleteTabs(savedId);
    },
  },
  {
    name: t("common.closeAll"),
    invoke: async () => {
      const ids = devModeStore.tabs.map((item) => {
        return item.id;
      });
      batchClose(devModeStore.tabs, ids);
    },
  },
];
</script>

<template>
  <div class="h-26px bg-gray/60 dark:bg-[#252526] text-s flex">
    <div
      class="whitespace-nowrap flex-1 overflow-hidden"
      @wheel.prevent="handleWheel"
    >
      <el-scrollbar ref="scrollbar">
        <div ref="sortableWrap" class="h-26px">
          <div
            v-for="item of devModeStore.tabs"
            :key="item.id"
            :title="item.name"
            class="group inline-flex h-full cursor-default items-center border-r border-solid border-line dark:border-transparent px-8 space-x-4 min-w-64 overflow-hidden max-w-280px relative"
            :class="
              devModeStore.activeTab?.id === item.id
                ? 'bg-fff activeTab dark:(bg-[#1e1e1e] text-fff/86)'
                : 'dark:(bg-[#2d2d2d] text-fff/60) hover:bg-black/10'
            "
            data-cy="opened-file"
            @click="setActiveTab(item)"
            @contextmenu.prevent.stop="contextMenu?.openMenu($event, item)"
          >
            <div
              v-show="item.changed"
              class="w-4 h-4 rounded-full bg-444 dark:bg-[#fffa] absolute left-4"
            />
            <el-icon class="iconfont" :class="item.icon || 'icon-code'" />
            <span class="select-none ellipsis" data-cy="file-name">{{
              item.name
            }}</span>
            <el-icon
              class="iconfont icon-close cursor-pointer hover:text-blue group-hover:opacity-100"
              :class="
                devModeStore.activeTab?.id === item.id
                  ? 'opacity-100'
                  : 'opacity-0'
              "
              data-cy="close"
              @click.stop="onRemoveTab(item)"
            />
          </div>
        </div>
      </el-scrollbar>
    </div>
    <div
      v-if="devModeStore.activeTab?.actions"
      class="flex items-center p-8 -space-x-4"
    >
      <template v-for="item of devModeStore.activeTab.actions" :key="item.name">
        <IconButton
          v-if="item.visible"
          :tip="item.display"
          :icon="item.icon"
          class="!p-0 !text-m dark:text-fff/60"
          @click="item.invoke()"
        />
      </template>
    </div>
    <ContextMenu ref="contextMenu" :actions="actions" />
  </div>
</template>

<style lang="scss" scoped>
:deep(.el-scrollbar__bar.is-horizontal) {
  height: 3px;
  bottom: 0;
}

.sortable-drag {
  @apply border border-solid border-blue;
}
</style>
