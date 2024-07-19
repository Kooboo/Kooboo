<template>
  <div
    v-if="data.length > 0 || (!currentFolder && folders.length > 0)"
    class="site-list"
  >
    <table cellspacing="0" cellpadding="0" border="0" class="site-list__table">
      <thead>
        <tr>
          <th width="70" />
          <th align="left">{{ t("common.name") }}</th>
          <th width="150">{{ t("common.onlineSituation") }}</th>
          <th width="120">{{ t("common.pages") }}</th>
          <th width="120">{{ t("common.images") }}</th>
          <th width="200" />
        </tr>
      </thead>
      <tbody>
        <template v-if="!currentFolder">
          <tr
            v-for="item in folders"
            :key="item.key"
            class="site-item"
            data-cy="folder-item"
          >
            <td class="cursor-pointer" @click="handleSelectFolder(item)">
              <div class="flex items-center justify-center">
                <img
                  class="site-item__img site-item__img--folder"
                  src="@/assets/images/folder.svg"
                />
              </div>
            </td>
            <td
              align="left"
              class="cursor-pointer"
              @click="handleSelectFolder(item)"
            >
              <div class="site-item__name">
                <div
                  class="ellipsis site-item__title"
                  :title="item.key"
                  data-cy="folder-name"
                >
                  {{ item.key }}
                </div>
                <div class="ellipsis site-item__url" data-cy="total-sites">
                  {{ t("common.totalSites", { total: item.value }) }}
                </div>
              </div>
            </td>
            <td />
            <td />
            <td />
            <td class="text-right pr-16 space-x-16">
              <el-dropdown
                class="w-40px"
                trigger="click"
                @visible-change="handleFolderActionVisible($event, item)"
              >
                <div @click.stop>
                  <el-tooltip
                    class="box-item"
                    effect="dark"
                    :content="t('common.moreActions')"
                    placement="top"
                  >
                    <div
                      class="site-item__circle-icon"
                      data-cy="folder-more-actions"
                    >
                      <div v-for="x in 3" :key="x" class="icon-dot" />
                    </div>
                  </el-tooltip>
                </div>
                <template #dropdown>
                  <el-dropdown-menu>
                    <el-dropdown-item
                      data-cy="rename-folder"
                      @click="handleRename(item)"
                    >
                      {{ t("common.rename") }}
                    </el-dropdown-item>
                    <el-dropdown-item
                      style="color: #ff7148"
                      data-cy="delete-folder"
                      @click="handleDelete(item)"
                    >
                      {{ t("common.delete") }}
                    </el-dropdown-item>
                  </el-dropdown-menu>
                </template>
              </el-dropdown>
            </td>
          </tr>
        </template>
        <tr
          v-for="item in data"
          :key="item.siteId"
          class="site-item"
          :class="{
            'site-item--selected': item.selected,
            'site-item--offline': !item.online,
          }"
          data-cy="site-item"
        >
          <td v-loading="item.inProgress">
            <el-checkbox
              v-model="item.selected"
              size="large"
              class="align-middle"
              @click.stop
            />
          </td>
          <td
            align="left"
            class="cursor-pointer"
            @click="gotoDashboard(item.siteId)"
          >
            <div class="site-item__name">
              <div class="ellipsis site-item__title" data-cy="site-name">
                {{ item.siteDisplayName }}
              </div>
              <div
                class="flex items-center mt-4 group cursor-pointer"
                data-cy="home-url"
                @click.stop="openInNewTab(item.homePageLink)"
              >
                <div
                  class="ellipsis site-item__url group-hover:underline mr-8"
                  :title="item.homePageLink"
                >
                  {{ item.homePageLink }}
                </div>

                <el-tooltip
                  v-if="item.homePageLink"
                  placement="top"
                  :show-after="300"
                  :content="t('common.preview')"
                >
                  <span
                    class="iconfont icon-eyes group-hover:text-blue text-m leading-18px dark:text-999"
                    data-cy="preview-in-list"
                  />
                </el-tooltip>
              </div>
            </div>
          </td>
          <td :class="{ 'cell--success': item.online }" data-cy="online-status">
            {{ item.online ? t("common.online") : t("siteList.offLine") }}
          </td>
          <td>{{ item.pageCount }}</td>
          <td>{{ item.imageCount }}</td>

          <td class="text-right pr-16 space-x-16" @click.stop>
            <!-- <a class="site-item__circle-icon">
              <IconButton
                class="text-16px"
                icon="icon-a-writein2"
                :tip="t('common.inlineEdit')"
                @click="gotoDesign(item)"
            /></a> -->
            <a class="site-item__circle-icon" data-cy="share">
              <IconButton
                class="text-16px text-blue"
                circle
                icon="icon-link2"
                :tip="t('common.share')"
                @click="share(item)"
            /></a>
            <el-dropdown
              trigger="click"
              @visible-change="handleActionVisible($event, item)"
            >
              <div @click.stop="">
                <el-tooltip
                  class="box-item"
                  effect="dark"
                  :content="t('common.moreActions')"
                  placement="top"
                >
                  <div
                    class="site-item__circle-icon"
                    data-cy="site-more-actions"
                  >
                    <div v-for="x in 3" :key="x" class="icon-dot" />
                  </div>
                </el-tooltip>
              </div>
              <template #dropdown>
                <el-dropdown-menu>
                  <template
                    v-for="dropdown in getMoreActions(item)"
                    :key="dropdown.text"
                  >
                    <el-dropdown-item
                      :style="dropdown!.style"
                      :data-cy="`site-action-${dropdown!.text.replace(
                        /\s+/g,
                        '-'
                      )}`"
                      @click="dropdown!.action(item)"
                    >
                      {{ dropdown!.text }}
                    </el-dropdown-item>
                  </template>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script lang="ts" setup>
import { useSiteList } from "../hooks/use-site-list";
import { useFolderList } from "../hooks/use-folder-list";
import { openInNewTab } from "@/utils/url";

import type { FolderItem, SiteItem } from "../type";

import { useI18n } from "vue-i18n";
defineProps<{
  data: SiteItem[];
  folders: FolderItem[];
  currentFolder?: FolderItem | null;
}>();
const emits = defineEmits<{
  (e: "delete", value: SiteItem): void;
  (e: "share", value: SiteItem): void;
  (e: "export", value: SiteItem): void;
  (e: "moveTo", value: SiteItem): void;
  (e: "select", value: FolderItem): void;
  (e: "remove-success", value: FolderItem): void;
  (e: "rename", value: FolderItem): void;
}>();
const { t } = useI18n();

const {
  getMoreActions,
  gotoDashboard,
  handleActionVisible,
  share,
  gotoDesign,
} = useSiteList(emits);

const {
  handleRename,
  handleSelectFolder,
  handleDelete,
  handleActionVisible: handleFolderActionVisible,
} = useFolderList(emits);
</script>

<style lang="scss" scoped>
.dark {
  .site-list {
    background-color: #333;
    color: rgba(#fff, 0.87);

    &__table {
      thead {
        background-color: #222;
      }
    }
  }

  .site-item {
    &__name {
      color: rgba(#fff, 0.87);
    }
    &__url {
      color: rgba(#fff, 0.5);
    }

    &__circle-icon {
      background-color: #444;
    }
  }
}
.site-list {
  background-color: #fff;
  color: #192845;

  &__table {
    width: 100%;
    text-align: center;
    table-layout: fixed;
    thead {
      background-color: #ebeef5;

      height: 40px;
      th {
        vertical-align: middle;
        font-weight: normal;
      }
    }
    tbody {
      font-size: 14px;
      tr {
        border-bottom: 1px solid rgba(25, 40, 69, 0.1);
        height: 73px;
      }
      td {
        vertical-align: middle;
      }
    }
  }
}

.site-item {
  &--offline {
    background-color: #f8f7f7;
    color: #a7b0be !important;
    .dark & {
      background-color: #444;
    }
  }

  &--selected {
    background-color: rgba($main-blue, 0.3);
    border-left: 2px solid $main-blue;
  }
  &:hover:not(.site-item--selected) {
    background-color: rgba($color: $main-blue, $alpha: 0.1);
  }
  &__img {
    height: 44px;
    width: 80px;
    &--folder {
      width: 42px;
      height: auto;
    }
  }
  &__name {
    max-width: 400px;
    color: $main-color;
  }
  &__title {
    font-weight: bold;
    font-size: 16px;
    line-height: 20px;
  }
  &__url {
    line-height: 18px;
    font-size: 12px;
    color: #444;
  }
  &__circle-icon {
    cursor: pointer;
    background-color: #fff;
    border-radius: 50%;
    width: 40px;
    height: 40px;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    color: $main-blue;
  }
  .icon-dot {
    width: 4px;
    height: 4px;
    border-radius: 50%;
    background-color: currentColor;
    margin: 0 2.5px;
  }
}

.cell {
  &--success {
    color: #68d48c;
  }
}

:deep(.home__list-toolbar[data-v-5954443c]) {
  padding-right: 14px;
}
:deep(.el-loading-mask) {
  @apply bg-transparent;
}
</style>
