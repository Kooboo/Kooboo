<script lang="ts" setup>
import { useRouter, useRoute } from "vue-router";
import { computed, nextTick, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useEmailStore } from "@/store/email";
import { dark } from "@/composables/dark";
import EditFolderDialog from "@/views/mail/folder/edit-folder-dialog.vue";
import { showConfirm } from "@/components/basic/confirm";
import { deleteEmailFolder } from "@/api/mail";
import type { EmailFolder } from "@/api/mail/types";
import SubMenu from "./sub-menu.vue";
import { ElMessage } from "element-plus";

const emailStore = useEmailStore();

const router = useRouter();
const route = useRoute();
const { t } = useI18n();

const editFolderDialog = ref(false);
const currentFolder = ref<string>("");
const editFolder = (fullName = "") => {
  editFolderDialog.value = true;
  currentFolder.value = fullName;
};

const menuRef = ref();

const reloadFolder = async (folderIndex: string | undefined) => {
  await emailStore.loadFolders();
  if (folderIndex) {
    expandSubMenu(folderIndex);
  }
};

const subMenu = ref([
  {
    label: t("common.inbox"),
    name: "inbox",
    icon: "icon-Inbox",
    show: true,
  },
  {
    label: t("mail.sent"),
    name: "sent",
    icon: "icon-send",
    show: true,
  },
  {
    label: t("common.folders"),
    name: "folder",
    icon: "icon-folder-fill",
    show: computed(() => {
      return emailStore.folderList?.length;
    }),
  },
]);

const menu = ref([
  {
    label: t("common.drafts"),
    name: "drafts",
    icon: "icon-draft",
  },
  {
    label: t("common.spam"),
    name: "spam",
    icon: "icon-a-alreadyremoved",
  },
  {
    label: t("common.trash"),
    name: "trash",
    icon: "icon-dustbin",
  },
]);

const changeMenu = async (index: any) => {
  emailStore.selectedMessage = [];
  emailStore.firstActiveMenu = "";

  let currentMenuIndex;
  if (index.name === "folder") {
    let folderName = index.folderName ?? "";
    currentMenuIndex = index.name + folderName;
  } else if (index.address) {
    currentMenuIndex = index.name + index.address;
  } else {
    currentMenuIndex = index.name;
  }

  // 点击已经激活的菜单时,手动更新未读数和列表数据
  if (emailStore.activeName === currentMenuIndex) {
    if (index.name === "folder") {
      await emailStore.loadFolders();
    } else {
      await emailStore.loadAddress(index.name);
    }
    await emailStore.refreshMessageList();
  }

  await router.push({
    name: index.name,
    query: {
      address: index.address ?? undefined,
      folderName: index.folderName ?? undefined,
    },
  });
};

const deleteFolder = async (row: EmailFolder) => {
  if (row.items?.length) {
    ElMessage.warning(
      t("common.pleaseDeleteTheSubFoldersInThisFolderBeforeRetrying")
    );
  } else {
    await showConfirm(t("common.deleteEmailFolderTips"));
    await deleteEmailFolder(row.id);
    await emailStore.loadFolders();
    if (
      (emailStore.activeName === "folder" && !emailStore.folderList.length) ||
      emailStore.activeName === "folder" + row.name
    ) {
      router.push({
        name: "inbox",
      });
    } else {
      await emailStore.refreshMessageList();
    }
  }
};

const getAddressList = (folder: string) => {
  if (folder === "sent") {
    return emailStore.sentAddressList;
  } else {
    return emailStore.addressList;
  }
};

// 获取需要展开的文件夹子菜单数组
function generateFolderArray(folderIndex: string) {
  const parts = folderIndex.split("/");
  let result = [];
  if (parts.length > 1) {
    result = parts
      .slice(0, -1)
      .map(
        (_item: string, index: number) =>
          "folder" + parts.slice(0, index + 1).join("/")
      );
    result.unshift("folder");
  } else {
    result = ["folder"];
  }

  return result;
}

// 展开文件夹子菜单
const expandSubMenu = async (folderIndex: string) => {
  let expandList = generateFolderArray(folderIndex);
  // 遍历展开
  for (let i = 0; i < expandList.length; i++) {
    if (menuRef.value) {
      await nextTick();
      menuRef.value.open(expandList[i]);
    }
  }
};
</script>

