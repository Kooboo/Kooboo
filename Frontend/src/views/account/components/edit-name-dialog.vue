<template>
  <el-dialog
    :model-value="show"
    :title="t('common.editName')"
    width="400px"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      :model="model"
      :rules="rules"
      label-position="top"
      @submit.prevent
      @keypress.enter="change"
    >
      <el-form-item prop="firstName" :label="t('common.firstName')">
        <el-input
          v-model="model.firstName"
          @blur="
            model.firstName = model.firstName.replace(/(^\s*)|(\s*$)/g, '')
          "
        />
      </el-form-item>
      <el-form-item prop="lastName" :label="t('common.lastName')">
        <el-input
          v-model="model.lastName"
          @blur="model.lastName = model.lastName.replace(/(^\s*)|(\s*$)/g, '')"
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar @confirm="change" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
<script lang="ts" setup>
import { inject, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { Rules } from "async-validator";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { updateName } from "@/api/user";
import { requiredRule } from "@/utils/validate";
import type { Load } from "../profile.vue";

const props = defineProps<{
  modelValue: boolean;
  name: {
    firstName: string;
    lastName: string;
  };
}>();

const { t } = useI18n();
const show = ref(true);
const form = ref();
const rules = {
  firstName: requiredRule(t("common.fieldRequiredTips")),
  lastName: requiredRule(t("common.fieldRequiredTips")),
} as Rules;
const model = ref<{ firstName: string; lastName: string }>({
  firstName: "",
  lastName: "",
});

const load = () => {
  model.value.firstName = props.name.firstName;
  model.value.lastName = props.name.lastName;
};
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const change = async () => {
  await form.value.validate();
  await updateName(model.value.firstName, model.value.lastName);
  reloadUser();
  show.value = false;
};
load();
const reloadUser = inject("reloadUser") as Load;
</script>
