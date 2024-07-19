<template>
  <el-scrollbar>
    <div class="pb-150px">
      <VePropsEdit
        v-if="activeWidget"
        ref="propRef"
        :key="activeWidget.id ?? 'empty'"
        :model="activeWidget"
        :group="activeGroup"
        :context="activeContext"
        :render-context="renderContext"
      />
      <template v-else>
        <div class="flex items-center px-24 py-12 leading-none">
          <el-radio-group v-model="currentTab" class="el-radio-group--rounded">
            <el-radio-button
              v-for="item of allTabs"
              :key="item.key"
              :label="item.key"
              :data-cy="item.key"
            >
              {{ item.value }}
            </el-radio-button>
          </el-radio-group>
          <p class="flex-1 text-m" />
          <MultilingualSelector />
        </div>
        <div :key="currentTab">
          <VeColumns v-if="currentTab === 'columns'" />
          <VeWidgets
            v-if="currentTab === 'widgets' && builtinWidgets"
            :items="builtinWidgets ?? []"
          />
          <VeWidgets
            v-if="currentTab === 'custom' && customWidgets"
            :items="customWidgets ?? []"
          />
          <template v-for="item of customTabs" :key="item.key">
            <slot v-if="currentTab === item.key" :name="item.key" />
          </template>
        </div>
      </template>
    </div>
  </el-scrollbar>
</template>

<script lang="ts" setup>
import MultilingualSelector from "@/components/multilingual-selector/index.vue";

import { ref, computed, onBeforeUnmount, onMounted } from "vue";

import VeWidgets from "../ve-widgets/index.vue";
import VeColumns from "../ve-columns/index.vue";
import VePropsEdit from "../ve-props-edit/index.vue";

import type { KeyValue } from "@/global/types";
import { useI18n } from "vue-i18n";
import { useGlobalStore } from "../../global-store";
import { useBuiltinWidgets } from "../ve-widgets";
import { useSidebarEffects, handleIframeMessages } from "./effects";
import type { VeRenderContext } from "../../types";
import { ignoreCaseContains } from "@/utils/string";
const {
  activeWidget,
  activeGroup,
  activeContext,
  selectWidget,
  customWidgets,
} = useGlobalStore();

const propRef = ref();
const { currentTab } = useSidebarEffects();
const { t } = useI18n();
const props = defineProps<{
  customTabs: KeyValue[];
  renderContext: VeRenderContext;
}>();
const { widgets } = useBuiltinWidgets();
const builtinWidgets = computed(() => {
  if (!props.renderContext?.classic) {
    return widgets.value;
  }

  return widgets.value?.filter((w) => !ignoreCaseContains(["youtube"], w.name));
});

const allTabs = computed<KeyValue[]>(() => {
  const tabs: KeyValue[] = [
    {
      key: "columns",
      value: t("ve.columns"),
    },
    {
      key: "widgets",
      value: t("ve.widgets"),
    },
  ];
  if (customWidgets.value?.length) {
    tabs.push({
      key: "custom",
      value: t("common.custom"),
    });
  }
  if (props.customTabs) {
    tabs.push(...props.customTabs);
  }

  return tabs;
});

onMounted(() => {
  handleIframeMessages();
});

onBeforeUnmount(() => {
  selectWidget(null);
});

async function validate() {
  await propRef.value?.validate();
}

defineExpose({
  validate,
});
</script>
