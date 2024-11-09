<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import type { Settings } from "@/api/commerce/settings";
import { saveSettings } from "@/api/commerce/settings";
import { ref, watch } from "vue";
import { useShortcut } from "@/hooks/use-shortcuts";
import { useCommerceStore } from "@/store/commerce";
import TimeDuration from "../components/time-duration.vue";
import LoginEarnEditor from "./earn-points-config/login-earn-editor.vue";
import OrderEarnEditor from "./earn-points-config/order-earn-editor.vue";

const { t } = useI18n();
const store = useCommerceStore();
const settings = ref<Settings>();

async function save() {
  await saveSettings(settings.value!);
  store.loadSettings();
}

watch(
  () => store.settings,
  () => {
    settings.value = JSON.parse(JSON.stringify(store.settings));
  },
  {
    immediate: true,
  }
);

useShortcut("save", save);
</script>

<template>
  <ElForm v-if="settings" label-position="top">
    <div
      class="rounded-normal bg-fff dark:bg-[#252526] mt-16 mb-24 py-24 px-56px"
    >
      <ElFormItem :label="t('common.rewardPointActiveDuration')">
        <TimeDuration
          v-model.number="settings.earnPoint.activeDuration"
          v-model:unit="settings.earnPoint.activeDurationUnit"
        />
      </ElFormItem>
      <LoginEarnEditor :earn-point="settings.earnPoint" />
      <OrderEarnEditor :earn-point="settings.earnPoint" />
    </div>
  </ElForm>
  <KBottomBar
    :permission="{
      feature: 'loyalty',
      action: 'edit',
    }"
    hidden-cancel
    @save="save"
  />
</template>
