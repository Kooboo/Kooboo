<script lang="ts" setup>
import type { OptionGroup, VariantOption } from "@/api/commerce/product";
import { errorMessage } from "@/components/basic/message";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useMultilingualStore } from "@/store/multilingual";
import VariantMultilingualDialog from "./variant-multilingual-dialog.vue";
import ImageCover from "@/components/basic/image-cover.vue";
import { ElButton, ElCard } from "element-plus";
import IconButton from "@/components/basic/icon-button.vue";
import { controls } from "./product-variant";

const { t } = useI18n();

const props = defineProps<{
  model: OptionGroup;
  variantOption: VariantOption;
  editing?: boolean;
}>();

const emit = defineEmits<{
  (e: "changeName", value: string): void;
  (e: "changeOption", oldValue: string, newValue: string): void;
  (e: "changeOptionImage", value: string, image: string): void;
  (e: "addOption", value: string): void;
  (e: "deleteOption", index: number): void;
  (e: "delete"): void;
}>();

const copyValue = ref<OptionGroup>();
const multilingualStore = useMultilingualStore();
const editing = ref(!!props.editing);
const showVariantMultilingualDialog = ref(false);
const newOptionValue = ref("");

function onChangeItem(index: number, value: string) {
  const oldValue = props.model.options[index];
  if (props.model.options.includes(value)) {
    errorMessage(t("common.valueExist"));
    copyValue.value!.options[index] = oldValue;
  } else if (!value) {
    copyValue.value!.options[index] = oldValue;
  } else {
    const variantOptionItem = props.variantOption.items.find(
      (f) => f.name == oldValue
    );
    if (variantOptionItem) variantOptionItem.name = value;
    emit("changeOption", oldValue, value);
  }
}

function onChangeOptionImage(option: string, image: string) {
  var item = props.variantOption.items.find((f) => f.name == option);
  if (item) {
    item.image = image;
  }
}

function updateVariantOption(value: VariantOption) {
  Object.assign(props.variantOption, value);
}

function addOptionValue(value: string) {
  newOptionValue.value = "";
  value = value?.trim();
  if (!value || props.model.options.includes(value)) {
    return;
  }
  emit("addOption", value);
}

function changeName(value: string) {
  emit("changeName", value);
  copyValue.value = JSON.parse(JSON.stringify(props.model));
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
  <ElCard v-if="copyValue" shadow="never">
    <div v-if="editing">
      <ElForm label-position="top">
        <div class="space-y-12">
          <ElFormItem :label="t('commerce.optionName')">
            <ElInput
              v-model="copyValue.name"
              :placeholder="t('commerce.variantSamples')"
              @change="changeName"
            >
              <template #append>
                <el-select
                  v-model="variantOption.type"
                  :placeholder="t('common.type')"
                  class="w-120px !m-0"
                >
                  <el-option
                    v-for="item of controls"
                    :key="item.key"
                    :label="item.value"
                    :value="item.key"
                  />
                </el-select>
              </template>
            </ElInput>
          </ElFormItem>
          <ElFormItem :label="t('commerce.optionValues')">
            <div class="space-y-4 w-full">
              <ElInput
                v-for="(item, index) of copyValue.options"
                :key="index"
                :model-value="item"
                @update:model-value="copyValue.options[index] = $event"
                @change="onChangeItem(index, $event)"
              >
                <template v-if="variantOption.type == 'color'" #prepend>
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
                <template v-if="variantOption.type == 'color'" #prepend>
                  <el-color-picker @update:model-value="addOptionValue" />
                </template>
              </ElInput>
              <ElInput
                v-if="newOptionValue?.trim()"
                :placeholder="t('common.addAnotherValue')"
              >
                <template v-if="variantOption.type == 'color'" #prepend>
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
      <div class="flex items-center">
        <div class="truncate flex-1 text-l font-bold">
          {{ model.name }}
        </div>
        <div
          v-if="multilingualStore.visible"
          class="flex-shrink-0"
          @click.stop="showVariantMultilingualDialog = true"
        >
          <IconButton
            circle
            icon="icon-language "
            :tip="t('common.multilingual')"
          />
        </div>
      </div>
      <div class="flex gap-8 flex-wrap">
        <div
          v-for="(item, index) of model.options"
          :key="index"
          class="bg-999/30 px-8 rounded-normal flex items-center gap-8"
        >
          <ImageCover
            class="inline"
            editable
            size="mini"
            :model-value="
              variantOption.items.find((f) => f.name == item)?.image
            "
            folder="/commerce/product"
            :prefix="new Date().getTime().toString()"
            @update:model-value="onChangeOptionImage(item, $event)"
          />
          <span>{{ item }}</span>
          <div
            v-if="variantOption.type == 'color'"
            :style="{ backgroundColor: item }"
            class="w-12 h-12 rounded-full shadow-s-4"
          />
        </div>
      </div>
    </div>
  </ElCard>
  <div>
    <VariantMultilingualDialog
      v-if="showVariantMultilingualDialog"
      v-model="showVariantMultilingualDialog"
      :variant-option="variantOption"
      @update:variant-option="updateVariantOption"
    />
  </div>
</template>
