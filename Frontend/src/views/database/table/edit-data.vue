<script lang="ts" setup>
import { ref } from "vue";
import { useTable } from "./use-table";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import { getQueryString } from "@/utils/url";
import { emptyGuid } from "@/utils/guid";
import EditDataForm from "./edit-data-form.vue";

import { useRoute, useRouter } from "vue-router";
const router = useRouter();
const table = getQueryString("table") as string;
const id = (getQueryString("id") as string) || emptyGuid;
const { dbType, appendQueryToRoute, getListRouteName } = useTable();

const form = ref();
const listRouteName = getListRouteName();
const route = useRoute();
route.meta.activeMenu = listRouteName;

async function onSave() {
  await form.value?.save();
  gotoDataPage();
}

function gotoDataPage() {
  router.push(
    appendQueryToRoute({
      name: "table-data",
      query: { table, pageNr: route.query.pageNr },
    })
  );
}

function onCancel() {
  gotoDataPage();
}
</script>

<template>
  <div class="p-32 edit-data pb-150px">
    <div class="rounded-normal bg-fff py-24 px-56px dark:bg-[#252526]">
      <EditDataForm :id="id" ref="form" :db-type="dbType" :table="table" />
    </div>
    <KBottomBar
      :permission="{
        feature: 'database',
        action: 'edit',
      }"
      @cancel="onCancel"
      @save="onSave"
    />
  </div>
</template>

<style scoped lang="scss">
:deep(.el-input),
:deep(.el-textarea),
:deep(.el-select) {
  width: 504px;
}
</style>
