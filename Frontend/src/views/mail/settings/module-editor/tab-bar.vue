<script lang="ts" setup>
import { ref, watch, onMounted, nextTick } from "vue";
import type { Tab } from "@/store/dev-mode";
import type { ElScrollbar } from "element-plus";
import Sortable from "sortablejs";
import { showConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";
import IconButton from "@/components/basic/icon-button.vue";
import TabContext from "./tab-context.vue";
import { useMailModuleEditorStore } from "@/store/mail-module-editor";

const { t } = useI18n();
const mailModuleEditorStore = useMailModuleEditorStore();
const scrollbar = ref<InstanceType<typeof ElScrollbar>>();
const sortableWrap = ref<HTMLElement>();
const tabContext = ref();

onMounted(() => {
  if (sortableWrap.value) {
    Sortable.create(sortableWrap.value, {
      forceFallback: true,
      filter: ".icon-close",
      dragClass: "sortable-drag",
      delay: 100,
      onChoose: function () {
        focusTab();
      },
      onEnd: function () {
        focusTab();
      },
    });
  }
});

watch(
  () => mailModuleEditorStore.tabs,
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
  () => mailModuleEditorStore.activeTab,
  () => {
    focusTab();
  }
);

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

  mailModuleEditorStore.deleteTab(item.id);
};
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
            v-for="item of mailModuleEditorStore.tabs"
            :key="item.id"
            :title="item.name"
            class="group inline-flex h-full cursor-default items-center border-r border-solid border-line dark:border-transparent px-8 space-x-4 min-w-64 overflow-hidden max-w-280px relative"
            :class="
              mailModuleEditorStore.activeTab?.id === item.id
                ? 'bg-fff activeTab dark:(bg-[#1e1e1e] text-fff/86)'
                : 'dark:(bg-[#2d2d2d] text-fff/60) hover:bg-black/10'
            "
            data-cy="opened-file"
            @click="mailModuleEditorStore.activeTab = item"
            @contextmenu.prevent.stop="tabContext?.openMenu($event, item)"
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
                mailModuleEditorStore.activeTab?.id === item.id
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
      v-if="mailModuleEditorStore.activeTab?.actions"
      class="flex items-center p-8 -space-x-4"
    >
      <template
        v-for="item of mailModuleEditorStore.activeTab.actions"
        :key="item.name"
      >
        <IconButton
          v-if="item.visible"
          :tip="item.display"
          :icon="item.icon"
          class="!p-0 !text-m dark:text-fff/60"
          @click="item.invoke()"
        />
      </template>
    </div>
    <TabContext ref="tabContext" />
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
