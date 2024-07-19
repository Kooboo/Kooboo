<script lang="ts" setup>
import { getContactListApi, deleteContactApi } from "@/api/mail";
import type { Contact } from "@/api/mail/types";
import EditContactDialog from "./edit-contact-dialog.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";

import { ref, watch } from "vue";
import KTable from "@/components/k-table";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const list = ref<Contact[]>([]);
const editContactDialog = ref(false);
const currentContact = ref({} as Contact);
const load = async () => {
  list.value = await getContactListApi();
};
load();

async function onDelete(rows: Contact[]) {
  await showDeleteConfirm(rows.length);
  const ids = rows.map((item) => item.id!);
  await deleteContactApi({ ids });
  load();
}

const editContact = (row: Contact) => {
  editContactDialog.value = true;
  currentContact.value = JSON.parse(JSON.stringify(row));
};
watch(
  () => editContactDialog.value,
  () => {
    if (!editContactDialog.value) {
      currentContact.value = {} as Contact;
    }
  }
);
</script>

<template>
  <div class="p-24">
    <el-button round class="mb-24" @click="editContactDialog = true">
      <el-icon class="iconfont icon-a-addto" />
      {{ t("common.addContact") }}
    </el-button>
    <KTable
      :data="list"
      show-check
      sort="name"
      order="ascending"
      @delete="onDelete"
    >
      <el-table-column :label="t('common.name')" prop="name" />
      <el-table-column :label="t('common.address')" prop="address" />
      <el-table-column align="center" width="60">
        <template #default="{ row }">
          <div class="flex items-center space-x-4">
            <IconButton
              icon="icon-a-writein"
              :tip="t('common.edit')"
              @click="editContact(row)"
            />
          </div>
        </template>
      </el-table-column>
    </KTable>
  </div>
  <EditContactDialog
    v-if="editContactDialog"
    v-model="editContactDialog"
    :current-contact="currentContact"
    :contact-list="list"
    @reload="load()"
  />
</template>
