<script lang="ts" setup>
import type { PagingParams } from "@/api/commerce/common";
import type { Member } from "@/api/commerce/loyalty";
import { getMembers, type MemberPagingResult } from "@/api/commerce/loyalty";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import DetailDialog from "./member/detail-dialog.vue";
import MembershipStatus from "./member/membership-status.vue";
import TruncateContent from "@/components/basic/truncate-content.vue";

const pagingResult = ref<MemberPagingResult>();
const { t } = useI18n();
const selected = ref<Member>();
const showDetailDialog = ref(false);

const queryParams = ref<PagingParams & { keyword: string }>({
  pageIndex: 1,
  pageSize: 10,
  keyword: "",
});

async function load(pageIndex?: number) {
  if (pageIndex) queryParams.value.pageIndex = pageIndex;
  pagingResult.value = await getMembers(queryParams.value);
}

load();
</script>

<template>
  <div>
    <div class="flex items-center mb-12 space-x-16">
      <div class="flex-1" />
      <SearchInput
        v-model="queryParams.keyword"
        :placeholder="t('common.search')"
        class="w-248px"
        @keydown.enter="load()"
      />
    </div>
    <KTable
      v-if="pagingResult"
      :data="pagingResult.list"
      :pagination="{
        currentPage: pagingResult.pageIndex,
        totalCount: pagingResult.count,
        pageSize: pagingResult.pageSize,
      }"
      @change="load"
    >
      <el-table-column :label="t('common.compellation')">
        <template #default="{ row }">
          <TruncateContent :tip="`${row.firstName} ${row.lastName}`"
            >{{ row.firstName }} {{ row.lastName }}</TruncateContent
          >
        </template>
      </el-table-column>
      <el-table-column :label="t('common.email')">
        <template #default="{ row }">
          <TruncateContent :tip="row.email">{{ row.email }}</TruncateContent>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.phone')">
        <template #default="{ row }">
          <div>{{ row.phone }}</div>
        </template>
      </el-table-column>

      <el-table-column :label="t('common.membership')">
        <template #default="{ row }">
          <MembershipStatus :status="row.status" />
        </template>
      </el-table-column>

      <el-table-column :label="t('commerce.points')" align="center" width="130">
        <template #default="{ row }">
          <ElTag round>
            {{ row.points }}
          </ElTag>
        </template>
      </el-table-column>

      <el-table-column align="right" width="60" fixed="right">
        <template #default="{ row }">
          <el-tooltip placement="top" :content="t('common.detail')">
            <el-icon
              class="iconfont icon-eyes hover:text-blue text-l"
              @click="
                selected = row;
                showDetailDialog = true;
              "
            />
          </el-tooltip>
        </template>
      </el-table-column>
    </KTable>
    <DetailDialog
      v-if="showDetailDialog"
      :id="selected!.id"
      v-model="showDetailDialog"
      @reload="load"
    />
  </div>
</template>
