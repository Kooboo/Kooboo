<script lang="ts" setup>
import { computed, onBeforeMount, ref } from "vue";
import { getEventList, getList } from "@/api/events";
import KTable from "@/components/k-table";
import { useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import type { EventItem } from "@/api/events/types";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
const router = useRouter();
const eventTypes = ref<Awaited<ReturnType<typeof getEventList>>>([]);
const eventList = ref<EventItem[]>([]);

onBeforeMount(async () => {
  const rsp = await Promise.all([getEventList(), getList()]);
  eventTypes.value = rsp[0];
  eventList.value = rsp[1];
});

const availableTypes = computed(() => {
  const result: { group: string; list: string[] }[] = [];

  for (const i of eventTypes.value) {
    if (eventList.value.some((s) => s.name === i.name)) continue;
    let group = result.find((f) => f.group === i.category);
    if (group) {
      group.list.push(i.name);
    } else {
      group = { group: i.category, list: [i.name] };
      result.push(group);
    }
  }

  return result;
});

const goToEdit = (name: string) => {
  router.push(
    useRouteSiteId({
      name: "frontevents-edit",
      query: {
        name: name,
      },
    })
  );
};
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.frontEvents')" />
    <div class="flex items-center py-24 space-x-16">
      <el-dropdown trigger="click" @command="goToEdit">
        <el-button
          v-hasPermission="{
            feature: 'frontEvents',
            action: 'edit',
          }"
          round
          data-cy="new-event"
        >
          <div class="flex items-center">
            {{ t("common.newEvent") }}
            <el-icon class="iconfont icon-pull-down text-12px ml-8 !mr-0" />
          </div>
        </el-button>
        <template #dropdown>
          <el-dropdown-menu>
            <template v-for="group of availableTypes" :key="group.group">
              <h3 class="p-4 dark:text-fff/86">
                {{ group.group }}
              </h3>
              <el-dropdown-item
                v-for="item of group.list"
                :key="item"
                :command="item"
                :data-cy="item"
              >
                <span>{{ item }}</span>
              </el-dropdown-item>
            </template>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>

    <KTable :data="eventList">
      <el-table-column :label="t('common.type')" prop="name" />
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
  </div>
</template>
