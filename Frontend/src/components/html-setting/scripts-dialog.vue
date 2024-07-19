<script lang="ts" setup>
import { computed, ref } from "vue";
import { useScriptStore } from "@/store/script";

import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

const scriptStore = useScriptStore();

const props = defineProps<{ modelValue: boolean; excludes: string[] }>();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "selected", value: string): void;
}>();
const { t } = useI18n();

const files = computed(() => {
  return scriptStore.external.filter(
    (f) => !props.excludes.some((s) => s === f.id)
  );
});

const groups = computed(() => {
  return scriptStore.group.filter(
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
      <el-form-item :label="t('common.script')">
        <el-select v-model="model" class="w-full">
          <label
            v-if="files.length"
            class="p-8 dark:text-fff/86"
            data-cy="script-file"
            >{{ t("common.file") }}</label
          >
          <el-option
            v-for="item of files"
            :key="item.id"
            :label="item.name"
            :value="item.id"
            data-cy="script-opt"
          />
          <label
            v-if="groups.length"
            class="p-8 dark:text-fff/86"
            data-cy="script-group"
            >{{ t("common.group") }}</label
          >
          <el-option
            v-for="item of groups"
            :key="item.id"
            :label="item.name"
            :value="item.id"
            data-cy="script-opt"
          />
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
