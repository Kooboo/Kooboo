<script lang="ts" setup>
import { ref, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import type { ShippingListItem } from "@/api/commerce/digital-shipping";
import { getShippings } from "@/api/commerce/digital-shipping";

const { t } = useI18n();
const show = ref(true);

defineProps<{
  modelValue: boolean;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "selected", value: ShippingListItem): void;
}>();

const list = ref<ShippingListItem[]>([]);

async function load() {
  list.value = await getShippings();
}

onMounted(async () => {
  load();
});

function onRowClick(row: ShippingListItem) {
  emit("selected", row);
  show.value = false;
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="800px"
    :title="t('commerce.selectShipping')"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <el-scrollbar max-height="400px">
      <ElTable
        :data="list"
        class="el-table--gray mb-24"
        @row-click="onRowClick"
      >
        <el-table-column :label="t('common.name')">
          <template #default="{ row }">
            <div class="flex items-center">
              <span class="text-black dark:text-[#cfd3dc]">{{ row.name }}</span>
              <ElTag v-if="row.isDefault" round size="small">{{
                t("common.default")
              }}</ElTag>
            </div>
          </template>
        </el-table-column>

        <el-table-column :label="t('common.description')" align="center">
          <template #default="{ row }">
            {{ row.description }}
          </template>
        </el-table-column>
      </ElTable>
    </el-scrollbar>
  </el-dialog>
</template>
