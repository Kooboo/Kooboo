<script lang="ts" setup>
import KTable from "@/components/k-table";
import { getLogVideo, getVersions, revert } from "@/api/site-log";
import { ref } from "vue";
import { useTime } from "@/hooks/use-date";
import { useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { Version } from "@/api/site-log/types";
import { getQueryString, openInNewTab } from "@/utils/url";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import monacoEventPlayer from "@/components/monaco-event-player/index.vue";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
const data = ref<Version[]>();
const router = useRouter();

const load = async () => {
  data.value = await getVersions({
    keyHash: getQueryString("keyHash")!,
    storeNameHash: getQueryString("storeNameHash")!,
    tableNameHash: getQueryString("tableNameHash"),
  });
};

const onUndo = async (selected: Version) => {
  await revert(selected.id);
  await load();
};

const onCompare = async (selected: Version[]) => {
  var url = router.resolve(
    useRouteSiteId({
      name: "version-compare",
      query: {
        left: selected[0].id,
        right: selected[1]?.id || -1,
      },
    })
  );
  openInNewTab(url.href);
};
load();

async function playVideo(versionId: number) {
  const video: any = await getLogVideo(versionId);
  // console.log(video); //replace with real play code video

  const events = JSON.parse(video);
  if (playerControl.value) {
    playerControl.value.load(versionId, events);
  }
}

const playerControl = ref<any>(null);
</script>
<template>
  <div class="p-24">
    <Breadcrumb
      :crumb-path="[
        { name: t('common.siteLogs'), route: { name: 'sitelogs' } },
        { name: t('common.versions') },
      ]"
    />
    <KTable v-if="data" :data="data" show-check hide-delete class="mt-24">
      <template #bar="{ selected }">
        <el-button
          v-if="selected.length === 1"
          round
          data-cy="undo"
          @click="onUndo(selected[0])"
        >
          {{ t("common.undo") }}
        </el-button>
        <el-button
          v-if="selected.length === 1"
          round
          data-cy="compare-with-current"
          @click="onCompare(selected)"
        >
          {{ t("common.compareWithCurrent") }}
        </el-button>
        <el-button
          v-if="selected.length === 2"
          round
          data-cy="compare"
          @click="onCompare(selected)"
        >
          {{ t("common.compare") }}
        </el-button>
      </template>
      <el-table-column :label="t('common.version')">
        <template #default="{ row, $index }">
          <span>{{ row.id }}</span>
          <span v-if="$index === 0"> {{ t("common.currentVersion") }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.user')">
        <template #default="{ row }">
          <span>{{ row.userName }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.date')">
        <template #default="{ row }">
          <span>{{ useTime(row.lastModified) }}</span>
        </template>
      </el-table-column>

      <el-table-column align="right" width="80px">
        <template #default="{ row }">
          <IconButton
            v-if="row.hasVideo"
            icon="icon-ve-video"
            :tip="t('common.playVideo')"
            :permission="{ feature: 'site', action: 'log' }"
            @click="playVideo(row.id)"
          />
        </template>
      </el-table-column>
    </KTable>
  </div>

  <MonacoEventPlayer @ready="(e:any) => playerControl=e" />
</template>
