<script lang="ts" setup>
import type { KeyValue } from "@/global/types";
import { toObject } from "@/utils/lang";
import { ref, watch } from "vue";
import { site, events } from "./settings";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
const customSettings = ref<KeyValue[]>([]);

events.onCustomSettingsSave = (s) => {
  s.customSettings = toObject(customSettings.value) as Record<string, string>;
};

if (site.value) {
  for (const key in site.value.customSettings) {
    customSettings.value.push({
      key: key,
      value: site.value.customSettings[key],
    });
  }
}

const onAdd = () => {
  customSettings.value.push({
    key: "",
    value: "",
  });
};

const onDelete = (key: number) => {
  customSettings.value.splice(key, 1);
};

//为了监听site.value.customSettings实时变化
watch(
  () => customSettings.value,
  () => {
    if (site.value) {
      site.value.customSettings = toObject(customSettings.value) as Record<
        string,
        string
      >;
    }
  },
  { deep: true }
);
</script>

<template>
  <div
    v-if="site"
    class="rounded-normal bg-fff dark:bg-[#252526] mt-16 py-24 px-56px"
  >
    <div class="max-w-504px">
      <el-form-item>
        <template #label>
          <div class="flex items-center">
            {{ t("common.customSettings") }}
            <Tooltip
              :tip="t('common.customSettingsTips')"
              custom-class="ml-4"
            />
          </div>
        </template>
        <KeyValueEditor v-model="customSettings" />
      </el-form-item>
    </div>
  </div>
</template>
