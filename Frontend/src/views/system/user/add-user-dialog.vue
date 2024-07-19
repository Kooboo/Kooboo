<script lang="ts" setup>
import { ref } from "vue";
import type { Rules } from "async-validator";
import type { User } from "@/api/site/user";
import { getAvailableUsers, getRoles, addUser } from "@/api/site/user";
import { requiredRule } from "@/utils/validate";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

defineProps<{ modelValue: boolean }>();
const { t } = useI18n();
const users = ref<User[]>([]);
const roles = ref<string[]>([]);

getAvailableUsers().then((rsp) => {
  users.value = rsp;
  if (users.value.length) model.value.userId = users.value[0].userId;
});

getRoles().then((rsp) => {
  roles.value = rsp;
  if (roles.value.length) model.value.role = roles.value[0];
});

const rules = {
  userId: requiredRule(t("common.userRequired")),
  role: requiredRule(t("common.roleRequiredTips")),
} as Rules;

const show = ref(true);
const form = ref();
const model = ref({
  userId: "",
  role: "",
});

const onSave = async () => {
  await form.value.validate();
  await addUser(model.value);
  show.value = false;
  emit("reload");
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.addUser')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form ref="form" label-position="top" :model="model" :rules="rules">
      <el-form-item :label="t('common.availableUser')" prop="userId">
        <el-select v-model="model.userId" class="w-full" data-cy="users">
          <el-option
            v-for="item of users"
            :key="item.userId"
            :value="item.userId"
            :label="`${item.userName}<${item.email}>`"
            data-cy="user-opt"
          />
        </el-select>
      </el-form-item>
      <el-form-item :label="t('common.role')" prop="role">
        <el-select v-model="model.role" class="w-full" data-cy="roles">
          <el-option
            v-for="item of roles"
            :key="item"
            :value="item"
            :label="item"
            data-cy="role-opt"
          />
        </el-select>
      </el-form-item>
    </el-form>

    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.add')"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
