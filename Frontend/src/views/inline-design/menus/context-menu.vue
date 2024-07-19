<script lang="ts" setup>
import { currentElement, clickPosition } from "@/views/inline-design/page";
import { editing } from "@/views/inline-design/inline-editor";
import { features } from "../features";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import ScaleIcon from "@/assets/images/inline-design/icon_scale.svg";
import ExtendIcon from "@/assets/images/inline-design/icon_extend.svg";
import SvgBtn from "@/components/basic/svg-btn.vue";
import { historyCount } from "../state";
import { getElementParents } from "@/utils/dom";
import { skipTags } from "../binding";
import { ElMessage, ElMessageBox } from "element-plus";
import { getWidgetMenus } from "../features/edit-widget";

export interface MenuItem {
  name: string;
  display: string;
  icon: string;
  order: number;
  immediate: boolean;
  invoke: (el?: HTMLElement) => void;
  confirm?: string;
}

const { t } = useI18n();
const visible = ref(false);
const triggerRef = ref();
const trigger = ref();
const menuItems = ref<MenuItem[]>([]);
const elPaths = ref<HTMLElement[]>([]);

const historyItems = computed(() =>
  menuItems.value.filter((f) => !f.immediate)
);

const immediateItems = computed(() =>
  menuItems.value.filter((f) => f.immediate)
);

const onAction = async (item: MenuItem, cmsEdit: boolean) => {
  if (historyCount.value && cmsEdit) {
    ElMessage.warning(t("common.existOperations"));
    return;
  }
  if (item.confirm) {
    const confirmed = await ElMessageBox.confirm(item.confirm, {
      confirmButtonText: t("common.ok"),
      cancelButtonText: t("common.cancel"),
      type: "warning",
    }).catch(() => "cancel");
    if (confirmed !== "confirm") {
      return;
    }
  }
  item.invoke(currentElement.value);
  onClose();
};

function getMenuItems() {
  const result: MenuItem[] = [];
  const el = currentElement.value;
  if (!el) return result;

  const widgetMenus = getWidgetMenus(el);
  if (widgetMenus) {
    return widgetMenus;
  }

  for (const feature of features) {
    if (feature?.active(el)) {
      result.push({
        name: feature.name,
        display: feature.display,
        icon: feature.icon,
        invoke: feature.invoke,
        immediate: feature.immediate,
      } as MenuItem);
    }
  }

  return result;
}

const onClose = () => {
  clickPosition.value = undefined;
  visible.value = false;
};
const getExpandEl = () => {
  const index = elPaths.value.indexOf(currentElement.value!);
  return elPaths.value[index + 1];
};

const canExpand = computed(() => {
  let el = getExpandEl();
  if (!el || skipTags.includes(el.tagName?.toLowerCase())) return false;
  return true;
});

const onExpand = () => {
  if (!canExpand.value) return;
  let el = getExpandEl();
  currentElement.value = el;
  menuItems.value = getMenuItems();
};

const getScaleEl = () => {
  const index = elPaths.value.indexOf(currentElement.value!);
  if (index == 0) return;
  return elPaths.value[index - 1];
};

const canScale = computed(() => {
  let el = getScaleEl();
  return !!el;
});

const onScale = () => {
  let el = getScaleEl();
  currentElement.value = el;
  menuItems.value = getMenuItems();
};

watch([clickPosition, editing], () => {
  visible.value = false;
  elPaths.value = [];
  const el = currentElement.value;
  if (!el) return;
  if (!clickPosition.value) return;
  if (editing.value) return;
  menuItems.value = getMenuItems();
  elPaths.value = getElementParents(currentElement.value!, true);
  visible.value = true;
  trigger.value = "click";

  setTimeout(() => {
    triggerRef.value?.click();
    trigger.value = "hover";
  }, 0);
});
</script>

<template>
  <el-popover
    v-if="visible"
    :trigger="trigger"
    placement="auto"
    popper-class="!p-0 overflow-hidden"
    @after-leave="onClose"
  >
    <template #reference>
      <div
        ref="triggerRef"
        class="absolute w-4 h-4"
        :style="{
          top: clickPosition!.y + 'px',
          left: clickPosition!.x + 'px',
        }"
      />
    </template>
    <div>
      <div
        class="flex items-center cursor-default px-12 py-8 border-b border-solid border-line dark:border-[#555] dark:bg-999"
      >
        <div class="ellipsis text-l relative">
          <span>
            {{ t("common.menu") }}
          </span>
        </div>
        <div class="flex-1" />
        <div class="flex items-center space-x-4">
          <SvgBtn
            :src="ScaleIcon"
            :title="t('common.narrowSelection')"
            :disabled="!canScale"
            @click="onScale"
          />
          <SvgBtn
            :title="t('common.expandSelection')"
            :src="ExtendIcon"
            :disabled="!canExpand"
            @click="onExpand"
          />
        </div>
      </div>

      <div
        v-for="item of historyItems"
        :key="item.name"
        class="hover:bg-blue/10 px-12 py-8 cursor-pointer flex items-center space-x-8"
        @click="onAction(item, false)"
      >
        <el-icon class="iconfont" :class="item.icon" />
        <span>{{ item.display }}</span>
      </div>
      <p
        v-if="immediateItems.length"
        class="text-s w-full p-4 pl-12 bg-gray/50 flex items-center"
      >
        <span class="transform scale-90 origin-left">{{
          t("common.cmsEdit")
        }}</span>
      </p>
      <div
        v-for="item of immediateItems"
        :key="item.name"
        class="px-12 py-8 cursor-pointer flex items-center space-x-8"
        @click="onAction(item, true)"
      >
        <el-icon class="iconfont" :class="item.icon" />
        <span>{{ item.display }}</span>
      </div>
    </div>
  </el-popover>
</template>
