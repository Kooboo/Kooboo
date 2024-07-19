<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import type { Address } from "@/api/commerce/customer";
import AddressForm from "./address-form.vue";

const { t } = useI18n();
const show = ref(true);

const props = defineProps<{
  modelValue: boolean;
  model: Address;
}>();

const model = ref<Address>(JSON.parse(JSON.stringify(props.model)));

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "update", value: Address): void;
}>();

async function onSave() {
  emit("update", model.value);
  show.value = false;
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :title="t('common.createAddress')"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <AddressForm v-model="model" :model="model" />
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
