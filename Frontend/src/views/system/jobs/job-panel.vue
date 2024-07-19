<script lang="ts" setup>
import type { Job } from "@/api/site/job";
import { deletes, getList, run } from "@/api/site/job";
import KTable from "@/components/k-table";
import BooleanTag from "@/components/k-tag/boolean-tag.vue";
import { useTime } from "@/hooks/use-date";
import { ref } from "vue";
import EditJobDialog from "./edit-job-dialog.vue";
import { timeType } from "./job";

import { useI18n } from "vue-i18n";
import { showDeleteConfirm } from "@/components/basic/confirm";
const { t } = useI18n();
const data = ref<Job[]>([]);
const showDialog = ref(false);
const selected = ref<Job>();

const load = async () => {
  data.value = await getList();
};

const onDelete = async (rows: Job[]) => {
  await showDeleteConfirm(rows.length);
  await deletes(rows.map((m) => m.id));
  load();
};

const onRun = async (row: Job) => {
  await run(row.id);
  load();
};

load();
</script>

<template>
  <div>
    <div class="flex items-center space-x-16 mb-16">
      <el-button
        v-hasPermission="{
          feature: 'job',
          action: 'edit',
        }"
        round
        data-cy="add-job"
        @click="
          selected = undefined;
          showDialog = true;
        "
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.addJob") }}
        </div>
      </el-button>
    </div>
    <KTable
      :data="data.sort()"
      show-check
      sort="startTime"
      :permission="{
        feature: 'job',
        action: 'delete',
      }"
      @delete="onDelete"
      @run="onRun"
    >
      <template #bar="{ selected }">
        <el-button
          v-if="selected.length === 1"
          v-hasPermission="{ feature: 'job', action: 'edit' }"
          round
          data-cy="edit"
          @click="onRun(selected[0])"
          >{{ t("common.run") }}</el-button
        >
      </template>

      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          <span data-cy="name">{{ row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.status')">
        <template #default="{ row }">
          <ElTag
            class="rounded-full"
            :type="row.finish ? 'info' : row.active ? 'success' : 'danger'"
          >
            {{
              row.finish
                ? t("common.finish")
                : row.active
                ? t("common.active")
                : t("common.disable")
            }}
          </ElTag>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.startAt')" prop="startTime">
        <template #default="{ row }"
          ><span data-cy="start-time">{{
            useTime(row.startTime + "Z")
          }}</span></template
        >
      </el-table-column>
      <el-table-column :label="t('common.repeat')">
        <template #default="{ row }">
          <BooleanTag :value="row.repeat" />
        </template>
      </el-table-column>

      <el-table-column :label="t('common.interval')">
        <template #default="{ row }">
          <span v-if="row.repeat" data-cy="interval">
            {{
              `${row.frequenceUnit} ${
                timeType.find((f) => f.id == row.frequence)?.display
              }`
            }}
          </span>
        </template>
      </el-table-column>
      <el-table-column width="150" align="right">
        <template #default="{ row }">
          <IconButton
            icon="icon-a-writein"
            :tip="t('common.edit')"
            @click="
              selected = row;
              showDialog = true;
            "
          />
        </template>
      </el-table-column>
    </KTable>

    <EditJobDialog
      v-if="showDialog"
      v-model="showDialog"
      :job="selected"
      @reload="load"
    />
  </div>
</template>
