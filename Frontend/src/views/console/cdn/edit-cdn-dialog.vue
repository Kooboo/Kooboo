<template>
  <el-dialog
    v-model="show"
    width="600px"
    :title="t('common.editCdn')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      label-position="top"
      :model="currentCDN"
      @submit.prevent
    >
      <el-form-item prop="domainName" :label="t('common.domain')">
        <el-input v-model="currentCDN!.domainName" disabled />
      </el-form-item>
      <el-form-item prop="siteCDN" :label="t('common.enable')">
        <el-switch v-model="currentCDN!.siteCDN" />
      </el-form-item>
      <el-form-item prop="icpCert" label="icpCert">
        <el-input v-model="currentCDN!.icpCert" />
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar @cancel="show = false" @confirm="save" />
    </template>
  </el-dialog>
</template>
<script lang="ts" setup>
import { updateCDN } from "@/api/console";
import type { CDN } from "@/api/console/types";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "editSuccess"): void;
}>();

const props = defineProps<{
  modelValue: boolean;
  cdn: CDN;
}>();
const form = ref();
const show = ref(true);
const currentCDN = ref<CDN>();
const save = async () => {
  await updateCDN(currentCDN.value!);
  show.value = false;
  emit("editSuccess");
};
const load = () => {
  currentCDN.value = JSON.parse(JSON.stringify(props.cdn));
};
load();
</script>
