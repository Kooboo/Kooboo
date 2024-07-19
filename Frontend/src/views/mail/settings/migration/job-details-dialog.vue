<template>
  <el-dialog
    v-model="show"
    width="500px"
    :title="
      model?.id ? t('common.editMigrationJob') : t('common.addMigrationJob')
    "
    :close-on-click-modal="false"
    @closed="emits('update:modelValue', false)"
  >
    <el-scrollbar class="pr-12 h-50vh">
      <el-form
        ref="form"
        class="el-form--label-normal"
        :model="model"
        :rules="rules"
        label-position="top"
        @submit.prevent
        @keydown.enter="handleConfirm"
      >
        <el-form-item prop="addressId" :label="t('common.koobooAddress')">
          <el-select
            v-model="model.addressId"
            class="w-full"
            :placeholder="t('common.selectAKoobooAddressToStoreMigratedMails')"
          >
            <el-option
              v-for="address in addressList"
              :key="address.id"
              :label="address.address"
              :value="address.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item prop="name" :label="t('common.jobName')">
          <el-input v-model="model.name" />
        </el-form-item>
        <el-form-item prop="emailAddress" :label="t('common.account')">
          <el-input v-model="model.emailAddress" @change="onEmailChanged" />
        </el-form-item>

        <el-form-item prop="password">
          <template #label>
            <span class="mr-8">
              {{ t("common.password") }}
            </span>
            <el-tooltip
              v-if="model?.id"
              class="box-item"
              :content="t('common.migrationPasswordTips')"
              placement="top"
            >
              <el-icon class="iconfont icon-problem mr-4" />
            </el-tooltip>
          </template>
          <el-input v-model="model.password" type="password" />
        </el-form-item>
        <el-form-item prop="host" :label="t('common.imapServer')">
          <el-input v-model="model.host" @change="hostChange" />
        </el-form-item>
        <el-form-item prop="forceSSL" label="SSL">
          <el-switch v-model="model.forceSSL" @change="forceSSLChange" />
        </el-form-item>
        <el-form-item prop="port" :label="t('common.port')">
          <el-input-number v-model="model.port" />
        </el-form-item>
      </el-form>
    </el-scrollbar>
    <template #footer>
      <DialogFooterBar
        :confirm-label="model?.id ? t('common.save') : t('common.add')"
        @confirm="handleConfirm"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { computed, ref } from "vue";
import { addEmailMigrationJob, updateEmailMigrationJob } from "@/api/mail";

import {
  emailRangeRule,
  emailRule,
  portRule,
  putIntegerNumberRule,
  rangeRule,
  requiredRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
import type { EmailMigration } from "@/api/mail/types";
import { useEmailStore } from "@/store/email";
const { addressList } = useEmailStore();

const model = ref<EmailMigration>({
  id: "",
  addressId: undefined,
  name: "",
  emailAddress: "",
  host: "",
  forceSSL: true,
  port: 993,
  password: "",
});

const hostUpdated = ref(false);
function hostChange() {
  hostUpdated.value = true;
}

function forceSSLChange(forceSSL: string | number | boolean) {
  model.value.port = forceSSL ? 993 : 143;
}

function onEmailChanged(email: string) {
  if (email && email.includes("@") && !hostUpdated.value) {
    const host = email.split("@")[1];
    if (host) {
      model.value.host = `imap.${host}`;
    }
  }
}

const props = defineProps<{
  modelValue: boolean;
  currentJob: EmailMigration;
}>();
const emits = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();
const { t } = useI18n();
const show = ref(true);

const form = ref();

const rules = computed(() => {
  return {
    name: [rangeRule(1, 64), requiredRule(t("common.nameRequiredTips"))],
    addressId: [
      {
        required: true,
        message: t("common.valueRequiredTips"),
        trigger: "change",
      },
    ],
    emailAddress: [
      requiredRule(t("common.pleaseEnterAddress")),
      emailRule,
      emailRangeRule(64),
    ],
    password: model.value?.id
      ? [rangeRule(1, 64)]
      : [rangeRule(1, 64), requiredRule(t("common.inputPasswordTips"))],
    host: [rangeRule(1, 64), requiredRule(t("common.valueRequiredTips"))],
    port: [portRule(), putIntegerNumberRule()],
  } as Rules;
});
const handleConfirm = async () => {
  await form.value.validate();
  if (model.value.id) {
    await updateEmailMigrationJob(model.value.id, model.value);
  } else {
    await addEmailMigrationJob(model.value);
  }
  emits("reload");
  show.value = false;
};

async function load() {
  model.value = JSON.parse(JSON.stringify(props.currentJob));
}

load();
</script>
