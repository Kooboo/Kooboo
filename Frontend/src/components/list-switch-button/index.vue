<template>
  <el-radio-group
    :model-value="current"
    class="el-radio-group--rounded"
    @update:model-value="handleSwitchListType as any"
  >
    <el-tooltip
      v-for="item in listTypes"
      :key="item.key"
      class="box-item"
      effect="dark"
      :content="
        item.key == 'grid' ? t('common.gridView') : t('common.listView')
      "
      placement="top"
    >
      <el-radio-button
        :key="item.key"
        class="w-42px h-26px rounded-full flex items-center justify-center cursor-pointer m-4"
        :label="item.key"
        :class="{
          'bg-[#fff] dark:bg-transparent text-blue shadow-s-10':
            current === item.key,
        }"
        :data-cy="item.key"
        @click="handleSwitchListType(item.key)"
      >
        <span
          class="iconfont text-size-12px"
          :class="current === item.key ? item.activeIcon : item.icon"
        />
      </el-radio-button>
    </el-tooltip>
  </el-radio-group>
</template>
<script lang="ts" setup>
import { useSync } from "@/hooks/use-sync";
import type { ListType } from "./types";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
const { t } = useI18n();

const props = defineProps<{
  current: ListType;
}>();
const emits = defineEmits<{
  (e: "update:current"): void;
  (e: "change", value: ListType): void;
}>();
const _current = useSync(props, "current", emits);

const listTypes: ReadonlyArray<{
  key: ListType;
  icon: string;
  activeIcon: string;
}> = [
  {
    key: "grid",
    icon: "icon-classification",
    activeIcon: "icon-classification2",
  },
  {
    key: "list",
    icon: "icon-list",
    activeIcon: "icon-list2",
  },
];

function handleSwitchListType(listType: ListType) {
  _current.value = listType;
  emits("change", listType);
}
</script>

<style lang="scss" scoped>
.el-radio-group {
  height: 40px;
}
</style>
