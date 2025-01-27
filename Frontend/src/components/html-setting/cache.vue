<script lang="ts" setup>
import { useI18n } from "vue-i18n";
const props = defineProps<{
  enable: boolean;
  enableVersion: boolean;
  versionType: number;
  enableDevice: boolean;
  enableCulture: boolean;
  minutes: number;
  queryKeys?: string;
}>();

const emit = defineEmits<{
  (e: "update:enable", value: boolean): void;
  (e: "update:enableVersion", value: boolean): void;
  (e: "update:versionType", value: number): void;
  (e: "update:enableDevice", value: boolean): void;
  (e: "update:enableCulture", value: boolean): void;
  (e: "update:minutes", value?: number): void;
  (e: "update:queryKeys", value: string): void;
}>();
const { t } = useI18n();

function changeVersionType(current: number) {
  if ((props.versionType & current) > 0) {
    emit("update:versionType", props.versionType - current);
  } else {
    emit("update:versionType", props.versionType + current);
  }
}
</script>

<template>
  <div class="px-24 py-16">
    <el-form label-position="top">
      <el-form-item :label="t('common.enable')">
        <el-switch
          :model-value="enable"
          data-cy="enable-cache"
          @update:model-value="$emit('update:enable', !!$event)"
        />
      </el-form-item>
      <template v-if="enable">
        <el-form-item>
          <template #label>
            <span>{{ t("common.cacheByDevice") }}</span>
            <Tooltip :tip="t('common.cacheByDeviceTip')" custom-class="ml-4" />
          </template>
          <el-switch
            :model-value="enableDevice"
            data-cy="enable-cache-by-device"
            @update:model-value="$emit('update:enableDevice', !!$event)"
          />
        </el-form-item>
        <el-form-item :label="t('common.cacheByCulture')">
          <el-switch
            :model-value="enableCulture"
            data-cy="enable-cache-by-culture"
            @update:model-value="$emit('update:enableCulture', !!$event)"
          />
        </el-form-item>
        <el-form-item :label="t('common.cacheByVersion')">
          <div>
            <el-switch
              :model-value="enableVersion"
              data-cy="enable-cache-by-version"
              @update:model-value="$emit('update:enableVersion', !!$event)"
            />
            <div v-if="enableVersion">
              <el-radio
                :model-value="versionType == 0"
                :label="true"
                @click.stop.prevent="$emit('update:versionType', 0)"
                >{{ t("common.all") }}</el-radio
              >
              <el-radio
                :model-value="versionType == -1"
                :label="true"
                @click.stop.prevent="$emit('update:versionType', -1)"
                >{{ t("common.self") }}</el-radio
              >
              <el-radio
                :model-value="versionType > 0"
                :label="true"
                @click.stop.prevent="$emit('update:versionType', 2)"
                >{{ t("common.custom") }}</el-radio
              >
              <div v-if="versionType > 0">
                <el-checkbox
                  :label="t('common.development')"
                  :model-value="(versionType & (1 << 1)) > 0"
                  @click.stop.prevent="changeVersionType(1 << 1)"
                />
                <el-checkbox
                  :label="t('common.content')"
                  :model-value="(versionType & (1 << 2)) > 0"
                  @click.stop.prevent="changeVersionType(1 << 2)"
                />
                <el-checkbox
                  :label="t('common.commerce')"
                  :model-value="(versionType & (1 << 3)) > 0"
                  @click.stop.prevent="changeVersionType(1 << 3)"
                />
              </div>
            </div>
          </div>
        </el-form-item>
        <el-form-item v-if="!enableVersion" :label="t('common.timeMinutes')">
          <el-input-number
            :model-value="minutes"
            data-cy="cache-time"
            @update:model-value="$emit('update:minutes', $event)"
          />
        </el-form-item>
        <el-form-item>
          <template #label>
            <span>{{ t("common.cacheQueryKeys") }}</span>
            <Tooltip :tip="t('common.cacheQueryKeysTip')" custom-class="ml-4" />
          </template>
          <el-input
            :model-value="queryKeys"
            data-cy="cache-query-keys"
            @update:model-value="$emit('update:queryKeys', $event)"
          />
        </el-form-item>
      </template>
    </el-form>
  </div>
</template>
