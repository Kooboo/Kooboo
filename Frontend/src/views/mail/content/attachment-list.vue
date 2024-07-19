<script lang="ts" setup>
import type { Attachment } from "@/api/mail/types";
import { openInNewTab } from "@/utils/url";
import { useI18n } from "vue-i18n";
import { bytesToSize } from "@/utils/common";
import { useEmailStore } from "@/store/email";

const { t } = useI18n();

const props = defineProps<{
  attachments: Attachment[];
  downloadAllAttachmentPath: string;
}>();
const emailStore = useEmailStore();

function download(
  attachment: { downloadUrl: string },
  forceDownload?: boolean
) {
  openInNewTab(
    attachment.downloadUrl + (forceDownload ? "?forceDownload=true" : "")
  );
}
function downloadAll() {
  openInNewTab(props.downloadAllAttachmentPath);
}
</script>
<template>
  <div class="w-full">
    <div class="w-full bg-gray h-1px dark:bg-[#555]" />
    <div class="my-16 text-999 flex justify-between">
      <span
        >{{ t("common.attachments") }}:
        {{ t("mail.quantity", { total: attachments.length }) }}</span
      >
      <span
        class="!hover:text-blue cursor-pointer text-999"
        @click="downloadAll()"
      >
        <el-icon
          v-if="attachments?.length"
          class="mr-4 iconfont icon-xiazai-wenjianxiazai-05 font-bold"
          data-cy="download"
        />{{ t("common.downloadAll") }}</span
      >
    </div>
    <div class="flex flex-wrap">
      <div
        v-for="attachment in attachments"
        :key="attachment.fileName"
        class="group cursor-pointer border border-gray dark:border-[#555] dark:text-fff/86 hover:bg-blue/10 rounded-normal p-12 flex items-center justify-between mr-8 mb-8"
        :title="attachment.fileName"
        @click="download(attachment)"
      >
        <div class="flex">
          <img
            :src="emailStore.getAttachmentType(attachment.subType!.toLowerCase())"
            class="mr-8 w-10 h-10"
          />
          <div class="ellipsis w-140px">
            <div
              class="ellipsis"
              :title="attachment.fileName"
              data-cy="attachment"
            >
              {{ attachment.fileName }}
            </div>
            <div class="leading-4 text-999">
              {{ bytesToSize(attachment.size) }}
            </div>
          </div>
        </div>
        <el-icon
          class="opacity-0 group-hover:opacity-100 ml-8 iconfont icon-xiazai-wenjianxiazai-05 cursor-pointer hover:text-blue font-bold text-999"
          data-cy="download"
          :title="t('common.download')"
          @click.stop="download(attachment, true)"
        />
      </div>
    </div>
  </div>
</template>
