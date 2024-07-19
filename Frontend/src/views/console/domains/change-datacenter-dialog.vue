<template>
  <el-dialog
    :model-value="show"
    width="350px"
    :close-on-click-modal="false"
    :title="t('common.changeDataCenter')"
    custom-class="changeDataCenterDialog"
    @closed="emit('update:modelValue', false)"
  >
    <el-form ref="form" class="el-form--label-normal" label-position="top">
      <el-form-item :label="t('common.dataCenter')">
        <el-select v-model="dataCenter" class="w-full" placeholder="Text">
          <el-option
            v-for="i in dataCenterList"
            :key="i.dataCenter"
            :value="i.dataCenter"
            :label="i.description"
          />
        </el-select>
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { ref } from "vue";
import type { ElForm } from "element-plus";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
import { assignDataCenter, getDataCenterList } from "@/api/console";

import { showConfirm } from "@/components/basic/confirm";
import type { dataCenterType } from "@/api/console/types";
interface PropsType {
  domain: string;
  dataCenter: string;
  modelValue: boolean;
}

const props = defineProps<PropsType>();
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "change-success"): void;
}>();

const { t } = useI18n();
const show = ref(true);
const form = ref<InstanceType<typeof ElForm>>();
const dataCenter = ref();
const dataCenterList = ref<dataCenterType[]>();

const onSave = async () => {
  await showConfirm(t("common.assignDataCenterTips"));
  await assignDataCenter(props.domain, dataCenter.value);
  show.value = false;
  emit("change-success");
};
const load = async () => {
  dataCenter.value = props.dataCenter;
  dataCenterList.value = await getDataCenterList(props.domain);
};
load();
</script>
<style>
/* 取消弹框的底部padding */
.changeDataCenterDialog .el-dialog__body {
  padding-bottom: 0px !important;
}
</style>
