<template>
  <el-dialog
    v-model="show"
    width="400px"
    :title="t('common.addMember')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      :modal="newUser"
      :rules="newUserRules"
      label-position="top"
      :model="newUser"
      @submit.prevent
    >
      <el-form-item prop="userName" :label="t('common.username')">
        <el-input v-model="newUser.userName" @keydown.enter="handleAddUser" />
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.add')"
        @confirm="handleAddUser"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
<script lang="ts" setup>
import { addUser } from "@/api/organization";
import { requiredRule, simpleNameRule } from "@/utils/validate";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useAppStore } from "@/store/app";

const appStore = useAppStore();

const { t } = useI18n();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "addSuccess"): void;
}>();

defineProps<{ modelValue: boolean }>();
const form = ref();
const newUser = ref({
  userName: "",
  organizationId: appStore.currentOrg!.id,
});
const show = ref(true);
const newUserRules = ref({
  userName: [requiredRule(t("common.fieldRequiredTips")), simpleNameRule()],
});

const handleAddUser = async () => {
  await form.value.validate();
  await addUser(newUser.value);
  show.value = false;
  newUser.value.userName = "";
  emit("addSuccess");
};
</script>
