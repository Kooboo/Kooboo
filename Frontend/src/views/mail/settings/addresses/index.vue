<template>
  <div class="p-24">
    <div class="flex items-center justify-between pb-24 space-x-16">
      <el-dropdown trigger="click">
        <el-button round data-cy="add-address">
          <div class="flex items-center">
            {{ t("common.newAddress") }}
            <el-icon class="iconfont icon-pull-down text-12px ml-8 !mr-0" />
          </div>
        </el-button>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item
              data-cy="add-normal-address"
              @click="addAddress('Normal')"
            >
              <el-icon class="iconfont icon-a-addto" />
              <span>{{ t("common.general") }}</span>
            </el-dropdown-item>
            <el-dropdown-item
              data-cy="add-wildcard-address"
              @click="addAddress('Wildcard')"
            >
              <el-icon class="iconfont icon-a-addto" />
              <span>{{ t("common.wildcard") }}</span>
            </el-dropdown-item>
            <el-dropdown-item
              data-cy="add-group-address"
              @click="addAddress('Group')"
            >
              <el-icon class="iconfont icon-a-addto" />
              <span>{{ t("common.group") }}</span>
            </el-dropdown-item>
            <el-dropdown-item
              data-cy="add-forward-address"
              @click="addAddress('Forward')"
            >
              <el-icon class="iconfont icon-a-addto" />
              <span>{{ t("common.forward") }}</span>
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
      <div>
        <span class="mr-12 text-m dark:text-fff/86">
          {{ t("common.defaultAddress") }}
        </span>
        <el-select
          :model-value="emailStore.defaultAddress"
          :placeholder="t('common.pleaseSelect')"
          :title="emailStore.defaultAddress"
          class="w-250px"
          @update:model-value="changeDefaultAddress"
        >
          <el-option
            v-for="opt in emailStore.addressList.filter(
              (f) => f.addressType !== 'Wildcard'
            )"
            :key="opt.id"
            :value="opt.address"
            :label="opt.address"
          />
        </el-select>
      </div>
    </div>

    <KTable
      v-if="emailStore.addressList"
      :data="emailStore.addressList"
      show-check
      @delete="onDelete"
    >
      <el-table-column
        :label="t('common.address')"
        prop="address"
        min-width="300"
      >
        <template #default="{ row }">
          <span class="ellipsis" :title="row.address" data-cy="address">{{
            row.address
          }}</span>
        </template></el-table-column
      >
      <el-table-column :label="t('common.name')" prop="name" min-width="150">
        <template #default="{ row }">
          <span class="ellipsis" :title="row.name" data-cy="name">{{
            row.name
          }}</span>
        </template></el-table-column
      >
      <el-table-column :label="t('common.useFor')" width="150">
        <template #default="{ row }">
          <el-tag size="small" class="rounded-full" data-cy="used-for">{{
            tagLabel[row.addressType]
          }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.remark')" width="200">
        <template #default="{ row }">
          <el-tag
            v-if="row.addressType === 'Forward'"
            type="warning"
            size="small"
            class="rounded-full"
            data-cy="remark-forward"
            :title="t('common.forwardAddress') + ':' + row?.forwardAddress"
          >
            {{ row?.forwardAddress }}
          </el-tag>
          <el-tag
            v-if="row.addressType === 'Group'"
            size="small"
            class="rounded-full cursor-pointer"
            data-cy="remark-group"
            @click="editGroup(row)"
          >
            {{ row?.count }} {{ t("mail.members") }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column align="right" width="120">
        <template #default="{ row }">
          <IconButton
            v-if="row.addressType !== 'Wildcard'"
            icon="icon-dianziqianming"
            :tip="t('common.editSignature')"
            @click="editSignature(row.id)"
          />
          <IconButton
            icon="icon-a-writein"
            :tip="t('common.edit')"
            @click="addAddress(row)"
          />

          <IconButton
            icon="icon-a-Emilenotopen"
            :tip="t('common.jumpToInbox')"
            class="text-blue"
            @click="toInbox(row)"
          />
        </template>
      </el-table-column>
    </KTable>

    <AddDialog
      v-if="showAddAddressDialog"
      v-model="showAddAddressDialog"
      :type="addressType"
      :address="currentAddress"
      @create-success="emailStore.loadAddress('inbox')"
    />

    <EditGroupDialog
      v-if="showEditGroupDialog"
      v-model="showEditGroupDialog"
      :group="currentGroup"
    />
    <EditSignatureDialog
      v-if="showEditSignatureDialog"
      v-model="showEditSignatureDialog"
      :current-id="currentId!"
    />
  </div>
</template>

<script lang="ts" setup>
import type { Address, AddressType } from "@/api/mail/types";
import { deleteAddress, setDefaultSender } from "@/api/mail";
import AddDialog from "./add-dialog.vue";
import EditGroupDialog from "./edit-group-dialog.vue";
import EditSignatureDialog from "./edit-signature-dialog.vue";

import { ref, reactive, watch, nextTick } from "vue";
import KTable from "@/components/k-table";
import { useRouter } from "vue-router";
import { useI18n } from "vue-i18n";
import { useEmailStore } from "@/store/email";
import { showDeleteEmailConfirm } from "@/components/basic/confirm";
import { showConfirm } from "@/components/basic/confirm";

const { t } = useI18n();
const router = useRouter();
const emailStore = useEmailStore();
const changeDefaultAddress = async (value: any) => {
  if (value === emailStore.defaultAddress) return;
  await showConfirm(
    t("common.areYouSureToSetThisAddressAsTheDefaultEmailAddress")
  );
  await setDefaultSender(value);
  emailStore.defaultAddress = value;
};

const addDialog = reactive({
  status: false,
  type: "" as AddressType,
  open(type: AddressType) {
    addDialog.type = type;
    addDialog.status = true;
  },
});
const showEditGroupDialog = ref(false);
const showAddAddressDialog = ref(false);
const showEditSignatureDialog = ref(false);
const currentId = ref<number>();

const currentAddress = ref<Address>();
const currentGroup = ref();
const addressType = ref();

const addAddress = (row: Address | string) => {
  if (typeof row !== "string") {
    currentAddress.value = row;
  }
  showAddAddressDialog.value = true;
  addressType.value = row;
};

watch(
  () => showAddAddressDialog.value,
  () => {
    if (!showAddAddressDialog.value) {
      currentAddress.value = undefined;
    }
  }
);

const editGroup = async (row: Address) => {
  showEditGroupDialog.value = true;
  currentGroup.value = row;
};

const editSignature = async (id: number) => {
  currentId.value = id;
  showEditSignatureDialog.value = true;
};
const tagLabel = ref<any>({
  Normal: t("common.general"),
  Wildcard: t("common.wildcard"),
  Group: t("common.group"),
  Forward: t("common.forward"),
});

function toInbox(row: Address) {
  router.push({
    name: "inbox",
    query: {
      address: row.address,
    },
  });
}

async function onDelete(addresses: Address[]) {
  if (addresses.length) {
    await showDeleteEmailConfirm();
    await deleteAddress(addresses.map((item) => item.id));
    if (addresses.map((m) => m.address).includes(emailStore.defaultAddress))
      emailStore.defaultAddress = "";
    emailStore.loadAddress("addresses");
  }
}
const load = async () => {
  await emailStore.loadAddress("addresses");
};
load();

//给tinymce编辑器的菜单弹框设置z-index样式
const index = ref();
watch(
  () => showEditSignatureDialog.value,
  () => {
    nextTick(() => {
      if (showEditSignatureDialog.value) {
        index.value = Array.from<HTMLElement>(
          document.querySelectorAll(".el-overlay")
        ).at(-1)?.style.zIndex;
        document.body.style.setProperty("--tox-tinymce-aux-index", index.value);
        document.body.classList.add("editSignatureDialog");
      } else {
        document.body.classList.remove("editSignatureDialog");
      }
    });
  }
);
</script>

<style>
.editSignatureDialog .tox-tinymce-aux {
  z-index: var(--tox-tinymce-aux-index);
}
</style>
