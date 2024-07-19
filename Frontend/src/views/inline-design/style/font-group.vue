<script lang="ts" setup>
import { useStyle } from "@/utils/dom";
import { useI18n } from "vue-i18n";
import { computed, inject } from "vue";
import { fontFamilies } from "@/global/style";
import type { DomValueWrapper } from "@/global/types";
import { element } from ".";

const { t } = useI18n();
const color = useStyle(element.value!, "color");
const lineHeight = useStyle(element.value!, "line-height");
const size = useStyle(element.value!, "font-size");
const weight = useStyle(element.value!, "font-weight");
const family = useStyle(element.value!, "font-family");
const style = getComputedStyle(element.value!);

const familyArray = computed({
  get() {
    return family.value
      ?.split(",")
      ?.map((m: string) => m?.trim())
      .filter((f: string) => !!f);
  },
  set(value: string[]) {
    family.value = value.join(",");
  },
});

const values = inject<DomValueWrapper[]>("values");
values?.push(size, weight, family, lineHeight, color);
</script>

<template>
  <div class="space-y-12 w-full">
    <div class="flex w-full space-x-12">
      <div class="flex-1">
        <ColorInput
          v-model="color"
          :show-alpha="true"
          :display-color="style.color"
        />
      </div>
      <div class="flex-1">
        <el-input v-model="weight" :placeholder="style.fontWeight">
          <template #append
            ><span class="px-12">{{ t("common.fontWeight") }}</span></template
          >
        </el-input>
      </div>
    </div>
    <div class="flex w-full space-x-12">
      <div class="flex-1">
        <el-input v-model="size" :placeholder="style.fontSize">
          <template #append
            ><span class="px-12">{{ t("common.size") }}</span></template
          >
        </el-input>
      </div>

      <div class="flex-1">
        <el-input v-model="lineHeight" :placeholder="style.lineHeight">
          <template #append
            ><span class="px-12">{{ t("common.lineHeight") }}</span></template
          >
        </el-input>
      </div>
    </div>
    <div>
      <el-select
        v-model="familyArray"
        multiple
        filterable
        allow-create
        default-first-option
        class="w-full"
        :placeholder="style.fontFamily"
        title="font-family"
      >
        <el-option
          v-for="item of fontFamilies"
          :key="item"
          :label="item"
          :value="item"
        />
      </el-select>
    </div>
  </div>
</template>
