<script lang="ts" setup>
import type { MembershipStatus } from "@/api/commerce/loyalty";
import { useI18n } from "vue-i18n";

defineProps<{ status: MembershipStatus }>();

const { t } = useI18n();
</script>

<template>
  <div v-if="status" class="space-x-4 inline">
    <ElTag round type="success">{{ status.membership.name }}</ElTag>
    <ElTag v-if="!status.active" round type="danger">{{
      t("common.expired")
    }}</ElTag>
  </div>
  <ElTag v-else round type="info">{{ t("common.nonMembership") }}</ElTag>
</template>
