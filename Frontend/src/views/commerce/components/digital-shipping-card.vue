<script lang="ts" setup>
import {
  getShipping,
  type ShippingListItem,
} from "@/api/commerce/digital-shipping";
import { onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import SelectShippingDialog from "../components/select-digital-shipping-dialog.vue";
import { emptyGuid } from "@/utils/guid";

const props = defineProps<{ modelValue?: string; readonly?: boolean }>();

const emit = defineEmits<{
  (e: "update:model-value", value: string | undefined): void;
}>();

const { t } = useI18n();
const showSelectShippingDialog = ref(false);
const shipping = ref<ShippingListItem>();

onMounted(async () => {
  shipping.value = await getShipping(props.modelValue || emptyGuid);
});

function onDelete() {
  shipping.value = undefined;
  emit("update:model-value", undefined);
}

function onSelected(value: ShippingListItem) {
  shipping.value = value;
  emit("update:model-value", value.id);
}
</script>

<template>
  <div
    v-if="shipping"
    class="border border-gray p-12 rounded-normal w-full h-115px w-max-400px dark:bg-444 dark:text-gray"
  >
    <div class="flex items-center space-x-8">
      <div class="flex-1" />
      <el-tooltip
        v-if="!readonly"
        placement="top"
        :content="t('common.select')"
      >
        <el-icon
          class="iconfont icon-a-writein text-blue text-l"
          @click="showSelectShippingDialog = true"
        />
      </el-tooltip>
    </div>
    <div class="p-4">
      <div class="text-l">{{ shipping.name }}</div>
      <div class="text-s truncate" :title="shipping.description">
        {{ shipping.description }}
      </div>
    </div>
  </div>
  <IconButton
    v-else-if="!readonly"
    circle
    class="text-blue"
    icon="icon-a-addto"
    :tip="t('common.add')"
    @click="showSelectShippingDialog = true"
  />
  <SelectShippingDialog
    v-if="showSelectShippingDialog"
    v-model="showSelectShippingDialog"
    @selected="onSelected"
  />
</template>
