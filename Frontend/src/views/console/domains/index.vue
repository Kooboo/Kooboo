<template>
  <div class="p-24">
    <div class="flex items-center pb-24 space-x-16">
      <el-button round @click="addDialog.open">
        <div class="flex items-center">
          {{ t("common.new") }}
        </div>
      </el-button>
      <el-button
        v-if="appStore.currentOrg?.isAdmin"
        round
        @click="router.push({ name: 'domain-search' })"
      >
        <div class="flex items-center">
          {{ t("common.domainPurchase") }}
        </div>
      </el-button>
    </div>

    <KTable :data="domainList" show-check @delete="onDelete">
      <el-table-column :label="t('common.domain')" min-width="300">
        <template #default="{ row }">
          <router-link
            class="text-blue ellipsis"
            :to="{ name: 'domain-binding', query: { id: row.id } }"
            data-cy="domain"
          >
            {{ row.domainName }}
          </router-link>
        </template>
      </el-table-column>
      <el-table-column
        v-if="appStore.header?.isOnlineServer"
        :label="t('common.dataCenter')"
        width="200"
        align="center"
      >
        <template #default="{ row }">
          <div
            v-if="row.dataCenter"
            style="max-width: fit-content"
            class="my-0 mx-auto px-12 ellipsis leading-7 h-28px rounded-full border border-[#a7b0be] border-opacity-40 border-solid text-14px text-666 dark:text-fff/86 hover:bg-666/10 cursor-pointer"
            :title="row.dataCenter"
            :class="
              appStore.currentOrg?.isAdmin
                ? ''
                : 'cursor-not-allowed  hover:bg-transparent'
            "
            @click="openChangeDataCenterDialog(row.domainName, row.dataCenter)"
          >
            {{ row.dataCenter }}
          </div>
        </template>
      </el-table-column>
      <el-table-column
        :label="t('common.expirationDate')"
        width="200"
        align="center"
      >
        <template #default="{ row }">
          <span data-cy="expire-date">{{ row.expires }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.record')" width="100" align="center">
        <template #default="{ row }">
          <span data-cy="record">{{ row.records }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.site')" width="100" align="center">
        <template #default="{ row }">
          <span data-cy="site-number">{{ row.sites }}</span>
        </template>
      </el-table-column>

      <el-table-column
        v-if="appStore.header?.isOnlineServer"
        :label="t('common.operation')"
        width="120"
        align="center"
      >
        <template #default="{ row }">
          <span
            v-if="row.enableDns"
            :class="
              appStore.currentOrg?.isAdmin
                ? 'text-blue cursor-pointer'
                : 'cursor-not-allowed text-999'
            "
            @click="settingDNS(row)"
            >{{ t("common.DNSSetting") }}</span
          >
        </template>
      </el-table-column>
    </KTable>

    <AddDialog v-model="addDialog.status" @create-success="load()" />
    <ChangeDataCenterDialog
      v-if="changeDataCenterDialog"
      v-model="changeDataCenterDialog"
      :data-center="currentDataCenter"
      :domain="currentDomainName"
      @change-success="load()"
    />
  </div>
</template>

<script lang="ts" setup>
import AddDialog from "./add-dialog.vue";
import ChangeDataCenterDialog from "./change-datacenter-dialog.vue";

import { reactive, ref } from "vue";
import KTable from "@/components/k-table";
import { useRouter } from "vue-router";
import { useI18n } from "vue-i18n";
import type { Domain } from "@/api/console/types";
import { deleteDomains, getList } from "@/api/console";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useAppStore } from "@/store/app";

const appStore = useAppStore();

const { t } = useI18n();
const router = useRouter();
const changeDataCenterDialog = ref(false);
const currentDomainName = ref();
const currentDataCenter = ref();
const domainList = ref<Domain[]>([]);
const addDialog = reactive({
  status: false,
  open() {
    addDialog.status = true;
  },
});
const openChangeDataCenterDialog = (domain: string, dataCenter: string) => {
  if (!appStore.currentOrg?.isAdmin) return;
  changeDataCenterDialog.value = true;
  currentDataCenter.value = dataCenter;
  currentDomainName.value = domain;
};

async function onDelete(list: Domain[]) {
  if (list.length) {
    await showDeleteConfirm(list.length);
    await deleteDomains(list.map((item) => item.id));
    load();
  }
}

const settingDNS = (row: any) => {
  if (!appStore.currentOrg?.isAdmin) return;
  router.push({
    name: "domainDNS",
    query: { domainName: row.domainName },
  });
};
const load = async () => {
  domainList.value = await getList();
};

load();
</script>
