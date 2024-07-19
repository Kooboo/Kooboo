<script lang="ts" setup>
import { useMultilingualStore } from "@/store/multilingual";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
const multilingualStore = useMultilingualStore();
</script>

<template>
  <el-dropdown
    v-if="multilingualStore.visible"
    :hide-on-click="false"
    trigger="click"
    data-cy="multilingual-dropdown"
  >
    <div class="inline-flex items-center cursor-pointer">
      <el-icon class="iconfont icon-language text-20px text-blue mx-16" />
      <div>
        <div>
          <span>{{ t("common.multilingual") }}</span>
          <el-icon class="iconfont icon-pull-down text-m ml-8" />
        </div>
        <p
          class="text-s text-999 text-s w-90px ellipsis"
          :title="multilingualStore.selected.join(',')"
          data-cy="lang-abbr"
        >
          {{ multilingualStore.selected.join(",") }}
        </p>
      </div>
    </div>
    <template #dropdown>
      <el-dropdown-menu>
        <el-dropdown-item
          v-for="(value, key) in multilingualStore.cultures"
          :key="key"
          data-cy="lang-item"
          @click="multilingualStore.selectedChanged(key)"
        >
          <div class="flex items-center">
            <el-checkbox
              class="pointer-events-none"
              :model-value="!!multilingualStore.selected.find((f) => f === key)"
              :disabled="key === multilingualStore.default"
              data-cy="lang-checkbox"
            />
            <span class="ml-8">{{ value }} - {{ key }}</span>
          </div>
        </el-dropdown-item>
      </el-dropdown-menu>
    </template>
  </el-dropdown>
</template>
