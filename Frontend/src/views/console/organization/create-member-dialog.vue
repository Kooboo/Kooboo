<template>
  <el-dialog
    :model-value="show"
    width="670px"
    :title="t('common.create')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      class="el-form--label-normal"
      :model="model"
      :rules="(rules as any)"
      label-position="left"
      label-width="150px"
      @submit.prevent
    >
      <el-form-item prop="userName" :label="t('common.username')">
        <el-input v-model="model.userName" class="w-400px" />
      </el-form-item>
      <el-form-item prop="password" :label="t('common.password')">
        <el-input v-model="model.password" class="w-400px" type="password" />
      </el-form-item>
      <el-form-item prop="email" :label="t('common.email')">
        <el-input v-model="model.email" class="w-400px" />
      </el-form-item>
      <el-form-item prop="phone" :label="t('common.phone')">
        <el-input v-model="model.phone" class="w-400px" />
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar @confirm="onCreate" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
<script lang="ts" setup>
import { createOrgUser } from "@/api/organization";
import {
  commerceValueRangeRule,
  emailRule,
  passwordRule,
  phoneRule,
  requiredRule,
} from "@/utils/validate";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

defineProps<{
  modelValue: boolean;
}>();
const { t } = useI18n();
const show = ref(true);

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "create-success"): void;
}>();

const model = ref({
  userName: "",
  password: "",
  email: "",
  phone: "",
});

const form = ref();
const rules = {
  userName: [
    requiredRule(t("common.fieldRequiredTips")),
    commerceValueRangeRule(t("common.username"), 2, 30),
  ],

  password: [
    passwordRule,
    requiredRule(t("common.inputPasswordTips")),
    commerceValueRangeRule(t("common.password"), 2, 30),
  ],
  email: [emailRule, requiredRule(t("common.inputEmailTips"))],
  phone: [phoneRule],
};

const onCreate = async () => {
  await form.value.validate();
  await createOrgUser(model.value);
  show.value = false;
  emit("create-success");
};
</script>
