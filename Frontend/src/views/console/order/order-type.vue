<script lang="ts" setup>
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps<{ type: number }>();
</script>

<template>
  <el-tag v-if="type == 1">{{ t("common.purchase") }}</el-tag>
  <el-tag v-else-if="type == 2">{{ t("common.membership") }}</el-tag>
  <el-tag v-else-if="type == 3">{{ t("common.bandwidth") }}</el-tag>
  <el-tag v-else-if="type == 4">{{ t("common.service") }}</el-tag>
  <el-tag v-else-if="type == 5">{{ t("common.server") }}</el-tag>
  <el-tag v-else-if="type == 6">{{ t("common.topup") }}</el-tag>
  <el-tag v-else-if="type == 7">{{ t("common.apiMarket") }}</el-tag>
  <el-tag v-else-if="type == 8">{{ t("common.appMarket") }}</el-tag>
  <el-tag v-else-if="type == 9">{{ t("common.renew") }}</el-tag>
</template>
