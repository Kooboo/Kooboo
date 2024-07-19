<script lang="ts" setup>
import { useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import KTable from "@/components/k-table";
import { ref } from "vue";
import type { ListItem } from "@/api/layout/types";
import RelationsTag from "@/components/relations/relations-tag.vue";
import { useTime } from "@/hooks/use-date";
import IconButton from "@/components/basic/icon-button.vue";
import CopyDialog from "./copy-dialog.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useLayoutStore } from "@/store/layout";

import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
import { showDeleteConfirm } from "@/components/basic/confirm";
const { t } = useI18n();
const layoutStore = useLayoutStore();
const siteStore = useSiteStore();
const router = useRouter();
const selected = ref<ListItem>();
const showCopyDialog = ref(false);

const onCopy = (item: ListItem) => {
  if (siteStore.hasAccess("layout", "edit")) {
    selected.value = item;
    showCopyDialog.value = true;
  }
};

const onDelete = async (rows: ListItem[]) => {
  await showDeleteConfirm(rows.length);
  layoutStore.deleteLayouts(rows.map((m) => m.id));
};

layoutStore.load();
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.layouts')" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{
          feature: 'layout',
          action: 'edit',
        }"
        round
        data-cy="new-layout"
        @click="router.push(useRouteSiteId({ name: 'layout-edit' }))"
      >
        <div class="flex items-center">
          <el-icon class="iconfont icon-a-addto" />
          {{ t("common.newLayout") }}
        </div>
      </el-button>
    </div>

    <KTable
      :data="layoutStore.list"
      show-check
      :permission="{
        feature: 'layout',
        action: 'delete',
      }"
      @delete="onDelete"
    >
      <template #bar="{ selected }">
        <IconButton
          v-if="selected.length === 1"
          :permission="{
            feature: 'layout',
            action: 'edit',
          }"
          icon="icon-copy"
          :tip="t('common.copy')"
          circle
          data-cy="copy"
          @click="onCopy(selected[0])"
        />
      </template>
      <el-table-column :label="t('common.name')" prop="name">
        <template #default="{ row }">
          <router-link
            :to="
              useRouteSiteId({
                name: 'layout-edit',
                query: {
                  id: row.id,
                },
              })
            "
          >
            <span
              :title="row.name"
              class="ellipsis text-blue cursor-pointer"
              data-cy="name"
              >{{ row.name }}</span
            >
          </router-link>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.usedBy')">
        <template #default="{ row }">
          <RelationsTag :id="row.id" :relations="row.relations" type="Layout" />
        </template>
      </el-table-column>
      <el-table-column :label="t('common.lastModified')" prop="lastModified">
        <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
      </el-table-column>
      <el-table-column width="60px" align="right">
        <template #default="{ row }">
          <IconButton
            :permission="{ feature: 'site', action: 'log' }"
            icon="icon-time"
            :tip="t('common.version')"
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
    <CopyDialog
      v-if="showCopyDialog"
      v-model="showCopyDialog"
      :selected="selected!"
    />
  </div>
</template>
