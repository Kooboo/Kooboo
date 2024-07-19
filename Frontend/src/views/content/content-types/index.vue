<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.contentTypes')" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{ feature: 'contentType', action: 'edit' }"
        round
        data-cy="create"
        @click="create()"
      >
        <el-icon class="iconfont icon-a-addto" />
        {{ t("common.create") }}
      </el-button>
    </div>

    <KTable
      :data="list"
      show-check
      :permission="{
        feature: 'contentType',
        action: 'delete',
      }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')" prop="name" />
      <el-table-column
        :label="t('common.fields')"
        prop="propertyCount"
        align="center"
      />
      <el-table-column width="70" align="right">
        <template #default="{ row }">
          <router-link
            v-if="siteStore.hasAccess('contentType')"
            :to="useRouteSiteId({ name: 'contenttype', query: { id: row.id } })"
            data-cy="edit"
            class="mx-8"
          >
            <el-tooltip placement="top" :content="t('common.edit')">
              <el-icon class="iconfont icon-a-writein hover:text-blue text-l" />
            </el-tooltip>
          </router-link>
          <IconButton
            v-else
            :permission="{ feature: 'contentType', action: 'view' }"
            icon="icon-a-writein"
            :tip="t('common.edit')"
          />
        </template>
      </el-table-column>
    </KTable>
  </div>
</template>

<script lang="ts" setup>
import KTable from "@/components/k-table";
import { onMounted, ref } from "vue";
import type { ContentTypeItem } from "@/api/content/content-type";
import { getList, deletes } from "@/api/content/content-type";
import { useRouteSiteId } from "@/hooks/use-site-id";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";

import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import { useSiteStore } from "@/store/site";
const { t } = useI18n();
const router = useRouter();
const list = ref<ContentTypeItem[]>([]);
const siteStore = useSiteStore();

onMounted(() => {
  load();
});

async function load() {
  list.value = await getList();
}

async function onDelete(rows: ContentTypeItem[]) {
  await showDeleteConfirm(rows.length);
  const ids = rows.map((item) => item.id);
  await deletes({ ids });
  load();
}
function create() {
  router.push(useRouteSiteId({ name: "contenttype" }));
}
</script>
