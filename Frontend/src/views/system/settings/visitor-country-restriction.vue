<script lang="ts" setup>
import { site, events } from "./settings";
import GroupPanel from "./group-panel.vue";
import { computed, ref, watch, onMounted } from "vue";
import { toObject } from "@/utils/lang";
import codes from "@/assets/static/country-and-region-codes.json";
import { useI18n } from "vue-i18n";
import { usePageStore } from "@/store/page";
import type { Page } from "@/api/pages/types";

const { locale, t } = useI18n();
const restrictions = ref<string[]>([]);
const codeLabelKey = computed(() =>
  locale.value === "zh" ? "nameCN" : "name"
);
const { list, load } = usePageStore();
const pageList = ref<Page[]>(list);

events.onVisitorCountryRestrictionsSave = (s) => {
  s.visitorCountryRestrictions = toObject(
    restrictions.value.map((it) => ({
      key: it,
      value: site.value?.visitorCountryRestrictionPage ?? "",
    }))
  ) as Record<string, string>;
};

onMounted(async () => {
  if (!pageList.value?.length) {
    pageList.value = await load();
  }
});

if (site.value) {
  site.value.enableVisitorCountryRestriction =
    site.value.enableVisitorCountryRestriction ?? false;
  site.value.visitorCountryRestrictionPage =
    site.value.visitorCountryRestrictionPage ?? "";
  if (site.value.visitorCountryRestrictions) {
    restrictions.value = Object.keys(site.value.visitorCountryRestrictions);
  }
}

const sortedCodes = computed(() => {
  return codes.sort((a, b) => {
    if (codeLabelKey.value === "nameCN") {
      return a.namePY.localeCompare(b.namePY);
    }
    return a.name.localeCompare(b.name);
  });
});

watch(
  () => restrictions.value,
  () => {
    if (site.value) {
      site.value.visitorCountryRestrictions = toObject(
        restrictions.value.map((it) => ({
          key: it,
          value: site.value?.visitorCountryRestrictionPage ?? "",
        }))
      ) as Record<string, string>;
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
      v-model="site.enableVisitorCountryRestriction"
      :label="t('common.visitorCountryRestriction')"
    >
      <el-form-item :label="t('common.countryAndRegion')">
        <el-select
          v-model="restrictions"
          multiple
          filterable
          default-first-option
          class="w-full"
          data-cy="VisitorCountryRestriction"
        >
          <el-option
            v-for="item of sortedCodes"
            :key="item.isoCode"
            :value="item.isoCode.toLowerCase()"
            :label="`${item[codeLabelKey]} (${item.isoCode})`"
            :data-cy="item.isoCode"
          />
        </el-select>
      </el-form-item>
      <el-form-item :label="t('common.visitorCountryRestrictionPage')">
        <el-select
          v-model="site.visitorCountryRestrictionPage"
          class="w-full"
          data-cy="VisitorCountryRestrictionPage"
        >
          <el-option
            value=""
            :label="t('common.default')"
            data-cy="DefaultPage"
          />
          <el-option
            v-for="page of pageList"
            :key="page.id"
            :value="page.path"
            :label="page.path"
            :data-cy="page.path"
          />
        </el-select>
      </el-form-item>
    </GroupPanel>
  </template>
</template>
