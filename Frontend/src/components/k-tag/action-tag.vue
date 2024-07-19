<script lang="ts" setup>
import { computed } from "vue";

import { useI18n } from "vue-i18n";
const props = defineProps<{ type: string }>();
const { t } = useI18n();

const defines = [
  {
    name: "Add",
    display: t("common.add"),
    elType: "success",
  },
  {
    name: "Update",
    display: t("common.update"),
    elType: "",
  },
  {
    name: "Delete",
    display: t("common.delete"),
    elType: "danger",
  },
];

const define = computed(() => {
  const found = defines.find((f) => f.name === props.type);
  if (found) return found;
  return {
    name: props.type,
    display: props.type,
    elType: "",
  };
});
</script>

<template>
  <el-tag
    class="rounded-full"
    :type="(define.elType as any)"
    size="small"
    data-cy="action"
  >
    {{ define.display }}
  </el-tag>
</template>
