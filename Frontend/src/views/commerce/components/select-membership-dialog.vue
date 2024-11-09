<script lang="ts" setup>
import type { MembershipListItem } from "@/api/commerce/loyalty";
import { useCommerceStore } from "@/store/commerce";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import CurrencyAmount from "../components/currency-amount.vue";
import TimeDuration from "../components/time-duration.vue";

const { t } = useI18n();
const show = ref(true);
const commerceStore = useCommerceStore();

const props = defineProps<{
  modelValue: boolean;
  excludes?: string[];
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "selected", value: MembershipListItem): void;
}>();

commerceStore.loadMemberships();

function onRowClick(row: MembershipListItem) {
  emit("selected", row);
  show.value = false;
}

const list = computed(() => {
  var result = commerceStore.memberships;
  if (props.excludes) {
    result = result?.filter((f) => !props.excludes?.includes(f.id));
  }
  return result;
});
</script>

<template>
  <el-dialog
    :model-value="show"
    width="800px"
    :title="t('commerce.selectMembership')"
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
            </div>
          </template>
        </el-table-column>

        <el-table-column :label="t('common.description')" align="center">
          <template #default="{ row }">
            {{ row.description }}
          </template>
        </el-table-column>

        <el-table-column :label="t('common.price')" align="center">
          <template #default="{ row }">
            <div v-if="row.allowPurchase">
              <CurrencyAmount :amount="row.price" />
              <span> / </span>
              <TimeDuration
                :model-value="row.duration"
                :unit="row.durationUnit"
                readonly
              />
            </div>
          </template>
        </el-table-column>
      </ElTable>
    </el-scrollbar>
  </el-dialog>
</template>
