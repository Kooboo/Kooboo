<script lang="ts" setup>
import type { Address } from "@/api/commerce/customer";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
defineProps<{ model: Address }>();
</script>

<template>
  <ElForm v-if="model" label-position="top">
    <ElFormItem :label="t('common.country')">
      <ElInput v-model="model.country" />
    </ElFormItem>
    <div class="grid grid-cols-2 gap-8">
      <ElFormItem :label="t('common.city')">
        <ElInput v-model="model.city" />
      </ElFormItem>
      <ElFormItem :label="t('common.province')">
        <ElInput v-model="model.province" />
      </ElFormItem>
    </div>
    <ElFormItem :label="t('common.address')">
      <ElInput v-model="model.address1" />
    </ElFormItem>
    <ElFormItem :label="t('common.postalCode')">
      <ElInput v-model="model.zip" />
    </ElFormItem>
    <div class="grid grid-cols-2 gap-8">
      <ElFormItem :label="t('common.firstName')">
        <ElInput v-model="model.firstName" />
      </ElFormItem>
      <ElFormItem :label="t('common.lastName')">
        <ElInput v-model="model.lastName" />
      </ElFormItem>
    </div>
    <ElFormItem :label="t('common.phone')">
      <ElInput v-model="model.phone" />
    </ElFormItem>
  </ElForm>
</template>
