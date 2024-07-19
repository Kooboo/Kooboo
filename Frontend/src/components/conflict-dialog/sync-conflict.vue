<script lang="ts" setup>
import DiffView from "@/components/monaco-editor/diff.vue";
import { useI18n } from "vue-i18n";
import { computed, ref } from "vue";
import type { PushFeedBack, PullFeedBack } from "@/api/publish/types";
import { forcePush, forcePull } from "@/api/publish";
import { close, syncSettingId } from "./sync-conflict";
import { useTime } from "@/hooks/use-date";

const show = ref(true);
const emit = defineEmits<{
  (e: "update:model-value", value: boolean): void;
}>();

const props = defineProps<{
  data: PushFeedBack | PullFeedBack;
  modelValue: boolean;
}>();

const isPush = computed(() => {
  return "remoteVersion" in props.data;
});

const { t } = useI18n();

const useRemote = async () => {
  if (isPush.value) {
    await forcePush(syncSettingId.value!, (props.data as any).siteLogId, true);
  } else {
    await forcePull(
      syncSettingId.value!,
      (props.data as any).currentSenderVersion,
      (props.data as any).senderVersion,
      (props.data as any).localLogId,
      false
    );
  }

  close(true);
};

const useLocal = async () => {
  if (isPush.value) {
    await forcePush(syncSettingId.value!, (props.data as any).siteLogId, false);
  } else {
    await forcePull(
      syncSettingId.value!,
      (props.data as any).currentSenderVersion,
      (props.data as any).senderVersion,
      (props.data as any).localLogId,
      true
    );
  }
  close(true);
};
</script>

<template>
  <el-dialog
    v-model="show"
    width="1200px"
    :close-on-click-modal="false"
    :title="t('common.versionConflict')"
    destroy-on-close
    @closed="close(false)"
  >
    <div class="flex w-full">
      <div class="flex-1 flex gap-4 justify-center">
        <span>{{ data.remoteUserName }}</span>
        <span>{{ useTime(data.remoteTime) }}</span>
        <span>{{
          t("conflict.remote", {
            version: (data as any).remoteVersion || (data as any).senderVersion,
          })
        }}</span>
      </div>
      <div class="flex-1 flex gap-4 justify-center">
        <span>{{ data.localUserName }}</span>
        <span>{{ useTime(data.localTime) }}</span>
        <span
          >{{ t("conflict.localReadOnly", { version: (data as any).siteLogId|| (data as any).localLogId}) }}</span
        >
      </div>
    </div>

    <template v-if="data.hasConflict">
      <div v-if="data.isImage" class="flex p-8 gap-12">
        <div
          class="w-1/2 flex justify-center items-center rounded-normal border border-line"
        >
          <img :src="`data:image/png;base64, ${data.remoteBody}`" />
        </div>
        <div
          class="w-1/2 flex justify-center items-center rounded-normal border border-line"
        >
          <img :src="`data:image/png;base64, ${data.localBody}`" />
        </div>
      </div>
      <DiffView
        v-else
        v-model:modified="data.localBody"
        class="w-full h-400px"
        :original="data.remoteBody"
      />
    </template>

    <template #footer>
      <el-button round @click="useRemote">
        {{
          isPush
            ? t("conflict.keepRemoteVersion")
            : t("conflict.useRemoteVersion")
        }}
      </el-button>
      <el-button type="primary" round @click="useLocal">
        {{
          isPush
            ? t("conflict.useLocalVersion")
            : t("conflict.keepLocalVersion")
        }}
      </el-button>
    </template>
  </el-dialog>
</template>
