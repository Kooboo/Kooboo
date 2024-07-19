<script lang="ts" setup>
import { ref } from "vue";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { useI18n } from "vue-i18n";
import { getProgress, setProgress } from "@/api/publish";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const props = defineProps<{ modelValue: boolean; id: string }>();
const { t } = useI18n();
const show = ref(true);
const originSyncProgress = ref<any>();
const syncProgress = ref<any>();

getProgress(props.id).then((rsp) => {
  originSyncProgress.value = JSON.parse(JSON.stringify(rsp));
  syncProgress.value = rsp;
});

const onSave = async () => {
  await setProgress(props.id, syncProgress.value);
  emit("reload");
  show.value = false;
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.syncProgress')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form v-if="syncProgress" label-position="top">
      <el-form-item
        :label="`${t('common.localProgress')} (${t('common.localLastId')}: ${
          syncProgress.localLastId
        })`"
      >
        <div class="flex items-center w-full space-x-16">
          <el-input-number
            v-model="syncProgress.localProgress"
            class="w-150px"
            :disabled="syncProgress.localLastId == 0"
            :step="1"
            step-strictly
          />
          <el-slider
            v-model="syncProgress.localProgress"
            class="flex-1"
            :min="0"
            :max="syncProgress.localLastId"
            :disabled="syncProgress.localLastId == 0"
          />
        </div>
      </el-form-item>
      <el-form-item
        :label="`${t('common.remoteProgress')} (${t('common.remoteLastId')}: ${
          syncProgress.remoteLastId
        })`"
      >
        <div class="flex items-center w-full space-x-16">
          <el-input-number
            v-model="syncProgress.remoteProgress"
            class="w-150px"
            :disabled="syncProgress.remoteLastId == 0"
            :step="1"
            step-strictly
          />
          <el-slider
            v-model="syncProgress.remoteProgress"
            :min="0"
            class="flex-1"
            :max="syncProgress.remoteLastId"
            :disabled="syncProgress.remoteLastId == 0"
          />
        </div>
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
