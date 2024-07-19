<template>
  <el-dialog
    v-model="visible"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.moveSiteToFolder')"
    @close="handleClose"
  >
    <el-form ref="form" class="el-form--label-normal" label-position="top">
      <el-form-item :label="t('common.moveSiteToThisFolder')">
        <el-select
          v-model="model.folderName"
          :placeholder="t('common.rootFolder')"
          filterable
          clearable
          data-cy="move-to-folder"
        >
          <el-option
            v-for="item in folderOptions"
            :key="item.key"
            :value="item.key"
            data-cy="folder-name-opt"
          />
        </el-select>
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.move')"
        @confirm="handleMove"
        @cancel="handleClose"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { computed, reactive, ref, watch } from "vue";
import type { ElForm } from "element-plus";
import { ElMessage } from "element-plus";
import useOperationDialog from "@/hooks/use-operation-dialog";
import type { UPDATE_MODEL_EVENT } from "@/constants/constants";
import { setSitesFolder } from "@/api/site";
import type { FolderItem, SiteItem } from "../type";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
interface PropsType {
  modelValue: boolean;
  folders: FolderItem[];
  sites: SiteItem[];
  currentFolder?: FolderItem | null;
}
interface EmitsType {
  (e: typeof UPDATE_MODEL_EVENT, value: boolean): void;
  (e: "move-success"): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const { visible, handleClose } = useOperationDialog(props, emits);

const form = ref<InstanceType<typeof ElForm>>();
const model = reactive({ folderName: "" });
const folderOptions = computed(() => {
  if (props.currentFolder) {
    return props.folders.filter((x) => x.key !== props.currentFolder?.key);
  }
  return props.folders;
});
watch(
  () => visible.value,
  (val) => {
    if (val) {
      // form.value?.resetFields()
      model.folderName = "";
    }
  }
);
async function handleMove() {
  const siteIds = props.sites.map((x) => x.siteId);
  await setSitesFolder({
    siteIds,
    folderName: model.folderName,
  });
  ElMessage.success(t("common.moveSuccess"));
  handleClose();
  emits("move-success");
}
</script>
<style lang="scss" scoped>
.el-select {
  width: 100%;
}
</style>
