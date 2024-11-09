<script lang="ts" setup>
import type { OrderLine } from "@/api/commerce/order";
import { bytesToSize } from "@/utils/common";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps<{
  line: OrderLine;
}>();
</script>

<template>
  <div>
    <el-popover placement="left" width="600" :disabled="!line.delivered">
      <template #reference>
        <ElTag
          v-if="line.delivered"
          round
          type="success"
          class="cursor-default"
          >{{ t("commerce.shipped") }}</ElTag
        >
        <ElTag v-else-if="line.errorMessage" round type="danger"
          >{{ t("common.failed") }}
          <Tooltip :tip="line.errorMessage" custom-class="ml-4" />
        </ElTag>
        <ElTag v-else round type="info">{{ t("commerce.unshipped") }} </ElTag>
      </template>
      <el-descriptions :title="t('common.detail')" :column="1" border>
        <template v-if="line.isDigital">
          <el-descriptions-item
            v-for="item of line.digitalItems"
            :key="item.id"
            :label="item.name"
            >{{
              item.type == "file" ? bytesToSize(item.size!) : item.value
            }}</el-descriptions-item
          >
        </template>
        <template v-else>
          <el-descriptions-item :label="t('commerce.shippingCarrier')">{{
            line.shippingCarrier
          }}</el-descriptions-item>
          <el-descriptions-item :label="t('commerce.trackingNumber')">{{
            line.trackingNumber
          }}</el-descriptions-item>
        </template>
      </el-descriptions>
    </el-popover>
  </div>
</template>
