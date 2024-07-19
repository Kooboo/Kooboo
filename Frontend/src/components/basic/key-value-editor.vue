<script lang="ts" setup>
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import DropdownInput from "@/components/basic/dropdown-input.vue";
import type { OptionItemType, AttributesBuilderType } from "@/global/types";
import type { FormItemRule } from "element-plus";
import { cloneDeep } from "lodash-es";

type AttributeBindingType =
  | AttributesBuilderType<OptionItemType>
  | Record<string, any>
  | undefined;

const props = defineProps<{
  modelValue: OptionItemType[];
  keyInputAttributes?: AttributeBindingType;
  valueInputAttributes?: AttributeBindingType;
  defaultOptions?: string[];
  keyRules?: FormItemRule[];
  valueRules?: FormItemRule[];
  prop?: string;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: OptionItemType[]): void;
  (e: "delete"): void;
  (e: "add"): void;
}>();

const model = ref<OptionItemType[]>([]);

watch(
  () => props.modelValue,
  (value) => {
    model.value = value ? cloneDeep(value) : [];
  },
  { deep: true, immediate: true }
);

const { t } = useI18n();

function onDelete(index: number) {
  model.value.splice(index, 1);
  emit("update:model-value", model.value);
  emit("delete");
}

function onAdd() {
  model.value.push({ key: "", value: "", options: props.defaultOptions });
  emit("update:model-value", model.value);
  emit("add");
}

function getBindings(
  binding: AttributeBindingType,
  item: OptionItemType,
  index: number,
  model: OptionItemType[]
) {
  if (typeof binding === "function") {
    return binding(item, index, model);
  }

  return binding;
}
</script>

<template>
  <div class="space-y-4">
    <div
      v-for="(item, index) of model"
      :key="index"
      class="flex items-center space-x-4 mb-16"
    >
      <el-form-item
        :prop="`${prop ?? ''}[${index}].key`"
        class="flex-1"
        :rules="keyRules"
      >
        <el-input
          v-model="item.key"
          :placeholder="t('common.key')"
          v-bind="getBindings(keyInputAttributes, item, index, model)"
          @change="emit('update:model-value', model)"
        />
      </el-form-item>
      <el-form-item
        :prop="`${prop ?? ''}[${index}].value`"
        class="flex-1"
        :rules="valueRules"
      >
        <DropdownInput
          v-if="(item.options ?? defaultOptions)?.length"
          v-model="item.value"
          :options="item.options ?? defaultOptions"
          class="w-full"
          :placeholder="t('common.value')"
          v-bind="getBindings(valueInputAttributes, item, index, model)"
          @update:model-value="emit('update:model-value', model)"
        />
        <el-input
          v-else
          v-model="item.value"
          :placeholder="t('common.value')"
          v-bind="getBindings(valueInputAttributes, item, index, model)"
          @change="emit('update:model-value', model)"
        />
      </el-form-item>
      <div>
        <IconButton
          circle
          class="hover:text-orange text-orange"
          icon="icon-delete "
          :tip="t('common.delete')"
          @click="onDelete(index)"
        />
      </div>
    </div>
    <IconButton
      circle
      class="text-blue"
      icon="icon-a-addto "
      :tip="t('common.add')"
      @click="onAdd"
    />
  </div>
</template>
