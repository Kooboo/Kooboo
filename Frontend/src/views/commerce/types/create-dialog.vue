<script lang="ts" setup>
import { createProductType } from "@/api/commerce/type";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import TermEditor from "./term-editor.vue";

const { t } = useI18n();
const show = ref(true);

defineProps<{
  modelValue: boolean;
}>();

const model = ref({
  name: "",
  attributes: [],
  options: [],
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
        <TermEditor :model="model.attributes" />
      </ElFormItem>
      <ElFormItem :label="t('commerce.variantOptions')">
        <TermEditor :model="model.options" force-selection />
      </ElFormItem>
    </ElForm>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
