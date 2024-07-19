<script lang="ts" setup>
import { site, events } from "./settings";
import GroupPanel from "./group-panel.vue";
import { getLighthouseItems } from "@/api/site";
import type { LighthouseItem, Site } from "@/api/site/site";

import { ref, watch } from "vue";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
const lighthouseItems = ref<LighthouseItem[]>([]);

events.onLighthouseSave = (s) => {
  updateLighthouseSettings(s);
};

const updateLighthouseSettings = (s: Site) => {
  const settings = [];

  for (const item of lighthouseItems.value) {
    const setting: Record<string, unknown> = {};

    if (item.setting) {
      for (const i of item.setting) {
        setting[i.name] = i.value;
      }
    }

    settings.push({
      name: item.name,
      enable: item.enable,
      setting: setting,
    });
  }

  s.lighthouseSettingsJson = JSON.stringify(settings);
};

if (site.value) {
  const data = JSON.parse(site.value.lighthouseSettingsJson || "[]");
  getLighthouseItems().then((lighthouse) => {
    for (const item of lighthouse) {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      const dataItem = data.find((f: any) => f.name === item.name);
      if (dataItem) {
        item.enable = dataItem.enable;
        for (const key in dataItem.setting) {
          const setting = item.setting.find((f) => f.name === key);
          if (setting) setting.value = dataItem[key];
        }
      } else {
        item.enable = true;
      }
      lighthouseItems.value.push(item);
    }
  });
}

//为了使site.value.lighthouseSettingsJson能够实时变化
watch(
  () => lighthouseItems.value,
  () => {
    if (site.value && site.value.lighthouseSettingsJson !== null) {
      updateLighthouseSettings(site.value);
    }
  },
  {
    deep: true,
  }
);
</script>

<template>
  <template v-if="site && lighthouseItems.length">
    <GroupPanel
      v-model="site.enableLighthouseOptimization"
      :label="t('common.lighthouseOptimization')"
    >
      <template v-for="item of lighthouseItems" :key="item.name">
        <el-form-item>
          <div class="flex items-center w-full">
            <div class="mr-auto">
              <div class="mr-12 text-666 flex items-center">
                {{ item.name }}
                <Tooltip :tip="item.description" custom-class="ml-4" />
              </div>
            </div>
            <el-switch v-model="item.enable" />
          </div>
          <div v-if="item.enable" class="w-full">
            <div v-for="unit of item.setting" :key="unit.name" class="mt-18px">
              <span class="text-666 font-bold">
                {{ unit.displayName || unit.name }}
              </span>
              <el-input
                v-if="unit.controlType === 'Text'"
                v-model="unit.value"
                class="w-full"
              />
              <el-input
                v-if="unit.controlType === 'Number'"
                v-model.number="unit.value"
                class="w-full"
              />
              <el-switch
                v-if="unit.controlType === 'Switch'"
                v-model="unit.value"
                class="float-right"
              />
            </div>
          </div>
        </el-form-item>
      </template>
    </GroupPanel>
  </template>
</template>
