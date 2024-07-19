<template>
  <GuidInfo v-if="!keywordsLength && allSitesInFolder < 1">
    {{ t(`common.emptySiteAndCreateNowTips`) }}
    <template #button>
      <el-button type="primary" round @click="emits('create-site')"
        ><el-icon class="iconfont icon-a-addto" />{{
          t("common.createSiteNow")
        }}</el-button
      >
    </template>
  </GuidInfo>

  <NoMatchingSite v-else-if="keywordsLength" />
</template>
<script lang="ts" setup>
import GuidInfo from "@/components/guid-info/index.vue";
import NoMatchingSite from "./no-matching-site.vue";

import { useI18n } from "vue-i18n";
const emits = defineEmits<{ (event: "create-site"): void }>();
const { t } = useI18n();
interface PropsType {
  keywordsLength: number;
  allSitesInFolder: number;
}
defineProps<PropsType>();
</script>
