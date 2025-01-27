<script lang="ts" setup>
import { ref } from "vue";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { useI18n } from "vue-i18n";
import type { VisitorLog } from "@/api/visitor-log/types";
import { useSiteStore } from "@/store/site";
import { saveSite } from "@/api/site";
import type { ReteSettings } from "@/api/site/site";

const emit = defineEmits<{
  (e: "reload"): void;
  (e: "update:modelValue", value: boolean): void;
}>();

const siteStore = useSiteStore();
const props = defineProps<{ visitorLog: VisitorLog }>();
const { t } = useI18n();
const show = ref(true);

const ipRateSettings = ref<ReteSettings>({ permitLimit: 10, withinSeconds: 3 });
const userAgentRateSettings = ref<ReteSettings>({
  permitLimit: 10,
  withinSeconds: 3,
});

const blockIp = ref(
  siteStore.site.accessLimitSettings.ipBlacklist.some(
    (f) => f == props.visitorLog.clientIP
  )
);
const existLimitIp =
  siteStore.site.rateLimitSettings.ipLimits[props.visitorLog.clientIP];
const limitIp = ref(!!existLimitIp);
if (existLimitIp) {
  ipRateSettings.value.permitLimit = existLimitIp.permitLimit;
  ipRateSettings.value.withinSeconds = existLimitIp.withinSeconds;
}

const forbiddenKeywords = ref(props.visitorLog.userAgent);
const limitKeywords = ref(props.visitorLog.userAgent);

const existForbiddenKeywords =
  siteStore.site.accessLimitSettings.blockUserAgentKeywords.find(
    (f) => props.visitorLog.userAgent.indexOf(f) > -1
  );
const existLimitKeywords = Object.keys(
  siteStore.site.rateLimitSettings.userAgentLimits
).find((f) => props.visitorLog.userAgent.indexOf(f) > -1);

if (existForbiddenKeywords) {
  forbiddenKeywords.value = existForbiddenKeywords;
}

if (existLimitKeywords) {
  limitKeywords.value = existLimitKeywords;
  userAgentRateSettings.value.permitLimit =
    siteStore.site.rateLimitSettings.userAgentLimits[
      existLimitKeywords
    ].permitLimit;
  userAgentRateSettings.value.withinSeconds =
    siteStore.site.rateLimitSettings.userAgentLimits[
      existLimitKeywords
    ].withinSeconds;
}

const blockKeywords = ref(!!existForbiddenKeywords);
const limitRateKeywords = ref(!!existLimitKeywords);

const onSave = async () => {
  if (
    blockIp.value &&
    !siteStore.site.accessLimitSettings.ipBlacklist.find(
      (f) => f == props.visitorLog.clientIP
    )
  ) {
    siteStore.site.accessLimitSettings.ipBlacklist.push(
      props.visitorLog.clientIP
    );
    siteStore.site.accessLimitSettings.enable = true;
  } else if (!blockIp.value) {
    siteStore.site.accessLimitSettings.ipBlacklist =
      siteStore.site.accessLimitSettings.ipBlacklist.filter(
        (f) => f != props.visitorLog.clientIP
      );
  }

  if (limitIp.value) {
    siteStore.site.rateLimitSettings.ipLimits[props.visitorLog.clientIP] =
      ipRateSettings.value;
    siteStore.site.rateLimitSettings.enable = true;
  } else {
    delete siteStore.site.rateLimitSettings.ipLimits[props.visitorLog.clientIP];
  }

  if (
    blockKeywords.value &&
    forbiddenKeywords.value &&
    !siteStore.site.accessLimitSettings.blockUserAgentKeywords.find(
      (f) => f == forbiddenKeywords.value
    )
  ) {
    siteStore.site.accessLimitSettings.blockUserAgentKeywords.push(
      forbiddenKeywords.value
    );
    siteStore.site.accessLimitSettings.enable = true;
  } else if (!blockKeywords.value) {
    siteStore.site.accessLimitSettings.blockUserAgentKeywords =
      siteStore.site.accessLimitSettings.blockUserAgentKeywords.filter(
        (f) => f != forbiddenKeywords.value
      );
  }

  if (limitRateKeywords.value && limitKeywords.value) {
    siteStore.site.rateLimitSettings.userAgentLimits[limitKeywords.value] =
      userAgentRateSettings.value;
    siteStore.site.rateLimitSettings.enable = true;
  } else {
    delete siteStore.site.rateLimitSettings.userAgentLimits[
      limitKeywords.value
    ];
  }

  show.value = false;
  emit("reload");
  await saveSite(siteStore.site);
  siteStore.loadSite();
};
</script>

<template>
  <div>
    <el-dialog
      :model-value="show"
      width="600px"
      :close-on-click-modal="false"
      :title="t('common.accessLimitSettings')"
      @closed="emit('update:modelValue', false)"
    >
      <el-form label-position="top">
        <el-form-item :label="t('common.rateLimit')">
          <div
            v-if="!siteStore.site.rateLimitSettings.limitAllRequest"
            class="pl-12 w-full"
          >
            <div>
              <el-checkbox
                v-model="limitIp"
                :label="`${t('common.limit')} ${visitorLog.clientIP} ${t(
                  'common.request'
                )}`"
              />
              <div v-if="limitIp" class="grid grid-cols-2">
                <el-form-item :label="t('common.withinSeconds')">
                  <el-input-number
                    v-model="ipRateSettings.withinSeconds"
                    :min="1"
                  />
                </el-form-item>
                <el-form-item :label="t('common.permitLimit')">
                  <el-input-number
                    v-model="ipRateSettings.permitLimit"
                    :min="1"
                  />
                </el-form-item>
              </div>
            </div>
            <div class="space-y-4">
              <el-checkbox
                v-model="limitRateKeywords"
                :label="`${t('common.limit')} ${t('common.userAgent')}`"
              />
              <el-input
                v-if="limitRateKeywords"
                v-model.trim="limitKeywords"
                type="textarea"
                :rows="3"
              />
              <div v-if="limitRateKeywords" class="grid grid-cols-2">
                <el-form-item :label="t('common.withinSeconds')">
                  <el-input-number
                    v-model="userAgentRateSettings.withinSeconds"
                    :min="1"
                  />
                </el-form-item>
                <el-form-item :label="t('common.permitLimit')">
                  <el-input-number
                    v-model="userAgentRateSettings.permitLimit"
                    :min="1"
                  />
                </el-form-item>
              </div>
            </div>
          </div>
          <div v-else class="pl-12 w-full">
            <el-alert
              :title="t('commmon.limitAllRequestTip')"
              type="warning"
              :closable="false"
            />
          </div>
        </el-form-item>
        <el-form-item :label="t('common.forbidden')">
          <div class="pl-12 w-full">
            <div>
              <el-checkbox
                v-model="blockIp"
                :label="`${t('common.forbidden')} ${visitorLog.clientIP} ${t(
                  'common.request'
                )}`"
              />
            </div>
            <div class="space-y-4">
              <el-checkbox
                v-model="blockKeywords"
                :label="`${t('common.forbidden')} ${t('common.userAgent')}`"
              />
              <el-input
                v-if="blockKeywords"
                v-model.trim="forbiddenKeywords"
                type="textarea"
                :rows="3"
              />
            </div>
          </div>
        </el-form-item>
      </el-form>

      <template #footer>
        <DialogFooterBar
          :confirm-label="t('common.save')"
          @confirm="onSave"
          @cancel="show = false"
        />
      </template>
    </el-dialog>
  </div>
</template>
