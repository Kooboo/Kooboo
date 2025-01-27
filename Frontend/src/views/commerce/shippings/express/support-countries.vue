<script lang="ts" setup>
import { type SupportCountry } from "@/api/commerce/shipping";
import { useCommerceStore } from "@/store/commerce";
import { systemDisplay } from "@/utils/commerce";
import { ElInputNumber } from "element-plus";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

const props = defineProps<{ modelValue: SupportCountry[] }>();
const emit = defineEmits<{
  (e: "update:modelValue", value: SupportCountry[]): void;
}>();

const { t } = useI18n();
const commerceStore = useCommerceStore();

const selectableList = computed(() => {
  return commerceStore.countries.filter(
    (f) => !props.modelValue.find((ff) => f.code == ff.name)
  );
});

function onAdd() {
  const code = selectableList.value[0]?.code;
  if (!code) return;
  emit("update:modelValue", [
    ...props.modelValue,
    {
      name: code,
      display: "",
      estimatedDaysOfArrival: 3,
    },
  ]);
}

function onDelete(index: number) {
  props.modelValue.splice(index, 1);
}
</script>

<template>
  <div class="w-840px space-y-8">
    <div
      v-for="(item, index) of modelValue"
      :key="index"
      class="flex items-center gap-8"
    >
      <ElSelect v-model="item.name">
        <ElOption
          v-for="country of commerceStore.countries"
          :key="country.code"
          :label="systemDisplay(country.nameTranslations, country.name)"
          :value="country.code"
        />
      </ElSelect>
      <ElInput
        v-model="item.display"
        class="w-240px"
        :placeholder="t('common.alias')"
      />
      <div class="flex items-center gap-4">
        <span class="dark:text-[#cfd3dc]">{{ t("common.estimate") }}</span>
        <ElInputNumber v-model="item.estimatedDaysOfArrival" :min="1" />
        <span class="dark:text-[#cfd3dc]">{{ t("common.daysOfArrival") }}</span>
      </div>
      <IconButton
        circle
        class="hover:text-orange text-orange"
        icon="icon-delete "
        :tip="t('common.delete')"
        @click="onDelete(index)"
      />
    </div>
    <IconButton
      circle
      icon="icon-a-addto"
      class="text-blue hover:text-blue"
      :tip="t('common.add')"
      @click="onAdd"
    />
  </div>
</template>
