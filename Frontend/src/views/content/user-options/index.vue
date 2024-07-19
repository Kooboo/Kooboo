<script lang="ts" setup>
import KTable from "@/components/k-table";
import { onMounted, ref } from "vue";
import type { HtmlBlock } from "@/api/content/html-block";
import type { UserOptions } from "@/api/content/user-options";
import { getUserOptionsList, deletes } from "@/api/content/user-options";
import { useTime } from "@/hooks/use-date";
import { useRouteSiteId } from "@/hooks/use-site-id";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import { useSiteStore } from "@/store/site";
const { t } = useI18n();
const router = useRouter();
const list = ref<UserOptions[]>([]);
const siteStore = useSiteStore();

onMounted(() => {
  load();
});

async function load() {
  list.value = await getUserOptionsList();
}
async function onDelete(rows: HtmlBlock[]) {
  await showDeleteConfirm(rows.length);
  const ids = rows.map((item) => item.id);
  await deletes({ ids });
  load();
}

function create() {
  router.push(useRouteSiteId({ name: "useroptions create" }));
}
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.userOptions')" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-if="siteStore.hasAccess('userOptions', 'setting')"
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
        feature: 'userOptions',
        action: 'delete',
      }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')" prop="name">
        <template #default="{ row }">
          <router-link
            :to="
              useRouteSiteId({
                name: 'useroptions edit',
                query: { id: row?.id },
              })
            "
            data-cy="edit"
            class="mx-8 text-blue"
          >
            <span class="ellipsis" data-cy="name">{{
              row.display || row.name
            }}</span>
          </router-link>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.lastModified')">
        <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
      </el-table-column>
      <el-table-column width="90" align="right">
        <template #default="{ row }">
          <router-link
            v-if="siteStore.hasAccess('userOptions', 'setting')"
            :to="
              useRouteSiteId({
                name: 'useroptions setting',
                query: { id: row?.id },
              })
            "
            data-cy="setting"
            class="mx-8"
          >
            <el-tooltip placement="top" :content="t('common.setting')">
              <el-icon class="iconfont icon-a-setup hover:text-blue text-l" />
            </el-tooltip>
          </router-link>
        </template>
      </el-table-column>
    </KTable>
  </div>
</template>
