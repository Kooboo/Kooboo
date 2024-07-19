<script lang="ts" setup>
import { getCustomer, type CustomerListItem } from "@/api/commerce/customer";
import { onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import SelectCustomerDialog from "../components/select-customer-dialog.vue";

const props = defineProps<{ modelValue?: string; readonly?: boolean }>();

const emit = defineEmits<{
  (e: "update:model-value", value: string | undefined): void;
}>();

const { t } = useI18n();
const showSelectCustomerDialog = ref(false);
const customer = ref<CustomerListItem>();

onMounted(async () => {
  if (props.modelValue) {
    customer.value = await getCustomer(props.modelValue);
  }
});

function onDelete() {
  customer.value = undefined;
  emit("update:model-value", undefined);
}

function onSelected(value: CustomerListItem) {
  customer.value = value;
  emit("update:model-value", value.id);
}
</script>

<template>
  <div
    v-if="customer"
    class="border border-gray p-12 rounded-normal w-400px dark:bg-444 dark:text-gray"
  >
    <div class="flex items-center space-x-8">
      <div class="flex-1" />
      <el-tooltip
        v-if="!readonly"
        placement="top"
        :content="t('common.delete')"
      >
        <el-icon
          class="iconfont icon-delete text-orange text-l"
          @click="onDelete"
        />
      </el-tooltip>
    </div>
    <div class="p-4">
      <div class="text-l">{{ customer.firstName }} {{ customer.lastName }}</div>
      <div class="text-s">{{ customer.email }}</div>
      <div class="text-s">{{ customer.phone }}</div>
    </div>
  </div>
  <IconButton
    v-else-if="!readonly"
    circle
    class="text-blue"
    icon="icon-a-addto"
    :tip="t('common.add')"
    @click="showSelectCustomerDialog = true"
  />
  <SelectCustomerDialog
    v-if="showSelectCustomerDialog"
    v-model="showSelectCustomerDialog"
    @selected="onSelected"
  />
</template>
