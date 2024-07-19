<template>
  <el-dialog
    :model-value="show"
    :title="t('common.generateAuthorizationCode')"
    width="550px"
    :close-on-click-modal="false"
    @closed="$emit('update:modelValue', false)"
  >
    <div
      class="text-center dark:text-[#fffa] py-16"
      style="word-break: break-word"
    >
      <el-icon class="iconfont icon-yes2 text-green text-36px" />
      <h1 class="mt-16 mb-12 text-18px text-[#000] dark:text-[#fffa]">
        IMAP/SMTP {{ t("common.authorizationCodeHasGenerated") }}
      </h1>
      <span class="text-m"
        >{{ t("common.pleaseEnterTheFollowingAuthorizationCode") }}:</span
      >
      <div
        class="py-12 bg-[#f4f4f5] dark:bg-[#333] mt-16 text-center mt-24 mb-16"
      >
        <span class="text-[#000] dark:text-[#fffa] mr-8">{{
          authorizationCode
        }}</span
        ><IconButton
          class="hover:text-blue"
          icon="icon-copy"
          :tip="t('common.copy')"
          @click="copyText(authorizationCode)"
        />
      </div>
      <span class="text-999 text-s">{{
        t("common.authorizationCodesTips")
      }}</span>
    </div>
  </el-dialog>
</template>

<script lang="ts" setup>
import { setAuthorizationCode } from "@/api/mail";
import { useEmailStore } from "@/store/email";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { copyText } from "@/hooks/use-copy-text";

const emailStore = useEmailStore();

defineProps<{
  modelValue: boolean;
}>();
defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const { t } = useI18n();
const show = ref(true);
const authorizationCode = ref();
const load = async () => {
  authorizationCode.value = await setAuthorizationCode(
    emailStore.defaultAddress
  );
};
load();
</script>
