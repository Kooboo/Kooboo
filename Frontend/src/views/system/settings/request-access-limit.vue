<script lang="ts" setup>
import type { ReteSettings } from "@/api/site/site";
import { useI18n } from "vue-i18n";
import GroupPanel from "./group-panel.vue";
import ListEditor from "@/components/basic/list-editor.vue";
import RateSettingsEditor from "./request-access-limit/rate-settings-editor.vue";
import { toList, toObject } from "@/utils/lang";
import { ref } from "vue";
import { events, site } from "./settings";

const { t } = useI18n();

events.onRequestAccessLimit = (s) => {
  s.rateLimitSettings.ipLimits = toObject(
    ipLimits.value.filter((f) => f.key.trim())
  ) as any;
  s.rateLimitSettings.userAgentLimits = toObject(
    userAgentLimits.value.filter((f) => f.key.trim())
  ) as any;
  s.accessLimitSettings.blockUserAgentKeywords =
    s.accessLimitSettings.blockUserAgentKeywords.filter((f) => f?.trim());
  s.accessLimitSettings.ipBlacklist = s.accessLimitSettings.ipBlacklist.filter(
    (f) => f?.trim()
  );
};

const ipLimits = ref<{ key: string; value: ReteSettings }[]>(
  toList(site.value!.rateLimitSettings.ipLimits) as any
);

const userAgentLimits = ref<{ key: string; value: ReteSettings }[]>(
  toList(site.value!.rateLimitSettings.userAgentLimits) as any
);
</script>

<template>
  <template v-if="site">
    <GroupPanel
      v-model="site.rateLimitSettings.enable"
      :label="t('common.ipRequestRateLimit')"
    >
      <el-form-item :label="t('common.limitAllRequest')">
        <el-switch v-model="site.rateLimitSettings.limitAllRequest" />
      </el-form-item>
      <div
        v-if="site.rateLimitSettings.limitAllRequest"
        class="grid grid-cols-2"
      >
        <el-form-item :label="t('common.withinSeconds')">
          <el-input-number
            v-model="
              site.rateLimitSettings.allRequestRateSettings.withinSeconds
            "
            :min="1"
          />
        </el-form-item>
        <el-form-item :label="t('common.permitLimit')">
          <el-input-number
            v-model="site.rateLimitSettings.allRequestRateSettings.permitLimit"
            :min="1"
          />
        </el-form-item>
      </div>
      <div v-else>
        <el-form-item :label="t('common.ipRateLimit')">
          <RateSettingsEditor v-model="ipLimits" placeholder="1.2.3.4" />
        </el-form-item>
        <el-form-item :label="t('common.userAgentRateLimit')">
          <RateSettingsEditor
            v-model="userAgentLimits"
            placeholder="Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.2 Safari/605.1.15"
          />
        </el-form-item>
      </div>
    </GroupPanel>

    <GroupPanel
      v-model="site.accessLimitSettings.enable"
      :label="t('common.requestForbidden')"
    >
      <div class="grid grid-cols-2">
        <el-form-item :label="t('common.ipBlacklist')" class="col-span-2">
          <ListEditor
            v-model="site.accessLimitSettings.ipBlacklist"
            class="w-full"
          />
        </el-form-item>

        <el-form-item
          :label="t('common.userAgentBlacklist')"
          class="col-span-2"
        >
          <ListEditor
            v-model="site.accessLimitSettings.blockUserAgentKeywords"
            class="w-full"
          />
        </el-form-item>
      </div>
    </GroupPanel>
  </template>
</template>
