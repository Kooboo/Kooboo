<script lang="ts" setup>
import type { MembershipListItem } from "@/api/commerce/loyalty";
import { deleteMemberships } from "@/api/commerce/loyalty";
import { showConfirm } from "@/components/basic/confirm";
import { useCommerceStore } from "@/store/commerce";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import CreateDialog from "./membership/create-dialog.vue";
import EditDialog from "./membership/edit-dialog.vue";
import TimeDuration from "@/views/commerce/components/time-duration.vue";
import CurrencyAmount from "@/views/commerce/components/currency-amount.vue";

const { t } = useI18n();
const commerceStore = useCommerceStore();
const showCreateDialog = ref(false);
const showEditDialog = ref(false);
const selected = ref<MembershipListItem>();

async function onDelete(rows: MembershipListItem[]) {
  await showConfirm(t("common.deleteMembershipTip"));
  await deleteMemberships(rows.map((m) => m.id));
  load();
}

function load() {
  commerceStore.loadMemberships(true);
}

load();
</script>

<template>
  <div class="space-y-12">
    <div class="flex items-center space-x-16">
      <el-button
        v-hasPermission="{ feature: 'loyalty', action: 'edit' }"
        round
        @click="showCreateDialog = true"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.createMembership") }}
        </div>
      </el-button>
    </div>
    <KTable
      v-if="commerceStore.memberships"
      :data="commerceStore.memberships"
      show-check
      :permission="{ feature: 'loyalty', action: 'delete' }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <div class="flex items-center">
            <span class="text-black dark:text-[#cfd3dc]">{{ row.name }}</span>
          </div>
        </template>
      </el-table-column>

      <el-table-column :label="t('common.description')">
        <template #default="{ row }">
          {{ row.description }}
        </template>
      </el-table-column>

      <el-table-column :label="t('common.level')" align="center">
        <template #default="{ row }">
          {{ row.priority }}
        </template>
      </el-table-column>
      <el-table-column :label="t('common.duration')" align="center">
        <template #default="{ row }">
          <TimeDuration
            :model-value="row.duration"
            :unit="row.durationUnit"
            readonly
          />
        </template>
      </el-table-column>
      <el-table-column :label="t('common.price')" align="center">
        <template #default="{ row }">
          <CurrencyAmount v-if="row.allowPurchase" :amount="row.price" />
        </template>
      </el-table-column>

      <el-table-column align="right" width="60">
        <template #default="{ row }">
          <el-tooltip placement="top" :content="t('common.edit')">
            <el-icon
              class="iconfont icon-a-writein hover:text-blue text-l"
              @click="
                selected = row;
                showEditDialog = true;
              "
            />
          </el-tooltip>
        </template>
      </el-table-column>
    </KTable>
  </div>
  <CreateDialog
    v-if="showCreateDialog"
    v-model="showCreateDialog"
    @reload="load"
  />
  <EditDialog
    v-if="showEditDialog"
    :id="selected!.id"
    v-model="showEditDialog"
    @reload="load"
  />
</template>
