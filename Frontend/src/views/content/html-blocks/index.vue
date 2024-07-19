<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.htmlBlocks')" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{
          feature: 'htmlBlock',
          action: 'edit',
        }"
        round
        data-cy="new"
        @click="create()"
      >
        <el-icon class="iconfont icon-a-addto" />
        {{ t("common.new") }}
      </el-button>
    </div>
    <KTable
      :data="list"
      show-check
      :permission="{
        feature: 'htmlBlock',
        action: 'delete',
      }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')" prop="name">
        <template #default="{ row }">
          <span class="ellipsis" :title="row.name" data-cy="name">{{
            row.name
          }}</span></template
        >
      </el-table-column>
      <el-table-column :label="t('common.usedBy')">
        <template #default="{ row }">
          <RelationsTag
            :id="row.id"
            :relations="row.relations"
            type="HtmlBlock"
          />
        </template>
      </el-table-column>
      <el-table-column :label="t('common.lastModified')">
        <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
      </el-table-column>
      <el-table-column width="90" align="right">
        <template #default="{ row }">
          <router-link
            :to="
              useRouteSiteId({
                name: 'htmlBlock-edit',
                query: { id: row?.id },
              })
            "
            data-cy="edit"
            class="mx-8"
          >
            <el-tooltip placement="top" :content="t('common.edit')">
              <el-icon class="iconfont icon-a-writein hover:text-blue text-l" />
            </el-tooltip>
          </router-link>

          <IconButton
            icon="icon-time"
            :tip="t('common.version')"
            :permission="{
              feature: 'site',
              action: 'log',
            }"
            data-cy="versions"
            @click="$router.goLogVersions(row.keyHash, row.storeNameHash)"
          />
        </template>
      </el-table-column>
    </KTable>
  </div>
</template>

<script lang="ts" setup>
import KTable from "@/components/k-table";
import { onMounted, ref } from "vue";
import type { HtmlBlock } from "@/api/content/html-block";
import { getList, deletes } from "@/api/content/html-block";
import { useTime } from "@/hooks/use-date";
import { useRouteSiteId } from "@/hooks/use-site-id";
import RelationsTag from "@/components/relations/relations-tag.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import IconButton from "@/components/basic/icon-button.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
const { t } = useI18n();
const router = useRouter();
const list = ref<HtmlBlock[]>([]);

onMounted(() => {
  load();
});

async function load() {
  const records = await getList();
  list.value = records.sort((a, b) =>
    a.lastModified > b.lastModified ? -1 : 1
  );
}
async function onDelete(rows: HtmlBlock[]) {
  await showDeleteConfirm(rows.length);
  const ids = rows.map((item) => item.id);
  await deletes({ ids });
  load();
}

function create() {
  router.push(useRouteSiteId({ name: "htmlBlock-edit" }));
}
</script>
