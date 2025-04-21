<script lang="ts" setup>
import { computed, ref } from "vue";
import { useStyleStore } from "@/store/style";

import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
const styleStore = useStyleStore();

const props = defineProps<{ modelValue: boolean; excludes: string[] }>();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "selected", value: string): void;
}>();
const { t } = useI18n();

const files = computed(() => {
  return styleStore.external.filter(
    (f) => !props.excludes.some((s) => s === f.id)
  );
});

const groups = computed(() => {
  return styleStore.group.filter(
    (f) => !props.excludes.some((s) => s === f.id)
  );
});

const all = computed(() => {
  return [...files.value, ...groups.value];
});

const show = ref(true);
const model = ref(all.value[0]?.id);

const onSave = async () => {
  emit("selected", model.value);
  show.value = false;
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="442px"
    :close-on-click-modal="false"
    :title="t('common.select')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form label-position="top">
      <el-form-item :label="t('common.style')">
        <el-select v-model="model" class="w-full">
          <label
            v-if="files.length"
            class="p-8 dark:text-fff/86"
            data-cy="style-file"
            >{{ t("common.file") }}</label
          >
          <el-option
            v-for="item of files"
            :key="item.id"
            :value="item.id"
            :label="item.name"
            data-cy="style-opt"
          >
            <div class="max-w-400px truncate">
              <span>{{ item.name }}</span>
              <span v-if="item.routeName">({{ item.routeName }})</span>
            </div>
          </el-option>
          <label
            v-if="groups.length"
            class="p-8 dark:text-fff/86"
            data-cy="style-group"
            >{{ t("common.group") }}</label
          >
          <el-option
            v-for="item of groups"
            :key="item.id"
            :value="item.id"
            :label="item.name"
            data-cy="group-opt"
          >
            <div class="max-w-400px truncate">
              <span>{{ item.name }}</span>
              <span v-if="item.relativeUrl">({{ item.relativeUrl }})</span>
            </div></el-option
          >
        </el-select>
      </el-form-item>
    </el-form>

    <template #footer>
      <DialogFooterBar
        :disabled="!model"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
