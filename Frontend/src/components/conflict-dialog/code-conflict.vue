<script lang="ts" setup>
import { useDiffStore } from "@/store/diff";
import DiffView from "@/components/monaco-editor/diff.vue";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import { showForceSaveConfirm } from "../basic/confirm";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

const diffStore = useDiffStore();
const show = ref(true);

const { t } = useI18n();

const save = () => {
  diffStore.action = "merge";
  diffStore.model.version = diffStore.conflict?.version;
  show.value = false;
};

const forceSave = async () => {
  await showForceSaveConfirm();
  diffStore.action = "force";
  diffStore.model.enableDiffChecker = false;
  show.value = false;
};
</script>

<template>
  <el-dialog
    v-model="show"
    width="1200px"
    :close-on-click-modal="false"
    :title="t('common.versionConflict')"
    destroy-on-close
    @closed="diffStore.closeDialog()"
  >
    <div class="flex w-full">
      <div class="flex-1 text-center">
        {{ t("conflict.remote", { version: diffStore.conflict?.version }) }}
      </div>
      <div class="flex-1 text-center">
        {{ t("conflict.local", { version: diffStore.model?.version }) }}
      </div>
    </div>

    <DiffView
      v-if="diffStore.conflict"
      v-model:modified="diffStore.model.body"
      class="w-full h-400px"
      :original="diffStore.conflict.body"
      editable
    />

    <template #footer>
      <DialogFooterBar @confirm="save" @cancel="show = false">
        <template #extra-buttons>
          <el-button type="danger" round @click="forceSave">
            {{ t("common.forceSave") }}
          </el-button>
        </template>
      </DialogFooterBar>
    </template>
  </el-dialog>
</template>
