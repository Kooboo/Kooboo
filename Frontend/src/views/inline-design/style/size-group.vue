<script lang="ts" setup>
import { useStyle } from "@/utils/dom";
import { useI18n } from "vue-i18n";
import { inject } from "vue";
import type { DomValueWrapper } from "@/global/types";
import { element } from ".";
const { t } = useI18n();

const width = useStyle(element.value!, "width");
const height = useStyle(element.value!, "height");
const style = getComputedStyle(element.value!);

const values = inject<DomValueWrapper[]>("values");
values?.push(width, height);
</script>

<template>
  <div class="flex w-full space-x-12">
    <div class="flex-1">
      <el-input v-model="width" :placeholder="style.width">
        <template #append
          ><span class="px-12">{{ t("common.width") }}</span></template
        >
      </el-input>
    </div>

    <div class="flex-1">
      <el-input v-model="height" :placeholder="style.height">
        <template #append
          ><span class="px-12">{{ t("common.height") }}</span></template
        >
      </el-input>
    </div>
  </div>
</template>
