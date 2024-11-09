<script lang="ts" setup>
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import {
  getSitemapList,
  deleteSitemap,
  getFeeds,
  submitSitemap,
} from "@/api/search-console";
import type { Sitemap } from "@/api/search-console/types";
import { useTime } from "@/hooks/use-date";
import { showDeletesConfirm } from "@/components/basic/confirm";

const list = ref<Sitemap[]>([]);
const feeds = ref<string[]>([]);
const props = defineProps<{ modelValue: boolean; siteUrl: string }>();
const { t } = useI18n();
const show = ref(true);

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

getFeeds(props.siteUrl).then((rsp) => {
  feeds.value = rsp;
});

async function load() {
  list.value = await getSitemapList(props.siteUrl);
}

load();

async function onDelete(sitemaps: Sitemap[]) {
  await showDeletesConfirm();

  for (const sitemap of sitemaps) {
    await deleteSitemap(props.siteUrl, sitemap.path);
  }

  await load();
}

async function onAdd(path: string) {
  await submitSitemap(props.siteUrl, path);
  await load();
}

const addableList = computed(() => {
  const result = [];
  for (const feed of feeds.value) {
    if (list.value.find((f) => f.path == feed)) continue;
    result.push(feed);
  }
  return result;
});
</script>

<template>
  <el-dialog
    :model-value="show"
    width="1000px"
    :close-on-click-modal="false"
    :title="t('common.sitemap')"
    @closed="emit('update:modelValue', false)"
  >
    <div>
      <div class="flex items-center pb-24 space-x-16">
        <el-dropdown
          trigger="click"
          class="ml-10px"
          :disabled="!addableList.length"
          @command="onAdd"
        >
          <el-button round data-cy="new-page">
            <el-icon class="iconfont icon-a-addto" />
            {{ t("common.submit") }}
          </el-button>
          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item
                v-for="item of addableList"
                :key="item"
                :command="item"
              >
                <span>{{ item }}</span>
              </el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </div>
      <KTable :data="list" show-check @delete="onDelete">
        <el-table-column :label="t('common.path')" prop="path" />
        <el-table-column :label="t('common.lastSubmitted')" prop="errors">
          <template #default="{ row }">
            {{ useTime(row.lastSubmitted) }}
          </template>
        </el-table-column>
        <el-table-column :label="t('common.lastDownloaded')" prop="errors">
          <template #default="{ row }">
            {{ row.lastDownloaded ? useTime(row.lastDownloaded) : "" }}
          </template>
        </el-table-column>
        <el-table-column :label="t('common.error')" prop="errors" width="80" />
        <el-table-column
          :label="t('common.warning')"
          prop="warnings"
          width="80"
        />
      </KTable>
    </div>
  </el-dialog>
</template>
