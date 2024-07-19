<script lang="ts" setup>
import { site } from "./settings";
import GroupPanel from "./group-panel.vue";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
</script>

<template>
  <template v-if="site">
    <GroupPanel
      v-model="site.enableImageBrowserCache"
      :label="t('common.imageBrowserCache')"
    >
      <el-form-item
        :label="t('common.numberOfExpirationDaysAndUseVersionControl')"
      >
        <el-input-number
          v-model="site.imageCacheDays"
          :precision="0"
          :min="-1"
        />
      </el-form-item>
    </GroupPanel>
  </template>
</template>
