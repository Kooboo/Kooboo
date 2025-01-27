<script lang="ts" setup>
import { computed, onBeforeMount, ref } from "vue";
import { getEventList, getList, deletes } from "@/api/events/backend";
import KTable from "@/components/k-table";
import { useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import type { EventItem } from "@/api/events/types";
import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
import type { Site } from "@/api/site/site";
import { saveSite } from "@/api/site";
import SelectEventDialog from "../front-events/select-event-dialog.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";

const { t } = useI18n();
const router = useRouter();
const eventTypes = ref<Awaited<ReturnType<typeof getEventList>>>([]);
const eventList = ref<EventItem[]>([]);
const siteStore = useSiteStore();
const site = ref<Site>(JSON.parse(JSON.stringify(siteStore.site)));
const showSelectEventDialog = ref(false);

async function load() {
  const rsp = await Promise.all([getEventList(), getList()]);
  eventTypes.value = rsp[0];
  eventList.value = rsp[1];
}

onBeforeMount(load);

const goToEdit = (name: string) => {
  router.push(
    useRouteSiteId({
      name: "backendevents-edit",
      query: {
        name: name,
        display: eventTypes.value.find((f) => f.name == name)?.display || name,
      },
    })
  );
};

async function saveSettings() {
  await saveSite(site.value);
  siteStore.loadSite();
  siteStore.loadSites();
}

async function onDelete(rows: any) {
  await showDeleteConfirm(rows.length);
  await deletes(rows.map((m: any) => m.name));
  load();
}
</script>
<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.backendEvents')" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{
          feature: 'frontEvents',
          action: 'edit',
        }"
        round
        data-cy="new-event"
        @click="showSelectEventDialog = true"
      >
        <div class="flex items-center">
          <el-icon class="iconfont icon-a-addto" />
          {{ t("common.newEvent") }}
        </div>
      </el-button>
      <div class="flex-1" />
      <span v-if="site.enableBackendEvents" class="text-blue">{{
        t("event.eventActive")
      }}</span>
      <span v-else class="text-999">{{ t("event.eventInactive") }}</span>
      <ElSwitch v-model="site.enableBackendEvents" @change="saveSettings" />
    </div>
    <KTable :data="eventList" show-check @delete="onDelete">
      <el-table-column :label="t('common.type')">
        <template #default="{ row }">
          {{ eventTypes.find((f) => f.name == row.name)?.display || row.name }}
        </template>
      </el-table-column>
      <el-table-column :label="t('common.rulesCount')">
        <template #default="{ row }">
          <el-tag
            type="success"
            size="small"
            class="rounded-full"
            data-cy="rules-count"
            >{{ row.count }}</el-tag
          >
        </template>
      </el-table-column>
      <el-table-column width="100px" align="right">
        <template #default="{ row }">
          <IconButton
            icon="icon-a-writein"
            :tip="t('common.edit')"
            data-cy="edit"
            @click="goToEdit(row.name)"
          />
        </template>
      </el-table-column>
    </KTable>
    <SelectEventDialog
      v-if="showSelectEventDialog"
      v-model="showSelectEventDialog"
      :events="eventTypes"
      :excludes="eventList.map((m) => m.name)"
      @selected="goToEdit"
    />
  </div>
</template>
