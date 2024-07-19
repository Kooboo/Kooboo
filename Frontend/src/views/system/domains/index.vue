<script lang="ts" setup>
import { ref } from "vue";
import KTable from "@/components/k-table";
import NewBindingDialog from "./new-binding-dialog.vue";
import { deletes, verifySSL, setSsl } from "@/api/binding";
import type { Binding } from "@/api/binding/types";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useSiteStore } from "@/store/site";

import { useI18n } from "vue-i18n";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useBindingStore } from "@/store/binding";
const { t } = useI18n();
const showNewBindingDialog = ref(false);
const siteStore = useSiteStore();
const bindingStore = useBindingStore();

const load = async () => {
  bindingStore.loadBindings();
};

const onDelete = async (rows: Binding[]) => {
  await showDeleteConfirm(rows.length);
  await deletes(rows.map((m) => m.id));
  siteStore.loadSite();
  load();
};

const onEnableSSL = async (value: Binding) => {
  await verifySSL({
    rootDomain: value.fullName,
  });

  await setSsl({
    rootDomain: value.fullName,
  });

  load();
};

load();
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.domains')" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{
          feature: 'domain',
          action: 'edit',
        }"
        round
        data-cy="new-binding"
        @click="showNewBindingDialog = true"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.newBinding") }}
        </div>
      </el-button>
    </div>
    <KTable
      :data="bindingStore.bindings"
      show-check
      :permission="{
        feature: 'domain',
        action: 'delete',
      }"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.domainOrPort')">
        <template #default="{ row }">
          <span data-cy="domain">{{
            row.defaultPortBinding ? row.port : row.fullName
          }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.redirectTo')">
        <template #default="{ row }">
          <span>{{ row.redirect }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.culture')">
        <template #default="{ row }">
          <span>{{ row.culture }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.SSLEnabled')" align="center">
        <template #default="{ row }">
          <div v-if="!row.defaultPortBinding">
            <el-tooltip
              class="box-item"
              effect="dark"
              :content="row.enableSsl ? '' : t('common.enableSSL')"
              :disabled="row.enableSsl"
              placement="top"
            >
              <el-switch
                :model-value="row.enableSsl"
                :disabled="
                  row.enableSsl || !siteStore.hasAccess('domain', 'edit')
                "
                :title="
                  !siteStore.hasAccess('domain', 'edit')
                    ? t('common.noPermission')
                    : ''
                "
                data-cy="ssl-enabled"
                @update:model-value="onEnableSSL(row)"
              />
            </el-tooltip>
          </div>
        </template>
      </el-table-column>
    </KTable>
  </div>
  <NewBindingDialog
    v-if="showNewBindingDialog"
    v-model="showNewBindingDialog"
    @reload="load"
  />
</template>
