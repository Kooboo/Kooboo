<template>
  <div v-if="data.length" class="site-list">
    <div
      v-for="item in data"
      :key="item.siteId"
      class="site-item relative"
      :class="{
        'site-item--offline': !item.online,
        'site-item--selected': item.selected,
        'site-item--hover': item.isHover,
      }"
      data-cy="site-item"
    >
      <div v-loading="item.inProgress" class="site-item__body">
        <div v-if="!item.online" class="site-item__offline">
          {{ t("siteList.offLine") }}
        </div>
        <PreviewFrame
          class="dark:opacity-70"
          :url="item.homePageLink"
          :scale="0.28"
          :site-id="item.siteId"
        />
        <div class="site-item__hover z-11">
          <div class="site-item__head">
            <el-checkbox v-model="item.selected" size="large" />
            <div class="site-item__more">
              <el-dropdown
                trigger="click"
                @visible-change="handleActionVisible($event, item)"
              >
                <div class="site-item__more-btn" data-cy="site-more-actions">
                  {{ t("common.moreActions") }}
                  <el-icon class="iconfont icon-pull-down" />
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
            </div>
          </div>
          <div class="site-item__hover-main">
            <router-link
              :to="{ name: 'dashboard', query: { SiteId: item.siteId } }"
            >
              <el-button type="primary" round data-cy="administration">
                {{ t("common.administration") }}
              </el-button></router-link
            >
            <div class="site-item__hover-foot">
              <!-- <a @click="gotoDesign(item)">
                <span class="iconfont icon-a-writein2" />
                {{ t("common.design") }}
              </a> -->
              <a
                class="w-full text-center"
                data-cy="share"
                @click="share(item)"
              >
                <span class="iconfont icon-link2" />
                {{ t("common.share") }}
              </a>
            </div>
          </div>
        </div>
      </div>
      <div
        class="site-item__hover__foot"
        :class="{ 'site-item__foot': item.selected }"
      >
        <div
          class="ellipsis site-item__title"
          :title="item.siteDisplayName"
          data-cy="site-name"
        >
          {{ item.siteDisplayName }}
        </div>
        <div
          class="flex justify-between items-center group cursor-pointer"
          data-cy="home-url"
          @click="openInNewTab(item.homePageLink)"
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
              class="iconfont icon-eyes group-hover:text-blue text-m dark:text-999"
              data-cy="preview-in-grid"
            />
          </el-tooltip>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { useSiteList } from "../hooks/use-site-list";
import type { SiteItem } from "../type";
import { openInNewTab } from "@/utils/url";

import { useI18n } from "vue-i18n";
defineProps<{
  data: SiteItem[];
}>();
const emits = defineEmits<{
  (e: "delete", value: SiteItem): void;
  (e: "share", value: SiteItem): void;
  (e: "export", value: SiteItem): void;
  (e: "moveTo", value: SiteItem): void;
}>();
const { t } = useI18n();
const { getMoreActions, handleActionVisible, share, gotoDesign } =
  useSiteList(emits);
</script>

<style lang="scss" scoped>
.site-list {
  display: grid;
  gap: 24px;
  grid-template-columns: repeat(3, 1fr);
  margin-top: 24px;
}
.site-item {
  --item-head-height: 24px;
  background-color: #fff;
  .dark & {
    background-color: rgb(131, 128, 128);
  }
  border-radius: 8px;
  overflow: hidden;
  position: relative;
  box-shadow: 0px 2px 4px 0px rgba(0, 0, 0, 0.2);
  &__head {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 16px 24px 0;
    min-height: 42px;
  }
  &--offline {
    background-color: rgba(188, 191, 197, 0.7);

    .dark & {
      background-color: rgba(61, 60, 60, 0.2);
    }
  }
  &__offline {
    top: 16px;
    background-color: rgba(188, 191, 197, 0.7);
    display: flex;
    justify-content: center;
    align-items: center;
    color: #fff;
    border-radius: 50px;
    height: var(--item-head-height);
    min-width: 76px;
    position: absolute;
    z-index: 1;
    left: 15%;
    font-size: 12px;
    transform: translateX(-50%);
    box-shadow: 0px 4px 8px 0px rgba(0, 0, 0, 0.2);
  }
  &__body {
    height: 175px;
    overflow: hidden;
    text-align: center;
    img {
      width: 100%;
    }
    position: relative;
  }
  &__title {
    .dark & {
      color: rgba(#fff, 0.86);
    }
    line-height: 20px;
    font-weight: bold;
    font-size: 14px;
  }
  &__url {
    color: #666;
    .dark & {
      color: rgba(#fff, 0.6);
    }
    line-height: 18px;
    font-size: 12px;
  }
  &__hover {
    opacity: 0;
    text-align: center;
    position: absolute;
    left: 0;
    top: 0;
    bottom: 0;
    right: 0;
    background: rgba($color: #fff, $alpha: 0.87);
    .dark & {
      background: rgba($color: #444, $alpha: 0.87);
    }
    &-main {
      width: 180px;
      margin: 28px auto 0;
      button {
        width: 100%;
      }
    }
    &__foot {
      padding: 16px 24px;
      background-color: rgba(233, 234, 240, 1);
      height: 100%;
      .dark & {
        background-color: #444;
      }
    }
    &-foot {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-top: 20px;
      font-size: 12px;
      @apply text-blue;
      a {
        color: inherit;
        cursor: pointer;
        .iconfont {
          font-size: 12px;
          margin-right: 5px;
        }
      }
    }
  }
  &__more {
    &-btn {
      display: flex;
      justify-content: space-between;
      align-items: center;
      height: var(--item-head-height);
      font-size: 12px;
      cursor: pointer;
    }
    .iconfont {
      margin-left: 8px;
      font-size: 10px;
      height: 12px;
      line-height: 12px;
    }
  }
  &--selected {
    .site-item {
      &__foot {
        @apply bg-blue;
        color: #fff;
      }
      &__url {
        color: inherit;
      }
      &__hover {
        opacity: 1;
      }
      &__more,
      &__hover-main {
        display: none;
      }
    }
  }
  &:hover,
  &--hover {
    box-shadow: 0px 4px 8px 0px rgba(0, 0, 0, 0.2);
    .site-item {
      &__hover,
      &__more,
      &__hover-main {
        opacity: 1;
      }
    }
  }
}

.el-dropdown-menu {
  border-radius: 8px;
}

:deep(.el-loading-mask) {
  z-index: 10;
}
</style>
