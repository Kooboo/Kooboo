<template>
  <el-dropdown
    v-if="currentFolder !== 'spam'"
    trigger="click"
    max-height="50vh"
    popper-class="max-w-200px"
    :disabled="selected.length ? false : true"
  >
    <div
      class="inline-flex rounded-full bg-[#F3F5F5] flex items-center h-30px px-16 cursor-pointer dark:bg-444 text-black dark:text-[#ffffffaa]"
      :class="
        selected.length
          ? ''
          : 'cursor-not-allowed text-999 text-opacity-60 dark:text-opacity-20'
      "
    >
      <span v-if="currentFolder === 'trash'">{{ t("common.restoreTo") }}</span>
      <span v-else data-cy="move-to">{{ t("common.moveTo") }}</span>

      <el-icon class="ml-8 iconfont icon-pull-down" />
    </div>
    <template #dropdown>
      <el-dropdown-menu>
        <template v-for="item in moveDropdowns" :key="item.name">
          <el-dropdown-item
            v-if="item.show"
            @click="moveMessage(selected, item.name)"
          >
            <span>{{ item.displayName }}</span>
          </el-dropdown-item>
        </template>

        <SubDropDown
          v-for="item in emailStore.folderMenuTree"
          :key="item.name"
          :dropdown-item="item"
          @move-message="moveMessage(selected, $event)"
        />
      </el-dropdown-menu>
    </template>
  </el-dropdown>
  <div
    v-if="currentFolder === 'spam'"
    class="inline-flex rounded-full bg-[#F3F5F5] flex items-center h-30px px-16 dark:bg-444 ml-10px"
    :class="!selected.length ? 'cursor-not-allowed ' : 'cursor-pointer'"
  >
    <span
      :class="
        !selected.length
          ? 'text-999 text-opacity-60 pointer-events-none cursor-not-allowed'
          : ''
      "
      data-cy="not-spam"
      @click="moveMessage(selected, 'inbox')"
    >
      {{ t("common.notSpam") }}</span
    >
  </div>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";
import { useEmailStore } from "@/store/email";
import SubDropDown from "./sub-dropdown.vue";
import type { SelectedEmail } from "@/api/mail/types";

const route = useRoute();

const emailStore = useEmailStore();
const { t } = useI18n();

interface PropsType {
  selected: SelectedEmail[];
  folderName?: string;
}

const props = defineProps<PropsType>();
const emit = defineEmits<{
  (e: "move-success", value: string): void;
}>();
const currentFolder = computed(() => {
  return props.folderName ?? emailStore.folder;
});

const moveDropdowns = computed(() => {
  return [
    {
      name: "inbox",
      displayName: t("common.inbox"),
      show: currentFolder.value !== "inbox",
    },
    {
      name: "sent",
      displayName: t("mail.sent"),
      show: currentFolder.value !== "sent",
    },
    {
      name: "drafts",
      displayName: t("common.drafts"),
      show: currentFolder.value === "trash",
    },
    {
      name: "spam",
      displayName: t("common.spam"),
      show: currentFolder.value !== "spam" && currentFolder.value !== "sent",
    },
  ];
});

const moveMessage = async (emails: SelectedEmail[], folder: string) => {
  if (route.query.folderName === folder) return;
  emit("move-success", folder);
};
</script>
