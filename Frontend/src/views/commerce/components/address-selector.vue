<script lang="ts" setup>
import type { Address, CustomerEdit } from "@/api/commerce/customer";
import { editCustomer, getCustomerEdit } from "@/api/commerce/customer";
import { computed, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import CreateAddressDialog from "../customers/create-address-dialog.vue";

const props = defineProps<{ customerId: string }>();
const emit = defineEmits<{
  (e: "selected", value: Address): void;
}>();
const selected = ref<number>();
const customer = ref<CustomerEdit>();
const { t } = useI18n();
const showCreateAddressDialog = ref(false);

async function load() {
  if (props.customerId) {
    customer.value = await getCustomerEdit(props.customerId);
    selected.value =
      customer.value.addresses.findIndex((f) => f.isDefault) ?? 0;
  }
}

onMounted(load);

const list = computed(() => {
  const result = [];

  if (customer.value) {
    for (const m of customer.value.addresses) {
      result.push(
        `${m.firstName} ${m.lastName} ${m.phone} ${m.province} ${m.city} ${m.address1} ${m.address2} ${m.zip}`
      );
    }
  }

  return result;
});

async function onCreate(value: Address) {
  customer.value!.addresses.forEach((m) => (m.isDefault = false));
  customer.value!.addresses.push(value);
  await editCustomer(customer.value);
  load();
}

watch(
  () => selected.value,
  () => {
    if (selected.value! > -1) {
      const address = customer.value?.addresses[selected.value!];
      if (address) {
        emit("selected", address);
      }
    }
  }
);
</script>

<template>
  <div class="space-y-4">
    <div
      v-if="list.length"
      class="bg-card dark:bg-444 rounded-normal px-16 py-8"
    >
      <ElRadioGroup v-model="selected" class="block">
        <div
          v-for="(item, index) of list"
          :key="index"
          class="flex items-center"
        >
          <ElRadio :label="index">{{ item }}</ElRadio>
        </div>
      </ElRadioGroup>
    </div>

    <IconButton
      circle
      class="text-blue"
      icon="icon-a-addto"
      :tip="t('common.add')"
      @click="showCreateAddressDialog = true"
    />
    <CreateAddressDialog
      v-if="showCreateAddressDialog"
      v-model="showCreateAddressDialog"
      @create="onCreate"
    />
  </div>
</template>
