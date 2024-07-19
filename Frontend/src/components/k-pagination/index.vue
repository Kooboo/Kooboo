<template>
  <template v-if="infinite">
    <div class="flex justify-center">
      <el-button
        v-if="(pagination.totalCount ?? 0) > (pagination.pageSize ?? 0)"
        @click="handleLoadMore"
      >
        {{ t("common.loadMore") }}
      </el-button>
      <div v-else>{{ t("common.noMoreData") }}</div>
    </div>
  </template>
  <div v-else-if="pagination.totalCount">
    <el-pagination
      v-model:currentPage="pagination.currentPage"
      :page-size="pagination.pageSize"
      layout="prev, pager, next"
      :total="pagination.totalCount"
      hide-on-single-page
      @current-change="handlePageChange"
    />
  </div>
</template>

<script lang="ts" setup>
import type { Pagination } from "../k-table/types";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const props = defineProps<{
  pagination: Pagination;
  infinite: boolean;
}>();

const emits = defineEmits<{
  (e: "current-change", value: number): void;
}>();

function handleLoadMore() {
  handlePageChange((props.pagination.currentPage ?? 0) + 1);
}

function handlePageChange(pageNumber: number) {
  emits("current-change", pageNumber);
}
</script>
