<template>
  <div class="folder-list">
    <div
      v-for="item in data"
      :key="item.key"
      class="folder-item !dark:bg-[#333] !dark:text-[#fff]"
      data-cy="fodler-item"
      @click="handleSelectFolder(item)"
    >
      <div class="folder-item__main">
        <img class="folder-item__icon" src="@/assets/images/folder.svg" />
        <div class="folder-item__body">
          <p class="ellipsis" :title="item.key" data-cy="folder-name">
            {{ item.key }}
          </p>
          <p
            class="folder-item__total !dark:text-[#fffa]"
            data-cy="total-sites"
          >
            {{ t("common.totalSites", { total: item.value }) }}
          </p>
        </div>
      </div>
      <div class="folder-item__more">
        <el-dropdown
          trigger="click"
          @visible-change="handleActionVisible($event, item)"
        >
          <div @click.stop>
            <el-tooltip
              class="box-item"
              effect="dark"
              :content="t('common.moreActions')"
              placement="top"
            >
              <div
                class="iconfont icon-more"
                :class="item.visibleMore ? 'icon-notice2' : 'icon-notice'"
                data-cy="folder-more-actions"
              />
            </el-tooltip>
          </div>

          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item
                data-cy="rename-folder"
                @click="handleRename(item)"
                >{{ t("common.rename") }}</el-dropdown-item
              >
              <el-dropdown-item
                style="color: #ff7148"
                data-cy="delete-folder"
                @click="handleDelete(item)"
                >{{ t("common.delete") }}</el-dropdown-item
              >
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import type { FolderItem } from "../type";
import { useFolderList } from "../hooks/use-folder-list";

import { useI18n } from "vue-i18n";
defineProps<{
  data: FolderItem[];
}>();

const emits = defineEmits<{
  (e: "select", value: FolderItem): void;
  (e: "remove-success", value: FolderItem): void;
  (e: "rename", value: FolderItem): void;
}>();
const { t } = useI18n();

const { handleRename, handleSelectFolder, handleDelete, handleActionVisible } =
  useFolderList(emits);
</script>

<style lang="scss" scoped>
.folder-list {
  margin-bottom: 24px;
  display: grid;
  gap: 24px;
  grid-template-columns: repeat(3, 1fr);
}
.folder-item {
  cursor: pointer;
  padding: 24px;
  background: #ffffff;
  @apply text-black;
  border-radius: 8px;
  box-shadow: 0px 2px 4px 0px rgba(0, 0, 0, 0.2);
  display: flex;
  align-items: center;
  justify-content: space-between;
  &__main {
    display: flex;
    align-items: center;
    min-width: 0;
  }
  &__body {
    font-size: 14px;
    line-height: 20px;
    margin-left: 16px;
    min-width: 0;
    max-width: 300px;
  }
  &__icon {
    width: 48px;
  }
  &__total {
    color: #444;
  }

  .icon-more {
    @apply text-blue;
    font-size: 24px;
  }
}
</style>
