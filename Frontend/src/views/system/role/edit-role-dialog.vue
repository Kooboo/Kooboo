<script lang="ts" setup>
import { ref } from "vue";
import type { Rules } from "async-validator";
import { getEdit, post } from "@/api/role";
import type { Role } from "@/api/role/types";
import { requiredRule, isUniqueNameRule, rangeRule } from "@/utils/validate";

import { useI18n } from "vue-i18n";
import { isUniqueName } from "@/api/site/role";
import PermissionPanel from "./permission-panel.vue";
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const props = defineProps<{ modelValue: boolean; name: string }>();
const { t } = useI18n();
const show = ref(true);
const form = ref();
const model = ref<Role>();

getEdit(props.name).then((rsp) => (model.value = rsp));

const rules = {
  name: props.name
    ? []
    : [
        requiredRule(t("common.roleRequiredTips")),
        isUniqueNameRule(isUniqueName, t("common.roleExists")),
        rangeRule(1, 50),
      ],
} as Rules;

const onSave = async () => {
  await form.value.validate();
  const body = { ...model.value };
  await post(body);
  show.value = false;
  emit("reload");
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="800px"
    :close-on-click-modal="false"
    :title="name ? t('common.editRole') : t('common.addRole')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      v-if="model"
      ref="form"
      label-position="top"
      :model="model"
      :rules="rules"
      @submit.prevent
      @keydown.enter="onSave"
    >
      <el-form-item :label="t('common.roleName')" prop="name">
        <el-input
          v-model="model.name"
          :disabled="name ? true : false"
          data-cy="role-name"
        />
      </el-form-item>
      <el-form-item :label="t('common.permission')" prop="subItems">
        <PermissionPanel :list="model.permissions" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button round data-cy="cancel" @click="show = false">{{
        t("common.cancel")
      }}</el-button>
      <el-button
        v-hasPermission="{ feature: 'role', action: 'edit' }"
        type="primary"
        round
        data-cy="save"
        @click="onSave"
      >
        {{ t("common.save") }}
      </el-button>
    </template>
  </el-dialog>
</template>
