<script lang="ts" setup>
import type { OrderLine } from "@/api/commerce/order";
import ImageCover from "@/components/basic/image-cover.vue";
import { buildOptionsDisplay } from "../products-management/product-variant";
import { bytesToSize } from "@/utils/common";
import IconButton from "@/components/basic/icon-button.vue";
import { useI18n } from "vue-i18n";
import DigitalItemsDialog from "./digital-items-dialog.vue";
import { ref } from "vue";

const { t } = useI18n();
defineProps<{ line: OrderLine; showSelected: boolean; selected: boolean }>();
const showEditDigitalItemsDialog = ref(false);
defineEmits<{
  (e: "update:selected"): void;
}>();
</script>

<template>
  <div class="bg-gray/50 rounded-normal">
    <div class="p-8 flex">
      <el-checkbox
        v-if="showSelected"
        v-model="selected"
        class="w-24"
        @click.stop.prevent="$emit('update:selected')"
      />
      <div class="flex-1 flex justify-between items-center">
        <ImageCover :model-value="line.image" size="small" />
        <div>
          <div>{{ line.title }}</div>
          <div class="text-s">{{ buildOptionsDisplay(line.options) }}</div>
        </div>
        <div>x{{ line.totalQuantity }}</div>
      </div>
    </div>
    <div v-if="line.isDigital" class="text-s p-8 flex items-center">
      <div v-if="line.digitalItems.length" class="flex-1">
        <div v-for="item of line.digitalItems" :key="item.id">
          <p class="truncate">
            {{ item.name }} :
            {{ item.type == "file" ? bytesToSize(item.size!) : item.value }}
          </p>
        </div>
      </div>
      <div v-else class="flex-1">-</div>
      <div class="flex-shrink-0 w-32">
        <IconButton
          icon="icon-a-writein"
          :tip="t('common.editDigitalItems')"
          @click="showEditDigitalItemsDialog = true"
        />
      </div>
    </div>

    <DigitalItemsDialog
      v-if="showEditDigitalItemsDialog"
      v-model="showEditDigitalItemsDialog"
      :line="line"
    />
  </div>
</template>
