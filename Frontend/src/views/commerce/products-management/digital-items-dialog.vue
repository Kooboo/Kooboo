<script lang="ts" setup>
import type { ProductVariant } from "@/api/commerce/product";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import DigitalItemsEditor from "../components/digital-items-editor.vue";

const { t } = useI18n();
const show = ref(true);

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

defineProps<{ variant: ProductVariant }>();
</script>

<template>
  <div>
    <teleport to="body">
      <ElDialog
        :model-value="show"
        width="600px"
        :title="t('common.digitalItems')"
        :close-on-click-modal="false"
        @closed="emit('update:modelValue', false)"
      >
        <DigitalItemsEditor
          :id="variant.id"
          v-model:items="variant.digitalItems"
          delete-remote-file
        >
          <template #bar>
            <el-checkbox
              v-model="variant.autoDelivery"
              :label="t('common.autoDelivery')"
            />
          </template>
        </DigitalItemsEditor>
      </ElDialog>
    </teleport>
  </div>
</template>
