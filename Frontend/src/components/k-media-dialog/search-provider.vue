<template>
  <el-select
    v-if="providers.length > 1"
    v-model="provider"
    class="right-24 w-130px"
    default-first-option
    @change="onProviderChanged"
  >
    <el-option
      v-for="p in providers"
      :key="p"
      :value="p"
      :label="t(`storage.${p}`)"
    />
  </el-select>
  <SearchInput
    v-model="innerKey"
    :placeholder="t('common.enterKeywords')"
    class="w-238px"
    data-cy="search"
    @search="refreshList"
  />
</template>

<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { ref, watch } from "vue";

const props = defineProps<{
  providers: string[];
  provider: string;
  searchKey: string;
}>();
const { t } = useI18n();

const innerKey = ref(props.searchKey ?? "");
const emits = defineEmits<{
  (e: "update:provider", value: string): void;
  (e: "update:search-key", value: string): void;
}>();

function refreshList() {
  emits("update:search-key", innerKey.value ?? "");
}
function onProviderChanged(value: string) {
  emits("update:provider", value);
}
watch(
  () => props.searchKey,
  (value) => {
    innerKey.value = value;
  }
);
</script>
