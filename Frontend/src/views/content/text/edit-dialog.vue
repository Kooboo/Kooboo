<script setup lang="ts">
import { ElForm } from "element-plus";
import type { KConfig } from "@/api/content/kconfig";
import { update } from "@/api/content/kconfig";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

interface PropsType {
  modelValue: boolean;
  current: KConfig;
  alert?: string;
}
interface EmitsType {
  (e: "update:modelValue", value: boolean): void;
  (e: "success", value: boolean): void;
}

const show = ref(true);
let success = false;
const props = defineProps<PropsType>();
const emit = defineEmits<EmitsType>();
const { t } = useI18n();
const hasBinding = computed(
  () => Object.keys(props.current.binding).length > 0
);

async function handleSave() {
  await update({
    id: props.current.id,
    binding: props.current.binding,
  });
  success = true;
  closed();
}

const closed = () => {
  emit("success", success);
  emit("update:modelValue", false);
};
</script>

<template>
  <el-dialog
    v-model="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.KoobooConfig')"
    custom-class="el-dialog--zero-padding"
    @closed="closed"
  >
    <Alert v-if="alert" :content="alert" />
    <el-form label-position="top" class="px-32 py-24" @submit.prevent>
      <template v-if="hasBinding">
        <el-form-item
          v-for="(_value, key) in current.binding"
          :key="key"
          :label="key"
        >
          <el-input v-model="current.binding[key]" type="textarea" autosize />
        </el-form-item>
      </template>
      <div v-else>
        {{ t("common.noConfigurationItem") }}
      </div>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :permission="{
          feature: 'text',
          action: 'edit',
        }"
        @confirm="handleSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
