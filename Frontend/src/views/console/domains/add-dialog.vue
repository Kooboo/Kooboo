<template>
  <el-dialog
    v-model="visible"
    width="500px"
    :close-on-click-modal="false"
    :title="t('common.newLocalDomain')"
    @close="handleClose"
  >
    <el-form
      ref="form"
      class="el-form--label-normal"
      :model="model"
      :rules="rules"
      label-position="top"
      @submit.prevent
    >
      <div v-if="appStore.header?.isOnlineServer" class="mb-16">
        <el-alert :closable="false" type="info" class="dark:bg-444">
          <template #header>
            <span class="text-2l font-bold">{{
              t("common.addADomainNameToYourAccount")
            }}</span>
          </template>
          <div class="space-y-4 leading-4">
            <div class="break-normal">
              {{ t("common.dnsServer") }}
            </div>
            <div v-for="item in serverInfo?.dnsServers" :key="item">
              {{ item }}
            </div>
            <p class="leading-2">&nbsp;</p>
            <div>
              {{ t("common.orCreateYourDomainWildcardTips") }}
            </div>
            <div>{{ serverInfo?.cName }}</div>
          </div>
        </el-alert>
      </div>
      <el-form-item prop="domainName" :label="t('common.domain')">
        <el-input
          v-model="model.domainName"
          data-cy="subdomain"
          @keydown.enter="handleCreate"
        />
      </el-form-item>
    </el-form>

    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.create')"
        @confirm="handleCreate"
        @cancel="handleClose"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { ref, watch } from "vue";
import type { ElForm } from "element-plus";
import type { UPDATE_MODEL_EVENT } from "@/constants/constants";
import useOperationDialog from "@/hooks/use-operation-dialog";
import { requiredRule } from "@/utils/validate";
import type { Rules } from "async-validator";
import type { ServerInfo } from "@/api/console/types";
import { postDomain, getServerInfo } from "@/api/console";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
import { useAppStore } from "@/store/app";
interface PropsType {
  modelValue: boolean;
}
interface EmitsType {
  (e: typeof UPDATE_MODEL_EVENT, value: boolean): void;
  (e: "create-success"): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const { visible, handleClose } = useOperationDialog(props, emits);
const appStore = useAppStore();
const form = ref<InstanceType<typeof ElForm>>();

const model = ref({
  domainName: "",
});
const serverInfo = ref<ServerInfo>();

const rules = {
  domainName: [requiredRule(t("common.domainRequiredTips"))],
} as Rules;

watch(
  () => visible.value,
  (val) => {
    if (val) {
      form.value?.resetFields();
      model.value.domainName = "";
      getServerInfo().then((res) => (serverInfo.value = res));
    }
  }
);

function handleCreate() {
  form.value?.validate(async (valid) => {
    if (valid) {
      await postDomain(model.value.domainName);
      handleClose();
      emits("create-success");
    }
  });
}
</script>

<style scoped>
:deep(.el-input-group__append) {
  width: 240px;
}

.el-select :deep(.el-input__suffix) {
  padding: 0;
  border: none;
}
:deep(.el-alert__description) {
  margin-top: 0;
}
</style>
