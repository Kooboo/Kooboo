<template>
  <div class="p-24">
    <Breadcrumb :name="routeName" />
    <template v-if="!loading">
      <template v-if="!errors.length">
        <div class="flex items-center py-24 space-x-16">
          <el-button
            v-hasPermission="{
              feature: 'database',
              action: 'edit',
            }"
            round
            data-cy="create-table"
            @click="visibleCreateTable = true"
          >
            <el-icon class="iconfont icon-a-addto" />
            {{ t("common.createTable") }}
          </el-button>
        </div>
        <KTable
          :data="tables"
          show-check
          :permission="{ feature: 'database', action: 'delete' }"
          @delete="onDelete"
        >
          <el-table-column :label="t('common.name')">
            <template #default="{ row }">
              <router-link
                :to="
                  appendQueryToRoute({
                    name: 'table-data',
                    query: { table: row },
                  })
                "
                data-cy="name"
              >
                <span class="text-blue cursor-pointer">
                  {{ row }}
                </span>
              </router-link>
            </template>
          </el-table-column>
          <el-table-column width="90" align="right">
            <template #default="{ row }">
              <router-link
                :to="
                  appendQueryToRoute({
                    name: 'table-columns',
                    query: { table: row },
                  })
                "
                data-cy="setting"
                class="mx-8"
              >
                <el-tooltip placement="top" :content="t('common.setting')">
                  <el-icon
                    class="iconfont icon-a-setup hover:text-blue text-l"
                  />
                </el-tooltip>
              </router-link>
            </template>
          </el-table-column>
        </KTable>
      </template>
      <GuidInfo v-else>
        <p v-for="(item, index) in errors" :key="index">{{ item }}</p>
        <template #button>
          <el-button
            v-if="hasConfig"
            v-hasPermission="{
              feature: 'config',
              action: 'edit',
            }"
            type="primary"
            round
            @click="gotoConfig"
          >
            <el-icon class="iconfont icon-a-setup" />
            {{ t("common.configNow") }}
          </el-button>
        </template>
      </GuidInfo>
    </template>
    <CreateTableDialog
      v-model="visibleCreateTable"
      :db-type="dbType"
      @create-success="load"
    />
  </div>
</template>

<script lang="ts" setup>
import KTable from "@/components/k-table";
import CreateTableDialog from "./components/create-table-dialog.vue";

import { onMounted, ref } from "vue";
import { deleteTables, getTables } from "@/api/database";
import { useTable } from "./use-table";
import { useRoute, useRouter } from "vue-router";
import type { AxiosError } from "axios";
import GuidInfo from "@/components/guid-info/index.vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";
const { t } = useI18n();
const router = useRouter();
const tables = ref<string[]>();
const visibleCreateTable = ref(false);
const route = useRoute();
const routeName = route.meta.title as string;
const loading = ref(true);
const errors = ref<string[]>([]);
onMounted(async () => {
  try {
    await load();
  } catch (e) {
    const config = e as AxiosError;
    const response = config.response?.data;
    if (Array.isArray(response)) {
      errors.value = response;
    }
  } finally {
    loading.value = false;
  }
});

const { dbType, appendQueryToRoute } = useTable();
const hasConfig = dbType !== "Database" && dbType !== "Sqlite";
async function load() {
  const records = await getTables(dbType);
  tables.value = records.sort();
}
async function onDelete(names: string[]) {
  try {
    await showDeleteConfirm(names.length);
    await deleteTables(dbType, { names });
    load();
  } catch {
    void 0;
  }
}

function gotoConfig() {
  router.push(useRouteSiteId({ name: "config", query: { group: "Database" } }));
}
</script>
