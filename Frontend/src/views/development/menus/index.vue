<script lang="ts" setup>
import KTable from "@/components/k-table";
import type { MenuItem } from "@/api/menu/types";
import { ref } from "vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import AddDialog from "./components/add-dialog.vue";
import RelationsTag from "@/components/relations/relations-tag.vue";
import { useTime } from "@/hooks/use-date";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { useMenuStore } from "@/store/menu";

import { useI18n } from "vue-i18n";
import { showDeleteConfirm } from "@/components/basic/confirm";
const { t } = useI18n();
const showAddDialog = ref(false);
const menuStore = useMenuStore();

const onDelete = async (rows: MenuItem[]) => {
  await showDeleteConfirm(rows.length);
  await menuStore.deleteMenus(rows.map((m) => m.id));
};

menuStore.load();
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.menus')" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{
          feature: 'menu',
          action: 'edit',
        }"
        round
        data-cy="add-menu"
        @click="showAddDialog = true"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.addMenu") }}
        </div>
      </el-button>
    </div>
    <KTable
      :data="menuStore.list"
      show-check
      :permission="{
        feature: 'menu',
        action: 'delete',
      }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <span
            :title="row.name"
            class="rounded-full cursor-pointer text-blue ellipsis"
            data-cy="name"
            @click="
              $router.push(
                useRouteSiteId({
                  name: 'menu-edit',
                  query: {
                    id: row.id,
                  },
                })
              )
            "
          >
            {{ row.name }}
          </span>
        </template>
      </el-table-column>
      <el-table-column prop="relations" :label="t('common.usedBy')">
        <template #default="{ row }">
          <RelationsTag :id="row.id" :relations="row.relations" type="Menu" />
        </template>
      </el-table-column>
      <el-table-column
        prop="lastModified"
        :label="t('common.lastModified')"
        align="center"
      >
        <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
      </el-table-column>
      <el-table-column align="right" width="80px">
        <template #default="{ row }">
          <IconButton
            icon="icon-time"
            :tip="t('common.version')"
            :permission="{
              feature: 'site',
              action: 'log',
            }"
            data-cy="versions"
            @click="
              $router.goLogVersions(
                row.keyHash,
                row.storeNameHash,
                row.tableNameHash
              )
            "
          />
        </template>
      </el-table-column>
    </KTable>

    <AddDialog v-if="showAddDialog" v-model="showAddDialog" />
  </div>
</template>
