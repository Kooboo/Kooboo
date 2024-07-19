<script lang="ts" setup>
import KTable from "@/components/k-table";
import RelationsTag from "@/components/relations/relations-tag.vue";
import { useTime } from "@/hooks/use-date";
import { ref } from "vue";
import type { Form } from "@/api/form/types";
import SettingDialog from "./setting-dialog.vue";
import IconButton from "@/components/basic/icon-button.vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { useFormStore } from "@/store/form";

import { useI18n } from "vue-i18n";
import { showDeleteConfirm } from "@/components/basic/confirm";
const { t } = useI18n();
const formStore = useFormStore();
const showSettingDialog = ref(false);
const selectedItem = ref<Form>();

const onDeletes = async (items: Form[]) => {
  await showDeleteConfirm(items.length);
  formStore.deleteForms(items.map((m) => m.id));
};
</script>

<template>
  <KTable
    :data="formStore.embedded"
    show-check
    :permission="{
      feature: 'form',
      action: 'delete',
    }"
    @delete="onDeletes"
  >
    <el-table-column :label="t('common.name')">
      <template #default="{ row }">
        <router-link
          :to="
            useRouteSiteId({
              name: 'form-edit',
              query: {
                id: row.id,
                type: 'embedded',
              },
            })
          "
          data-cy="name"
        >
          <span :title="row.name" class="ellipsis text-blue cursor-pointer">{{
            row.name
          }}</span>
        </router-link>
      </template>
    </el-table-column>
    <el-table-column :label="t('common.data')">
      <template #default="{ row }">
        <router-link
          :to="
            useRouteSiteId({
              name: 'form-values',
              query: {
                id: row.id,
              },
            })
          "
        >
          <el-tag class="rounded-full" size="small">{{
            row.valueCount
          }}</el-tag>
        </router-link>
      </template>
    </el-table-column>
    <el-table-column :label="t('common.usedBy')">
      <template #default="{ row }">
        <RelationsTag :id="row.id" :relations="row.references" type="form" />
      </template>
    </el-table-column>
    <el-table-column :label="t('common.lastModified')">
      <template #default="{ row }">{{ useTime(row.lastModified) }}</template>
    </el-table-column>
    <el-table-column width="80px" align="right">
      <template #default="{ row }">
        <IconButton
          icon="icon-a-setup"
          :tip="t('common.setting')"
          data-cy="setting"
          @click="
            selectedItem = row;
            showSettingDialog = true;
          "
        />
      </template>
    </el-table-column>
  </KTable>
  <SettingDialog
    v-if="showSettingDialog"
    :id="selectedItem!.id"
    v-model="showSettingDialog"
  />
</template>
