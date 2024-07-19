<script lang="ts" setup>
import { provide } from "vue";
import { useI18n } from "vue-i18n";
import { PAGE_STATE_KEY, PAGE_AUTH_URL, usePageState } from "./k-page";
import KText from "./k-text.vue";
import type { Size } from "./types";
import { GetModuleUrl } from "./utils";

interface Props {
  size?: Size;
  back?: string;
  title?: string;
  auth?: string;
}

const props = defineProps<Props>();
const { t } = useI18n();
const pageState = usePageState();
provide(PAGE_STATE_KEY, pageState);
provide(PAGE_AUTH_URL, props.auth);

function onBack() {
  if (!props.back) return;
  location.href = GetModuleUrl(props.back);
}
</script>

<template>
  <el-config-provider :size="size">
    <el-scrollbar>
      <div class="p-24 space-y-8">
        <el-page-header
          v-if="back"
          :title="t('common.back')"
          :content="title"
          @back="onBack"
        />
        <KText v-else-if="title" size="large">{{ title }}</KText>
        <slot />
      </div>
    </el-scrollbar>
  </el-config-provider>
</template>
