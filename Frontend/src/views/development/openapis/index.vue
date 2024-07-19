<script lang="ts" setup>
import { deletes, getList, updateDoc } from "@/api/openapi";
import type { OpenApi } from "@/api/openapi/types";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import IconButton from "@/components/basic/icon-button.vue";
import KTable from "@/components/k-table";
import { useTime } from "@/hooks/use-date";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { ref } from "vue";
import { useRouter } from "vue-router";

import { useI18n } from "vue-i18n";
import { showDeleteConfirm } from "@/components/basic/confirm";
const { t } = useI18n();
const router = useRouter();
const data = ref<OpenApi[]>([]);

const load = async () => {
  data.value = await getList();
};

const onDelete = async (rows: OpenApi[]) => {
  await showDeleteConfirm(rows.length);
  await deletes(rows.map((m) => m.id));
  load();
};

load();
</script>

<template>
  <div class="p-24">
    <Breadcrumb name="OpenApis" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{
          feature: 'openApi',
          action: 'edit',
        }"
        round
        data-cy="create"
        @click="router.push(useRouteSiteId({ name: 'openapi-edit' }))"
      >
        <div class="flex items-center">
          <el-icon class="iconfont icon-a-addto" />
          {{ t("common.create") }}
        </div>
      </el-button>
    </div>
    <KTable
      :data="data"
      show-check
      sort="lastModified"
      :permission="{ feature: 'openApi', action: 'delete' }"
      @delete="onDelete"
    >
      <el-table-column width="240px" :label="t('common.name')" prop="name">
        <template #default="{ row }">
          <span class="ellipsis" :title="row.name" data-cy="name">{{
            row.name
          }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.type')" width="90">
        <template #default="{ row }">
          <el-tag size="small" class="rounded-full" data-cy="type">{{
            row.type
          }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="URL">
        <template #default="{ row }">
          <span class="ellipsis" data-cy="url">{{ row.url }}</span>
        </template>
      </el-table-column>
      <el-table-column
        width="180px"
        :label="t('common.lastModified')"
        prop="lastModified"
      >
        <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
      </el-table-column>
      <el-table-column width="120px" align="right">
        <template #default="{ row }">
          <IconButton
            v-if="row.type == 'url'"
            icon="icon-Refresh"
            :tip="t('common.updateDocument')"
            @click="updateDoc(row.id)"
          />

          <router-link
            :to="
              useRouteSiteId({
                name: 'openapi-authorizes',
                query: { id: row.id },
              })
            "
            data-cy="authorize"
            class="mx-8"
          >
            <el-tooltip placement="top" :content="t('common.authorize')">
              <el-icon class="iconfont icon-lock hover:text-blue text-l" />
            </el-tooltip>
          </router-link>

          <router-link
            :to="
              useRouteSiteId({
                name: 'openapi-edit',
                query: { id: row.id },
              })
            "
            data-cy="edit"
            class="mx-8"
          >
            <el-tooltip placement="top" :content="t('common.edit')">
              <el-icon class="iconfont icon-a-writein hover:text-blue text-l" />
            </el-tooltip>
          </router-link>
        </template>
      </el-table-column>
    </KTable>
  </div>
</template>
