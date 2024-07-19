<script lang="ts" setup>
import { site } from "./settings";
import GroupPanel from "./group-panel.vue";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
</script>

<template>
  <template v-if="site">
    <GroupPanel
      v-model="site.codeLogSettings.enable"
      :label="t('common.codeLog')"
    >
      <el-form-item :label="t('codelog.level')">
        <el-select
          v-model="site.codeLogSettings.logLevel"
          class="w-full"
          data-cy="level"
        >
          <el-option value="Debug" :label="t('common.debug')" data-cy="Debug" />
          <el-option
            value="Information"
            :label="t('common.information')"
            data-cy="Information"
          />
          <el-option
            value="Warning"
            :label="t('common.warning')"
            data-cy="Warning"
          />
          <el-option value="Error" :label="t('common.error')" data-cy="Error" />
          <el-option
            value="Critical"
            :label="t('common.critical')"
            data-cy="Critical"
          />
        </el-select>
      </el-form-item>
      <el-form-item :label="t('common.keepDays')">
        <el-input-number
          v-model="site.codeLogSettings.keepDays"
          :precision="0"
          :min="1"
          data-cy="keey-days"
        />
      </el-form-item>
    </GroupPanel>
  </template>
</template>
