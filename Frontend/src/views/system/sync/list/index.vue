<script lang="ts" setup>
import { ref } from "vue";
import KTable from "@/components/k-table";
import AddSyncDialog from "./add-sync-dialog.vue";
import SettingsSyncDialog from "./settings-sync-dialog.vue";
import { getList, deletes, importBearer } from "@/api/publish";
import type { Publish } from "@/api/publish/types";
import { useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { showDeleteConfirm } from "@/components/basic/confirm";
import IgnoreSettingsDialog from "./ignore-settings-dialog.vue";
import SyncProgressDialog from "./sync-progress-dialog.vue";
import { errorMessage } from "@/components/basic/message";
import AddCooperationBearerDialog from "./add-cooperation-bearer-dialog.vue";
import { useSiteStore } from "@/store/site";

const { t } = useI18n();
const data = ref<Publish[]>([]);
const showAddSyncDialog = ref(false);
const showSettingsSyncDialog = ref(false);
const showIgnoreSettingsDialog = ref(false);
const showSyncProgressDialog = ref(false);
const router = useRouter();
const currentItem = ref<string>();
const showCooperationBearerDialog = ref(false);
const siteStore = useSiteStore();

const load = async () => {
  data.value = await getList();
};

const onDelete = async (rows: Publish[]) => {
  await showDeleteConfirm(rows.length);
  await deletes(rows.map((m) => m.id));
  load();
};

const showSettingSync = (id: string) => {
  currentItem.value = id;
  showSettingsSyncDialog.value = true;
};

const showIgnoreSettings = (id: string) => {
  currentItem.value = id;
  showIgnoreSettingsDialog.value = true;
};

const showSyncProgress = (id: string) => {
  currentItem.value = id;
  showSyncProgressDialog.value = true;
};

async function onImportBearer(file: any) {
  try {
    var bearer = JSON.parse(await file.raw.text());
    await importBearer({
      remoteServerUrl: bearer.server,
      remoteWebSiteId: bearer.siteId,
      remoteSiteName: bearer.name,
      serverName: bearer.name,
      accessToken: bearer.token,
    });
    load();
  } catch (error) {
    errorMessage(t("common.bearerInvalid"));
  }
}

load();
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.sync')" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{ feature: 'sync', action: 'edit' }"
        round
        data-cy="new-sync"
        @click="showAddSyncDialog = true"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.newSync") }}
        </div>
      </el-button>
      <el-button
        v-if="siteStore.isAdmin"
        v-hasPermission="{ feature: 'sync', action: 'edit' }"
        round
        data-cy="new-sync"
        @click="showCooperationBearerDialog = true"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.newCooperationBearer") }}
        </div>
      </el-button>
      <el-upload
        :show-file-list="false"
        :action="''"
        accept=".json"
        :auto-upload="false"
        :on-change="onImportBearer"
        data-cy="import-bearer"
      >
        <el-button v-hasPermission="{ feature: 'sync', action: 'edit' }" round>
          <div class="flex items-center">
            <el-icon class="mr-16 iconfont icon-a-Pullin" />
            {{ t("common.importCooperationBearer") }}
          </div>
        </el-button>
      </el-upload>

      <div class="flex-1" />
      <IconButton
        :permission="{ feature: 'sync', action: 'edit' }"
        circle
        icon="icon-yunfuwuqi"
        :tip="t('common.server')"
        @click="
          router.push(
            useRouteSiteId({
              name: 'sync-server',
            })
          )
        "
      />
    </div>
    <KTable
      :data="data"
      show-check
      :permission="{ feature: 'sync', action: 'delete' }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.server')">
        <template #default="{ row }">
          <span :title="row.serverName">
            {{ row.serverName }}
          </span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.remoteSite')">
        <template #default="{ row }">
          <span data-cy="remote-site">{{ row.remoteSiteName }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.serverUrl')">
        <template #default="{ row }">
          <span data-cy="server">{{ row.remoteServerUrl }}</span>
        </template>
      </el-table-column>

      <el-table-column :label="t('common.difference')" align="center">
        <template #default="{ row }">
          <router-link
            :to="
              useRouteSiteId({
                name: 'sync-list',
                query: {
                  id: row.id,
                },
              })
            "
          >
            <div class="space-x-4 cursor-pointer inline-flex items-center">
              <div class="inline-flex items-center justify-end w-80px">
                <el-tooltip :content="t('common.localChanges')" placement="top">
                  <el-tag round>
                    <div class="space-x-2px flex relative">
                      <span>
                        {{
                          row.localDifference > 999
                            ? "999+"
                            : row.localDifference
                        }}
                      </span>
                      <div
                        class="relative w-12px h-12px top-2px overflow-hidden"
                      >
                        <el-icon
                          class="iconfont icon-a-debug-step-out absolute top-[-4px] left-0"
                        />
                      </div>
                    </div>
                  </el-tag>
                </el-tooltip>
              </div>
              <div class="inline-flex items-center justify-start w-80px">
                <el-tooltip
                  :content="t('common.remoteChanges')"
                  placement="top"
                >
                  <el-tag type="success" round>
                    <div class="space-x-2px flex relative">
                      <div
                        class="relative w-12px h-12px bottom-2px overflow-hidden"
                      >
                        <el-icon
                          class="iconfont icon-debug-step-into absolute bottom-[-4px] left-0"
                        />
                      </div>
                      <span>
                        {{
                          row.remoteDifference > 999
                            ? "999+"
                            : row.remoteDifference
                        }}
                      </span>
                    </div>
                  </el-tag>
                </el-tooltip>
              </div>
            </div>
          </router-link>
        </template>
      </el-table-column>

      <el-table-column align="right" width="120">
        <template #default="{ row }">
          <div class="py-4">
            <IconButton
              icon="icon-a-debug-step-over"
              class="text-m"
              :tip="t('common.syncProgress')"
              @click="showSyncProgress(row.id)"
            />
            <IconButton
              icon="icon-correct"
              class="text-m"
              :tip="t('common.ignoreSettings')"
              @click="showIgnoreSettings(row.id)"
            />
            <IconButton
              icon="icon-a-setup"
              :tip="t('common.siteSettingsSync')"
              @click="showSettingSync(row.id)"
            />
          </div>
        </template>
      </el-table-column>
    </KTable>
  </div>
  <AddSyncDialog
    v-if="showAddSyncDialog"
    v-model="showAddSyncDialog"
    @reload="load"
  />
  <SettingsSyncDialog
    v-if="showSettingsSyncDialog"
    :id="currentItem!"
    v-model="showSettingsSyncDialog"
  />
  <IgnoreSettingsDialog
    v-if="showIgnoreSettingsDialog"
    :id="currentItem!"
    v-model="showIgnoreSettingsDialog"
    @reload="load"
  />
  <SyncProgressDialog
    v-if="showSyncProgressDialog"
    :id="currentItem!"
    v-model="showSyncProgressDialog"
    @reload="load"
  />
  <AddCooperationBearerDialog
    v-if="showCooperationBearerDialog"
    v-model="showCooperationBearerDialog"
  />
</template>
