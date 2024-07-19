<template>
  <div class="p-24">
    <el-button round class="mb-24" @click="editJob(undefined, false)">
      {{ t("common.addMigrationJob") }}
    </el-button>
    <div class="text-s mb-12 text-666 space-x-4">
      <el-icon class="iconfont icon-Tips" />
      <span>
        {{ t("common.emailMigrationTips") }}
      </span>
    </div>
    <KTable :data="jobs">
      <el-table-column :label="t('common.name')" prop="name" />
      <el-table-column :label="t('common.account')" prop="emailAddress" />
      <el-table-column :label="t('common.imapServer')" prop="host" />
      <el-table-column label="SSL" prop="forceSSL" align="center">
        <template #default="{ row }">
          <span :class="row.forceSSL ? 'text-green' : 'text-orange'">{{
            row.forceSSL ? t("common.yes") : t("common.no")
          }}</span>
        </template>
      </el-table-column>
      <el-table-column
        :label="t('common.port')"
        prop="port"
        width="60"
        align="center"
      />
      <el-table-column
        :label="t('common.latestExecutionStatus')"
        prop="forceSSL"
        align="center"
      >
        <template #default="{ row }">
          <span
            :class="{
              'text-green': row.status === 'Success',
              'text-orange': row.status === 'Failure',
              'text-gray': row.status === 'Running ',
            }"
          >
            <el-popover
              v-if="row.errorMessage"
              trigger="hover"
              popper-class="!w-auto max-w-300px !p-0"
              placement="left"
            >
              <template #reference>
                <el-icon class="iconfont icon-fasongshibai text-14px" />
              </template>
              <el-scrollbar
                max-height="50vh"
                class="p-12 text-left !break-normal"
              >
                <div>{{ row.errorMessage }}</div>
              </el-scrollbar>
            </el-popover>
            {{
              row.status === "Success"
                ? t("common.success")
                : row.status === "Failure"
                ? t("common.failed")
                : t("common.executing")
            }}
          </span>
        </template>
      </el-table-column>

      <el-table-column width="120" align="right">
        <template #default="{ row }">
          <div class="flex justify-between items-center">
            <IconButton
              class="!text-20px"
              :icon="row.active ? 'icon-quxiaozhihang' : 'icon-putongzhihang'"
              :tip="row.active ? t('common.stop') : t('common.execute')"
              data-cy="execute"
              @click="jobAction(row)"
            />
            <IconButton
              icon="icon-a-writein"
              :tip="t('common.edit')"
              :class="
                row.active
                  ? 'cursor-not-allowed !hover:text-[#989b9c] !hover:text-opacity-40  !text-opacity-40'
                  : ''
              "
              data-cy="edit"
              @click="editJob(row)"
            />
            <IconButton
              icon="icon-delete "
              class="text-orange hover:text-orange"
              :class="
                row.active
                  ? 'cursor-not-allowed !hover:text-orange !hover:text-opacity-40  !text-opacity-40 '
                  : ''
              "
              :tip="t('common.delete')"
              data-cy="remove"
              @click="deleteJob(row)"
            />
          </div>
        </template>
      </el-table-column>
    </KTable>
  </div>
  <JobDetailsDialog
    v-if="detailDialog"
    v-model="detailDialog"
    :current-job="currentJob"
    @reload="load"
  />
</template>

<script lang="ts" setup>
import JobDetailsDialog from "./job-details-dialog.vue";
import { ref, onMounted } from "vue";
import {
  getEmailMigrationJobs,
  runJob,
  cancelJob,
  deleteJob as deleteJobApi,
} from "@/api/mail";
import type { EmailMigration } from "@/api/mail/types";
import { useI18n } from "vue-i18n";
import { useEmailStore } from "@/store/email";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { onBeforeRouteLeave } from "vue-router";

const { t } = useI18n();

const { loadAddress } = useEmailStore();

const detailDialog = ref(false);
const jobs = ref<EmailMigration[]>([]);

let timeoutId: any = undefined;
onMounted(async () => {
  await loadAddress("addresses");
});

onBeforeRouteLeave(() => {
  clearTimeout(timeoutId);
});

async function load() {
  jobs.value = await getEmailMigrationJobs();
  loopStatus();
}
function loopStatus() {
  clearTimeout(timeoutId);
  timeoutId = setTimeout(async () => {
    jobs.value = await getEmailMigrationJobs(true);
    if (jobs.value.some((it) => it.active)) {
      loopStatus();
    }
  }, 1000);
}

const currentJob = ref();
const editJob = (row: EmailMigration | undefined, isEdit = true) => {
  if (isEdit) {
    currentJob.value = row;
  } else {
    currentJob.value = {
      id: "",
      addressId: undefined,
      name: "",
      emailAddress: "",
      host: "",
      forceSSL: true,
      port: 993,
      password: "",
    };
  }
  if (row?.active) return;

  detailDialog.value = true;
};

const jobAction = async (job: EmailMigration) => {
  let action = job.active ? cancelJob : runJob;

  await action(job.id!);
  job.active = !job.active;
  loopStatus();
};

const deleteJob = async (row: EmailMigration) => {
  if (row.active) return;
  await showDeleteConfirm();
  await deleteJobApi(row.id!);
  load();
};

load();
</script>
