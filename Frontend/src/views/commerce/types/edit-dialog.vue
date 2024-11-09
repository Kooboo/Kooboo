<script lang="ts" setup>
import type { ProductType } from "@/api/commerce/type";
import { editProductType } from "@/api/commerce/type";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import TermEditor from "./term-editor.vue";
import DigitalFields from "./digital-fields.vue";

const { t } = useI18n();
const show = ref(true);

const props = defineProps<{
  modelValue: boolean;
  model: ProductType;
}>();

const copiedModel = ref<ProductType>(JSON.parse(JSON.stringify(props.model)));

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

async function onSave() {
  await editProductType(copiedModel.value);
  emit("reload");
  show.value = false;
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :title="t('common.edit')"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <ElForm label-position="top">
      <ElFormItem :label="t('common.name')">
        <ElInput v-model="copiedModel.name" />
      </ElFormItem>
      <ElFormItem :label="t('common.attributes')">
        <TermEditor
          :model="copiedModel.attributes"
          :name-label="t('common.name')"
          :value-label="t('common.value')"
          :name-placeholder="t('common.attributeSamples')"
        />
      </ElFormItem>
      <ElFormItem :label="t('commerce.variantOptions')">
        <TermEditor :model="copiedModel.options" force-selection />
      </ElFormItem>
      <DigitalFields :model="copiedModel" />
    </ElForm>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
