<script setup lang="ts">
import { ElForm } from "element-plus";
import type { Label } from "@/api/content/label";
import { update, create } from "@/api/content/label";
import { computed, ref } from "vue";
import { useMultilingualStore } from "@/store/multilingual";
import { useI18n } from "vue-i18n";
import type { KeyValue } from "@/global/types";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

interface PropsType {
  modelValue: boolean;
  current: Label;
  alert?: string;
}

interface EmitsType {
  (e: "update:modelValue", value: boolean): void;
  (e: "success", value: boolean): void;
}

const props = defineProps<PropsType>();
const emit = defineEmits<EmitsType>();
const { t } = useI18n();
const show = ref(true);
const success = ref(false);
const multilingualStore = useMultilingualStore();
const model = ref<Label>();
const load = () => {
  model.value = JSON.parse(JSON.stringify(props.current));
};
load();
const getTitleLabel = (item: KeyValue) => {
  if (item.key === multilingualStore.default) {
    return sortedCultures.value.length > 1
      ? t("common.value") + " - " + item.key + " (" + t("common.default") + ")"
      : t("common.value");
  } else return t("common.value") + " - " + item.key;
};

async function handleSave() {
  if (!props.current.id) {
    await create({
      key: model.value!.id,
      values: model.value!.values,
    });
  } else {
    await update({
      id: model.value!.id,
      values: model.value!.values,
    });
  }

  success.value = true;
  show.value = false;
}

const sortedCultures = computed(() => {
  const cultures = multilingualStore.selected.map((key) => ({
    key,
    value: multilingualStore.cultures[key],
  }));
  return cultures;
});

const closed = () => {
  emit("update:modelValue", false);
  emit("success", success.value);
};
</script>

<template>
  <el-dialog
    v-model="show"
    width="600px"
    :close-on-click-modal="false"
    :title="current.id ? t('common.editLabel') : t('common.newLabel')"
    custom-class="el-dialog--zero-padding"
    @closed="closed"
  >
    <Alert v-if="alert" :content="alert" />
    <div class="px-32 py-24">
      <div class="flex justify-end">
        <MultilingualSelector />
      </div>
      <el-scrollbar max-height="50vh">
        <el-form label-position="top" @submit.prevent>
          <el-form-item v-if="!current.id" :label="t('common.name')">
            <el-input v-model="model!.id" />
          </el-form-item>
          <el-form-item v-else :label="t('common.name')">
            <el-input v-model="model!.name" disabled />
          </el-form-item>
          <el-form-item
            v-for="item in sortedCultures"
            :key="item.key"
            :label="getTitleLabel(item)"
          >
            <el-input
              v-model="model!.values[item.key]"
              type="textarea"
              autosize
            />
          </el-form-item>
        </el-form>
      </el-scrollbar>
    </div>

    <template #footer>
      <DialogFooterBar
        :permission="{
          feature: 'label',
          action: 'edit',
        }"
        :confirm-label="current.id ? t('common.save') : t('common.create')"
        @confirm="handleSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
