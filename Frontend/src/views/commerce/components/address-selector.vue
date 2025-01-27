<script lang="ts" setup>
import type { Address, CustomerEdit } from "@/api/commerce/customer";
import { editCustomer, getCustomerEdit } from "@/api/commerce/customer";
import { computed, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import CreateAddressDialog from "../customers/create-address-dialog.vue";
import { getDetails } from "@/api/commerce/address";
import { systemDisplay } from "@/utils/commerce";

const props = defineProps<{ customerId: string }>();
const emit = defineEmits<{
  (e: "selected", value: Address): void;
}>();
const selected = ref<number>();
const customer = ref<CustomerEdit>();
const { t } = useI18n();
const showCreateAddressDialog = ref(false);
const details = ref<any[]>([]);

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
    for (let i = 0; i < customer.value.addresses.length; i++) {
      const m = details.value[i] || customer.value.addresses[i];
      result.push(
        `${m.firstName ?? ""} ${m.lastName ?? ""} ${
          m.phone ?? ""
        } ${systemDisplay(
          m?.countryDetail?.nameTranslations,
          m.country
        )} ${systemDisplay(
          m?.provinceDetail?.nameTranslations,
          m.province
        )} ${systemDisplay(m?.cityDetail?.nameTranslations, m.city)} ${
          m.address1 ?? ""
        } ${m.address2 ?? ""} ${m.zip ?? ""}`
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

watch(
  () => customer.value?.addresses,
  async () => {
    if (!customer.value?.addresses.length) return;
    details.value = await getDetails(customer.value.addresses);
  },
  {
    immediate: true,
    deep: true,
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
