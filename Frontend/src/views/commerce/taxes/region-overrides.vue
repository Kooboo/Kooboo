<script lang="ts" setup>
import type { RegionOverrides, OverridesType } from "@/api/commerce/tax";
import { systemDisplay } from "@/utils/commerce";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
const props = defineProps<{
  regions: any[];
  list: RegionOverrides[];
  baseTax: number;
}>();
const pageIndex = ref(1);
const pageSize = ref(6);
const keyword = ref("");

const data = computed(() => {
  let result = [];
  const kw = keyword.value?.toLowerCase()?.trim();
  for (const element of props.regions) {
    if (kw && !JSON.stringify(element).toLowerCase().includes(kw)) {
      continue;
    }
    result.push(element);
  }
  const start = (pageIndex.value - 1) * pageSize.value;
  return {
    list: result.slice(start, start + pageSize.value),
    count: result.length,
  };
});

function reset() {
  keyword.value = "";
  props.list.splice(0, props.list.length);
}

function currentOverrides(name: string) {
  return props.list.find((f) => f.region == name);
}

function setOverridesTaxes(name: string, value: number) {
  let current = currentOverrides(name);
  if (!current) {
    current = { region: name, tax: value, type: "Added" };
    props.list.push(current);
  } else {
    current.tax = value;
  }
}

function setOverridesType(name: string, value: OverridesType) {
  let current = currentOverrides(name);
  if (!current) {
    current = { region: name, tax: 0, type: value };
    props.list.push(current);
  } else {
    current.type = value;
  }
}
</script>

<template>
  <ElCard :shadow="false" class="w-full max-w-800px">
    <template #header>
      <div class="flex justify-between">
        <SearchInput
          v-model="keyword"
          class="w-208px"
          :placeholder="t('common.keyword')"
        />
        <ElButton round @click="reset">{{ t("common.reset") }}</ElButton>
      </div>
    </template>
    <div class="divide-y divide-line divide-solid">
      <div
        v-for="item of data.list"
        :key="item.name"
        class="flex items-center gap-12 py-8"
      >
        <div class="w-180px">
          {{
            systemDisplay(
              regions.find((f) => f.name == item.name)?.nameTranslations,
              item.name
            )
          }}
        </div>
        <div class="flex items-center gap-4">
          <ElInputNumber
            :model-value="currentOverrides(item.name)?.tax || 0"
            :step="0.1"
            :min="0"
            @update:model-value="setOverridesTaxes(item.name, $event)"
          />
          <span class="dark:text-[#cfd3dc]">%</span>
        </div>
        <ElSelect
          :model-value="currentOverrides(item.name)?.type || 'Added'"
          class="flex-1"
          @update:model-value="setOverridesType(item.name, $event)"
        >
          <ElOption
            value="Added"
            :label="
              t('common.addedToBaseTax', {
                tax: baseTax,
              })
            "
          />
          <ElOption
            value="Instead"
            :label="
              t('common.insteadOfBaseTax', {
                tax: baseTax,
              })
            "
          />
          <ElOption
            value="Compounded"
            :label="
              t('common.compoundedOnTopOfBaseTax', {
                tax: baseTax,
              })
            "
          />
        </ElSelect>
      </div>
      <el-pagination
        class="pt-12"
        layout="prev, pager, next"
        :page-size="pageSize"
        :total="data.count"
        @current-change="pageIndex = $event"
      />
    </div>
  </ElCard>
</template>
