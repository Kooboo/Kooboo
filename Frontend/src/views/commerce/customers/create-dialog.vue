<script lang="ts" setup>
import { createCustomer } from "@/api/commerce/customer";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import type { CustomerCreate } from "@/api/commerce/customer";
import EditableTags from "@/components/basic/editable-tags.vue";
import { useTag } from "../useTag";
import AddressesForm from "./addresses-form.vue";

const { t } = useI18n();
const show = ref(true);
const { tags, removeTag } = useTag("Customer");

defineProps<{
  modelValue: boolean;
}>();

const model = ref<CustomerCreate>({
  firstName: "",
  lastName: "",
  email: "",
  phone: "",
  tags: [],
  addresses: [],
});

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

async function onSave() {
  await createCustomer(model.value);
  emit("reload");
  show.value = false;
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="800px"
    :title="t('common.create')"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <ElForm label-position="top">
      <div class="grid grid-cols-2 gap-8">
        <ElFormItem :label="t('common.firstName')">
          <ElInput v-model="model.firstName" />
        </ElFormItem>
        <ElFormItem :label="t('common.lastName')">
          <ElInput v-model="model.lastName" />
        </ElFormItem>
      </div>

      <ElFormItem :label="t('common.email')">
        <ElInput v-model="model.email" />
      </ElFormItem>
      <ElFormItem :label="t('common.phone')">
        <ElInput v-model="model.phone" />
      </ElFormItem>
      <ElFormItem :label="t('common.tag')">
        <EditableTags
          v-model="model.tags"
          option-deletable
          :options="tags"
          @delete-option="removeTag"
        />
      </ElFormItem>
      <ElFormItem :label="t('common.addresses')">
        <AddressesForm :list="model.addresses" />
      </ElFormItem>
    </ElForm>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
