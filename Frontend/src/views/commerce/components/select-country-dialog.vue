<script lang="ts" setup>
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { Country } from "@/api/commerce/address";
import { useCommerceStore } from "@/store/commerce";
import { systemDisplay } from "@/utils/commerce";

const { t } = useI18n();
const show = ref(true);
const keyword = ref("");

const props = defineProps<{
  modelValue: boolean;
  excludes?: string[];
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "selected", value: Country): void;
}>();

const commerceStore = useCommerceStore();

function onRowClick(row: Country) {
  emit("selected", row);
  show.value = false;
}

const list = computed(() => {
  const result = [];
  for (const element of commerceStore.countries) {
    const filter = keyword.value?.toLowerCase()?.trim();

    if (filter && !JSON.stringify(element).toLowerCase()?.includes(filter)) {
      continue;
    }

    if (props.excludes?.find((f) => element.code == f || element.name == f)) {
      continue;
    }

    result.push(element);
  }
  return result;
});
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :title="t('common.selectCountry')"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <div class="flex items-center pb-12 space-x-16">
      <SearchInput
        v-model="keyword"
        class="w-full"
        :placeholder="t('common.keyword')"
      />
    </div>
    <ElTable
      :data="list"
      class="el-table--gray mb-24"
      height="400"
      row-class-name="cursor-pointer"
      @row-click="onRowClick"
    >
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <div class="flex items-center gap-4">
            <Country :only-flag="true" :name-or-code="row.code" />
            <span class="text-black dark:text-[#cfd3dc]">{{
              systemDisplay(row.nameTranslations, row.name)
            }}</span>
          </div>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.code')">
        <template #default="{ row }">
          <span class="text-black dark:text-[#cfd3dc]">{{ row.code }}</span>
        </template>
      </el-table-column>
    </ElTable>
  </el-dialog>
</template>
