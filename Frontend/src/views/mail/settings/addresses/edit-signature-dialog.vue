<template>
  <el-dialog
    v-model="show"
    width="600px"
    :title="t('common.editSignature')"
    :close-on-click-modal="false"
    @closed="emits('update:modelValue', false)"
  >
    <el-form class="el-form--label-normal" label-position="top">
      <el-form-item :label="t('common.signature')">
        <KEditor
          v-model="signature"
          :manual-upload="true"
          :hidden-code="true"
          :min_height="250"
          :max_height="350"
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar @confirm="handleConfirm" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { ref } from "vue";
import { getSignature, updateSignature } from "@/api/mail";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { useI18n } from "vue-i18n";

const props = defineProps<{
  modelValue: boolean;
  currentId: number;
}>();
const emits = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();
const { t } = useI18n();
const show = ref(true);

const signature = ref();

const handleConfirm = async () => {
  await updateSignature(props.currentId, signature.value);
  show.value = false;
};
const load = async () => {
  signature.value = await getSignature(props.currentId);
};
load();
</script>
