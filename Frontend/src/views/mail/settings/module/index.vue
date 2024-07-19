<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import AddDialog from "./add-dialog.vue";
import ImportDialog from "./import-dialog.vue";
import SearchDialog from "./search-dialog.vue";
import {
  deleteMailModules,
  exportMailModule,
  shareMailModule,
  updateMailModuleStatus,
} from "@/api/mail/mail-module";
import { showConfirm, showDeleteConfirm } from "@/components/basic/confirm";
import type { MailModule } from "@/api/mail/types";
import { getMailModuleList } from "@/api/mail/mail-module";
import SettingDialog from "./setting-dialog.vue";
import ReadMeDialog from "./read-me-dialog.vue";

const { t } = useI18n();
const list = ref<MailModule[]>([]);
const showAddDialog = ref(false);
const showImportDialog = ref(false);
const showSearchDialog = ref(false);
const showSettingDialog = ref(false);
const showReadMeDialog = ref(false);
const showManageDialog = ref(false);
const selected = ref<string>();
let backUrl = ref<string>();

async function load() {
  list.value = await getMailModuleList();
}

load();

async function onDelete(items: MailModule[]) {
  await showDeleteConfirm(items.length);
  await deleteMailModules(items.map((m) => m.id));
  load();
}

const changeState = async (row: MailModule) => {
  await updateMailModuleStatus(row.id, !row.online);
  load();
};

const onExportModule = (name: string) => {
  exportMailModule(name);
};

const onShare = async (id: string) => {
  await showConfirm(t("common.shareModuleConfirm"));
  await shareMailModule(id);
};

const onSetting = (id: string) => {
  selected.value = id;
  showSettingDialog.value = true;
};

const onReadMe = (id: string) => {
  selected.value = id;
  showReadMeDialog.value = true;
};
</script>

<template>
  <div class="p-24">
    <div class="flex items-center pb-24 space-x-16">
      <el-button round data-cy="create-module" @click="showAddDialog = true">
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.create") }}
        </div>
      </el-button>

      <el-button round data-cy="import" @click="showImportDialog = true">
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-Pullin" />
          {{ t("common.import") }}
        </div>
      </el-button>
      <el-button round data-cy="search" @click="showSearchDialog = true">
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-search" />
          {{ t("common.search") }}
        </div>
      </el-button>
    </div>
    <KTable :data="list" show-check @delete="onDelete">
      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <router-link
            :to="{
              name: 'mail-module-editor',
              query: {
                id: row.id,
              },
            }"
            data-cy="name"
          >
            <span class="text-blue cursor-pointer">{{ row.name }}</span>
          </router-link>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.online')" width="150">
        <template #default="{ row }">
          <ElSwitch
            :model-value="row.online"
            data-cy="online"
            @click="changeState(row)"
          />
        </template>
      </el-table-column>
      <el-table-column width="100" :label="t('common.backend')">
        <template #default="{ row }">
          <el-button
            v-if="row.backendViewUrl"
            size="small"
            type="primary"
            round
            data-cy=""
            @click="
              backUrl = row.backendViewUrl;
              showManageDialog = true;
            "
            >{{ t("common.manage") }}</el-button
          >
        </template>
      </el-table-column>

      <el-table-column align="right" width="180px" prop="creationDate">
        <template #default="{ row }">
          <IconButton
            icon="icon-a-setup"
            :tip="t('common.settings')"
            data-cy="setting"
            @click="onSetting(row.id)"
          />
          <IconButton
            icon="icon-page"
            :tip="t('common.description')"
            data-cy="description"
            @click="onReadMe(row.id)"
          />
          <IconButton
            icon="icon-share"
            :tip="t('common.export')"
            data-cy="export"
            @click="onExportModule(row.name)"
          />
          <IconButton
            icon="icon-link !bg-transparent"
            :tip="t('common.share')"
            data-cy="share"
            @click="onShare(row.id)"
          />
        </template>
      </el-table-column>
    </KTable>
    <AddDialog v-if="showAddDialog" v-model="showAddDialog" @reload="load" />
    <ImportDialog
      v-if="showImportDialog"
      v-model="showImportDialog"
      @reload="load"
    />
    <SettingDialog
      v-if="showSettingDialog"
      :id="selected!"
      v-model="showSettingDialog"
    />
    <SearchDialog
      v-if="showSearchDialog"
      v-model="showSearchDialog"
      :list="list"
      @reload="load"
    />
    <ReadMeDialog
      v-if="showReadMeDialog"
      :id="selected!"
      v-model="showReadMeDialog"
    />
    <el-dialog
      v-model="showManageDialog"
      width="1000px"
      :close-on-click-modal="false"
      custom-class="el-dialog--zero-padding"
      :title="t('common.manage')"
      @closed="showManageDialog = false"
    >
      <iframe v-if="showManageDialog" :src="backUrl" class="w-full h-500px" />
    </el-dialog>
  </div>
</template>
