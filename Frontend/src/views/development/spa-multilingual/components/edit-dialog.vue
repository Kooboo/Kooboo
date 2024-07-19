<template>
  <el-dialog
    v-model="props.modelValue"
    width="650px"
    :close-on-click-modal="false"
    :title="t('common.setting')"
    @close="handleClose"
  >
    <el-form label-position="top" @submit.prevent>
      <el-form-item v-for="key in keys" :key="key" :label="key">
        <el-input
          v-if="model?.value"
          v-model="model.value[key]"
          :rows="2"
          type="textarea"
        /> </el-form-item
    ></el-form>

    <template #footer>
      <DialogFooterBar @confirm="handleSave" @cancel="handleClose" />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { post } from "@/api/development/spa-multilingual";
import type { Multilingual, Langs } from "@/api/development/spa-multilingual";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

interface PropsType {
  modelValue: boolean;
  data: Multilingual;
  keys: Langs[];
}
interface EmitsType {
  (e: "close"): void;
  (e: "save-success"): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const model = ref<Multilingual>();

watch(
  () => props.data,
  (data) => {
    model.value = { ...data };
    if (model.value.value) {
      model.value.value = { ...model.value.value };
    }
  },
  { immediate: true, deep: true }
);

const handleClose = () => {
  emits("close");
};

async function handleSave() {
  if (!model.value) return;
  await post(model.value);
  emits("save-success");
  handleClose();
}
</script>
