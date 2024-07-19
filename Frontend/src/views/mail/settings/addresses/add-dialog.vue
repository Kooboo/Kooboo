<template>
  <el-dialog
    v-model="visible"
    width="600px"
    :close-on-click-modal="false"
    :title="props.address ? editAddressTitle[type] : newAddressTitle[type]"
    @closed="emits('update:modelValue', false)"
  >
    <el-form
      ref="form"
      class="el-form--label-normal"
      :model="model"
      :rules="rules"
      label-position="top"
      @submit.prevent
      @keydown.enter="handleConfirm"
    >
      <div class="mb-16">
        <el-alert
          v-if="type === 'Wildcard'"
          :title="t('common.information')"
          :closable="false"
          class="dark:bg-444"
          :description="t('common.tipsWildcard')"
          type="info"
        />
      </div>
      <el-form-item prop="name" :label="t('common.name')">
        <el-input
          v-model="model.name"
          style="width: 300px"
          @blur="model.name = model.name?.replace(/(^\s*)|(\s*$)/g, '')"
        />
      </el-form-item>
      <div class="flex items-center space-x-8">
        <el-form-item prop="local" :label="label[type]" class="flex-1">
          <el-input
            v-model="model.local"
            class="w-full"
            :disabled="props.address ? true : false"
          />
        </el-form-item>

        <el-form-item prop="name" label="&nbsp;">
          <el-select
            v-model="model.domain"
            style="width: 240px"
            :disabled="props.address ? true : false"
          >
            <el-option
              v-for="item in emailStore.domainList"
              :key="item.id"
              :value="item.domainName"
              :label="'@' + item.domainName"
            />
          </el-select>
        </el-form-item>
      </div>

      <el-form-item
        v-if="type === 'Forward'"
        prop="forwardAddress"
        :label="t('common.forwardAddress')"
      >
        <el-input v-model="model.forwardAddress" />
      </el-form-item>

      <el-alert
        v-if="appStore.header?.isOnlineServer"
        :closable="false"
        type="info"
        class="dark:bg-444 text-666 dark:text-gray"
        :title="t('common.emailAreEnabledForDomainsThatUseOurDNSServer')"
      />
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="props.address ? t('common.save') : t('common.create')"
        @confirm="handleConfirm"
        @cancel="visible = false"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { computed, ref, watch } from "vue";
import type { Address, AddressType, PostAddress } from "@/api/mail/types";
import { postAddress, updateForward, updateName } from "@/api/mail";
import type { ElForm } from "element-plus";
import type { UPDATE_MODEL_EVENT } from "@/constants/constants";
import { useEmailStore } from "@/store/email";
import { useAppStore } from "@/store/app";
import {
  addressIsUniqueNameRule,
  emailAddressLengthRule,
  emailRangeRule,
  emailRule,
  frontEmailRule,
  rangeRule,
  requiredRule,
  wildcardEmailRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
const emailStore = useEmailStore();
interface PropsType {
  modelValue: boolean;
  type: AddressType;
  address?: Address;
}
interface EmitsType {
  (e: typeof UPDATE_MODEL_EVENT, value: boolean): void;
  (e: "create-success"): void;
  (e: "update:modelValue", value: boolean): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const visible = ref(true);
const appStore = useAppStore();

const form = ref<InstanceType<typeof ElForm>>();
let model = ref({} as PostAddress);
const type = computed(() => {
  return props.address?.addressType || props.type;
});

const newAddressTitle = ref({
  Normal: t("common.newGeneralAddress"),
  Wildcard: t("common.newWildcardAddress"),
  Group: t("common.newGroup"),
  Forward: t("common.newForwardingAddress"),
});

const editAddressTitle = ref({
  Normal: t("common.editGeneralAddress"),
  Wildcard: t("common.editWildcardAddress"),
  Group: t("common.editGroup"),
  Forward: t("common.editForwardingAddress"),
});

const label = ref({
  Normal: t("common.address"),
  Wildcard: t("common.wildcardAddress"),
  Group: t("common.group"),
  Forward: t("common.agentAddress"),
});

const rules = computed(() => {
  return {
    name: [rangeRule(1, 64)],
    local: !props.address
      ? [
          requiredRule(t("common.pleaseEnterAddress")),
          props.type === "Wildcard" ? wildcardEmailRule : frontEmailRule,
          addressIsUniqueNameRule(model.value),
          props.type === "Wildcard"
            ? model.value.local?.includes("*")
              ? emailAddressLengthRule(model.value, 150)
              : emailAddressLengthRule(model.value, 149)
            : emailAddressLengthRule(model.value, 150),
          props.type === "Wildcard"
            ? model.value.local?.includes("*")
              ? emailRangeRule(64)
              : emailRangeRule(63)
            : emailRangeRule(64),
        ]
      : [],
    forwardAddress: [
      requiredRule(t("common.pleaseEnterAddress")),
      emailRule,
      emailRangeRule(150),
    ],
  } as Rules;
});

watch(
  () => visible.value,
  (val) => val && form.value?.resetFields()
);

async function handleConfirm() {
  await form.value?.validate();
  if (props.address) {
    await updateName(props.address.id, model.value.name);
    if (model.value.forwardAddress && !model.value.addressType) {
      await updateForward(props.address.id, model.value.forwardAddress);
    }
  } else {
    await postAddress(model.value);
  }

  visible.value = false;
  emits("create-success");
}
const load = async () => {
  if (props.address) {
    let index = props.address?.address.indexOf("@");
    model.value.name = props.address.name;
    model.value.local = props.address.address.substring(0, index);
    model.value.domain = props.address.address.substring(index + 1);
    //props.address.forwardAddress存在代表这是编辑forwardAddress弹框
    if (props.address.forwardAddress) {
      model.value.forwardAddress = props.address?.forwardAddress;
    }
  } else if (props.type) {
    await emailStore.loadDomains();
    model.value.addressType = props.type;
    model.value.domain = emailStore.domainList[0]?.domainName || "";
  }
};
load();
</script>

<style scoped>
:deep(.el-input-group__append) {
  width: 240px;
}

.el-select :deep(.el-input__suffix) {
  padding: 0;
  border: none;
}
</style>
