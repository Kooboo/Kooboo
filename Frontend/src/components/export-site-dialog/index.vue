<template>
  <el-dialog
    v-model="show"
    width="600px"
    :close-on-click-modal="false"
    custom-class="el-dialog--zero-padding"
    :title="t('common.pleaseSelectExportWay')"
    @closed="emit('update:modelValue', false)"
  >
    <div class="export-site">
      <el-form ref="form">
        <el-form-item :label="t('common.exportType')" class="flex--middle">
          <el-radio-group v-model="type" class="el-radio-group--rounded">
            <el-radio-button
              v-for="item of exportTypes"
              :key="item.key"
              :label="item.key"
              @click="handleSelectExportType(item)"
              >{{ item.value }}</el-radio-button
            >
          </el-radio-group>
        </el-form-item>
        <template v-if="type === 'custom'">
          <el-form-item
            :label="t('common.exportContent')"
            class="export-site__tags-row"
            prop="selectedStores"
          >
            <div class="export-site__tags">
              <div class="arrow-up absolute" />
              <div
                v-for="item in storeNames"
                :key="item.name"
                class="export-site__tags__item"
                :class="{ 'export-site__tags__item--selected': item.selected }"
                @click="handleSelectTag(item)"
              >
                {{ item.displayName }}
              </div>
            </div>
          </el-form-item>
        </template>
        <el-form-item v-if="type == 'complete'" :label="t('common.copyMode')">
          <el-radio-group v-model="copyMode">
            <el-radio
              v-for="item of copyModes"
              :key="item.key"
              :label="item.key"
              >{{ item.value }}</el-radio
            >
          </el-radio-group>
        </el-form-item>
      </el-form>
    </div>

    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.export')"
        @confirm="handleExport"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { ElForm, ElMessage } from "element-plus";
import type { StoreName as StoreNameAPI } from "@/api/site";
import { exportSite, exportStore, exportStoreNames } from "@/api/site";
import type { KeyValue } from "@/global/types";

import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { openInHiddenFrame } from "@/utils/url";
interface PropsType {
  modelValue: boolean;
  siteId: string;
}
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();
const props = defineProps<PropsType>();
const { t } = useI18n();
const show = ref(true);

const exportTypes: ReadonlyArray<KeyValue> = [
  {
    key: "complete",
    value: t("export.complete"),
  },
  {
    key: "custom",
    value: t("common.custom"),
  },
];

const copyModes: ReadonlyArray<KeyValue> = [
  {
    key: "normal",
    value: t("common.normalMode"),
  },
  {
    key: "fast",
    value: t("common.fullMode"),
  },
];

const exportType = ref(exportTypes[0]);
const type = ref(exportType.value.key);
const copyMode = ref(copyModes[0].key);

type StoreName = StoreNameAPI & {
  selected?: boolean;
};
const storeNames = ref<StoreName[]>([]);

const unwatch = watch(
  () => exportType.value.key,
  async (val) => {
    if (storeNames.value.length > 0) {
      unwatch();
      return;
    }
    if (val === "custom") {
      storeNames.value = await exportStoreNames();
    }
  }
);

const selectedStores = computed(() => {
  return storeNames.value
    .filter((item) => item.selected)
    .map((item) => item.name);
});

function handleSelectExportType(item: KeyValue) {
  exportType.value = item;
}
function handleSelectTag(item: StoreName) {
  item.selected = !item.selected;
}

async function handleExport() {
  if (exportType.value.key === "complete") {
    var file = await exportSite({
      siteId: props.siteId,
      copyMode: copyMode.value,
    });

    openInHiddenFrame(
      `${import.meta.env.VITE_API}/Site/export?siteId=${
        props.siteId
      }&exportfile=${file}`
    );
    show.value = false;
    return;
  } else {
    const valid = selectedStores.value.length > 0;
    if (valid) {
      file = await exportStore({
        siteId: props.siteId,
        stores: selectedStores.value.join(","),
        copyMode: copyMode.value,
      });
      openInHiddenFrame(
        `${import.meta.env.VITE_API}/Site/ExportStore?siteId=${
          props.siteId
        }&copyMode=${copyMode.value}&exportfile=${file}`
      );

      show.value = false;
    } else {
      ElMessage.warning("Please select the content you want before exporting.");
    }
  }
}
</script>

<style lang="scss" scoped>
.export-site {
  &__switch {
    display: inline-flex;
    justify-content: center;
    align-items: center;
    background-color: rgba(243, 245, 245, 1);
    border-radius: 100px;
    padding: 3px 4px;
    text-align: center;

    &__item {
      min-width: 60px;
      padding: 0 16px;
      border-radius: 100px;
      cursor: pointer;
      line-height: 30px;

      &--active {
        background-color: #fff;
        color: $main-blue;
        box-shadow: 0px 1px 2px 0px rgba(0, 0, 0, 0.1);
      }
    }
  }

  &__tags {
    &-row {
      background-color: #f6f7f9;

      .dark & {
        background-color: #333;
      }

      padding-bottom: 12px !important;
    }

    display: flex;
    flex-wrap: wrap;

    &__item {
      border: 1px solid rgba(233, 234, 240, 1);
      border-radius: 8px;
      padding: 4px 16px;
      line-height: 20px;
      color: #666;

      .dark & {
        color: rgba(255, 255, 255, 0.6);
        border: 1px solid rgba(233, 234, 240, 0.5);
      }

      cursor: pointer;
      margin: 0 6px 12px 0;

      &:hover,
      &--selected {
        color: $main-blue;

        .dark & {
          color: rgba(255, 255, 255, 0.86);
          background-color: rgba($color: $main-blue, $alpha: 0.5);
        }

        background-color: rgba($color: $main-blue, $alpha: 0.1);
        border-color: inherit;
      }
    }
  }

  .el-form-item {
    padding: 24px 32px;
    margin-bottom: 0;

    :deep(.el-form-item__label) {
      margin-right: 14px;
      height: 30px;
    }
  }
}

.arrow-up {
  width: 0;
  height: 0;
  left: 25%;
  top: -52px;
  border-width: 15px;
  border-style: dashed dashed solid dashed;
  border-color: transparent transparent #f6f7f9 transparent;
}
.dark .arrow-up {
  border-color: transparent transparent rgba(51, 51, 51, 1) transparent;
}

.el-form-item:first-child {
  align-items: center;
}
</style>