<template>
  <aside class="w-202px h-full relative z-50 dark:bg-[#252526] pb-[112px]">
    <el-scrollbar>
      <el-menu
        ref="menuRef"
        :default-active="emailStore.activeName"
        class="!dark:text-blue"
      >
        <div class="py-18px px-5">
          <el-button
            type="primary"
            class="w-full leading-10"
            @click="
            router.push({
              name: 'compose',
              query: {
                address: route.query.address,
                oldFolder: route.name==='compose'? route.query.oldFolder: route.name as string,
                type:'compose'
              },
            })
          "
          >
            <el-icon class="iconfont icon-a-addto !text-14px !text-fff" />
            {{ t("common.compose") }}
          </el-button>
        </div>

        <div class="h-16px border-t border-solid border-line dark:border-444" />

        <el-sub-menu
          v-for="menu in subMenu.filter((menu) => menu.show)"
          :key="menu.name"
          :index="menu.name"
          :class="{
            dark: dark,
            active:
              emailStore.firstActiveMenu === menu.name ||
              (menu.name === route.name &&
                !route.query.address?.toString() &&
                !route.query.folderName?.toString()) ||
              menu.name == route.query.activeMenu?.toString(),
          }"
        >
          <template #title
            ><div
              class="w-full dark:text-999 text-444 flex items-center h-14 leading-18px pl-5"
              @click.stop=""
              @click="changeMenu({ name: menu.name })"
            >
              <el-icon class="iconfont" :class="menu.icon" />
              <span>{{ menu.label }}</span>
              <div v-if="menu.name === 'folder'">
                <span
                  v-if="menu.name === 'folder' && emailStore.folderUnreadCount"
                  class="font-bold pl-4"
                  >({{ emailStore.folderUnreadCount }})</span
                >
                <el-icon
                  class="iconfont icon-a-addto hover:text-blue !text-s ml-12 !text-444 !dark:text-999"
                  @click.stop="editFolder()"
                />
              </div>

              <span
                v-else-if="menu.name === 'inbox' && emailStore.inboxUnreadCount"
                class="font-bold pl-4"
                >({{ emailStore.inboxUnreadCount }})</span
              >
              <span
                v-else-if="menu.name === 'sent' && emailStore.sentUnreadCount"
                class="font-bold pl-4"
                >({{ emailStore.sentUnreadCount }})</span
              >
            </div></template
          >

          <div v-if="menu.name !== 'folder'" @click.stop="">
            <el-menu-item
              v-for="item in getAddressList(menu.name)"
              :key="menu.name + item.address"
              :index="menu.name + item.address"
              class="text-12px !h-40px !leading-10"
              @click="changeMenu({ name: menu.name, address: item.address })"
            >
              <div class="w-full flex items-center pl-26px">
                <span class="ellipsis h-10" :title="item.address"
                  >{{ item.address }}
                </span>
                <span v-if="item.unRead !== 0" class="font-bold ml-4 h-10"
                  >({{ item.unRead }})</span
                >
              </div>
            </el-menu-item>
          </div>
          <SubMenu
            v-for="item in emailStore.folderMenuTree"
            v-else
            :key="item.name"
            :menu="item"
            @change-menu="changeMenu"
            @edit-folder="editFolder"
            @delete-folder="deleteFolder"
            @expand-sub-menu="expandSubMenu"
          />
        </el-sub-menu>

        <el-menu-item
          v-for="item in menu"
          :key="item.name"
          :index="item.name ? item.name : route.query.folder?.toString()"
          @click="changeMenu({ name: item.name })"
        >
          <el-icon class="iconfont" :class="item.icon" />
          <span class="dark:text-999">{{ item.label }}</span>
        </el-menu-item>

        <div
          v-if="!emailStore.folderList?.length"
          class="px-5 flex items-center py-16 mt-16 text-blue text-14px cursor-pointer border-t border-solid border-line dark:border-444"
          @click.stop="editFolder()"
        >
          <el-icon class="iconfont icon-a-addto !w-18px !text-blue" />
          <span>{{ t("common.newFolder") }}</span>
        </div>
      </el-menu>
    </el-scrollbar>

    <div
      class="absolute bottom-56px bg-fff left-0 right-0 h-[56px] flex items-center pl-5 !hover:bg-[#d3eafd] cursor-pointer dark:bg-[#252526] text-444 dark:text-999"
      :class="
        route.name === 'calendar'
          ? 'bg-[#d3eafd]  dark:bg-[#244764] !dark:hover:bg-[#244764]'
          : '!dark:hover:bg-[#18222c] '
      "
      @click="
        router.push({
          name: 'calendar',
        })
      "
    >
      <el-icon
        class="iconfont icon-richeng text-18px text-999 text-opacity-60 mr-8"
      />
      <span>{{ t("common.calendar") }}</span>
    </div>

    <div
      class="absolute bottom-0 left-0 right-0 h-[56px] bg-fff flex items-center pl-5 space-x-8 hover:bg-blue/20 cursor-pointer dark:bg-[#252526] text-444 dark:text-999"
      @click="
        router.push({
          name: 'addresses',
          query: {
            ...router.currentRoute.value.query,
            oldFolder: route.name?.toString(),
          },
        })
      "
    >
      <el-icon
        class="iconfont icon-shezhixitongshezhigongnengshezhishuxing-xian text-18px text-999 text-opacity-60"
      />
      <span>{{ t("common.settings") }}</span>
    </div>
  </aside>
  <EditFolderDialog
    v-if="editFolderDialog"
    v-model="editFolderDialog"
    :current-folder="currentFolder"
    @reload="reloadFolder"
  />
</template>
<style scoped>
:deep(.el-sub-menu__title) {
  padding-right: 40px !important;
  padding-left: 0 !important;
}
:deep(li.el-menu-item) {
  padding-right: 15px;
}

/* 一级菜单的暗黑模式选中样式 */
:deep(.el-sub-menu.active.dark > .el-sub-menu__title) {
  background: linear-gradient(
    to right,
    #2296f3 0%,
    #2296f3 2%,
    #173a56 2%,
    #173a56 100%
  );
}

/* 一级菜单的明亮模式选中样式 */
:deep(.el-sub-menu.active > .el-sub-menu__title) {
  background: linear-gradient(
    to right,
    #2296f3 0%,
    #2296f3 2%,
    #bcdffb 2%,
    #bcdffb 100%
  );
}
</style>
