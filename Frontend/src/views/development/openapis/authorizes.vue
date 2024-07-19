<script lang="ts" setup>
import { getQueryString } from "@/utils/url";
import KTable from "@/components/k-table";
import { ref, computed } from "vue";
import { get, getAuthorizes, deleteAuthorizes } from "@/api/openapi";
import { useTime } from "@/hooks/use-date";
import IconButton from "@/components/basic/icon-button.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import type { OpenApi, Authorize } from "@/api/openapi/types";
import EditAuthorizeDialog from "./components/edit-authorize-dialog.vue";

import { useI18n } from "vue-i18n";
import { showDeleteConfirm } from "@/components/basic/confirm";
const { t } = useI18n();
const data = ref<Authorize[]>([]);
const openApi = ref<OpenApi>();
const showDialog = ref(false);
const dialogId = ref("");
const siteId = ref(getQueryString("SiteId"));

const onLoad = async () => {
  const id = getQueryString("id") as string;
  data.value = await getAuthorizes(id);
  openApi.value = await get(id);
};

const onSave = async () => {
  onLoad();
};

const onDelete = async (rows: Authorize[]) => {
  await showDeleteConfirm(rows.length);
  await deleteAuthorizes(rows.map((m) => m.id));
  onLoad();
};

const onOPenDialog = (id = "") => {
  dialogId.value = id;
  showDialog.value = true;
};

const authorizationCodeFlow = computed(() => {
  if (!openApi.value) return null;
  const doc = JSON.parse(openApi.value.jsonData);
  if (!doc.components || !doc.components.securitySchemes) return null;
  for (const key in doc.components.securitySchemes) {
    const security = doc.components.securitySchemes[key];
    if (security.type !== "oauth2" || !security.flows) continue;
    for (const flowKey in security.flows) {
      if (flowKey !== "authorizationCode") continue;
      return { key, value: security.flows[flowKey] };
    }
  }
  return null;
});

const redirectUrl = computed(() => {
  return `${location.origin}/_api/openapioauth2callback/${siteId.value}`;
});

const getAuthorizationCodeFlowData = (row: Authorize) => {
  if (!authorizationCodeFlow.value?.key) return;
  for (const key in row.securities) {
    if (key === authorizationCodeFlow.value.key) return row.securities[key];
  }
};
const challenge = (item: Authorize) => {
  let url = "";
  if (!openApi.value) return;
  if (authorizationCodeFlow.value) {
    const flow: any = authorizationCodeFlow.value;
    const data = getAuthorizationCodeFlowData(item);
    if (!(flow && data)) return;

    url = `${
      flow.authorizationUrl
    }?response_type=code&state=${new Date().getTime()}&client_id=${
      data.clientId
    }&redirect_uri=${redirectUrl.value}/${item.id}/${
      authorizationCodeFlow.value.key
    }`;

    if (flow.scopes) {
      const scopes = [];
      for (const key in flow.scopes) scopes.push(key);
      url += `&scope=${encodeURI(scopes.join(" "))}`;
    }
  } else {
    const redirect = `${location.origin}/_api/OpenApi/SaveToken?SiteId=${siteId.value}&id=${item.id}`;
    url = `${openApi.value.authUrl}?redirect=${encodeURIComponent(redirect)}`;
  }
  window.open(url, new Date().getTime().toString());
};

onLoad();
</script>

<template>
  <div class="p-24">
    <div class="flex items-center">
      <Breadcrumb
        :crumb-path="[
          {
            name: 'OpenApis',
            route: { name: 'openapis' },
          },
          { name: 'Authorizes' },
        ]"
      />
    </div>
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{
          feature: 'openApi',
          action: 'edit',
        }"
        round
        @click="onOPenDialog()"
      >
        <div class="flex items-center">
          <el-icon class="iconfont icon-a-addto" />
          {{ t("common.create") }}
        </div>
      </el-button>
    </div>
    <KTable
      :data="data"
      show-check
      :permission="{
        feature: 'openApi',
        action: 'edit',
      }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')" prop="authorizeName">
        <template #default="{ row }">
          <span class="ellipsis">{{ row.authorizeName }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.lastModified')">
        <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
      </el-table-column>
      <el-table-column width="90" align="right">
        <template #default="{ row }">
          <!-- 授权挑战 -->
          <IconButton
            v-if="
              openApi?.authUrl ||
              (authorizationCodeFlow && getAuthorizationCodeFlowData(row))
            "
            :tip="t('common.challenge')"
            icon="icon-debugging"
            @click="challenge(row)"
          />
          <IconButton
            icon="icon-a-writein"
            :tip="t('common.edit')"
            @click="onOPenDialog(row.id)"
          />
        </template>
      </el-table-column>
    </KTable>
    <EditAuthorizeDialog
      v-if="showDialog"
      :id="dialogId"
      v-model="showDialog"
      :open-api="openApi!"
      @confirm="onSave"
    />
  </div>
</template>
