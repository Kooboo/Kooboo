<script lang="ts" setup>
import { site, events } from "./settings";
import GroupPanel from "./group-panel.vue";
import { computed, ref, watch } from "vue";
import type { KeyValue } from "@/global/types";
import { getCultures } from "@/api/site";
import { toObject } from "@/utils/lang";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
const culture = ref<KeyValue[]>([]);
const cultureList = ref<KeyValue[]>([]);
const culturePath = ref<KeyValue[]>([]);

events.onMultilingualSave = (s) => {
  if (!s.enableMultilingual) {
    s.enableSitePath = false;

    culture.value = culture.value.filter(
      (f) => f.key === site.value?.defaultCulture
    );

    culturePath.value = culturePath.value.filter(
      (f) => f.key === site.value?.defaultCulture
    );
  }

  s.culture = toObject(culture.value) as Record<string, string>;
  s.sitePath = toObject(culturePath.value) as Record<string, string>;
};

if (site.value) {
  for (const key in site.value.sitePath) {
    culturePath.value.push({
      key: key,
      value: site.value.sitePath[key],
    });
  }

  for (const key in site.value.culture) {
    culture.value.push({
      key: key,
      value: site.value.culture[key],
    });

    const path = culturePath.value.find((f) => f.key === key);
    if (!path) {
      culturePath.value.push({
        key: key,
        value: key,
      });
    }
  }

  getCultures().then((cultures) => {
    for (const key in cultures) {
      cultureList.value.push({
        key: key,
        value: cultures[key],
      });
    }
  });
}

const availableCultures = computed(() => {
  const list: KeyValue[] = [];

  for (const item of cultureList.value) {
    if (!culture.value.find((f) => f.key === item.key)) {
      list.push(item);
    }
  }

  return list;
});

const onAdd = () => {
  if (!availableCultures.value.length) return;
  const first = availableCultures.value[0];
  culture.value.push({ ...first });
  updateCulture();
};

const onDelete = (key: number) => {
  culture.value.splice(key, 1);
  updateCulture();
};

const onChangeCulture = (value: string) => {
  const defaultName = cultureList.value.find((f) => f.key === value)?.value;
  if (defaultName) {
    const selected = culture.value.find((f) => f.key === value);
    if (selected) {
      selected.value = defaultName;
    }
  }

  updateCulture();
};

const updateCulture = () => {
  for (const path of culturePath.value.map((m) => m.key)) {
    if (!culture.value.find((f) => f.key === path)) {
      culturePath.value = culturePath.value.filter((f) => f.key !== path);
    }
  }

  for (const i of culture.value) {
    if (!culturePath.value.find((f) => f.key === i.key)) {
      culturePath.value.push({
        key: i.key,
        value: i.key,
      });
    }
  }

  if (
    site.value &&
    !culture.value.find((f) => f.key === site.value?.defaultCulture)
  ) {
    site.value.defaultCulture = culture.value[0]?.key;
  }
};

//为了使site.value.culture能够实时变化
watch(
  () => culture.value,
  () => {
    if (site.value) {
      site.value.culture = toObject(culture.value) as Record<string, string>;
    }
  },
  {
    deep: true,
  }
);
//为了使site.value.culturePath能够实时变化
watch(
  () => culturePath.value,
  () => {
    if (site.value) {
      site.value.sitePath = toObject(culturePath.value) as Record<
        string,
        string
      >;
    }
  },
  {
    deep: true,
  }
);
</script>

<template>
  <template v-if="site">
    <GroupPanel
      v-model="site.enableMultilingual"
      :label="t('common.multilingual')"
    >
      <el-form-item>
        <div class="space-y-4 w-full">
          <div
            v-for="(item, index) of culture"
            :key="index"
            class="flex items-center space-x-4"
          >
            <el-select v-model="item.key" @change="onChangeCulture">
              <el-option
                v-for="i of availableCultures"
                :key="i.key"
                :value="i.key"
                :label="i.key"
              />
            </el-select>
            <el-input v-model="item.value" :placeholder="t('common.value')" />
            <div>
              <IconButton
                v-if="culture.length > 1"
                circle
                class="text-orange"
                icon="icon-delete"
                :tip="t('common.delete')"
                @click="onDelete(index)"
              />
            </div>
          </div>
          <IconButton
            v-if="availableCultures.length"
            circle
            class="text-blue"
            icon="icon-a-addto"
            :tip="t('common.add')"
            @click="onAdd"
          />
        </div>
      </el-form-item>

      <el-form-item :label="t('common.defaultLanguage')">
        <el-select v-model="site.defaultCulture" class="w-full">
          <el-option
            v-for="item of culture"
            :key="item.key"
            :value="item.key"
            :label="item.value"
          />
        </el-select>
      </el-form-item>

      <el-form-item>
        <div class="flex items-center w-full">
          <span class="font-bold text-666">{{ t("common.languagePath") }}</span>
          <Tooltip :tip="t('common.languagePathTips')" custom-class="ml-4" />
          <div class="flex-1" />
          <el-switch v-model="site.enableSitePath" />
        </div>
        <div v-if="site.enableSitePath" class="space-y-4">
          <div v-for="item of culturePath" :key="item.key">
            <el-input v-model="item.value">
              <template #suffix>
                <span class="px-12 text-999">{{ item.key }}</span>
              </template>
            </el-input>
          </div>
        </div>
      </el-form-item>
      <el-form-item>
        <span class="font-bold text-666">{{
          t("common.autoDetectCulture")
        }}</span>
        <div class="flex-1" />
        <el-switch v-model="site.autoDetectCulture" />
      </el-form-item>
    </GroupPanel>
  </template>
</template>
