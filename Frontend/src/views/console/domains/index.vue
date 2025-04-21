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
        :label="t('common.action')"
        width="160"
        align="center"
      >
        <template #default="{ row }">
          <div class="flex gap-8 items-center justify-center">
            <el-button
              v-if="
                appStore.currentOrg?.isAdmin && row.source == 'OpenProvider'
              "
              size="small"
              type="primary"
              round
              class="mr-8"
              @click="renew(row)"
              >{{ t("common.renew") }}</el-button
            >
            <div
              v-if="
                appStore.currentOrg?.isAdmin && row.source == 'OpenProvider'
              "
              class="hover:cursor-pointer"
              @click="showTransfer(row)"
            >
              <svg
                width="16"
                height="16"
                viewBox="0 0 16 16"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M14 4.00098H9.994L6.98 2.00098H2C1.73478 2.00098 1.48043 2.10633 1.29289 2.29387C1.10536 2.48141 1 2.73576 1 3.00098V12.999C1 13.2642 1.10536 13.5185 1.29289 13.7061C1.48043 13.8936 1.73478 13.999 2 13.999H14C14.2652 13.999 14.5196 13.8936 14.7071 13.7061C14.8946 13.5185 15 13.2642 15 12.999V5.00098C15 4.73576 14.8946 4.48141 14.7071 4.29387C14.5196 4.10633 14.2652 4.00098 14 4.00098ZM14 12.499C14 12.6316 13.9473 12.7588 13.8536 12.8525C13.7598 12.9463 13.6326 12.999 13.5 12.999H2.5C2.36739 12.999 2.24021 12.9463 2.14645 12.8525C2.05268 12.7588 2 12.6316 2 12.499V3.50098C2 3.36837 2.05268 3.24119 2.14645 3.14742C2.24021 3.05366 2.36739 3.00098 2.5 3.00098H6.997L10.001 5.00098H13.5C13.6326 5.00098 13.7598 5.05366 13.8536 5.14742C13.9473 5.24119 14 5.36837 14 5.50098V12.499Z"
                  fill="rgb(153, 153, 153)"
                />
                <path
                  d="M8.93598 6.19379C8.88831 6.14291 8.83091 6.10214 8.76717 6.07389C8.70344 6.04564 8.63467 6.0305 8.56497 6.02937C8.49526 6.02823 8.42604 6.04112 8.36142 6.06727C8.29679 6.09343 8.23809 6.13231 8.18879 6.1816C8.1395 6.2309 8.10062 6.2896 8.07446 6.35423C8.04831 6.41885 8.03542 6.48807 8.03656 6.55778C8.03769 6.62748 8.05283 6.69625 8.08108 6.75998C8.10933 6.82372 8.1501 6.88112 8.20098 6.92879L9.26698 7.99479H4.00098V9.03379H9.20298L8.20198 10.0348C8.1511 10.0824 8.11033 10.1399 8.08208 10.2036C8.05383 10.2673 8.03869 10.3361 8.03756 10.4058C8.03642 10.4755 8.04931 10.5447 8.07546 10.6093C8.10162 10.674 8.1405 10.7327 8.18979 10.782C8.23909 10.8313 8.29779 10.8701 8.36241 10.8963C8.42704 10.9225 8.49626 10.9353 8.56597 10.9342C8.63567 10.9331 8.70444 10.9179 8.76817 10.8897C8.83191 10.8614 8.88931 10.8207 8.93698 10.7698L10.858 8.84879C10.9554 8.75128 11.0101 8.6191 11.0101 8.48129C11.0101 8.34347 10.9554 8.21129 10.858 8.11379L8.93598 6.19379Z"
                  fill="rgb(153, 153, 153)"
                />
              </svg>
            </div>
            <div
              v-if="appStore.currentOrg?.isAdmin && row.enableDns"
              class="hover:cursor-pointer"
              @click="setDns(row)"
            >
              <svg
                width="16"
                height="16"
                viewBox="0 0 16 16"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M9.76552 1L13.2345 3L12.5625 4.172C12.7919 4.443 13.0002 4.73983 13.1875 5.0625C13.3749 5.38517 13.5312 5.7185 13.6565 6.0625H15.0005V10.0625H13.6565C13.4065 10.7605 13.0419 11.3907 12.5625 11.953L13.2345 13.125L9.76552 15.125L9.09352 13.953C8.36419 14.0883 7.63502 14.0883 6.90602 13.953L6.23402 15.125L2.76502 13.125L3.43702 11.953C3.19736 11.6717 2.98636 11.3722 2.80402 11.0545C2.62169 10.7368 2.46802 10.4062 2.34302 10.0625H0.999023V6.0625H2.34302C2.59302 5.3645 2.95769 4.73433 3.43702 4.172L2.76502 3L6.23402 1L6.90602 2.172C7.63536 2.03667 8.36452 2.03667 9.09352 2.172L9.76552 1ZM10.1405 2.3595L9.95302 2.672L9.60902 3.2815L8.90602 3.1565C8.30169 3.04183 7.69752 3.04183 7.09352 3.1565L6.39052 3.2815L5.85902 2.3595L4.14002 3.3595L4.65552 4.2815L4.20252 4.813C3.80652 5.28167 3.50436 5.80767 3.29602 6.391L3.06152 7.063H1.99902V9.063H3.04602L3.29602 9.735C3.40036 10.037 3.52536 10.313 3.67102 10.563C3.81669 10.813 3.99386 11.063 4.20252 11.313L4.65552 11.8445L4.14002 12.7665L5.85902 13.7665L6.39052 12.8445L7.09352 12.9695C7.69786 13.0842 8.30202 13.0842 8.90602 12.9695L9.60902 12.8445L10.1405 13.7665L11.8595 12.7665L11.344 11.8445L11.797 11.313C12.193 10.8337 12.4952 10.3077 12.7035 9.735L12.938 9.063H14.0005V7.063H12.9535L12.7035 6.391C12.6099 6.10967 12.4849 5.83617 12.3285 5.5705C12.1722 5.30483 11.995 5.05217 11.797 4.8125L11.344 4.281L11.8595 3.359L10.1405 2.3595ZM8.00002 4.5625C8.98969 4.5835 9.81519 4.92467 10.4765 5.586C11.1379 6.24733 11.479 7.07283 11.5 8.0625C11.479 9.05217 11.1379 9.87767 10.4765 10.539C9.81519 11.2003 8.98969 11.5415 8.00002 11.5625C7.01036 11.5415 6.18486 11.2003 5.52352 10.539C4.86219 9.87767 4.52102 9.05217 4.50002 8.0625C4.52102 7.07283 4.86219 6.24733 5.52352 5.586C6.18486 4.92467 7.01036 4.5835 8.00002 4.5625ZM8.00002 5.5625C7.29169 5.5835 6.70319 5.82833 6.23452 6.297C5.76586 6.76567 5.52102 7.35417 5.50002 8.0625C5.52102 8.77083 5.76586 9.35933 6.23452 9.828C6.70319 10.2967 7.29169 10.5415 8.00002 10.5625C8.70836 10.5415 9.29686 10.2967 9.76552 9.828C10.2342 9.35933 10.479 8.77083 10.5 8.0625C10.479 7.35417 10.2342 6.76567 9.76552 6.297C9.29686 5.82833 8.70836 5.5835 8.00002 5.5625V5.5625Z"
                  fill="rgb(153, 153, 153)"
                />
              </svg>
            </div>
          </div>
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
    <GetTransferCodeDialog
      v-if="showGetTransferCodeDialog"
      v-model="showGetTransferCodeDialog"
      :domain="currentDomainName"
    />
    <RenewDomainDialog
      v-if="showRenewDomainDialog"
      v-model="showRenewDomainDialog"
      :domain="currentDomainName"
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
import GetTransferCodeDialog from "./get-transfer-code-dialog.vue";
import RenewDomainDialog from "./renew-domain-dialog.vue";

const appStore = useAppStore();

const { t } = useI18n();
const router = useRouter();
const changeDataCenterDialog = ref(false);
const showGetTransferCodeDialog = ref(false);
const showRenewDomainDialog = ref(false);
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

const load = async () => {
  domainList.value = await getList();
};

load();

function setDns(row: any) {
  if (!appStore.currentOrg?.isAdmin) return;
  router.push({
    name: "domainDNS",
    query: { domainName: row.domainName },
  });
}

function showTransfer(row: any) {
  if (!appStore.currentOrg?.isAdmin) return;
  currentDomainName.value = row.domainName;
  showGetTransferCodeDialog.value = true;
}

function renew(row: any) {
  if (!appStore.currentOrg?.isAdmin) return;
  currentDomainName.value = row.domainName;
  showRenewDomainDialog.value = true;
}
</script>
