<script lang="ts" setup>
import { useI18n } from "vue-i18n";

const props = defineProps<{ order: any }>();
const { t } = useI18n();
</script>

<template>
  <ElTag v-if="props.order.canceled" round type="danger">{{
    t("commerce.canceled")
  }}</ElTag>
  <ElTag v-else-if="!props.order.paid" round type="info">{{
    t("common.notPaid")
  }}</ElTag>
  <ElTag v-else round type="success">{{ t("common.paid") }}</ElTag>
</template>
