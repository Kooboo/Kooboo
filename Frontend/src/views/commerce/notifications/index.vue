<script lang="ts" setup>
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { useShortcut } from "@/hooks/use-shortcuts";
import { ref, watch } from "vue";
import {
  saveSettings,
  getEmailEvents,
  getWebhookEvents,
} from "@/api/commerce/settings";
import type {
  EmailEvent,
  Settings,
  WebhookEvent,
} from "@/api/commerce/settings";
import { useCommerceStore } from "@/store/commerce";
import { refreshMonacoCache } from "@/components/monaco-editor/monaco";
import { useEmailStore } from "@/store/email";
import EmailNotificationEditor from "./email-notification-editor.vue";
import SmtpSettingsDialog from "./smtp-settings-dialog.vue";
import WebhookEditor from "./webhook-editor.vue";
import EmailLogDialog from "./email-log-dialog.vue";
import WebhookLogDialog from "./webhook-log-dialog.vue";

const { t } = useI18n();
const store = useCommerceStore();
const settings = ref<Settings>();
const emailEvents = ref<EmailEvent[]>([]);
const webhookEvents = ref<WebhookEvent[]>([]);
const emailStore = useEmailStore();
const showSmtpSettingsDialog = ref(false);
const showEmailLogDialog = ref(false);
const showWebhookLogDialog = ref(false);

getEmailEvents().then((rsp) => (emailEvents.value = rsp));
getWebhookEvents().then((rsp) => (webhookEvents.value = rsp));

const mailServers = [
  {
    key: "kooboo",
    value: "Kooboo",
  },
  {
    key: "custom",
    value: t("common.custom"),
  },
];

const onSave = async () => {
  await saveSettings(settings.value!);
  store.loadSettings();
  refreshMonacoCache();
};

watch(
  () => store.settings,
  () => {
    settings.value = JSON.parse(JSON.stringify(store.settings));
  },
  {
    immediate: true,
  }
);

watch(
  () => settings.value?.mailServerType,
  () => {
    if (settings.value?.mailServerType == "kooboo") {
      emailStore.loadAddress("addresses");
    }
  },
  {
    immediate: true,
  }
);

useShortcut("save", onSave);
</script>

<template>
  <div class="p-24 pb-150px">
    <Breadcrumb :name="t('commerce.notification')" />
    <ElForm v-if="settings" label-position="top">
      <div
        class="rounded-normal bg-fff dark:bg-[#252526] mt-16 mb-24 py-24 px-56px relative"
      >
        <div class="max-w-504px">
          <el-form-item :label="t('common.enableEmailNotification')">
            <ElSwitch v-model="settings.enableEmailNotification" />
          </el-form-item>

          <el-form-item :label="t('common.mailServer')">
            <el-radio-group
              v-model="settings.mailServerType"
              class="el-radio-group--rounded"
            >
              <el-radio-button
                v-for="item of mailServers"
                :key="item.key"
                :label="item.key"
                >{{ item.value }}</el-radio-button
              >
            </el-radio-group>
          </el-form-item>
          <div
            v-if="settings.mailServerType == 'custom'"
            class="flex items-center space-x-8 mb-12"
          >
            <el-button
              type="primary"
              size="small"
              round
              @click="showSmtpSettingsDialog = true"
              >{{ t("common.setServerInfo") }}</el-button
            >
            <span class="text-s text-999">{{
              settings.customMailServer?.server
            }}</span>
            <span class="text-s text-999">{{
              settings.customMailServer?.port
            }}</span>
            <span class="text-s text-999">{{
              settings.customMailServer?.userName
            }}</span>
          </div>
          <el-form-item v-else>
            <template #label>
              <div class="inline-flex items-center space-x-4">
                <div>{{ t("common.fromAddress") }}</div>
                <Tooltip
                  :tip="t('common.koobooEmailAddressTip')"
                  custom-class="ml-4"
                />
              </div>
            </template>
            <el-select
              v-model="settings.koobooEmailAddress"
              :placeholder="t('common.pleaseSelect')"
              class="w-250px"
            >
              <el-option
                v-for="opt in emailStore.addressList.filter(
                  (f) => f.addressType !== 'Wildcard'
                )"
                :key="opt.id"
                :value="opt.address"
                :label="opt.address"
              />
            </el-select>
          </el-form-item>
          <el-form-item :label="t('common.event')">
            <EmailNotificationEditor
              :items="settings.emailNotifications"
              :events="emailEvents"
            />
          </el-form-item>
        </div>
        <el-button
          round
          size="small"
          type="primary"
          class="absolute top-24 right-32"
          plain
          @click="showEmailLogDialog = true"
          >{{ t("common.logs") }}</el-button
        >
        <SmtpSettingsDialog
          v-if="showSmtpSettingsDialog"
          v-model="showSmtpSettingsDialog"
          :model="settings.customMailServer"
          @success="settings!.customMailServer = $event"
        />
      </div>

      <div
        v-if="settings"
        class="rounded-normal bg-fff dark:bg-[#252526] mt-16 mb-24 py-24 px-56px relative"
      >
        <div class="max-w-504px">
          <el-form-item :label="t('common.enableWebhook')">
            <ElSwitch v-model="settings.enableWebhook" />
          </el-form-item>
          <el-form-item>
            <template #label>
              <div class="flex items-center">
                <span>Webhooks</span>
                <el-popover placement="top-start" width="600">
                  <template #reference>
                    <el-icon
                      class="ml-4 iconfont icon-problem"
                      @click.prevent
                    />
                  </template>
                  <el-descriptions
                    :title="t('common.webhookDescription')"
                    :column="1"
                    border
                  >
                    <el-descriptions-item label="secret">{{
                      settings.webhookSecret
                    }}</el-descriptions-item>
                    <el-descriptions-item label="Verification"
                      >SHA256Hash(request.body+secret)==
                      request.header["X-Kooboo-Hmac-SHA256"]</el-descriptions-item
                    >
                  </el-descriptions>
                </el-popover>
              </div>
            </template>
            <WebhookEditor :events="webhookEvents" :items="settings.webhooks" />
          </el-form-item>
        </div>
        <el-button
          round
          size="small"
          type="primary"
          class="absolute top-24 right-32"
          plain
          @click="showWebhookLogDialog = true"
          >{{ t("common.logs") }}</el-button
        >
      </div>
    </ElForm>
    <EmailLogDialog v-if="showEmailLogDialog" v-model="showEmailLogDialog" />
    <WebhookLogDialog
      v-if="showWebhookLogDialog"
      v-model="showWebhookLogDialog"
    />
    <KBottomBar hidden-cancel @save="onSave" />
  </div>
</template>
