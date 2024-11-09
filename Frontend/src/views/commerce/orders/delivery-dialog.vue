<script lang="ts" setup>
import { onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { OrderLine } from "@/api/commerce/order";
import { deliveryOrder } from "@/api/commerce/order";
import OrderLinePreview from "./order-line-preview.vue";
import { ElButton } from "element-plus";
import { getShippings, type ShippingListItem } from "@/api/commerce/shipping";
import DropdownInput from "@/components/basic/dropdown-input.vue";

const { t } = useI18n();
const show = ref(true);

const shippings = ref<ShippingListItem[]>([]);

onMounted(async () => {
  shippings.value = await getShippings();
});

const props = defineProps<{
  modelValue: boolean;
  id: string;
  lines: OrderLine[];
}>();

const selected = ref<string[]>([]);
const partialDelivery = ref(false);

const lines: OrderLine[] = JSON.parse(
  JSON.stringify(props.lines.filter((f) => !f.delivered))
);
const physicalLines = ref<OrderLine[]>(lines.filter((f) => !f.isDigital));
const digitalLines = ref<OrderLine[]>(lines.filter((f) => f.isDigital));

const model = ref({
  id: props.id,
  shippingCarrier: "",
  trackingNumber: "",
});

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

async function onSave() {
  const data: any = {
    id: model.value.id,
    items: [],
  };

  for (const line of lines) {
    if (partialDelivery.value && !selected.value.includes(line.id)) continue;
    data.items.push({
      id: line.id,
      shippingCarrier: model.value.shippingCarrier,
      trackingNumber: model.value.trackingNumber,
      digitalItems: line.digitalItems,
    });
  }
  await deliveryOrder(data);
  emit("reload");
  show.value = false;
}

function changeSelected(id: string) {
  if (selected.value.includes(id)) {
    selected.value = selected.value.filter((f) => f != id);
  } else {
    selected.value.push(id);
  }
}

function matchingCarrier(value: string) {
  for (const i of shippings.value) {
    if (!i.trackingNumberMatching) return;
    if (
      new RegExp(i.trackingNumberMatching?.toLowerCase()).test(
        value?.toLowerCase()
      )
    ) {
      model.value.shippingCarrier = i.name;
    }
  }
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="800px"
    :title="t('commerce.delivery')"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <ElForm v-if="model" label-position="top">
      <template v-if="physicalLines.length">
        <el-divider v-if="digitalLines.length" class="mt-0">{{
          t("common.physicalProduct")
        }}</el-divider>
        <div class="grid grid-cols-2 gap-8">
          <ElFormItem :label="t('commerce.trackingNumber')">
            <ElInput v-model="model.trackingNumber" @change="matchingCarrier" />
          </ElFormItem>

          <ElFormItem :label="t('commerce.shippingCarrier')">
            <DropdownInput
              v-model="model.shippingCarrier"
              :options="shippings.map((m) => m.name)"
            />
          </ElFormItem>
        </div>
        <div class="space-y-4">
          <OrderLinePreview
            v-for="item of physicalLines"
            :key="item.id"
            :line="item"
            :show-selected="partialDelivery"
            :selected="selected.includes(item.id)"
            @update:selected="changeSelected(item.id)"
          />
        </div>
      </template>
      <template v-if="digitalLines.length">
        <el-divider>{{ t("common.digitalProduct") }}</el-divider>
        <div class="space-y-4">
          <OrderLinePreview
            v-for="item of digitalLines"
            :key="item.id"
            :line="item"
            :show-selected="partialDelivery"
            :selected="selected.includes(item.id)"
            @update:selected="changeSelected(item.id)"
          />
        </div>
      </template>
    </ElForm>

    <template #footer>
      <div class="flex">
        <el-checkbox
          v-if="lines.length > 1"
          v-model="partialDelivery"
          :label="t('commerce.partialDelivery')"
        />
        <div class="flex-1" />
        <ElButton @click="show = false">{{ t("common.cancel") }}</ElButton>
        <ElButton type="primary" @click="onSave">{{
          t("commerce.delivery")
        }}</ElButton>
      </div>
    </template>
  </el-dialog>
</template>
