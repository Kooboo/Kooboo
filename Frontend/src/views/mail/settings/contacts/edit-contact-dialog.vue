<template>
  <el-dialog
    v-model="show"
    width="400px"
    :title="
      props.currentContact.id ? t('common.editContact') : t('common.addContact')
    "
    :close-on-click-modal="false"
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
      <el-form-item prop="name" :label="t('common.name')">
        <el-input v-model="model.name" />
      </el-form-item>
      <el-form-item prop="address" :label="t('common.address')">
        <el-input v-model="model.address" />
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="
          props.currentContact.id ? t('common.save') : t('common.add')
        "
        @confirm="handleConfirm"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { computed, ref, watch } from "vue";
import type { Contact } from "@/api/mail/types";
import { addContactApi, updateContactApi } from "@/api/mail";

import {
  emailRangeRule,
  emailRule,
  rangeRule,
  requiredRule,
  contactUniqueAddressRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";

const props = defineProps<{
  modelValue: boolean;
  currentContact: Contact;
  contactList: Contact[];
}>();
const emits = defineEmits<{
  (e: "reload"): void;
  (e: "update:modelValue", value: boolean): void;
}>();
const { t } = useI18n();
const show = ref(true);

const form = ref();
let model = ref({} as Contact);

const rules = computed(() => {
  return {
    name: [rangeRule(1, 64), requiredRule(t("common.nameRequiredTips"))],
    address: [
      requiredRule(t("common.pleaseEnterAddress")),
      emailRule,
      emailRangeRule(64),
      !props.currentContact.id
        ? contactUniqueAddressRule(
            props.contactList.map((m) => m.address),
            t("common.emailAddressExistsTips")
          )
        : "",
    ],
  } as Rules;
});
const handleConfirm = async () => {
  await form.value.validate();
  if (props.currentContact.id) {
    await updateContactApi(model.value);
  } else {
    await addContactApi({
      name: model.value.name,
      address: model.value.address,
    });
  }
  show.value = false;
  emits("reload");
};
const load = () => {
  model.value = props.currentContact;
};
load();
</script>
