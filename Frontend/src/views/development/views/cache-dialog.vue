<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const props = defineProps<{ modelValue: boolean; model: any }>();
const { t } = useI18n();
const show = ref(true);

const model = ref({
  enableCache: props.model.enableCache,
  cacheByVersion: props.model.cacheByVersion,
  cacheVersionType: props.model.cacheVersionType,
  cacheByDevice: props.model.cacheByDevice,
  cacheByCulture: props.model.cacheByCulture,
  cacheMinutes: props.model.cacheMinutes,
  cacheQueryKeys: props.model.cacheQueryKeys,
});

const onSave = async () => {
  props.model.enableCache = model.value.enableCache;
  props.model.cacheByVersion = model.value.cacheByVersion;
  props.model.cacheVersionType = model.value.cacheVersionType;
  props.model.cacheByDevice = model.value.cacheByDevice;
  props.model.cacheMinutes = model.value.cacheMinutes;
  props.model.cacheQueryKeys = model.value.cacheQueryKeys;
  props.model.cacheByCulture = model.value.cacheByCulture;
  show.value = false;
};

function changeVersionType(current: number) {
  if ((model.value.cacheVersionType & current) > 0) {
    model.value.cacheVersionType = model.value.cacheVersionType - current;
  } else {
    model.value.cacheVersionType = model.value.cacheVersionType + current;
  }
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.cache')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      label-position="top"
      @submit.prevent
      @keydown.enter="onSave"
    >
      <el-form-item :label="t('common.enable')" prop="name">
        <el-switch v-model="model.enableCache" data-cy="enable-cache" />
      </el-form-item>
      <template v-if="model.enableCache">
        <el-form-item>
          <template #label>
            <span>{{ t("common.cacheByDevice") }}</span>
            <Tooltip :tip="t('common.cacheByDeviceTip')" custom-class="ml-4" />
          </template>
          <el-switch
            v-model="model.cacheByDevice"
            data-cy="enable-cache-by-device"
          />
        </el-form-item>
        <el-form-item :label="t('common.cacheByCulture')">
          <el-switch
            v-model="model.cacheByCulture"
            data-cy="enable-cache-by-culture"
          />
        </el-form-item>
        <el-form-item :label="t('common.cacheByVersion')">
          <div>
            <el-switch
              v-model="model.cacheByVersion"
              data-cy="enable-cache-by-version"
            />
            <div v-if="model.cacheByVersion">
              <el-radio
                :model-value="model.cacheVersionType == 0"
                :label="true"
                @click.stop.prevent="model.cacheVersionType = 0"
                >{{ t("common.all") }}</el-radio
              >
              <el-radio
                :model-value="model.cacheVersionType == -1"
                :label="true"
                @click.stop.prevent="model.cacheVersionType = -1"
                >{{ t("common.self") }}</el-radio
              >
              <el-radio
                :model-value="model.cacheVersionType > 0"
                :label="true"
                @click.stop.prevent="model.cacheVersionType = 2"
                >{{ t("common.custom") }}</el-radio
              >
              <div v-if="model.cacheVersionType > 0">
                <el-checkbox
                  :label="t('common.development')"
                  :model-value="(model.cacheVersionType & (1 << 1)) > 0"
                  @click.stop.prevent="changeVersionType(1 << 1)"
                />
                <el-checkbox
                  :label="t('common.content')"
                  :model-value="(model.cacheVersionType & (1 << 2)) > 0"
                  @click.stop.prevent="changeVersionType(1 << 2)"
                />
                <el-checkbox
                  :label="t('common.commerce')"
                  :model-value="(model.cacheVersionType & (1 << 3)) > 0"
                  @click.stop.prevent="changeVersionType(1 << 3)"
                />
              </div>
            </div>
          </div>
        </el-form-item>
        <el-form-item
          v-if="!model.cacheByVersion"
          :label="t('common.timeMinutes')"
        >
          <el-input-number v-model="model.cacheMinutes" data-cy="cache-time" />
        </el-form-item>
        <el-form-item>
          <template #label>
            <span>{{ t("common.cacheQueryKeys") }}</span>
            <Tooltip :tip="t('common.cacheQueryKeysTip')" custom-class="ml-4" />
          </template>
          <el-input v-model="model.cacheQueryKeys" data-cy="cache-query-keys" />
        </el-form-item>
      </template>
    </el-form>

    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
