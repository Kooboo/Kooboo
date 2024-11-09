<script lang="ts" setup>
import { getCountries, type SupportCountry } from "@/api/commerce/shipping";
import type { KeyValue } from "@/global/types";
import { ElInputNumber } from "element-plus";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

const props = defineProps<{ modelValue: SupportCountry[] }>();
const emit = defineEmits<{
  (e: "update:modelValue", value: SupportCountry[]): void;
}>();

const { t } = useI18n();
const counties = ref<KeyValue[]>([]);
getCountries().then((rsp) => (counties.value = rsp));

const selectableList = computed(() => {
  return counties.value.filter(
    (f) => !props.modelValue.find((ff) => f.key == ff.name)
  );
});

function onAdd() {
  const name = selectableList.value[0]?.key;
  if (!name) return;
  emit("update:modelValue", [
    ...props.modelValue,
    {
      name: name,
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
          v-for="country of counties"
          :key="country.key"
          :label="country.value"
          :value="country.key"
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
