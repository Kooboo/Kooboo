<script lang="ts" setup>
import type { PagingResult } from "@/api/commerce/common";
import {
  customerMemberships,
  changeMembership,
  renewMembership,
  type CustomerPoint,
} from "@/api/commerce/loyalty";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useTime } from "@/hooks/use-date";
import SelectMembershipDialog from "@/views/commerce/components/select-membership-dialog.vue";
import { showConfirm } from "@/components/basic/confirm";

const { t } = useI18n();
const props = defineProps<{
  memberId: string;
  membershipId?: string;
}>();

const emit = defineEmits<{
  (e: "reload"): void;
}>();

const pagingResult = ref<PagingResult<CustomerPoint>>();
const showSelectMembershipDialog = ref(false);

const params = {
  pageIndex: 1,
  pageSize: 5,
  id: props.memberId,
};

async function load(index: number) {
  params.pageIndex = index;
  pagingResult.value = await customerMemberships(params);
}

async function change({ id, name }: any) {
  await showConfirm(t("common.changeMembershipTip", { membership: name }));
  await changeMembership(props.memberId, id);
  load(1);
  emit("reload");
}

async function renew() {
  await showConfirm(t("common.renewMembershipTip"));
  await renewMembership(props.memberId);
  load(1);
}

load(1);
</script>

<template>
  <div v-if="membershipId">
    <div class="flex items-center pb-12 space-x-16">
      <el-button
        v-hasPermission="{ feature: 'loyalty', action: 'edit' }"
        round
        type="primary"
        @click="showSelectMembershipDialog = true"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-Refresh" />
          {{ t("common.change") }}
        </div>
      </el-button>

      <el-button
        v-hasPermission="{ feature: 'loyalty', action: 'edit' }"
        round
        @click="renew"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.renew") }}
        </div>
      </el-button>
      <div class="flex-1" />
    </div>
    <ElTable
      v-if="pagingResult"
      :data="pagingResult.list"
      class="el-table--gray"
      :row-class-name="({ row }:any) => (row.disabled ? 'opacity-50' : '')"
    >
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          {{ row.membership }}
          <ElTag v-if="row.isRenew">{{ t("common.renew") }}</ElTag>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.startAt')">
        <template #default="{ row }">
          {{ useTime(row.startAt + "Z") }}
        </template>
      </el-table-column>

      <el-table-column :label="t('common.endAt')">
        <template #default="{ row }">
          {{ useTime(row.endAt + "Z") }}
        </template>
      </el-table-column>
    </ElTable>
    <div class="text-center">
      <el-pagination
        class="py-8"
        layout="prev, pager, next"
        hide-on-single-page
        :page-size="pagingResult?.pageSize"
        :current-page="pagingResult?.pageIndex"
        :total="pagingResult?.count"
        @current-change="load"
      />
    </div>
  </div>
  <div v-else class="flex items-center justify-center p-8">
    <el-button round type="primary" @click="showSelectMembershipDialog = true">
      <div class="flex items-center">
        <el-icon class="mr-16 iconfont icon-Collection" />
        {{ t("common.joinMembership") }}
      </div>
    </el-button>
  </div>
  <SelectMembershipDialog
    v-if="showSelectMembershipDialog"
    v-model="showSelectMembershipDialog"
    :excludes="membershipId ? [membershipId] : []"
    @selected="change"
  />
</template>
