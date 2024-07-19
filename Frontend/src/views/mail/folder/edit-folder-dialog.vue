<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="model.oldName ? t('common.renameFolder') : t('common.newFolder')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      class="el-form--label-normal"
      :model="model"
      :rules="rules"
      label-position="top"
      @submit.prevent
    >
      <el-form-item prop="name" :label="t('common.name')">
        <el-input
          v-model="model.name"
          data-cy="folder-name"
          @keydown.enter="handleEdit"
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="model.oldName ? t('common.rename') : t('common.create')"
        @confirm="handleEdit"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { ref, onMounted } from "vue";
import type { ElForm } from "element-plus";

import { requiredRule, rangeRule, folderNameRule } from "@/utils/validate";
import type { Rules } from "async-validator";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
import { newEmailFolder, renameEmailFolder } from "@/api/mail";
import { useRoute } from "vue-router";
import { useEmailStore } from "@/store/email";

const route = useRoute();
const emailStore = useEmailStore();

interface PropsType {
  modelValue: boolean;
  currentFolder: string;
}
interface EmitsType {
  (e: "update:modelValue", value: boolean): void;
  (e: "reload", value?: string): void;
  (e: "create-success"): void;
}

const show = ref(true);
const props = defineProps<PropsType>();
const emit = defineEmits<EmitsType>();
const { t } = useI18n();

const form = ref<InstanceType<typeof ElForm>>();
const model = ref({ name: "", relativeName: "", oldName: "" });

const rules = {
  name: [
    requiredRule(t("common.inputFolderNameTips")),
    rangeRule(1, 50),
    folderNameRule,
  ],
} as Rules;

const updateActiveFolderMenu = (name: string) => {
  const qs = new URLSearchParams(location.search);
  qs.set("folderName", name);
  history.replaceState(
    null,
    "",
    location.pathname + "?" + decodeURIComponent(qs.toString())
  );
  emailStore.firstActiveMenu = "folder" + name;
};

const handleEdit = async () => {
  await form.value?.validate();
  let newFolderName = model.value.relativeName + model.value.name.trim();
  const urlParams = new URL(window.location.href).searchParams;
  const oldFolderName = urlParams.get("folderName");
  let currentActiveFolder = "";
  if (model.value.oldName) {
    await renameEmailFolder(model.value.oldName, newFolderName);
    // 改变当前路由以便正确的菜单激活
    if (oldFolderName === model.value.oldName) {
      // 对激活的文件夹菜单(el-menu-item)重命名时,更新url以便于保持激活
      await updateActiveFolderMenu(newFolderName);
    } else if (
      oldFolderName &&
      oldFolderName?.toString().split("/")[0] ===
        model.value.oldName.split("/")[0] &&
      oldFolderName.includes(model.value.oldName)
    ) {
      // 对激活的文件夹菜单的父文件夹(el-sub-menu)重命名时,更新url以便于保持激活
      currentActiveFolder = oldFolderName
        .toString()
        .replace(new RegExp(model.value.oldName, "g"), newFolderName);
      await updateActiveFolderMenu(currentActiveFolder);
    }
  } else {
    await newEmailFolder(model.value.name.trim());
  }
  emit("update:modelValue", false);
  emit("reload", currentActiveFolder);
};

onMounted(() => {
  model.value.oldName = props.currentFolder;
  var lastIndex = props.currentFolder.lastIndexOf("/");

  model.value.relativeName =
    lastIndex !== -1 ? props.currentFolder.substring(0, lastIndex + 1) : "";
  model.value.name =
    lastIndex !== -1
      ? props.currentFolder.substring(lastIndex + 1)
      : props.currentFolder;
});
</script>
