<script lang="ts" setup>
import type { OptionGroup, VariantOption } from "@/api/commerce/product";
import { errorMessage } from "@/components/basic/message";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import EditableTags from "@/components/basic/editable-tags.vue";
import { useMultilingualStore } from "@/store/multilingual";
import VariantMultilingualDialog from "./variant-multilingual-dialog.vue";

const { t } = useI18n();

const props = defineProps<{
  model: OptionGroup;
  variantOption: VariantOption;
}>();

const emit = defineEmits<{
  (e: "changeName", value: string): void;
  (e: "changeOption", oldValue: string, newValue: string): void;
  (e: "addOption", value: string): void;
  (e: "deleteOption", index: number): void;
  (e: "delete"): void;
}>();

const copyValue = ref<OptionGroup>();
const multilingualStore = useMultilingualStore();
const showVariantMultilingualDialog = ref(false);

function onChangeItem(index: number, value: string) {
  if (props.model.options.includes(value)) {
    errorMessage(t("common.valueExist"));
  } else {
    const oldValue = props.model.options[index];
    const variantOptionItem = props.variantOption.items.find(
      (f) => f.name == oldValue
    );
    if (variantOptionItem) variantOptionItem.name = value;
    emit("changeOption", oldValue, value);
  }
}

function updateVariantOption(value: VariantOption) {
  Object.assign(props.variantOption, value);
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
  <div v-if="copyValue">
    <div class="flex items-center space-x-4">
      <ElInput
        v-model="copyValue.name"
        @change="emit('changeName', $event)"
        @keydown.enter.stop.prevent=""
      />
      <div v-if="multilingualStore.visible">
        <IconButton
          circle
          icon="icon-language "
          :tip="t('common.multilingual')"
          @click="showVariantMultilingualDialog = true"
        />
      </div>
      <div>
        <IconButton
          circle
          class="hover:text-orange text-orange"
          icon="icon-delete "
          :tip="t('common.delete')"
          @click="emit('delete')"
        />
      </div>
    </div>
    <EditableTags
      :readonly="!copyValue.name"
      :model-value="copyValue.options"
      class="mt-4 mb-12 mr-44px"
      @change-item="onChangeItem"
      @add-item="emit('addOption', $event)"
      @remove-item="emit('deleteOption', $event)"
    />
  </div>
  <div>
    <VariantMultilingualDialog
      v-if="showVariantMultilingualDialog"
      v-model="showVariantMultilingualDialog"
      :variant-option="variantOption"
      @update:variant-option="updateVariantOption"
    />
  </div>
</template>
