<script lang="ts" setup>
import { computed, ref, watch } from "vue";
import {
  getSettingDifferences,
  pushSettings,
  pullSettings,
} from "@/api/publish";
import { useI18n } from "vue-i18n";
import type { Difference } from "@/api/publish/types";
import { ElMessage } from "element-plus";
import Alert from "@/components/basic/alert.vue";

var settingsSyncStateString = localStorage.getItem("settingsSyncState");
var settingsSyncState: Record<string, string[]> = settingsSyncStateString
  ? JSON.parse(settingsSyncStateString)
  : {};

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const props = defineProps<{ modelValue: boolean; id: string }>();
const { t } = useI18n();
const show = ref(true);
const differences = ref<(Difference & { selected: boolean })[]>();

const loadDifferences = async () => {
  const excludes = settingsSyncState[props.id] ?? [];
  differences.value = (await getSettingDifferences(props.id)).map((m) => ({
    ...m,
    selected: excludes.every((f) => f != m.name),
  }));
};

loadDifferences();

async function push() {
  if (!differences.value) return;
  await pushSettings(
    props.id,
    differences.value
      .filter((f) => f.selected)
      .map((m) => ({
        name: m.name,
        value: m.value,
      }))
  );
  ElMessage.success(t("common.pushSuccess"));
  loadDifferences();
}

async function pull() {
  if (!differences.value) return;
  await pullSettings(
    differences.value
      .filter((f) => f.selected)
      .map((m) => ({
        name: m.name,
        value: m.remoteValue,
      }))
  );
  ElMessage.success(t("common.pullSuccess"));
  loadDifferences();
}

watch(
  () => differences.value,
  () => {
    if (!differences.value) return;

    settingsSyncState[props.id] = differences.value
      .filter((f) => !f.selected)
      .map((m) => m.name);

    localStorage.setItem(
      "settingsSyncState",
      JSON.stringify(settingsSyncState)
    );
  },
  {
    deep: true,
  }
);

const disableOperator = computed(() => {
  return !differences.value?.filter((f) => f.selected)?.length;
});
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.siteSettingDifferences')"
    custom-class="el-dialog--zero-padding"
    @closed="emit('update:modelValue', false)"
  >
    <template v-if="differences?.length">
      <Alert :content="t('common.settingsSyncTip')" />
      <div class="px-32 mt-8">
        <div
          v-for="item in differences"
          :key="item.name"
          class="flex items-center"
        >
          <p class="flex-1">{{ t("common." + item.name) }}</p>
          <ElSwitch v-model="item.selected" />
        </div>
      </div>
    </template>
    <template v-else-if="differences">
      <el-empty :description="t('common.allSettingsSynced')" />
    </template>
    <template #footer>
      <el-button data-cy="cancel-in-dialog" @click="show = false">
        {{ t("common.cancel") }}
      </el-button>
      <el-button type="primary" :disabled="disableOperator" @click="pull">
        {{ t("common.pull") }}
      </el-button>
      <el-button type="primary" :disabled="disableOperator" @click="push">
        {{ t("common.push") }}
      </el-button>
    </template>
  </el-dialog>
</template>
