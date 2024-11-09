<script lang="ts" setup>
import type { Term } from "@/api/commerce/type";
import { errorMessage } from "@/components/basic/message";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { controls } from "@/views/commerce/products-management/product-variant";

const props = defineProps<{
  model: Term;
  forceSelection?: boolean;
  editing: boolean;
  nameLabel?: string;
  valueLabel?: string;
  namePlaceholder?: string;
}>();
const editing = ref(props.editing);
const { t } = useI18n();
const copyValue = ref<Term>();
const newOptionValue = ref("");

const termTypes = [
  { name: "Selection", display: t("common.selection") },
  { name: "Custom", display: t("common.custom") },
];

const emit = defineEmits<{
  (e: "changeName", value: string): void;
  (e: "changeOption", oldValue: string, newValue: string): void;
  (e: "addOption", value: string): void;
  (e: "deleteOption", index: number): void;
  (e: "delete"): void;
}>();

function onChangeItem(index: number, value: string) {
  const oldValue = props.model.options[index];
  if (props.model.options.includes(value)) {
    errorMessage(t("common.valueExist"));
    copyValue.value!.options[index] = oldValue;
  } else if (!value) {
    copyValue.value!.options[index] = oldValue;
  } else {
    emit("changeOption", oldValue, value);
  }
}

function changeName(value: string) {
  emit("changeName", value);
  copyValue.value = JSON.parse(JSON.stringify(props.model));
}

function addOptionValue(value: string) {
  newOptionValue.value = "";
  value = value?.trim();
  if (!value || props.model.options.includes(value)) {
    return;
  }
  emit("addOption", value);
}

watch(
  () => props.model,
  () => {
    copyValue.value = JSON.parse(JSON.stringify(props.model));
  },
  { deep: true, immediate: true }
);
</script>

<template>
  <ElCard shadow="never">
    <div v-if="editing">
      <ElForm label-position="top">
        <div class="space-y-12">
          <ElFormItem :label="nameLabel || t('commerce.optionName')">
            <div class="flex gap-4 w-full">
              <ElInput
                v-model="model.name"
                :placeholder="namePlaceholder || t('commerce.variantSamples')"
                @change="changeName"
              >
                <template v-if="forceSelection" #append>
                  <el-select
                    v-model="model.valueType"
                    :placeholder="t('common.type')"
                    class="w-120px !m-0"
                  >
                    <el-option
                      v-for="item of controls"
                      :key="item.key"
                      :label="item.value"
                      :value="item.key"
                    />
                  </el-select> </template
              ></ElInput>
              <ElSelect
                v-if="!forceSelection"
                v-model="model.type"
                class="w-220px"
              >
                <ElOption
                  v-for="item in termTypes"
                  :key="item.name"
                  :label="item.display"
                  :value="item.name"
                />
              </ElSelect>
            </div>
          </ElFormItem>
          <ElFormItem
            v-if="model.type == 'Selection'"
            :label="valueLabel || t('commerce.optionValues')"
          >
            <div class="space-y-4 w-full">
              <ElInput
                v-for="(item, index) of copyValue!.options"
                :key="index"
                :model-value="item"
                @update:model-value="copyValue!.options[index] = $event"
                @change="onChangeItem(index, $event)"
              >
                <template v-if="model.valueType == 'color'" #prepend>
                  <el-color-picker
                    :model-value="item"
                    @update:model-value="onChangeItem(index, $event)"
                  />
                </template>
                <template #suffix>
                  <IconButton
                    class="hover:text-orange"
                    :tip="t('common.delete')"
                    icon="icon-delete"
                    @click="emit('deleteOption', index)"
                  />
                </template>
              </ElInput>
              <ElInput
                v-model="newOptionValue"
                :placeholder="
                  model.options.length
                    ? t('common.addAnotherValue')
                    : t('common.addValue')
                "
                @change="addOptionValue"
              >
                <template v-if="model.valueType == 'color'" #prepend>
                  <el-color-picker @update:model-value="addOptionValue" />
                </template>
              </ElInput>
              <ElInput
                v-if="newOptionValue"
                :placeholder="t('common.addAnotherValue')"
              >
                <template v-if="model.valueType == 'color'" #prepend>
                  <el-color-picker @update:model-value="addOptionValue" />
                </template>
              </ElInput>
            </div>
          </ElFormItem>
        </div>
      </ElForm>
      <div class="flex items-center justify-between mt-12">
        <IconButton
          circle
          class="hover:text-orange text-orange"
          icon="icon-delete "
          :tip="t('common.delete')"
          @click="emit('delete')"
        />
        <ElButton round type="primary" @click="editing = false">{{
          t("common.done")
        }}</ElButton>
      </div>
    </div>
    <div v-else class="space-y-8 cursor-pointer" @click="editing = true">
      <div class="truncate flex-1 text-l font-bold">
        {{ model.name }}
      </div>
      <div v-if="model.type == 'Selection'" class="flex gap-8 flex-wrap">
        <div
          v-for="(item, index) of model.options"
          :key="index"
          class="bg-999/30 px-8 rounded-normal flex items-center gap-4"
        >
          <span>{{ item }}</span>
          <div
            v-if="model.valueType == 'color'"
            :style="{ backgroundColor: item }"
            class="w-12 h-12 rounded-full shadow-s-4"
          />
        </div>
      </div>
    </div>
  </ElCard>
</template>
