<script lang="ts" setup>
import { site } from "./settings";
import GroupPanel from "./group-panel.vue";

import { useI18n } from "vue-i18n";
const { t } = useI18n();

function change(current: number) {
  if (!site.value) return;
  if ((site.value.resourceCaches & current) > 0) {
    site.value.resourceCaches = site.value.resourceCaches - current;
  } else {
    site.value.resourceCaches = site.value.resourceCaches + current;
  }
}
</script>

<template>
  <template v-if="site">
    <GroupPanel
      v-model="site.enableResourceCache"
      :label="t('common.enableResourceCache')"
    >
      <el-checkbox
        :label="t('common.style')"
        :model-value="(site.resourceCaches & 1) > 0"
        @click.stop.prevent="change(1)"
      />
      <el-checkbox
        :label="t('common.script')"
        :model-value="(site.resourceCaches & (1 << 1)) > 0"
        @click.stop.prevent="change(1 << 1)"
      />
      <el-checkbox
        :label="t('common.image')"
        :model-value="(site.resourceCaches & (1 << 2)) > 0"
        @click.stop.prevent="change(1 << 2)"
      />
      <el-checkbox
        :label="t('common.content')"
        :model-value="(site.resourceCaches & (1 << 3)) > 0"
        @click.stop.prevent="change(1 << 3)"
      />
    </GroupPanel>
  </template>
</template>
