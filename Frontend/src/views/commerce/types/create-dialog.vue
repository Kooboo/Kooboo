<script lang="ts" setup>
import type { ProductType } from "@/api/commerce/type";
import { createProductType } from "@/api/commerce/type";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import TermEditor from "./term-editor.vue";
import DigitalFields from "./digital-fields.vue";

const { t } = useI18n();
const show = ref(true);

defineProps<{
  modelValue: boolean;
}>();

const model = ref<ProductType>({
  name: "",
  attributes: [],
  options: [],
  isDigital: false,
  maxDownloadCount: undefined,
});

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

async function onSave() {
  await createProductType(model.value);
  emit("reload");
  show.value = false;
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :title="t('common.create')"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <ElForm label-position="top">
      <ElFormItem :label="t('common.name')">
        <ElInput v-model="model.name" />
      </ElFormItem>
      <ElFormItem :label="t('common.attributes')">
        <TermEditor
          :model="model.attributes"
          :name-label="t('common.name')"
          :value-label="t('common.value')"
          :name-placeholder="t('common.attributeSamples')"
        />
      </ElFormItem>
      <ElFormItem :label="t('commerce.variantOptions')">
        <TermEditor :model="model.options" force-selection />
      </ElFormItem>
      <DigitalFields :model="model" />
    </ElForm>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
