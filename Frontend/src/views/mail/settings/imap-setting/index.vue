<template>
  <div class="p-24">
    <el-button
      v-if="emailStore.defaultAddress"
      class="mb-24"
      round
      @click="showAuthorizationCodeDialog = true"
    >
      {{ t("common.generateAuthorizationCode") }}
    </el-button>
    <div class="flex justify-between text-[#606266] dark:text-fff/60 text-14px">
      <div
        class="w-[calc(50%-8px)] bg-fff p-24 rounded-normal shadow-s-10 dark:bg-[#333]"
      >
        <h1
          class="text-000 dark:text-fff/86 text-30px font-medium mb-24px h-7 leading-7"
        >
          {{ t("common.imapSetting") }}
        </h1>
        <el-divider class="mt-28px mb-20px" />
        <div class="flex h-9 leading-9">
          <span class="w-100px">{{ t("common.server") }}:</span>
          <span>{{ imapSetting?.imap?.url }}</span>
        </div>

        <div class="flex h-9 leading-9">
          <span class="w-100px">{{ t("common.username") }}:</span>
          <span class="flex-1 ellipsis">
            <span v-if="!emailStore.defaultAddress"
              >{{ t("common.pleaseSetADefaultEmailAddress")
              }}<span
                class="text-blue cursor-pointer ml-4"
                @click="
                  $router.push({
                    name: 'addresses',
                    query: { ...$router.currentRoute.value.query },
                  })
                "
              >
                {{ t("common.goToSet") }}</span
              ></span
            >
            <span v-else :title="imapSetting?.imap?.userName">
              {{ imapSetting?.imap?.userName }}</span
            >
          </span>
        </div>

        <div class="flex h-9 leading-9">
          <span class="w-100px">{{ t("common.port") }}:</span>
          <span>{{ imapSetting?.imap?.port }}</span>
        </div>

        <div class="flex h-9 leading-9">
          <span class="w-100px">SSL:</span>
          <span>{{ imapSetting?.imap?.ssl }}</span>
        </div>
      </div>

      <div
        class="w-[calc(50%-8px)] bg-fff p-24 rounded-normal shadow-s-10 dark:bg-[#333]"
      >
        <h1
          class="text-000 dark:text-fff/86 text-30px font-medium mb-24px h-7 leading-7"
        >
          {{ t("common.smtpSetting") }}
        </h1>
        <el-divider class="mt-28px mb-20px" />
        <div class="flex h-9 leading-9">
          <span class="w-100px">{{ t("common.server") }}:</span>
          <span>{{ imapSetting?.smtp?.url }}</span>
        </div>

        <div class="flex h-9 leading-9">
          <span class="w-100px">{{ t("common.username") }}:</span>
          <span class="flex-1 ellipsis">
            <span v-if="!emailStore.defaultAddress"
              >{{ t("common.pleaseSetADefaultEmailAddress")
              }}<span
                class="text-blue cursor-pointer ml-4"
                @click="
                  $router.push({
                    name: 'addresses',
                    query: { ...$router.currentRoute.value.query },
                  })
                "
              >
                {{ t("common.goToSet") }}</span
              ></span
            >
            <span v-else :title="imapSetting?.smtp?.userName">
              {{ imapSetting?.smtp?.userName }}</span
            ></span
          >
        </div>

        <div class="flex h-9 leading-9">
          <span class="w-100px">{{ t("common.port") }}:</span>
          <span>{{ imapSetting?.smtp?.port }}</span>
        </div>

        <div class="flex h-9 leading-9">
          <span class="w-100px">SSL:</span>
          <span>{{ imapSetting?.smtp?.ssl }}</span>
        </div>
      </div>
    </div>
  </div>
  <AuthorizationCodeDialog
    v-if="showAuthorizationCodeDialog"
    v-model="showAuthorizationCodeDialog"
  />
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { getImapSetting } from "@/api/mail";
import AuthorizationCodeDialog from "./authorization-code-dialog.vue";
import { useEmailStore } from "@/store/email";

const { t } = useI18n();
const imapSetting = ref();
const emailStore = useEmailStore();
const showAuthorizationCodeDialog = ref(false);
const load = async () => {
  await emailStore.loadAddress("addresses");
  imapSetting.value = await getImapSetting();
};
load();
</script>
