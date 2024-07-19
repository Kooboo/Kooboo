<script lang="ts" setup>
import { useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import KTable from "@/components/k-table";
import { ref } from "vue";
import type { View } from "@/api/view/types";
import RelationsTag from "@/components/relations/relations-tag.vue";
import { useTime } from "@/hooks/use-date";
import IconButton from "@/components/basic/icon-button.vue";
import CopyDialog from "./copy-dialog.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useViewStore } from "@/store/view";
import { openInNewTab } from "@/utils/url";

import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
import { showDeleteConfirm } from "@/components/basic/confirm";
const { t } = useI18n();
const viewStore = useViewStore();
const siteStore = useSiteStore();
const router = useRouter();
const selectedItem = ref<View>();
const showCopyDialog = ref(false);

const onCopy = (item: View) => {
  if (siteStore.hasAccess("view", "edit")) {
    selectedItem.value = item;
    showCopyDialog.value = true;
  }
};

const onDelete = async (rows: View[]) => {
  await showDeleteConfirm(rows.length);
  viewStore.deleteViews(rows.map((m) => m.id));
};

viewStore.load();
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.views')" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{ feature: 'view', action: 'edit' }"
        round
        data-cy="new-view"
        @click="router.push(useRouteSiteId({ name: 'view-edit' }))"
      >
        <div class="flex items-center">
          <el-icon class="iconfont icon-a-addto" />
          {{ t("common.newView") }}
        </div>
      </el-button>
    </div>
    <KTable
      :data="viewStore.list"
      show-check
      :permission="{ feature: 'view', action: 'delete' }"
      @delete="onDelete"
    >
      <template #bar="{ selected }">
        <IconButton
          v-if="selected.length === 1"
          :permission="{ feature: 'view', action: 'edit' }"
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
                name: 'view-edit',
                query: {
                  id: row.id,
                },
              })
            "
            data-cy="name"
          >
            <span :title="row.name" class="ellipsis text-blue cursor-pointer">{{
              row.name
            }}</span>
          </router-link>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.usedBy')">
        <template #default="{ row }">
          <RelationsTag :id="row.id" :relations="row.relations" type="View" />
        </template>
      </el-table-column>
      <el-table-column :label="t('common.preview')">
        <template #default="{ row }">
          <a
            class="text-blue ellipsis cursor-pointer"
            :title="row.preview"
            @click="openInNewTab(row.preview)"
            >{{ row.relativeUrl }}</a
          >
        </template>
      </el-table-column>

      <el-table-column :label="t('common.lastModified')" prop="lastModified">
        <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
      </el-table-column>
      <el-table-column width="60px" align="center">
        <template #default="{ row }">
          <IconButton
            :permission="{ feature: 'site', action: 'log' }"
            icon="icon-time rounded-full"
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
      :selected="selectedItem!"
    />
  </div>
</template>
