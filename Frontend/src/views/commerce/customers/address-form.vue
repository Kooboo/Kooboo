<script lang="ts" setup>
import type { Address } from "@/api/commerce/customer";
import { onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { getProvinces, getCities } from "@/api/commerce/address";
import { useCommerceStore } from "@/store/commerce";
import { systemDisplay } from "@/utils/commerce";

const { t } = useI18n();
const props = defineProps<{ model: Address }>();
const provinces = ref<any[]>([]);
const cities = ref<any[]>([]);
const commerceStore = useCommerceStore();

onMounted(async () => {
  if (props.model.country) {
    provinces.value = await getProvinces(props.model.country);
    cities.value = await getCities(props.model.country, props.model.province);
  }
});

watch(
  () => props.model.country,
  async () => {
    props.model.province = "";
    props.model.city = "";
    provinces.value = await getProvinces(props.model.country);
    if (provinces.value.length) {
      cities.value = [];
    } else {
      cities.value = await getCities(props.model.country, props.model.province);
    }
  }
);

watch(
  () => props.model.province,
  async () => {
    props.model.city = "";
    cities.value = await getCities(props.model.country, props.model.province);
  }
);
</script>

<template>
  <ElForm v-if="model" label-position="top">
    <ElFormItem :label="t('common.country')">
      <ElSelect v-model="model.country" class="w-full">
        <ElOption
          v-for="item in commerceStore.countries"
          :key="item.code"
          :value="item.name"
          :label="systemDisplay(item.nameTranslations, item.name)"
        />
      </ElSelect>
    </ElFormItem>
    <div class="grid grid-cols-2 gap-8">
      <ElFormItem :label="t('common.city')">
        <ElSelect
          v-model="model.city"
          :disabled="!cities.length && !model.city"
          class="w-full"
        >
          <ElOption
            v-for="item in cities"
            :key="item.name"
            :value="item.name"
            :label="systemDisplay(item.nameTranslations, item.name)"
          />
        </ElSelect>
      </ElFormItem>
      <ElFormItem :label="t('common.province')">
        <ElSelect
          v-model="model.province"
          :disabled="!provinces.length && !model.province"
          class="w-full"
        >
          <ElOption
            v-for="item in provinces"
            :key="item.name"
            :value="item.name"
            :label="systemDisplay(item.nameTranslations, item.name)"
          />
        </ElSelect>
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

<style scoped>
.el-form-item {
  @apply mb-8;
}
</style>
