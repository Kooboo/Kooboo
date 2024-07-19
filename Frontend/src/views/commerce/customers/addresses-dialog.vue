<script lang="ts" setup>
import { editCustomer, getCustomerEdit } from "@/api/commerce/customer";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import type { Address, CustomerEdit } from "@/api/commerce/customer";
import CreateAddressDialog from "./create-address-dialog.vue";
import EditAddressDialog from "./edit-address-dialog.vue";

const { t } = useI18n();
const show = ref(true);
const showCreateAddressDialog = ref(false);
const showEditAddressDialog = ref(false);
const selected = ref<Address>();

const props = defineProps<{
  modelValue: boolean;
  id: string;
}>();

const model = ref<CustomerEdit>();

getCustomerEdit(props.id).then((rsp) => {
  model.value = rsp;
});

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

async function onSave() {
  await editCustomer(model.value);
  emit("reload");
  show.value = false;
}

function onCreate(value: Address) {
  model.value!.addresses.forEach((m) => (m.isDefault = false));
  model.value!.addresses.push(value);
}

function onUpdate(value: Address) {
  var index = model.value!.addresses.indexOf(selected.value!);
  model.value!.addresses.splice(index, 1, value);
}

function setAsDefault(value: Address) {
  model.value!.addresses.forEach((m) => (m.isDefault = false));
  value.isDefault = true;
}

function onDelete(index: number) {
  model.value!.addresses.splice(index, 1);
  if (
    model.value!.addresses.length &&
    model.value!.addresses.every((e) => !e.isDefault)
  ) {
    model.value!.addresses[0].isDefault = true;
  }
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="800px"
    :title="t('common.addresses')"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <el-scrollbar v-if="model" max-height="400px">
      <div v-if="model.addresses.length" class="space-y-8">
        <div
          v-for="(item, index) of model.addresses"
          :key="index"
          class="border border-gray p-12 rounded-normal space-y-8"
        >
          <div class="flex items-center space-x-8">
            <ElTag v-if="item.isDefault" round type="success">{{
              t("common.default")
            }}</ElTag>
            <ElTag
              v-else
              round
              type="info"
              class="cursor-pointer"
              @click="setAsDefault(item)"
              >{{ t("common.setAsDefault") }}</ElTag
            >
            <div class="flex-1" />
            <el-tooltip placement="top" :content="t('common.edit')">
              <el-icon
                class="iconfont icon-a-writein hover:text-blue text-l"
                @click="
                  selected = item;
                  showEditAddressDialog = true;
                "
              />
            </el-tooltip>
            <el-tooltip placement="top" :content="t('common.delete')">
              <el-icon
                class="iconfont icon-delete text-orange text-l"
                @click="onDelete(index)"
              />
            </el-tooltip>
          </div>
          <div class="p-4 space-y-4">
            <div>{{ item.firstName }} {{ item.lastName }} {{ item.phone }}</div>
            <div>{{ item.country }}</div>
            <div>{{ item.city }} {{ item.province }} {{ item.zip }}</div>
            <div>{{ item.address1 }}</div>
          </div>
        </div>
      </div>
      <el-empty v-else />
    </el-scrollbar>

    <template #footer>
      <div class="flex">
        <el-button round @click="showCreateAddressDialog = true">
          <div class="flex items-center">
            <el-icon class="mr-16 iconfont icon-a-addto" />
            {{ t("common.create") }}
          </div>
        </el-button>
        <div class="flex-1" />
        <DialogFooterBar @confirm="onSave" @cancel="show = false" />
      </div>
    </template>
  </el-dialog>
  <CreateAddressDialog
    v-if="showCreateAddressDialog"
    v-model="showCreateAddressDialog"
    @create="onCreate"
  />
  <EditAddressDialog
    v-if="showEditAddressDialog"
    v-model="showEditAddressDialog"
    :model="selected!"
    @update="onUpdate"
  />
</template>
